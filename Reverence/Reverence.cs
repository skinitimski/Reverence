using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

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




        private Reverence()
        {
            _width = Config.Instance.WindowWidth;
            _height = Config.Instance.WindowHeight;
        }



        private bool Draw()
        {
            Gdk.Threads.Enter();

            Gdk.GC gc = new Gdk.GC(_pixmap);
            _pixmap.DrawRectangle(gc, true, 0, 0, _width, _height);
            
            
            
            #region Draw Grid
            
            Cairo.Context g = Gdk.CairoHelper.Create(_pixmap);
            g.Color = new Cairo.Color(.8, .8, .8);
            for (int i = 1; i < 8; i++)
            {
                g.MoveTo(i * _width / 8, 0);
                g.LineTo(i * _width / 8, _height);
                g.Stroke();
            }
            for (int j = 1; j < 7; j++)
            {
                g.MoveTo(0, j * _height / 7);
                g.LineTo(_width, j * _height / 7);
                g.Stroke();
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
            
            #endregion Draw Grid

            Gdk.Threads.Leave();

            return true;
        }
        
        
        private void OnExposed(object o, ExposeEventArgs args)
        {
            Gtk.DrawingArea area = (DrawingArea)o;
            Gdk.Window win = area.GdkWindow;
            Gdk.GC gc2 = new Gdk.GC(win);
            
            win.DrawDrawable(gc2, _pixmap, 0, 0, 0, 0, Config.Instance.WindowWidth, Config.Instance.WindowHeight);
        }


        private void OnWinDelete(object o, DeleteEventArgs args)
        {
            Application.Quit();
            _quit = true;
        }
        
        
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


        private void Init()
        {
            Application.Init();

            if (!GLib.Thread.Supported)
                GLib.Thread.Init();
            Gdk.Threads.Init();

            Gdk.Threads.Enter();

            _window = new Window(Config.Instance.WindowTitle);
            _window.Resize(_width, _height);
            _window.AppPaintable = true;
            _window.DoubleBuffered = false;
            _window.DeleteEvent += OnWinDelete;
            _window.KeyPressEvent += OnKeyPress;
            _window.KeyReleaseEvent += OnKeyRelease;

            _pixmap = new Gdk.Pixmap(_window.GdkWindow, _width, _height);
            
            DrawingArea da = new DrawingArea();
            da.ExposeEvent += OnExposed;
            
            Gdk.Color col = new Gdk.Color(0, 0, 0);
            _window.ModifyBg(StateType.Normal, col);
            da.ModifyBg(StateType.Normal, col);
                        
            GLib.Timeout.Add(33, new GLib.TimeoutHandler(Draw));

            _window.Add(da);
            _window.ShowAll();

            Gdk.Threads.Leave();
        }

        private void Run()
        {
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

            game.Init();

            game.Run();
        }
    }
}
