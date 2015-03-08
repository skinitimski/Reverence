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
        private Window _window;

        private Gdk.Pixmap _pixmap;

        private GameState _state;

        private readonly Cairo.Color _gridColor;

        private int _anime;



        int _oldWidth;
        int _oldHeight;



        private Reverence()
        {
            _gridColor = Config.Instance.Grid;
        }


        
        [GdkMethod()]
        private void OnExposed(object sender, Gtk.ExposeEventArgs e)
        {
            int width, height;
            e.Event.Window.GetSize(out width, out height);

            _pixmap = new Gdk.Pixmap(_window.GdkWindow, width, height);
            
            Gdk.GC gc = new Gdk.GC(_pixmap);
            _pixmap.DrawRectangle(gc, true, 0, 0, width, height);


            using (Context context = Gdk.CairoHelper.Create(_pixmap))
            {
#if DEBUG 

                context.Color = _gridColor;
                
                int ew = width / 8;
                int eh = height / 7;
                
                for (int i = 0; i < 9; i++)
                {
                    int x = i * ew + _anime % ew;
                    
                    context.MoveTo(x, 0);
                    context.LineTo(x, height);
                    context.Stroke();
                }
                for (int j = 0; j < 8; j++)
                {
                    int y = j * eh + _anime % eh;
                    
                    context.MoveTo(0, y);
                    context.LineTo(width, y);
                    context.Stroke();
                }
#endif
                
                //_state.Draw(pixbuf, width, height);
            }
                        
            
            Gtk.DrawingArea area = (DrawingArea)sender;
            Gdk.Window win = area.GdkWindow;
            Gdk.GC gc2 = new Gdk.GC(win);
            
            win.DrawDrawable(gc2, _pixmap, 0, 0, 0, 0, width, height);
        }
        

        [GdkMethod()]
        private bool Update()
        {
            _anime++;
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

        #endregion Key Handling



        private void Go()
        {
            Application.Init();

            
            _window = new Window(Config.Instance.WindowTitle);
            _window.SetDefaultSize(Config.Instance.WindowWidth, Config.Instance.WindowHeight);
            _window.DeleteEvent += OnWinDelete;
            _window.KeyPressEvent += OnKeyPress;
            _window.KeyReleaseEvent += OnKeyRelease;
            _window.ConfigureEvent += OnWinConfigure;


            GLib.Timeout.Add(1000 / Config.Instance.RefreshRate, new GLib.TimeoutHandler(Update));
                        
            DrawingArea da = new DrawingArea();
            da.ExposeEvent += OnExposed;

            _window.Add(da);
            _window.ShowAll();
            
            _pixmap = new Gdk.Pixmap(_window.GdkWindow, Config.Instance.WindowWidth, Config.Instance.WindowHeight, -1);
            _window.AppPaintable = true;
            _window.DoubleBuffered = false;


            
            Application.Run();

        }
        
        public void Quit()
        {
            Application.Quit();
        }







        public static void Main(string[] args)
        {
            Reverence game = new Reverence();

            game.Go();
        }
    }
}
