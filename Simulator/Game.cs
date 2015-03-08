using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using LuaInterface;
using Gtk;

namespace Atmosphere.BattleSimulator
{
    public class Game
    {
        public static Lua Lua;

        private static Gtk.Window _window;
        private static State _state;
        private static Random _random;
        private static Clock _clock;

        private static bool _quit = false;

        public static BattleState Battle;
        public static PostBattleState PostBattle;
        public static readonly MenuState MainMenu;

        static Game()
        {
            MainMenu = new MenuState();
        }




        #region Gtk Methods

        static void OnExposed(object o, ExposeEventArgs args)
        {
            if (Graphics.DrawingThread.IsAlive)
                Graphics.DrawingThread.Join();

            Gdk.Threads.Enter();

            Gtk.DrawingArea area = (DrawingArea)o;
            Gdk.Window win = area.GdkWindow;
            Gdk.GC gc = new Gdk.GC(win);
            //Cairo.Context g = CairoHelper.Create(win);

            win.DrawDrawable(gc, Graphics.Pixmap, 0, 0, 0, 0, Globals.WIDTH, Globals.HEIGHT);

            //((IDisposable)g.Target).Dispose();
            //((IDisposable)g).Dispose();

            Gdk.Threads.Leave();
        }
        static void OnWinDelete(object o, DeleteEventArgs args)
        {
            Gdk.Threads.Enter();
            Application.Quit();
            Gdk.Threads.Leave();
            _quit = true;
        }


        [GLib.ConnectBefore()]
        static void OnKeyPress(object o, Gtk.KeyPressEventArgs args)
        {
#if DEBUG
            Console.WriteLine("Pressed  {0}", args.Event.Key.ToString());
#endif
            #region Switch
            switch (args.Event.Key)
            {
                case Gdk.Key.q:
                case Gdk.Key.Q:
                    Game.Quit();
                    break;
                case Gdk.Key.s:
                case Gdk.Key.S:
                    _state.KeyPressHandle(Key.Square);
                    break;
                case Gdk.Key.x:
                case Gdk.Key.X:
                    _state.KeyPressHandle(Key.X);
                    break;
                case Gdk.Key.t:
                case Gdk.Key.T:
                    _state.KeyPressHandle(Key.Triangle);
                    break;
                case Gdk.Key.c:
                case Gdk.Key.C:
                    _state.KeyPressHandle(Key.Circle);
                    break;
                case Gdk.Key.Up:
                    _state.KeyPressHandle(Key.Up);
                    break;
                case Gdk.Key.Down:
                    _state.KeyPressHandle(Key.Down);
                    break;
                case Gdk.Key.Right:
                    _state.KeyPressHandle(Key.Right);
                    break;
                case Gdk.Key.Left:
                    _state.KeyPressHandle(Key.Left);
                    break;
                case Gdk.Key.Key_1:
                    _state.KeyPressHandle(Key.L1);
                    break;
                case Gdk.Key.Key_2:
                    _state.KeyPressHandle(Key.L2);
                    break;
                case Gdk.Key.Key_3:
                    _state.KeyPressHandle(Key.R1);
                    break;
                case Gdk.Key.Key_4:
                    _state.KeyPressHandle(Key.R2);
                    break;
                case Gdk.Key.comma:
                    _state.KeyPressHandle(Key.Select);
                    break;
                case Gdk.Key.period:
                    _state.KeyPressHandle(Key.Start);
                    break;
                default:
                    break;
            }
            #endregion Switch

        }

        [GLib.ConnectBefore()]
        static void OnKeyRelease(object o, Gtk.KeyReleaseEventArgs args)
        {
#if DEBUG
            Console.WriteLine("Released {0}", args.Event.Key.ToString());
#endif
            #region Switch
            switch (args.Event.Key)
            {
                case Gdk.Key.q:
                case Gdk.Key.Q:
                    Game.Quit();
                    break;
                case Gdk.Key.s:
                case Gdk.Key.S:
                    _state.KeyReleaseHandle(Key.Square);
                    break;
                case Gdk.Key.x:
                case Gdk.Key.X:
                    _state.KeyReleaseHandle(Key.X);
                    break;
                case Gdk.Key.t:
                case Gdk.Key.T:
                    _state.KeyReleaseHandle(Key.Triangle);
                    break;
                case Gdk.Key.c:
                case Gdk.Key.C:
                    _state.KeyReleaseHandle(Key.Circle);
                    break;
                case Gdk.Key.Up:
                    _state.KeyReleaseHandle(Key.Up);
                    break;
                case Gdk.Key.Down:
                    _state.KeyReleaseHandle(Key.Down);
                    break;
                case Gdk.Key.Right:
                    _state.KeyReleaseHandle(Key.Right);
                    break;
                case Gdk.Key.Left:
                    _state.KeyReleaseHandle(Key.Left);
                    break;
                case Gdk.Key.Key_1:
                    _state.KeyReleaseHandle(Key.L1);
                    break;
                case Gdk.Key.Key_2:
                    _state.KeyReleaseHandle(Key.L2);
                    break;
                case Gdk.Key.Key_3:
                    _state.KeyReleaseHandle(Key.R1);
                    break;
                case Gdk.Key.Key_4:
                    _state.KeyReleaseHandle(Key.R2);
                    break;
                case Gdk.Key.comma:
                    _state.KeyReleaseHandle(Key.Select);
                    break;
                case Gdk.Key.period:
                    _state.KeyReleaseHandle(Key.Start);
                    break;
                default:
                    break;
            }
            #endregion Switch

        }

        #endregion Gtk


        #region Initialize Lua

        private static void InitializeLua()
        {
            Lua = new Lua();
            Lua.DoString("luanet.load_assembly(\"Simulator\")");

            RegisterEnumerations();
            RegisterClasses();
        }
        private static void RegisterEnumerations()
        {
            Lua.DoString("Status = luanet.import_type(\"Atmosphere.BattleSimulator.Status\")");
            Lua.DoString("Element = luanet.import_type(\"Atmosphere.BattleSimulator.Element\")");
            Lua.DoString("TargetType = luanet.import_type(\"Atmosphere.BattleSimulator.TargetType\")");
            Lua.DoString("TargetGroup = luanet.import_type(\"Atmosphere.BattleSimulator.TargetGroup\")");
        }
        private static void RegisterClasses()
        {
            // Nonstatic
            Lua.DoString("AbilityState = luanet.import_type(\"Atmosphere.BattleSimulator.AbilityState\")");

            // Static
            Lua.DoString("Item = luanet.import_type(\"Atmosphere.BattleSimulator.Item\")");
            Lua.DoString("Character = luanet.import_type(\"Atmosphere.BattleSimulator.Character\")");
            Lua.DoString("Globals = luanet.import_type(\"Atmosphere.BattleSimulator.Globals\")");
            Lua.DoString("Formula = luanet.import_type(\"Atmosphere.BattleSimulator.Formula\")");
        }
        #endregion



        private static void Init()
        {
            _random = new Random();

            InitializeLua();
            
            Application.Init();
            if (!GLib.Thread.Supported)
                GLib.Thread.Init();
            Gdk.Threads.Init();
            
            Gdk.Threads.Enter();

            _window = new Gtk.Window(Globals.WINDOW_NAME);
            _window.SetDefaultSize(Globals.WIDTH, Globals.HEIGHT);
            _window.AppPaintable = true;
            _window.DoubleBuffered = false;
            _window.DeleteEvent += OnWinDelete;
            _window.KeyPressEvent += OnKeyPress;
            _window.KeyReleaseEvent += OnKeyRelease;
            //_window.ConfigureEvent += OnWindowConfigure;

            DrawingArea da = new DrawingArea();
            da.ExposeEvent += OnExposed;

            Gdk.Color col = new Gdk.Color(0, 0, 0);
            _window.ModifyBg(StateType.Normal, col);
            da.ModifyBg(StateType.Normal, col);

            GLib.Timeout.Add(33, new GLib.TimeoutHandler(Graphics.TimedDraw));

            _window.Add(da);
            _window.ShowAll();

            Gdk.Threads.Leave();

            Graphics.Init();    // depends on _window being initialized
            Item.Init();
            Enemy.Init();       // depends on Globals ctor
            Weapon.Init();      // depends on Globals ctor
            Armor.Init();       // depends on Globals ctor
            Accessory.Init();   // depends on Globals ctor
            Materia.Init();     // depends on Globals ctor
            Character.Init();   // depends on [Weapons|Armor|Materia].Init()
            Globals.Init();     // depends on Character.Init()
            MenuScreen.Init();  // depends on Globals.Init()
            Inventory.Init();   // depends on a whole lot of things
            Spell.Init();       // depends on Globals ctor
            Materiatory.Init(); // depends on Materia.Init()

            int time = Int32.Parse(Globals.SaveGame.SelectSingleNode("//time").InnerText);
            _clock = new Clock(time); // depends on Globals ctor

            // Go to Main Menu
            _state = MainMenu;

            // Go to new Battle
            //GoToBattleState();

            // Go to Post-Battle
            //List<IItem> i = new List<IItem>();
            //i.Add(Item.ItemTable["powersource"]);
            //i.Add(Item.ItemTable["powersource"]);
            //i.Add(Item.ItemTable["potion"]);
            //PostBattle = new PostBattleState(234, 12, 1200, i);
            //_state = PostBattle; 

            _state.Init();

            if (Globals.Party[0] == null && Globals.Party[1] == null && Globals.Party[2] == null)
                throw new GamedataException("No character in party!");

            // Level-up demo
            //using (StreamWriter w = new StreamWriter(@"c:\scripts\test.txt"))
            //{
            //    while (Character.Cloud.Level < 98)
            //    {
            //        Character.Cloud.GainExperience(Character.Cloud.ToNextLevel + 10);
            //        w.WriteLine(Character.Cloud.ToString());
            //    }
            //    w.Flush();
            //}
        }


        private static void SwitchState(State s)
        {
            _state.Dispose();
            _state = s;
            s.Init();
        }
        public static void GoToBattleState()
        {
            //if (Battle != null)
            //    Battle.Dispose();
            Battle = new BattleState();
            SwitchState(Battle);
        }
        public static void GoToMenuState()
        {
            SwitchState(MainMenu);
        }
        public static void GoToPostBattle()
        {
            if (_state != Battle)
                throw new GameImplementationException("Cannot go to Post-Battle state except from Battle state");
            PostBattle = new PostBattleState(Battle);
            SwitchState(PostBattle);
        }


        static void Run()
        {
            while (!_quit)
            {
                try
                {
                    _state.RunIteration();
                }
                catch (Exception e)
                {
                }

                Gdk.Threads.Enter();
                if (Application.EventsPending())
                    try
                    {
                        Application.RunIteration();
                    }
                    catch (Exception e)
                    {
                    }
                Gdk.Threads.Leave();
            }
            Gdk.Threads.Enter();
            Application.Quit();
            Gdk.Threads.Leave();
        }

        static void Main(string[] args)
        {
            Init();
            
            Run();

            _state.Dispose();

            //while (!Terminal.CommandYN("Good?")) ;
        }

        public static void Quit()
        {
            _quit = true;
        }


        public static Random Random
        { get { return _random; } }
        public static Gtk.Window Window
        { get { return _window; } }
        public static State State
        { get { return _state; } }
        public static Clock Clock
        { get { return _clock; } }
    }
}
