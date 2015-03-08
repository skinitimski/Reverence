using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;

using Cairo;
using NLua;
using Gtk;

using Atmosphere.Reverence.State;
using GameState = Atmosphere.Reverence.State.State;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence
{
    public class Reverence
    {
        private bool _quit = false;

        private Window _window;
        private Gdk.Pixmap _pixmap;

        private GameState _state;

        private readonly int _width;
        private readonly int _height;
        private int _thingy;

        
        private bool _isDrawing = false;
        private bool _firstDraw = true;
        private Thread _drawThread;


        private Reverence()
        {
            _width = Config.Instance.WindowWidth;
            _height = Config.Instance.WindowHeight;
        }


        
        [GdkMethod()]
        private void OnExposed(object sender, Gtk.ExposeEventArgs e)
        {
            using (Context context = Gdk.CairoHelper.Create(e.Event.Window))
            {
                int w, h;
                e.Event.Window.GetSize(out w, out h);

                Draw(context, w, h);
            }
        }

        private void Draw(Context context, int width, int height)
        {
            context.Save();

            context.Color = new Cairo.Color(.8, .8, .8);

            int ew = _width / 8;
            int eh = _height / 7;

            for (int i = 1; i < 8; i++)
            {
                int x = i * ew + _thingy % ew;

                context.MoveTo(x, 0);
                context.LineTo(x, _height);
                context.Stroke();
            }
            for (int j = 1; j < 7; j++)
            {
                int y = j * eh + _thingy % eh;

                context.MoveTo(0, y);
                context.LineTo(_width, y);
                context.Stroke();
            }

            context.Restore();
        }
        

        private bool Update()
        {
            _thingy++;
            _window.QueueDraw();
            return true;
        }
        
        [GdkMethod()]
        private void OnWinDelete(object o, DeleteEventArgs args)
        {
            Application.Quit();
            _quit = true;
        }        

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


        private void Go()
        {
            Application.Init();

            
            _window = new Window(Config.Instance.WindowTitle);
            _window.Resize(_width, _height);
            _window.DeleteEvent += OnWinDelete;
            _window.KeyPressEvent += OnKeyPress;
            _window.KeyReleaseEvent += OnKeyRelease;

            
            GLib.Timeout.Add(10, new GLib.TimeoutHandler(Update));
                        
            DrawingArea da = new DrawingArea();
            da.ExposeEvent += OnExposed;

            _window.Add(da);

            

            _window.ShowAll();

            
            Application.Run();

        }
        
        public void Quit()
        {
            _quit = true;
            Application.Quit();
        }







        public static void Main(string[] args)
        {
            Reverence game = new Reverence();

            game.Go();
        }
    }
}
