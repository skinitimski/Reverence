//#define DRAWGRID
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using NLua;

using Cairo;
using Gtk;

using Atmosphere.Reverence.Attributes;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence
{
    public abstract class Game
    {
        private Window _window;
        private Gdk.Pixmap _pixmap;

        private bool _isDrawing;
        private int _oldWidth;
        private int _oldHeight;
        private bool _quit;
        
#if DRAWGRID
        private readonly Cairo.Color _gridColor;
#endif



        
        private class InitialState : State
        {
            public const int TIMEOUT_MS = 500;
            private const int HALF_TIMEOUT_MS = TIMEOUT_MS / 2;

            private Time.Timer _timer;

            private double r, g, b;

            public InitialState(Game game, Color splashScreenColor)
                : base(game)
            {
                _timer = new Time.Timer(2000);

                Color fullcolor = splashScreenColor;

                r = fullcolor.R;
                g = fullcolor.G;
                b = fullcolor.B;
            }

            public override void KeyPressHandle(Key k)
            {                
            }
            
            public override void KeyReleaseHandle(Key k)
            {
            }
            
            public override void RunIteration()
            {                
            }
            
            public override void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
            {                
                g.MoveTo(0, 0);
                g.LineTo(width, 0);
                g.LineTo(width, height);
                g.LineTo(0, height);
                g.LineTo(0, 0);
                g.ClosePath();
                g.Color = GetCurrentColor();
                g.Fill();
            }

            private Color GetCurrentColor()
            {
                double t = _timer.TotalMilliseconds;
                double c;

                if (t < HALF_TIMEOUT_MS)
                {
                    c = t / HALF_TIMEOUT_MS;
                }
                else
                {
                    c = (TIMEOUT_MS - t) / HALF_TIMEOUT_MS;
                }

                return new Color(r, g, b, c);
            }
            
            protected override void InternalInit()
            {
            }
            
            protected override void InternalDispose()
            {
            }
        }












        private Game()
        {
        }

        protected Game(string configPath)
            : this()
        {
#if DRAWGRID
            _gridColor = Config.Instance.Grid;
#endif
            Configuration = new Config(configPath);

            State = new InitialState(this, Configuration.SplashScreenColor);
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

                bool screenChanged = w != _oldWidth || h != _oldHeight;

                Gdk.GC gc = new Gdk.GC(_pixmap);            
                _pixmap.DrawRectangle(gc, true, 0, 0, w, h);
                
                
                using (Context context = Gdk.CairoHelper.Create(_pixmap))
                {
#if DRAWGRID 
                    
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

                    try
                    {
                        Cairo.Context g = Gdk.CairoHelper.Create(_pixmap);

                        State.Draw(_pixmap, g, w, h, screenChanged);

                        if (MessageBox != null)
                        {
                            MessageBox.Draw(_pixmap, g, w, h, screenChanged);
                        }

                        ((IDisposable)g.Target).Dispose();
                        ((IDisposable)g).Dispose();
                    }
                    catch (Exception e)
                    {
                        HandleFatalException(e, " experienced while attempting to run State.Draw()");
                    }
                }
                
                lock (_window)
                {
                    _isDrawing = false;
                }
            }

            _window.GetSize(out _oldWidth, out _oldHeight);
            _window.QueueDrawArea(0, 0, _oldWidth, _oldHeight);

            return true;
        }
        
        [GdkMethod()]
        private void OnWinDelete(object o, DeleteEventArgs args)
        {
            _quit = true;
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
            if (MessageBox != null)
            {
                return;
            }

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
            if (MessageBox != null)
            {
                return;
            }

            #region Switch
            switch (args.Event.Key)
            {
                case Gdk.Key.r:
                case Gdk.Key.R:
                    Reset();
                    break;
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
        
        protected abstract void Cleanup();

        protected void QueueInitialState()
        {
            new Thread(new ThreadStart(GoToInitialState)).Start();
        }

        protected void GoToInitialState()
        {
            Thread.Sleep(InitialState.TIMEOUT_MS);

            State state = GetInitialState();
            state.Init();

            SetState(state).Dispose();
        }

        protected void Reset()
        {           
            Cleanup();
                      
            Configuration.ResetMenuColors();

            State = new InitialState(this, Configuration.SplashScreenColor);

            QueueInitialState();
        }


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
            try
            {
                Application.Init();

                _window = new Window(Configuration.WindowTitle);
                _window.SetDefaultSize(Configuration.WindowWidth, Configuration.WindowHeight);
                _window.DeleteEvent += OnWinDelete;
                _window.KeyPressEvent += OnKeyPress;
                _window.KeyReleaseEvent += OnKeyRelease;
                _window.ConfigureEvent += OnWinConfigure;
                _window.ExposeEvent += OnExposed;
            
                _oldWidth = Configuration.WindowWidth;
                _oldHeight = Configuration.WindowHeight;
            
                GLib.Timeout.Add(1000 / Configuration.RefreshRate, new GLib.TimeoutHandler(OnTimedDraw));
            
                _window.ShowAll();

                _pixmap = new Gdk.Pixmap(_window.GdkWindow, Configuration.WindowWidth, Configuration.WindowHeight, -1);
                _window.AppPaintable = true;
                _window.DoubleBuffered = false;
                         

                QueueInitialState();

            
                while (!_quit)
                {
                    try
                    {
                        if (MessageBox != null && MessageBox.Timer.IsUp)
                        {
                            MessageBox = null;
                        }

                        State.RunIteration();
                    }
                    catch (Exception e)
                    {
                        HandleFatalException(e);
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
                            HandleFatalException(e);
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
            finally
            {
                Cleanup();
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


        private void HandleFatalException(Exception e, string msg = " experienced")
        {            
            Console.WriteLine("FATAL ERROR" + msg);

            int depth = 0;

            while (e != null)
            {                
                Console.WriteLine("Depth: " + depth++);
                Console.WriteLine("{0}: {1}{2}{3}", e.GetType().Name, e.Message, Environment.NewLine, e.StackTrace);

                e = e.InnerException;
            }

            _quit = true;
        }
        
        public void ShowMessage(TimedDialogue message, int timeout = MessageBox.DEFAULT_TIMEOUT)
        {
            ScreenState screenState = new ScreenState
            {
                Width = _oldWidth,
                Height = _oldHeight
            };
            
            MessageBox = new MessageBox(screenState, message, timeout);            
        }


        protected State State { get; private set; }

        public Config Configuration { get; private set; }
        
        private MessageBox MessageBox { get; set; }
    }
}
