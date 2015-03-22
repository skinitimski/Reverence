using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Gtk;
using Gdk;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using GameState = Atmosphere.Reverence.State;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.State
{    
    internal class BattleState : GameState
    {
        // Singleton
        
        
        
        #region Member Data


        private Queue<Ally> _turnQueue;
        
        private bool _holdingSquare;

        
        private Mutex _abilityMutex;

        
#endregion
        
        
        
        public BattleState()
        {
            Screen = new BattleScreen();

            _turnQueue = new Queue<Ally>();
            
            AbilityQueue = new Queue<BattleEvent>();
            
            _abilityMutex = new Mutex();
            Random = new Random();
            
            Items = new List<IInventoryItem>();
        }
        
        protected override void InternalInit()
        {
            BattleClock = new Clock(Seven.Party.BattleSpeed);

            
            
            Allies = new Ally[3];
            
            if (Seven.Party[0] != null)
            {
                Allies[0] = new Ally(Seven.Party[0], 550, 100, 2000);
            }
            if (Seven.Party[1] != null)
            {
                Allies[1] = new Ally(Seven.Party[1], 580, 200, 2500);
            }
            if (Seven.Party[2] != null)
            {
                Allies[2] = new Ally(Seven.Party[2], 610, 300, 2300);
            }
            
            if (Allies[0] == null && Allies[1] == null && Allies[2] == null)
            {
                throw new GameDataException("Must have at least one ally in battle.");
            }
            
            
            int numberOfEnemies = 1;//Game.Random.Next(1, 3);
            //EnemyList = new List<Enemy>(numberOfEnemies);
            //for (int i = 0; i < numberOfEnemies; i++)
            //    EnemyList.Add(Enemy.GetRandomEnemy(Game.Random.Next(100, 250), Game.Random.Next(100, 300)));
            
            EnemyList = new List<Enemy>();
            EnemyList.Add(Enemy.CreateEnemy("mothslasher", 100, 100));
           
            foreach (Enemy e in EnemyList)
            {
                e.AIThread.Start();
            }
        }
        
        private bool IsReady(Ally a)
        {
            bool ready = true;
            ready = ready && a != null;                         // they're not null
            ready = ready && a.TurnTimer.IsUp;                  // they're ready
            //ready = ready && !AbilityQueue.Contains(a.Ability); // they're not in the ability queue
            //ready = ready && ActiveAbility != a.Ability;        // they're not acting now
            ready = ready && !a.CannotAct;                      // they're able to act
            ready = ready && !_turnQueue.Contains(a);           // they're not already in the turn queue
            ready = ready && Commanding != a;                   // they're not in control

            return ready;
        }
        
        public override void RunIteration()
        {
            SetControl();
            
            CheckAbilityQueue();
            
            CheckCombatantTimers();
            
            ClearDeadEnemies();
            
            if (CheckForVictory())
            {
                Seven.Instance.EndBattle();
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
            for (int i = 0; i < 3; i++)
            {
                if (IsReady(Allies[i]))
                {
                    _turnQueue.Enqueue(Allies[i]);
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
        
        private void CheckAbilityQueue()
        {
            _abilityMutex.WaitOne();
            
            // If the current ability is done, clear it
            if (AbilityThread != null && !AbilityThread.IsAlive)
            {
#if DEBUG
                Console.WriteLine("Killing action:");
                Console.WriteLine(ActiveAbility.ToString());
#endif
//                if (ActiveAbility.Performer is Ally)
//                {
//                    LastPartyAction = (AbilityState)ActiveAbility.Clone();
//                }

                ActiveAbility = null;
                AbilityThread = null;
            }
            // Dequeue next ability if none is in progress
            if (AbilityThread == null && AbilityQueue.Count > 0)
            {
                ActiveAbility = AbilityQueue.Dequeue();
                AbilityThread = new Thread(new ThreadStart(ActiveAbility.DoAction));
                AbilityThread.Start();
            }
            
            _abilityMutex.ReleaseMutex();
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
        
        
        
        
        
        public void EnqueueAction(BattleEvent a)
        {
            _abilityMutex.WaitOne();

            AbilityQueue.Enqueue(a);
#if DEBUG
            Console.WriteLine("Added ability to Queue. Current queue state:");
            foreach (BattleEvent s in AbilityQueue)
            {
                Console.WriteLine(s.ToString());
            }
#endif
            _abilityMutex.ReleaseMutex();
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
            
            try
            {
                Screen.Draw(d, !_holdingSquare);
            }
            catch (Exception e)
            {
            }

            foreach (Combatant a in Allies)
            {
                if (a != null)
                {
                    a.Draw(g);
                }
            }
            foreach (Combatant e in EnemyList)
            {
                e.Draw(g);
            }
            
            if (Commanding != null)
            {
                Shapes.RenderCursor(g, new Cairo.Color(.8, .8, 0), Commanding.X, Commanding.Y - 15);
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
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
            if (Commanding != null)
                Screen.Control.ControlHandle(k);
        }
        [GLib.ConnectBefore()]
        public override void KeyReleaseHandle(Key k)
        {
            switch (k)
            {
                case Key.Square:
                    _holdingSquare = false;
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
            //EnqueueAction(Commanding.Ability);
            ClearControl();
        }
        public void ActionAbort()
        {
            Screen.PopControl();
        }
        
        
        
        protected override void InternalDispose()
        {
            if (AbilityThread != null)
            {
                AbilityThread.Abort();
            }
            
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
        
        public int Exp  { get; private set; }
        public int AP { get; private set; }
        public int Gil  { get; private set; }
        public List<IInventoryItem> Items { get; private set; }

        public Thread AbilityThread { get; private set; }

        public Random Random { get; private set; }

        public Queue<BattleEvent> AbilityQueue { get; private set; }
        
        #endregion Properties
        
        
    }
}


