using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{


    public abstract class Menu
    {
        #region Menu Colors

        // top left
        // top right
        // bottom right
        // bottom left

        #region Blues
        public const float R0 = 0.1f;
        public const float G0 = 0.2f;
        public const float B0 = 0.7f;

        public const float R1 = 0.0f;
        public const float G1 = 0.1f;
        public const float B1 = 0.5f;

        public const float R2 = 0.0f;
        public const float G2 = 0.0f;
        public const float B2 = 0.2f;

        public const float R3 = 0.0f;
        public const float G3 = 0.0f;
        public const float B3 = 0.3f;
        #endregion Blues
        #region Reds
        //public const float R0 = 0.7f;
        //public const float G0 = 0.2f;
        //public const float B0 = 0.1f;

        //public const float R1 = 0.5f;
        //public const float G1 = 0.1f;
        //public const float B1 = 0.0f;

        //public const float R2 = 0.2f;
        //public const float G2 = 0.0f;
        //public const float B2 = 0.0f;

        //public const float R3 = 0.3f;
        //public const float G3 = 0.0f;
        //public const float B3 = 0.0f;
        #endregion Reds
        #region Yellows
        //public const float R0 = 0.8f;
        //public const float G0 = 0.8f;
        //public const float B0 = 0.3f;

        //public const float R1 = 0.6f;
        //public const float G1 = 0.6f;
        //public const float B1 = 0.2f;

        //public const float R2 = 0.1f;
        //public const float G2 = 0.1f;
        //public const float B2 = 0.1f;

        //public const float R3 = 0.3f;
        //public const float G3 = 0.3f;
        //public const float B3 = 0.1f;
        #endregion Yellows

        #endregion Menu Colors

        int _x;
        int _y;
        int _w;
        int _h;
        bool _opaque;
        bool _visible;
        Gdk.Pixbuf _background;

        static Character _selected;
        static int _selection = 0;

        protected Menu(int x, int y, int w, int h) : this(x, y, w, h, true) { }
        protected Menu(int x, int y, int w, int h, bool opaque)
        {
            _x = x;
            _y = y;
            _w = w;
            _h = h;
            _opaque = opaque;
            _visible = true;

            UpdateBackground();
        }

        private void UpdateBackground()
        {
            byte[] pd = new byte[_w * _h * 4];
            _background = new Gdk.Pixbuf(pd, Gdk.Colorspace.Rgb, true, 8, _w, _h, _w * 4);
            Graphics.RenderTriangle(_background, CP2, CP3, CP0);
            Graphics.RenderTriangle(_background, CP0, CP1, CP2);
        }


        public void Draw(Gdk.Drawable d)
        {
            if (!Visible)
                return;

            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            d.DrawPixbuf(gc, _background, 0, 0, _x, _y, _w, _h, Gdk.RgbDither.None, 0, 0);

            int x0 = _x, x1 = _x + _w;
            int y0 = _y, y1 = _y + _h;

            g.MoveTo(x0 + 3, y0);
            g.LineTo(x1 - 3, y0);
            g.LineTo(x1, y0 + 3);
            g.LineTo(x1, y1 - 3);
            g.LineTo(x1 - 3, y1);
            g.LineTo(x0 + 3, y1);
            g.LineTo(x0, y1 - 3);
            g.LineTo(x0, y0 + 3);
            g.LineTo(x0 + 3, y0);
            g.ClosePath();
            g.LineWidth = 6;
            g.Color = new Color(0.8, 0.8, 0.8, _opaque ? 1 : 0.6);
            g.Stroke();

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
                                                                                         
            DrawContents(d);
        }


        protected abstract void DrawContents(Gdk.Drawable d);

        public static void SetSelection(Character c)
        {
            _selected = c;
        }
        public static void SetSelection()
        {
            if (Globals.Party[0] != null)
                _selection = 0;
            else if (Globals.Party[1] != null)
                _selection = 1;
            else if (Globals.Party[2] != null)
                _selection = 2;
            else throw new GameImplementationException("Party has no members.");

            _selected = Globals.Party[_selection];
        }
        public static void IncrementSelection()
        {
            do
                _selection = (_selection + 1) % 3;
            while (Globals.Party[_selection] == null);

            SetSelection(Globals.Party[_selection]);
        }
        public static void DecrementSelection()
        {
            do
                _selection = (_selection + 2) % 3;
            while (Globals.Party[_selection] == null);

            SetSelection(Globals.Party[_selection]);
        }



        #region Properties

        #region Hidden
        private ColoredPoint CP0
        { get { return new ColoredPoint(0, 0, R0, G0, B0, _opaque ? 1 : .6f); } }
        private ColoredPoint CP1
        { get { return new ColoredPoint(_w, 0, R1, G1, B1, _opaque ? 1 : .6f); } }
        private ColoredPoint CP2
        { get { return new ColoredPoint(_w, _h, R2, G2, B2, _opaque ? 1 : .6f); } }
        private ColoredPoint CP3
        { get { return new ColoredPoint(0, _h, R3, G3, B3, _opaque ? 1 : .6f); } }
        #endregion Hidden

        #region Public
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
        public int W { get { return _w; } }
        public int H { get { return _h; } }
        protected int Width { get { return _w; } set { _w = value; UpdateBackground(); } }
        protected int Height { get { return _h; } set { _h = value; UpdateBackground(); } }
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public static Character Selected { get { return _selected; } }
        #endregion Public

        #endregion Properties

    }

    
    public abstract class ControlMenu : Menu, IController
    {
        protected bool _isControl = false;

        protected ControlMenu(int x, int y, int w, int h) : this(x, y, w, h, true) { }
        protected ControlMenu(int x, int y, int w, int h, bool opaque)
            : base(x, y, w, h, opaque) { }

        public abstract void ControlHandle(Key k);

        public bool IsControl { get { return _isControl; } }

        public virtual void Reset() { }

        public virtual void SetAsControl() 
        {
            _isControl = true;
        }
        public virtual void SetNotControl()
        {
            _isControl = false;
        }

        public abstract string Info { get; }
    }

    
}
