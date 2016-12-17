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
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Battle.Event;
using Atmosphere.Reverence.Seven.Screen.BattleState;
using NLua;

namespace Atmosphere.Reverence.Seven.State
{    
    internal class BattleState : State
    {
        private enum EventPriority
        {
            ClearEnemies = 0,
            System,
            Limit,
            Counter,
            Normal,
        }


        #region Member Data
        
        private Queue<BattleIcon> _battleIcons;

        private Queue<Ally> _turnQueue;
        
        private bool _holdingSquare;

        private Formation _formation;
        
        private BattleEvent _victoryEvent;
        private BattleEvent _lossEvent;
                
        #endregion




        
        private class PriorityQueue
        {
            private static readonly int QUEUE_COUNT = Enum.GetValues(typeof(EventPriority)).Length;
            
            private Queue<BattleEvent>[] _queues;
            
            public PriorityQueue()
            {
                _queues = new Queue<BattleEvent>[QUEUE_COUNT];
                
                for (int i = 0; i < QUEUE_COUNT; i++)
                {
                    _queues[i] = new Queue<BattleEvent>();
                }
            }
            
            public void Enqueue(BattleEvent @event, EventPriority priority)
            {
                lock (_queues)
                {
                    _queues[(int)priority].Enqueue(@event);
                }
            }
            
            public BattleEvent Dequeue()
            {
                BattleEvent @event = null;
                
                lock (_queues)
                {
                    for (int i = 0; i < QUEUE_COUNT; i++)
                    {
                        if (_queues[i].Count > 0)
                        {
                            @event = _queues[i].Dequeue();
                            break;
                        }
                    }
                }
                
                return @event;
            }

            public void PruneEvents(Predicate<CombatantActionEvent> shouldDrop, string msg)
            {
                lock (_queues)
                {
                    // start at 1 since we can skip the System queue
                    
                    for (int i = 1; i < QUEUE_COUNT; i++)
                    {
                        Queue<BattleEvent> temp = new Queue<BattleEvent>();
                        
                        while (_queues[i].Count > 0)
                        {
                            BattleEvent e = _queues[i].Dequeue();
                            
                            CombatantActionEvent action = e as CombatantActionEvent;

                            // If this isn't a combatant action
                            //    OR
                            // If our predicate holds
                            //   ...
                            // Put that action right back into the queue.
                            if (action == null || !shouldDrop(action))
                            {
                                temp.Enqueue(e);
                            }
                            else
                            {
#if DEBUG
                                Console.WriteLine("Dropped event ({0}): {1}", msg, e);
#endif
                            }
                        }
                        
                        _queues[i] = temp;
                    }
                }
            }

            public List<CombatantActionEvent> GetEventsFromSource(Combatant source)
            {
                List<CombatantActionEvent> events = new List<CombatantActionEvent>();

                lock (_queues)
                {
                    // start at 1 since we can skip the System queue
                    
                    for (int i = 1; i < QUEUE_COUNT; i++)
                    {                      
                        events.AddRange(_queues[i].Where(e => e is CombatantActionEvent && ((CombatantActionEvent)e).Source == source).Cast<CombatantActionEvent>());
                    }
                }

                return events;
            }

            
            public override string ToString()
            {     
                StringBuilder rep = new StringBuilder();
                
                Array values = Enum.GetValues(typeof(EventPriority));
                
                for (int i = 0; i < values.Length; i++)
                {
                    rep.AppendLine("Events (last-in first) of priority " + values.GetValue(i));
                    
                    foreach (BattleEvent s in _queues[i])
                    {
                        rep.AppendLine(s.ToString());
                    }
                }
                
                return rep.ToString();
            }
            
            public int Count 
            {
                get
                {
                    lock (_queues)
                    {
                        int count = 0;
                        
                        for (int i = 0; i < _queues.Length; i++)
                        {
                            count += _queues[i].Count;
                        }
                        
                        return count;
                    }
                }
            }
        }

        private class RemoveEnemyEvent : BattleEvent
        {
            public const int DURATION = 500;
            public const int PAUSE = DURATION / 2;

            public RemoveEnemyEvent(BattleState battle, List<Enemy> enemies)
                : base()
            {
                Enemies = enemies;
                Battle = battle;
                Text = "(clear dead)";
            }
            
            protected override void RunIteration(long elapsed, bool isLast)
            {
                if (DURATION > PAUSE && !HasRemoved)
                {
                    foreach (Enemy enemy in Enemies)
                    {
                        int index = Battle.EnemyList.IndexOf(enemy);

                        Battle.EnemyList.RemoveAt(index);                
                        Battle.DeadEnemies.Add(enemy);
                        Battle.EventQueue.PruneEvents(x => Enemies.Contains(x.Source), "source removed from battle");
                    }

                    HasRemoved = true;
                }                
            }
            
            protected override string GetStatus(long elapsed)
            {
                return Text;
            }
            
            public override string ToString()
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(" Removing the following enemies:");
                foreach (Enemy enemy in Enemies)
                {
                    sb.AppendLine("    " + enemy.Name);
                }
                return sb.ToString();
            }


            protected override int Duration { get { return DURATION; } }
            private bool HasRemoved { get; set; }
            private string Text { get; set; }
            private List<Enemy> Enemies { get; set; }
            private BattleState Battle { get; set; }
        }

        private class EndOfBattleEvent : BattleEvent
        {
            public const int DURATION = 4000;

            
            public EndOfBattleEvent(string text)
                : base()
            {
                Text = text;
            }

            protected override void RunIteration(long elapsed, bool isLast) { }
            
            protected override string GetStatus(long elapsed)
            {
                return Text;
            }
            
            public override string ToString()
            {
                return " Battle is ending; status = " + Text;
            }
            
            protected override int Duration { get { return DURATION; } }

            private string Text { get; set; }
        }


        private BattleState(Seven seven)
            : base(seven)
        {
        }
        
        public BattleState(Seven seven, string formationId)
            : this(seven)
        {            
            _turnQueue = new Queue<Ally>();
            _battleIcons = new Queue<BattleIcon>();

            EventQueue = new PriorityQueue();
            
            Random = new Random();

            Lua = Seven.GetLua();            
            Lua.DoString(Resource.GetTextFromResource("lua.scripts.battle", typeof(Seven).Assembly));
            Lua[typeof(BattleState).Name] = this;

            DeadEnemies = new List<Enemy>();

            CombatantClocks = new ClockCollection();
            
            _formation = seven.Data.GetFormation(formationId);
        }
        
        protected override void InternalInit()
        {
            ScreenState state = new ScreenState
            {
                Width = Seven.Configuration.WindowWidth,
                Height = Seven.Configuration.WindowHeight
            };
                        
            _victoryEvent = new EndOfBattleEvent("Victory!");
            _lossEvent = new EndOfBattleEvent("Annihilated!");

            Screen = new BattleScreen(this, state);

            SpeedValue = (32768 / (120 + (Seven.Party.BattleSpeed * 15 / 8)));

            GlobalClock = new BattleClock(SpeedValue);
                       
            Allies = new Ally[Party.PARTY_SIZE];

            int[] e = _formation.GetAllyTurnTimersElapsed(Party);

            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                if (Party[i] != null)
                {
                    Allies[i] = new Ally(this, i, e[i]);
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
                    CombatantClocks.Add(ally.TurnTimer);
                    CombatantClocks.Add(ally.V_Timer);
                    CombatantClocks.Add(ally.C_Timer);
                    ally.TurnTimer.Unpause();
                }
            }
            
            foreach (Enemy enemy in EnemyList)
            {
                CombatantClocks.Add(enemy.TurnTimer);
                CombatantClocks.Add(enemy.V_Timer);
                CombatantClocks.Add(enemy.C_Timer);
                enemy.EnterBattle();
            }

            foreach (Enemy enemy in EnemyList)
            {
                enemy.TurnTimer.Unpause();
            }
        }





        
        public override void RunIteration()
        {
            if (!Paused)
            {
                if (!Loss && CheckForLoss())
                {
                    Loss = true;

                    EnqueueAction(_lossEvent, EventPriority.System);
                }
                
                if (!Loss && !Victory && CheckForVictory())
                {
                    Victory = true;
                    
                    EnqueueAction(_victoryEvent, EventPriority.System);
                }

                SetControl();
            
                CheckEventQueue();
            
                CheckCombatantTimers();

                CheckEnemyTurnTimers();
            
                CheckIcons();
            }
        }
        
        private void SetControl()
        {
            // Check for disabled controlling ally
            if (Commanding != null)
            {
                if (Commanding.CannotAct || Commanding.Confusion)
                {
                    ClearControl();
                }
            }
            
            // Check turn timers and enqueue if up
            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                Ally ally = Allies[i];

                bool isReady = ally != null     // they're not null
                    && ally.TurnTimer.IsUp      // they're ready
                    && !ally.WaitingToResolve   // they're not already acting
                    && !ally.CannotAct;         // they can actually act

                if (isReady)
                {
                    if (ally.Confusion)
                    {
                        ally.RunAIConfu();
                    }
                    else if (ally.Berserk)
                    {
                        ally.RunAIBerserk();
                    }
                    else
                    {
                        if (!_turnQueue.Contains(ally) && ally != Commanding)
                        {
                            _turnQueue.Enqueue(ally);
                        }
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
                    if (EventQueue.Count > 0)
                    {
                        BattleEvent activeEvent = EventQueue.Dequeue();

                        if (activeEvent != null)
                        {
                            activeEvent.Begin();

                            ActiveAbility = activeEvent;
#if DEBUG
                            Console.WriteLine("Event has begun:" + ActiveAbility.ToString());
#endif
                        }
                    }
                }

                // If the current ability is done, clear it
                if (ActiveAbility != null)
                {
                    bool isDone = ActiveAbility.RunIteration();

                    if (isDone)
                    {
#if DEBUG
                        Console.WriteLine("Event has completed:" + ActiveAbility.ToString());
#endif
    //                if (ActiveAbility.Performer is Ally)
    //                {
    //                    LastPartyAction = (AbilityState)ActiveAbility.Clone();
    //                }

                        if (ActiveAbility == _victoryEvent)
                        {
                            int exp = 0;
                            int ap = 0;
                            int gil = 0;
                            List<IInventoryItem> items = new List<IInventoryItem>();

                            foreach (Enemy enemy in DeadEnemies)
                            {
                                exp += enemy.Exp;
                                ap += enemy.AP;
                                gil += enemy.Gil;
                                IInventoryItem item = enemy.WinItem();
                                if (item != null)
                                {
                                    items.Add(item);
                                }
                            }

                            Seven.EndBattle(exp, ap, gil, items);
                        }

                        if (ActiveAbility == _lossEvent)
                        {
                            Seven.LoseGame();
                        }

                        ActiveAbility = null;

                        CheckForDeadEnemies();

                        // We need to prune all events from a source who cannot act.
                        // We must do the whole queue at once. We can't just look at
                        //   the head of the queue.
                        // Example. You attack with a character -- an event goes in the 
                        //   queue, possibly behind many other events. Then that ally is 
                        //   paralyzed. Then that ally is attacked, and it reaches its
                        //   limit break. Then that character is cured by, say, one of 
                        //   Aeris's limit breaks (all of these things went in the queue
                        //   before said ally's attack). Then you use that ally's limit 
                        //   break immediately upon regaining control of it.
                        // In theory, the attack event could still happen AFTER the limit
                        //   break. It needs to come out of the queue NOW if the ally
                        //   cannot act.
                        EventQueue.PruneEvents(x => x.Source.CannotAct, "source cannot act");
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
        
        private void CheckForDeadEnemies()
        {
            List<Enemy> enemiesToClear = new List<Enemy>();

            for (int i = 0; i < EnemyList.Count(); i++)
            {
                Enemy enemy = EnemyList[i];

                if (enemy.Death && !enemy.FlaggedForRemoval)
                {
                    enemy.FlaggedForRemoval = true;
                    enemiesToClear.Add(enemy);
                }
            }

            if (enemiesToClear.Count > 0)
            {
                RemoveEnemyEvent remove = new RemoveEnemyEvent(this, enemiesToClear);
                
                EnqueueAction(remove, EventPriority.ClearEnemies);
            }
        }

        
        private bool CheckForVictory()
        {
            return EnemyList.Count == 0 || EnemyList.All(e => e.IsDead);
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
        
        
        
        public void EnqueueAction(BattleEvent e)
        {
            EnqueueAction(e, EventPriority.Normal);
        }
        
        public void EnqueueCounterAction(BattleEvent e)
        {
            EnqueueAction(e, EventPriority.Counter);
        }
        
        public void EnqueueLimit(BattleEvent e)
        {
            EnqueueAction(e, EventPriority.Limit);
        }
        
        private void EnqueueAction(BattleEvent a, EventPriority priority)
        {
            lock (EventQueue)
            {
                EventQueue.Enqueue(a, priority);
#if DEBUG
                Console.WriteLine();
                Console.WriteLine(" An event has been added to the queue!");
                Console.WriteLine();
                Console.WriteLine(EventQueue.ToString());
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
        
        public void AddDeathIcon(Combatant receiver)
        {
            DeathIcon icon = new DeathIcon(this, receiver);
            
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
        
        public override void Draw(Gdk.Drawable d, int width, int height, bool screenChanged)
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
                if (!Paused)
                {
                    Paused = true;
                    
                    if (ActiveAbility != null)
                    {
                        ActiveAbility.ActionTimer.Pause();
                    }

                    foreach (BattleIcon icon in _battleIcons)
                    {
                        icon.AnimationTimer.Pause();
                    }

                    Party.Clock.Pause();

                    CombatantClocks.PauseAllClocks();
                }
                else
                {
                    CombatantClocks.UnpauseAllClocks();

                    Party.Clock.Unpause();
                    
                    foreach (BattleIcon icon in _battleIcons)
                    {
                        icon.AnimationTimer.Unpause();
                    }
                    
                    if (ActiveAbility != null)
                    {
                        ActiveAbility.ActionTimer.Unpause();
                    }

                    Paused = false;
                }
            }

            if (!Paused)
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


        public void PruneEventsFromSource(Combatant source, string msg)
        {
            EventQueue.PruneEvents(x => x.Source == source, msg);
        }

        public List<CombatantActionEvent> GetEventsFromSource(Combatant source)
        {
            return EventQueue.GetEventsFromSource(source);
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
        protected List<Enemy> DeadEnemies { get; set; }
        public BattleClock GlobalClock  { get; private set; }
        public BattleEvent ActiveAbility { get; private set; }
        public BattleEvent LastPartyAbility { get; private set; }
        public Queue<BattleEvent> PendingAbilities { get; private set; }
        public BattleScreen Screen { get; private set; }
        
        public int SpeedValue { get; private set; }
        
        public bool Paused { get; private set; }
        
        private bool Victory { get; set; }
        private bool Loss { get; set; }

        public Lua Lua { get; private set; }

        public Random Random { get; private set; }

        private PriorityQueue EventQueue { get; set; }

        private ClockCollection CombatantClocks { get; set; }
        
        #endregion Properties
    }
}


