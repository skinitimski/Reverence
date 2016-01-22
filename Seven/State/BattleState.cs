using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thread = System.Threading.Thread;
using ThreadStart = System.Threading.ThreadStart;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Gtk;
using Gdk;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Time;
using GameState = Atmosphere.Reverence.State;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.State
{    
    internal class BattleState : GameState
    {
        #region Member Data
        
        private Queue<BattleIcon> _battleIcons;

        private Queue<Ally> _turnQueue;
        
        private bool _holdingSquare;

        private Formation _formation;
        
        private BattleEvent _victoryEvent;
        private BattleEvent _lossEvent;
                
        #endregion


        private class EndOfBattleEvent : BattleEvent
        {
            public const int DURATION = 4000;

            
            public EndOfBattleEvent(TimeFactory factory, string text)
                : base(factory, DURATION)
            {
                Text = text;
            }

            protected override void RunIteration(long elapsed, bool isLast)
            {

            }
            
            protected override string GetStatus(long elapsed)
            {
                return Text;
            }

            private string Text { get; set; }
        }
        

        private BattleState()
        {
        }
        
        public BattleState(string formationId)
            : this()
        {            
            _turnQueue = new Queue<Ally>();
            _battleIcons = new Queue<BattleIcon>();
            
            EventQueue = new Queue<BattleEvent>();
            PriorityQueue = new Queue<BattleEvent>();
            
            Random = new Random();
            
            Items = new List<IInventoryItem>();
            
            _formation = Formation.Get(formationId);
        }
        
        protected override void InternalInit()
        {
            ScreenState state = new ScreenState
            {
                Width = Seven.Config.WindowWidth,
                Height = Seven.Config.WindowHeight
            };
                        
            _victoryEvent = new EndOfBattleEvent(TimeFactory, "Victory!");
            _lossEvent = new EndOfBattleEvent(TimeFactory, "Annihilated!");

            Screen = new BattleScreen(state);

            BattleClock = TimeFactory.CreateClock(Seven.Party.BattleSpeed);
                       
            Allies = new Ally[Party.PARTY_SIZE];

            int x0 = 550;
            int y0 = 125;

            int xs = 30;
            int ys = 250 / Party.PARTY_SIZE;

            int[] e = _formation.GetAllyTurnTimersElapsed();

            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                if (Seven.Party[i] != null)
                {
                    int x = x0 + (i * xs);
                    int y = y0 + (i * ys);

                    Allies[i] = new Ally(this, Seven.Party[i], x, y, e[i]);
                    Allies[i].InitMenu(state);
                }
            }

            if (Allies.All(a => a == null))
            {
                throw new GameDataException("Must have at least one ally in battle.");
            }

            EnemyList = _formation.GetEnemyList(this);
            
            foreach (Ally ally in Allies)
            {
                if (ally != null)
                {
                    ally.TurnTimer.Unpause();
                }
            }
            
            foreach (Enemy enemy in EnemyList)
            {
                enemy.EnterBattle();
            }

            foreach (Enemy enemy in EnemyList)
            {
                enemy.TurnTimer.Unpause();
            }
        }




        
        private bool IsReady(Ally a)
        {
            bool ready = true;

            ready = ready && a != null;                         // they're not null
            ready = ready && a.TurnTimer.IsUp;                  // they're ready
            //ready = ready && !AbilityQueue.Contains(a.Ability); // they're not in the ability queue
            //ready = ready && ActiveAbility != a.Ability;        // they're not acting now
            ready = ready && !a.WaitingToResolve;
            ready = ready && !a.CannotAct;                      // they're able to act
            ready = ready && !_turnQueue.Contains(a);           // they're not already in the turn queue
            ready = ready && Commanding != a;                   // they're not in control

            return ready;
        }
        
        public override void RunIteration()
        {
            if (Paused == null)
            {
                if (!Loss && CheckForLoss())
                {
                    Loss = true;

                    EnqueueAction(_lossEvent, true);
                }
            
                if (!Loss && !Victory && CheckForVictory())
                {
                    Victory = true;

                    EnqueueAction(_victoryEvent, true);
                }

                SetControl();
            
                CheckEventQueue();
            
                CheckCombatantTimers();

                CheckEnemyTurnTimers();
            
                CheckIcons();
            
                ClearDeadEnemies();
            }
        }
        
        private void SetControl()
        {
            // Check for disabled controlling ally
            if (Commanding != null)
            {
                if (Commanding.CannotAct)
                {
                    ClearControl();
                }
            }
            
            // Check turn timers and enqueue if up
            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                if (IsReady(Allies[i]))
                {
                    if (Allies[i].Confusion)
                    {
                        Allies[i].RunAIConfu();
                    }
                    else if (Allies[i].Berserk)
                    {
                        Allies[i].RunAIBerserk();
                    }
                    else
                    {
                        _turnQueue.Enqueue(Allies[i]);
                    }
                }
            }
            
            // Check turn queue and set control if none set
            if (Commanding == null)
            {
                if (_turnQueue.Count > 0)
                {
                    Commanding = _turnQueue.Dequeue();
                    Screen.PushControl(Commanding.BattleMenu);
                }
            }
        }
        
        private void CheckEventQueue()
        {
            lock (EventQueue)
            {                
                // Dequeue next ability if none is in progress
                if (ActiveAbility == null)
                {
                    if (PriorityQueue.Count > 0)
                    {
                        ActiveAbility = PriorityQueue.Dequeue();
                    }
                    else if (EventQueue.Count > 0)
                    {
                        ActiveAbility = EventQueue.Dequeue();
                    }

                    if (ActiveAbility != null)
                    {
                        ActiveAbility.Begin();
                    }
                }

                // If the current ability is done, clear it
                if (ActiveAbility != null)
                {
                    bool isDone = ActiveAbility.RunIteration();

                    if (isDone)
                    {
#if DEBUG
                        Console.WriteLine("Event has completed:");
                        Console.WriteLine(ActiveAbility.ToString());
#endif
    //                if (ActiveAbility.Performer is Ally)
    //                {
    //                    LastPartyAction = (AbilityState)ActiveAbility.Clone();
    //                }

                        if (ActiveAbility == _victoryEvent)
                        {
                            Seven.Instance.EndBattle();
                        }

                        if (ActiveAbility == _lossEvent)
                        {
                            Seven.Instance.LoseGame();
                        }

                        ActiveAbility = null;
                    }
                }
            }
        }
        
        private void CheckCombatantTimers()
        {
            foreach (Combatant a in Allies)
            {
                if (a != null)
                {
                    a.CheckTimers();
                }
            }
            
            foreach (Combatant e in EnemyList)
            {
                e.CheckTimers();
            }
        }
        
        private void CheckEnemyTurnTimers()
        {
            if (!Loss)
            {
                foreach (Enemy e in EnemyList)
                {
                    if (e.TurnTimer.IsUp && !e.WaitingToResolve)
                    {
                        e.WaitingToResolve = true;

                        e.TakeTurn();
                    }
                }
            }
        }
        
        private void CheckIcons()
        {
            if (_battleIcons.Count > 0)
            {
                BattleIcon icon = _battleIcons.Peek();

                if (icon.IsDone)
                {
                    _battleIcons.Dequeue();
                }

                // don't loop over all of them...just get it the next iteration
            }
        }
        
        private void ClearDeadEnemies()
        {
            IEnumerable<Enemy> liveEnemies = EnemyList.Where(e => !e.Death);

            foreach (Enemy e in EnemyList.Except(liveEnemies))
            {
                Exp += e.Exp;
                AP += e.AP;
                Gil += e.Gil;
                IInventoryItem temp = e.WinItem();
                if (temp != null)
                {
                    Items.Add(temp);
                }
                e.Dispose();
            }

            EnemyList = liveEnemies.ToList();
        }
        
        private bool CheckForVictory()
        {
            return EnemyList.Count == 0;
        }

        private bool CheckForLoss()
        {
            bool loss = true;

            for (int i = 0; i < Allies.Length; i++)
            {
                if (Allies[i] != null && !Allies[i].IsDead)
                {
                    loss = false;
                }
            }

            return loss;
        }
        
        
        
        
        
        public void EnqueueAction(BattleEvent a, bool priority = false)
        {
            lock (EventQueue)
            {
                if (priority)
                {
                    PriorityQueue.Enqueue(a);
                }
                else
                {
                    EventQueue.Enqueue(a);
                }
#if DEBUG
                Console.WriteLine("Added event to queue.");
                Console.WriteLine("Priority events (last-in first):");
                
                foreach (BattleEvent s in PriorityQueue)
                {
                    Console.WriteLine(s.ToString());
                }

                Console.WriteLine("Events (last-in first):");
                
                foreach (BattleEvent s in EventQueue)
                {
                    Console.WriteLine(s.ToString());
                }
#endif
            }
        }

        public void AddDamageIcon(int amount, Combatant receiver, bool mp = false)
        {
            DamageIcon icon = new DamageIcon(this, amount, receiver, mp);

            _battleIcons.Enqueue(icon);
        }
        
        public void AddMissIcon(Combatant receiver)
        {
            MissIcon icon = new MissIcon(this, receiver);
            
            _battleIcons.Enqueue(icon);
        }
        
        public void AddRecoveryIcon(Combatant receiver)
        {
            RecoveryIcon icon = new RecoveryIcon(this, receiver);
            
            _battleIcons.Enqueue(icon);
        }
        
        
        
        
        #region Member Methods
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("Status:");

            if (Allies[0] != null)
            {
                sb.AppendLine(Allies[0].ToString());
            }
            if (Allies[1] != null)
            {
                sb.AppendLine(Allies[1].ToString());
            }
            if (Allies[2] != null)
            {
                sb.AppendLine(Allies[2].ToString());
            }

            foreach (Enemy e in EnemyList)
            {
                sb.AppendLine(" Enemy " + e.ToString());
            }

            return sb.ToString();
        }
        
        #endregion Member
        
        
        
        
        
        #region Override Methods
        
        public override void Draw(Gdk.Drawable d, int width, int height)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            Screen.Draw(d, !_holdingSquare);

            foreach (Combatant a in Allies)
            {
                if (a != null)
                {
                    a.Draw(d, g, width, height);
                }
            }

            foreach (Combatant e in EnemyList)
            {
                e.Draw(d, g, width, height);
            }
            
            if (Commanding != null)
            {
                Shapes.RenderInvertedTriangle(g, Colors.YELLOW, Commanding.X, Commanding.Y - 20, 20);
            }

            foreach (BattleIcon icon in _battleIcons)
            {
                icon.Draw(d);
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }


        private class PauseState
        {

            public bool WasPartyClockPaused { get; set; }
        }
        
        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
        {
            if (k == Key.Start)
            {
                if (Paused == null)
                {
                    Paused = new PauseState
                    {
                        WasPartyClockPaused = Seven.Party.Clock.Pause()
                    };
                    
                    TimeFactory.PauseAllClocks();
                }
                else if (Paused != null)
                {
                    TimeFactory.UnpauseAllClocks();
                    
                    if (Paused.WasPartyClockPaused)
                    {
                        Seven.Party.Clock.Unpause();
                    }
                    
                    Paused = null;
                }
            }

            if (Paused == null)
            {
                switch (k)
                {
                    case Key.Select:
                        Screen.InfoBar.Visible = !Screen.InfoBar.Visible;
                        break;

                    case Key.Square:
                        _holdingSquare = true;
                        break;

                    case Key.Triangle:
                        if (Commanding != null)
                        {
                            _turnQueue.Enqueue(Commanding);
                            ClearControl();
                        }
                        break;

                    default:
                        break;
                }
            }

            if (Commanding != null && !Victory)
            {
                Screen.Control.ControlHandle(k);
            }
        }
        [GLib.ConnectBefore()]
        public override void KeyReleaseHandle(Key k)
        {
            switch (k)
            {
                case Key.Square:
                    _holdingSquare = false;
                    break;

                case Key.Start:
                    break;

                default:
                    break;
            }
        }
        
        #endregion Override
        
        
        
        
        
        
        
        public void ClearControl()
        {
            Commanding = null;
            Screen.ClearControl();
        }
        
        public void ActionHook()
        {
            Commanding.WaitingToResolve = true;
            ClearControl();
        }

        public void ActionAbort()
        {
            Screen.PopControl();
        }
        
        
        
        protected override void InternalDispose()
        {            
            foreach (Ally a in Allies)
            {
                if (a != null)
                {
                    a.Dispose();
                }
            }

            foreach (Enemy e in EnemyList)
            {
                e.Dispose();
            }
        }


        
        
        
        #region Properties
        
        public Ally Commanding { get; private set; }
        public Ally[] Allies  { get; private set; }
        public List<Enemy> EnemyList { get; private set; }
        public Clock BattleClock  { get; private set; }
        public BattleEvent ActiveAbility { get; private set; }
        public BattleEvent LastPartyAbility { get; private set; }
        public Queue<BattleEvent> PendingAbilities { get; private set; }
        public BattleScreen Screen { get; private set; }
        
        private bool Victory { get; set; }
        private bool Loss { get; set; }
        public int Exp  { get; private set; }
        public int AP { get; private set; }
        public int Gil  { get; private set; }
        public List<IInventoryItem> Items { get; private set; }

        public Random Random { get; private set; }

        public Queue<BattleEvent> EventQueue { get; private set; }

        public Queue<BattleEvent> PriorityQueue { get; private set; }

        private PauseState Paused { get; set; }
        
        #endregion Properties
    }
}


