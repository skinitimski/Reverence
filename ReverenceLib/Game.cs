using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using NLua;

using Cairo;
using Gtk;

using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence
{
    public abstract class Game
    {
        private Window _window;
        private Gdk.Pixmap _pixmap;
        private readonly Cairo.Color _gridColor;
        private int _anime;
        private bool _isDrawing;
        private int _oldWidth;
        private int _oldHeight;
        private bool _quit;





        protected Game()
        {
            _gridColor = Config.Instance.Grid;

            State = GetInitialState();
            LuaEnvironment = new Lua();
        }



        #region GDK methods
        
        [GdkMethod()]
        private void OnExposed(object sender, Gtk.ExposeEventArgs e)
        {
            using (Context context = Gdk.CairoHelper.Create(e.Event.Window))
            {
                Gdk.CairoHelper.SetSourcePixmap(context, _pixmap, e.Event.Area.X, e.Event.Area.Y);
                context.Rectangle(e.Event.Area.X, e.Event.Area.Y, e.Event.Area.Width, e.Event.Area.Height);
                context.Fill();
            }
        }
        
        [GdkMethod()]
        private bool OnTimedDraw()
        {
            _anime++;
            
            bool draw = false;
            
            
            lock (_window)
            {
                if (!_isDrawing)
                {
                    _isDrawing = true;
                    draw = true;
                }
            }
            
            if (draw)
            {
                int w, h;
                
                _pixmap.GetSize(out w, out h);
                Gdk.GC gc = new Gdk.GC(_pixmap);            
                _pixmap.DrawRectangle(gc, true, 0, 0, w, h);
                
                
                using (Context context = Gdk.CairoHelper.Create(_pixmap))
                {
#if DEBUG 
                    
                    context.Color = _gridColor;
                    
                    int ew = w / 8;
                    int eh = h / 7;
                    
                    for (int i = 0; i < 9; i++)
                    {
                        int x = i * ew + _anime % ew;
                        
                        context.MoveTo(x, 0);
                        context.LineTo(x, h);
                        context.Stroke();
                    }
                    for (int j = 0; j < 8; j++)
                    {
                        int y = j * eh + _anime % eh;
                        
                        context.MoveTo(0, y);
                        context.LineTo(w, y);
                        context.Stroke();
                    }
#endif
                    
                    State.Draw(_pixmap, w, h);
                }
                
                lock (_window)
                {
                    _isDrawing = false;
                }
            }
            
            
            int width, height;
            _window.GetSize(out width, out height);
            _window.QueueDrawArea(0, 0, width, height);
            return true;
        }
        
        [GdkMethod()]
        private void OnWinDelete(object o, DeleteEventArgs args)
        {
            Application.Quit();
        }
        
        [GdkMethod()]
        [GLib.ConnectBefore()]
        private void OnWinConfigure(object o, ConfigureEventArgs args)
        {
            Gdk.EventConfigure e = args.Event;
            
            if (_oldWidth != e.Width || _oldHeight != e.Height)
            {
                Gdk.Pixmap tmp = new Gdk.Pixmap(_window.GdkWindow, e.Width, e.Height, -1);
                
                int minw = e.Width < _oldWidth ? e.Width : _oldWidth;
                int minh = e.Height < _oldHeight ? e.Height : _oldHeight;
                
                using (Context context = Gdk.CairoHelper.Create(tmp))
                {
                    Gdk.CairoHelper.SetSourcePixmap(context, _pixmap, 0, 0);
                    context.Rectangle(0, 0, minw, minh);
                    context.Fill();
                }
                
                _pixmap = tmp;
            }
            
            _oldWidth = e.Width;
            _oldHeight = e.Height;
        }
        
        
        #region Key Handling
        
        [GdkMethod()]
        [GLib.ConnectBefore()]
        private void OnKeyPress(object o, Gtk.KeyPressEventArgs args)
        {
#if DEBUG
            Console.WriteLine("Pressed  {0}", args.Event.Key.ToString());
#endif
            #region Switch
            switch (args.Event.Key)
            {
                case Gdk.Key.q:
                case Gdk.Key.Q:
                    Quit();
                    break;
                case Gdk.Key.s:
                case Gdk.Key.S:
                    State.KeyPressHandle(Key.Square);
                    break;
                case Gdk.Key.x:
                case Gdk.Key.X:
                    State.KeyPressHandle(Key.X);
                    break;
                case Gdk.Key.t:
                case Gdk.Key.T:
                    State.KeyPressHandle(Key.Triangle);
                    break;
                case Gdk.Key.c:
                case Gdk.Key.C:
                    State.KeyPressHandle(Key.Circle);
                    break;
                case Gdk.Key.Up:
                    State.KeyPressHandle(Key.Up);
                    break;
                case Gdk.Key.Down:
                    State.KeyPressHandle(Key.Down);
                    break;
                case Gdk.Key.Right:
                    State.KeyPressHandle(Key.Right);
                    break;
                case Gdk.Key.Left:
                    State.KeyPressHandle(Key.Left);
                    break;
                case Gdk.Key.Key_1:
                    State.KeyPressHandle(Key.L1);
                    break;
                case Gdk.Key.Key_2:
                    State.KeyPressHandle(Key.L2);
                    break;
                case Gdk.Key.Key_3:
                    State.KeyPressHandle(Key.R1);
                    break;
                case Gdk.Key.Key_4:
                    State.KeyPressHandle(Key.R2);
                    break;
                case Gdk.Key.comma:
                    State.KeyPressHandle(Key.Select);
                    break;
                case Gdk.Key.period:
                    State.KeyPressHandle(Key.Start);
                    break;
                default:
                    break;
            }
            #endregion Switch
            
        }
        
        [GdkMethod()]
        [GLib.ConnectBefore()]
        private void OnKeyRelease(object o, Gtk.KeyReleaseEventArgs args)
        {
#if DEBUG
            Console.WriteLine("Released {0}", args.Event.Key.ToString());
#endif
            #region Switch
            switch (args.Event.Key)
            {
                case Gdk.Key.q:
                case Gdk.Key.Q:
                    Quit();
                    break;
                case Gdk.Key.s:
                case Gdk.Key.S:
                    State.KeyReleaseHandle(Key.Square);
                    break;
                case Gdk.Key.x:
                case Gdk.Key.X:
                    State.KeyReleaseHandle(Key.X);
                    break;
                case Gdk.Key.t:
                case Gdk.Key.T:
                    State.KeyReleaseHandle(Key.Triangle);
                    break;
                case Gdk.Key.c:
                case Gdk.Key.C:
                    State.KeyReleaseHandle(Key.Circle);
                    break;
                case Gdk.Key.Up:
                    State.KeyReleaseHandle(Key.Up);
                    break;
                case Gdk.Key.Down:
                    State.KeyReleaseHandle(Key.Down);
                    break;
                case Gdk.Key.Right:
                    State.KeyReleaseHandle(Key.Right);
                    break;
                case Gdk.Key.Left:
                    State.KeyReleaseHandle(Key.Left);
                    break;
                case Gdk.Key.Key_1:
                    State.KeyReleaseHandle(Key.L1);
                    break;
                case Gdk.Key.Key_2:
                    State.KeyReleaseHandle(Key.L2);
                    break;
                case Gdk.Key.Key_3:
                    State.KeyReleaseHandle(Key.R1);
                    break;
                case Gdk.Key.Key_4:
                    State.KeyReleaseHandle(Key.R2);
                    break;
                case Gdk.Key.comma:
                    State.KeyReleaseHandle(Key.Select);
                    break;
                case Gdk.Key.period:
                    State.KeyReleaseHandle(Key.Start);
                    break;
                default:
                    break;
            }
            #endregion Switch
            
        }
        
        #endregion Key Handling
        
        #endregion GDK methods





        protected abstract State GetInitialState();

        protected abstract void Init();


        /// <summary>
        /// Sets the game state to the given <paramref name="newState" />,
        /// </summary>
        /// <returns>
        /// The previous state.
        /// </returns>
        /// <param name='newState'>
        /// The new active state of the game.
        /// </param>
        protected State SetState(State newState)
        {
            State previous = State;
            State = newState;
            return previous;
        }


        private void Go()
        {
            Init();

            Application.Init();

            
            
            _window = new Window(Config.Instance.WindowTitle);
            _window.SetDefaultSize(Config.Instance.WindowWidth, Config.Instance.WindowHeight);
            _window.DeleteEvent += OnWinDelete;
            _window.KeyPressEvent += OnKeyPress;
            _window.KeyReleaseEvent += OnKeyRelease;
            _window.ConfigureEvent += OnWinConfigure;
            _window.ExposeEvent += OnExposed;
            
            
            GLib.Timeout.Add(1000 / Config.Instance.RefreshRate, new GLib.TimeoutHandler(OnTimedDraw));
            
            _window.ShowAll();

            _pixmap = new Gdk.Pixmap(_window.GdkWindow, Config.Instance.WindowWidth, Config.Instance.WindowHeight, -1);
            _window.AppPaintable = true;
            _window.DoubleBuffered = false;
            

            
            while (!_quit)
            {
                try
                {
                    State.RunIteration();
                }
                catch (Exception e)
                {
                    Console.WriteLine("FATAL ERROR: {0}: {1}{2}{3}", e.GetType().Name, e.Message, Environment.NewLine, e.StackTrace);
                }
                
                Gdk.Threads.Enter();
                
                if (Application.EventsPending())
                {
                    try
                    {
                        Application.RunIteration();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("FATAL ERROR: {0}: {1}{2}{3}", e.GetType().Name, e.Message, Environment.NewLine, e.StackTrace);
                    }
                    finally
                    {
                        Gdk.Threads.Leave();
                    }
                }
            }
            
            Gdk.Threads.Enter();
            
            try
            {
                Application.Quit();
            }
            finally
            {
                Gdk.Threads.Leave();
            }            
        }



        protected void Quit()
        {
            _quit = true;
        }



        public static void RunGame(Game game)
        {
            game.Go();
        }

        
        protected State State { get; private set; }
        protected Lua LuaEnvironment { get; private set; }

    }
}
