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


namespace Atmosphere.BattleSimulator
{

    public class BattleState : State
    {
        // Singleton



        #region Member Data

        private Ally[] _allies;
        private List<Enemy> _enemies;

        private Ally _control;
        private Queue<Ally> _turnQueue;

        private bool _holdingSquare;

        private ScaledClock _clock;

        private AbilityState _activeAbility;
        private AbilityState _lastPartyAction;
        private Queue<AbilityState> _abilityQueue;
        private Thread _abilityThread;

        private Mutex _abilityMutex;

        private int _gainedExp;
        private int _gainedAP;
        private int _gainedGil;
        private List<IItem> _gainedItems;
        
        #endregion



        public BattleState()
        {
            _clock = new ScaledClock(Globals.BattleSpeed);


            _allies = new Ally[3];

            if (Globals.Party[0] != null) _allies[0] = new Ally(Globals.Party[0], 550, 100, 2000);
            if (Globals.Party[1] != null) _allies[1] = new Ally(Globals.Party[1], 580, 200, 2500);
            if (Globals.Party[2] != null) _allies[2] = new Ally(Globals.Party[2], 610, 300, 2300);

            if (_allies[0] == null && _allies[1] == null && _allies[2] == null)
                throw new GamedataException("Must have at least one ally in battle.");


            int numberOfEnemies = 1;//Game.Random.Next(1, 3);
            //_enemies = new List<Enemy>(numberOfEnemies);
            //for (int i = 0; i < numberOfEnemies; i++)
            //    _enemies.Add(Enemy.GetRandomEnemy(Game.Random.Next(100, 250), Game.Random.Next(100, 300)));

            _enemies = new List<Enemy>();
            _enemies.Add(new Enemy("mothslasher"));

            _turnQueue = new Queue<Ally>();

            _abilityQueue = new Queue<AbilityState>();

            _abilityMutex = new Mutex();

            _gainedItems = new List<IItem>();
        }

        protected override void InternalInit()
        {
            foreach (Enemy e in _enemies)
                e.AIThread.Start();

            _control = null;
            Screen.Init();
        }

        private bool IsReady(Ally a)
        {
            bool ready = true;
            ready = ready && a != null; // they're not null
            ready = ready && a.TurnTimer.IsUp; // they're ready
            ready = ready && !_abilityQueue.Contains(a.Ability); // they're not in the ability queue
            ready = ready && _activeAbility != a.Ability; // they're not acting now
            ready = ready && !a.CannotAct; // they're able to act
            ready = ready && !_turnQueue.Contains(a); // they're not already in the turn queue
            ready = ready && _control != a; // they're not in control
            return ready;
        }

        public override void RunIteration()
        {
            SetControl();

            CheckAbilityQueue();

            CheckCombatantTimers();
            
            ClearDeadEnemies();
            
            if (CheckForVictory())
                Game.GoToPostBattle();
        }

        private void SetControl()
        {
            // Check for disabled controlling ally
            if (_control != null)
                if (_control.CannotAct)
                    ClearControl();

            // Check turn timers and enqueue if up
            for (int i = 0; i < 3; i++)
                if (IsReady(_allies[i]))
                    _turnQueue.Enqueue(_allies[i]);

            // Check turn queue and set control if none set
            if (_control == null)
                if (_turnQueue.Count > 0)
                {
                    _control = _turnQueue.Dequeue();
                    Screen.PushControl(_control.BattleMenu);
                }
        }

        private void CheckAbilityQueue()
        {
            _abilityMutex.WaitOne();

            // If the current ability is done, clear it
            if (_abilityThread != null && !_abilityThread.IsAlive)
            {
#if DEBUG
                Console.WriteLine("Killing action:");
                Console.WriteLine(_activeAbility.ToString());
#endif
                if (_activeAbility.Performer is Ally)
                    _lastPartyAction = (AbilityState)_activeAbility.Clone();
                _activeAbility.Performer.Ability.Reset();
                _activeAbility = null;
                _abilityThread = null;
            }
            // Dequeue next ability if none is in progress
            if (_abilityThread == null && _abilityQueue.Count > 0)
            {
                _activeAbility = _abilityQueue.Dequeue();
                _abilityThread = new Thread(new ThreadStart(_activeAbility.DoAction));
                _abilityThread.Start();
            }

            _abilityMutex.ReleaseMutex();
        }
        
        private void CheckCombatantTimers()
        {
            foreach (ICombatant a in _allies)
                if (a != null)
                    a.CheckTimers();
            foreach (ICombatant e in _enemies)
                e.CheckTimers();
        }

        private void ClearDeadEnemies()
        {
            List<Enemy> liveEnemies = Util.Filter<Enemy>(_enemies, delegate(Enemy e) { return !e.Death; });
            foreach (Enemy e in _enemies.Except(liveEnemies))
            {
                _gainedExp += e.Exp;
                _gainedAP += e.AP;
                _gainedGil += e.Gil;
                IItem temp = e.WinItem();
                if (temp != null)
                    _gainedItems.Add(temp);
                e.Dispose();
            }
            _enemies = liveEnemies;
        }

        private bool CheckForVictory()
        {
            if (_enemies.Count == 0)
                return true;
            else return false;
        }





        public void EnqueueAction(AbilityState a)
        {
            _abilityMutex.WaitOne();
            _abilityQueue.Enqueue(a);
#if DEBUG
            Console.WriteLine("Added ability to Queue. Current queue state:");
            foreach (AbilityState s in _abilityQueue)
                Console.WriteLine(s.ToString());
#endif
            _abilityMutex.ReleaseMutex();
        }




        #region Member Methods

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Status:");
            if (_allies[0] != null) sb.AppendLine(_allies[0].ToString());
            if (_allies[1] != null) sb.AppendLine(_allies[1].ToString());
            if (_allies[2] != null) sb.AppendLine(_allies[2].ToString());
            foreach (Enemy e in _enemies)
                sb.AppendLine(" Enemy " + e.ToString());
            return sb.ToString();
        }

        #endregion Member





        #region Override Methods

        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);


            Screen.Draw(d, !_holdingSquare);
            
            foreach (ICombatant a in _allies)
                if (a != null)
                    a.Draw(g);
            foreach (ICombatant e in _enemies)
                e.Draw(g);

            if (_control != null)
                Graphics.RenderCursor(g, new Cairo.Color(.8, .8, 0), _control.X, _control.Y - 15);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
        {
            switch (k)
            {
                case Key.Select:
                    InfoBar.Instance.Visible = !InfoBar.Instance.Visible;
                    break;
                case Key.Square:
                    _holdingSquare = true;
                    break;
                case Key.Triangle:
                    if (_control != null)
                    {
                        _turnQueue.Enqueue(_control);
                        ClearControl();
                    }
                    break;
                default:
                    break;
            }
            if (_control != null)
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
            _control = null;
            Screen.ClearControl();
        }

        public void ActionHook()
        {
            EnqueueAction(Commanding.Ability);
            ClearControl();
        }
        public void ActionAbort()
        {
            Screen.PopControl();
        }



        protected override void InternalDispose()
        {
            if (_abilityThread != null)
                _abilityThread.Abort();

            foreach (Ally a in _allies)
                if (a != null)
                    a.Dispose();
            foreach (Enemy e in _enemies)
                e.Dispose();
        }



        #region Properties

        public Ally Commanding { get { return _control; } }
        public Ally[] Allies { get { return _allies; } }
        public List<Enemy> EnemyList { get { return _enemies; } }
        public Clock BattleClock { get { return _clock; } }
        public AbilityState ActiveAbility { get { return _activeAbility; } }
        public AbilityState LastPartyAbility { get { return _lastPartyAction; } }
        public Queue<AbilityState> PendingAbilities { get { return _abilityQueue; } }
        public BattleScreen Screen { get { return BattleScreen.Instance; } }

        public int Exp { get { return _gainedExp; } }
        public int AP { get { return _gainedAP; } }
        public int Gil { get { return _gainedGil; } }
        public List<IItem> Items { get { return _gainedItems; } }

        #endregion Properties


    }
}
