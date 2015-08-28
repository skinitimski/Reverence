using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

using Atmosphere.Reverence.Graphics;

namespace Atmosphere.Reverence.Menu
{
    public abstract class Menu : IDisposable
    {
        private static readonly Color border = new Color(0.8, 0.8, 0.8, 1);
        private static readonly Color borderTransparent = new Color(0.8, 0.8, 0.8, 0.6);



        private static readonly float[][] _corners = new float[][]
        {
            new float[] { 0.1f, 0.2f, 0.7f },
            new float[] { 0.0f, 0.1f, 0.5f },
            new float[] { 0.0f, 0.0f, 0.2f },
            new float[] { 0.0f, 0.0f, 0.3f }
        };

        #region Menu Colors

        // top left         0       1
        // top right
        // bottom right
        // bottom left      3       2

        #region Blues

//        public const float R0 = 0.1f;
//        public const float G0 = 0.2f;
//        public const float B0 = 0.7f;
//        public const float R1 = 0.0f;
//        public const float G1 = 0.1f;
//        public const float B1 = 0.5f;
//        public const float R2 = 0.0f;
//        public const float G2 = 0.0f;
//        public const float B2 = 0.2f;
//        public const float R3 = 0.0f;
//        public const float G3 = 0.0f;
//        public const float B3 = 0.3f;

        #endregion Blues

        #region Reds

//        public const float R0 = 0.7f;
//        public const float G0 = 0.2f;
//        public const float B0 = 0.1f;
//        public const float R1 = 0.5f;
//        public const float G1 = 0.1f;
//        public const float B1 = 0.0f;
//        public const float R2 = 0.2f;
//        public const float G2 = 0.0f;
//        public const float B2 = 0.0f;
//        public const float R3 = 0.3f;
//        public const float G3 = 0.0f;
//        public const float B3 = 0.0f;

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
        bool _visible;
        bool _opaque;
        Gdk.Pixbuf _background;

        private Menu()
        {
        }


        protected Menu(int x, int y, int w, int h) : this(x, y, w, h, true)
        {
        }

        protected Menu(int x, int y, int w, int h, bool opaque)
        {
            _x = x;
            _y = y;
            _w = w;
            _h = h;
            _opaque = opaque;
            _visible = true;

            BorderColor = opaque ? border : borderTransparent;

            UpdateBackground();
        }

        private void UpdateBackground()
        {
            byte[] pd = new byte[_w * _h * 4];
            _background = new Gdk.Pixbuf(pd, Gdk.Colorspace.Rgb, true, 8, _w, _h, _w * 4);
            Shapes.RenderTriangle(_background, CP2, CP3, CP0);
            Shapes.RenderTriangle(_background, CP0, CP1, CP2);
        }

        public void Draw(Gdk.Drawable d)
        {
            if (!_visible)
            {
                return;
            }

            if (_w > 0 && _h > 0)
            {
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
                g.Color = BorderColor;
                g.Stroke();

                ((IDisposable)g.Target).Dispose();
                ((IDisposable)g).Dispose();
            }
                                                                                         
            DrawContents(d);
        }

        protected abstract void DrawContents(Gdk.Drawable d);

        protected void Move(int x, int y)
        {
            _x = x;
            _y = y;
        }

        
        
        public virtual void Dispose()
        {
        }


        public static void SetCornerColor(int corner, int rIndex, int gIndex, int bIndex)
        {
            if (rIndex < 0 || rIndex > 10)
            {
                throw new ArgumentException("Must be between 0 and 10 (inclusive)", "rIndex");
            }
            if (gIndex < 0 || gIndex > 10)
            {
                throw new ArgumentException("Must be between 0 and 10 (inclusive)", "gIndex");
            }
            if (bIndex < 0 || bIndex > 10)
            {
                throw new ArgumentException("Must be between 0 and 10 (inclusive)", "bIndex");
            }

            _corners[corner][0] = rIndex * 0.1f;
            _corners[corner][1] = gIndex * 0.1f;
            _corners[corner][2] = bIndex * 0.1f;
        }

        public static void GetCornerColor(int corner, out int rIndex, out int gIndex, out int bIndex)
        {
            rIndex = (int)(_corners[corner][0] * 10);
            gIndex = (int)(_corners[corner][1] * 10);
            bIndex = (int)(_corners[corner][2] * 10);
        }




        #region Properties

        #region Hidden

        private ColoredPoint CP0 { get { return new ColoredPoint(0, 0, _corners[0][0], _corners[0][1], _corners[0][2], _opaque ? 1 : .6f); } }

        private ColoredPoint CP1 { get { return new ColoredPoint(_w, 0, _corners[1][0], _corners[1][1], _corners[1][2], _opaque ? 1 : .6f); } }

        private ColoredPoint CP2 { get { return new ColoredPoint(_w, _h, _corners[2][0], _corners[2][1], _corners[2][2], _opaque ? 1 : .6f); } }

        private ColoredPoint CP3 { get { return new ColoredPoint(0, _h, _corners[3][0], _corners[3][1], _corners[3][2], _opaque ? 1 : .6f); } }

        private Color BorderColor { get; set; }

        #endregion Hidden

        #region Public

        public int X { get { return _x; } }

        public int Y { get { return _y; } }

        public int W { get { return _w; } }

        public int H { get { return _h; } }

        protected int Width
        {
            get { return _w; }
            set
            {
                if (value != _w)
                {
                    _w = value;
                    UpdateBackground();
                }
            }
        }

        protected int Height
        {
            get { return _h; }
            set
            {
                if (value != _h)
                {
                    _h = value;
                    UpdateBackground();
                }
            }
        }

        public bool Visible
        {
            get
            {
                return _visible;
            }
            set
            {
                _visible = value;

                if (_visible)
                {
                    UpdateBackground();
                }
            }
        }

        #endregion Public

        #endregion Properties

    }
    
}
