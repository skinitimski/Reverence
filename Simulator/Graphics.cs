using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Gdk;

namespace Atmosphere.BattleSimulator
{
    struct ColoredPoint
    {
        public int X;
        public int Y;
        public float R;
        public float G;
        public float B;
        public float A;

        public ColoredPoint(int x, int y, float r, float g, float b, float a)
        {
            X = x;
            Y = y;
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }


    static class Graphics
    {
        #region Member Data

        public const int SHADOW_SPACING = 3;
        public const int PROFILE_WIDTH = 120;
        public const int PROFILE_HEIGHT = 136;
        public const int PROFILE_WIDTH_SMALL = 89;
        public const int PROFILE_HEIGHT_SMALL = 101;

        private static bool _drawing = false;
        private static bool _firstDraw = true;

        private static Dictionary<string, Pixbuf> _profileTable;
        private static Dictionary<string, Pixbuf> _phsTable;

        private static Pixbuf _img_cloud;
        private static Pixbuf _img_tifa;
        private static Pixbuf _img_barret;
        private static Pixbuf _img_aeris;
        private static Pixbuf _img_redxiii;
        private static Pixbuf _img_yuffie;
        private static Pixbuf _img_caitsith;
        private static Pixbuf _img_vincent;
        private static Pixbuf _img_cid;
        private static Pixbuf _img_sephiroth;

        private static Pixbuf _img_cloud_small;
        private static Pixbuf _img_tifa_small;
        private static Pixbuf _img_barret_small;
        private static Pixbuf _img_aeris_small;
        private static Pixbuf _img_redxiii_small;
        private static Pixbuf _img_yuffie_small;
        private static Pixbuf _img_caitsith_small;
        private static Pixbuf _img_vincent_small;
        private static Pixbuf _img_cid_small;
        private static Pixbuf _img_sephiroth_small;

        private static Gdk.Pixmap _pixmap;

        private static Thread _drawThread;

        #endregion Member Data



        public static void Init()
        {
            // we initialize this here so that other classes can ask it questions immediately
            _drawThread = new Thread(new ThreadStart(Draw));
            _pixmap = new Pixmap(Game.Window.GdkWindow, Globals.WIDTH, Globals.HEIGHT);


            _img_cloud = new Pixbuf(@"Images\cloud.JPG");
            _img_tifa = new Pixbuf(@"Images\tifa.JPG");
            _img_aeris = new Pixbuf(@"Images\aeris.JPG");
            _img_barret = new Pixbuf(@"Images\barret.JPG");
            _img_redxiii = new Pixbuf(@"Images\redxiii.JPG");
            _img_yuffie = new Pixbuf(@"Images\yuffie.JPG");
            _img_caitsith = new Pixbuf(@"Images\caitsith.JPG");
            _img_vincent = new Pixbuf(@"Images\vincent.JPG");
            _img_cid = new Pixbuf(@"Images\cid.JPG");
            _img_sephiroth = new Pixbuf(@"Images\sephiroth.JPG");

            _img_cloud_small = new Pixbuf(@"Images\cloud_small.JPG");
            _img_tifa_small = new Pixbuf(@"Images\tifa_small.JPG");
            _img_aeris_small = new Pixbuf(@"Images\aeris_small.JPG");
            _img_barret_small = new Pixbuf(@"Images\barret_small.JPG");
            _img_redxiii_small = new Pixbuf(@"Images\redxiii_small.JPG");
            _img_yuffie_small = new Pixbuf(@"Images\yuffie_small.JPG");
            _img_caitsith_small = new Pixbuf(@"Images\caitsith_small.JPG");
            _img_vincent_small = new Pixbuf(@"Images\vincent_small.JPG");
            _img_cid_small = new Pixbuf(@"Images\cid_small.JPG");
            _img_sephiroth_small = new Pixbuf(@"Images\sephiroth_small.JPG");

            _profileTable = new Dictionary<string, Pixbuf>();
            _profileTable.Add("Cloud", _img_cloud);
            _profileTable.Add("Tifa", _img_tifa);
            _profileTable.Add("Aeris", _img_aeris);
            _profileTable.Add("Barret", _img_barret);
            _profileTable.Add("RedXIII", _img_redxiii);
            _profileTable.Add("Yuffie", _img_yuffie);
            _profileTable.Add("CaitSith", _img_caitsith);
            _profileTable.Add("Vincent", _img_vincent);
            _profileTable.Add("Cid", _img_cid);
            _profileTable.Add("Sephiroth", _img_sephiroth);

            _phsTable = new Dictionary<string, Pixbuf>();
            _phsTable.Add("Cloud", _img_cloud_small);
            _phsTable.Add("Tifa", _img_tifa_small);
            _phsTable.Add("Aeris", _img_aeris_small);
            _phsTable.Add("Barret", _img_barret_small);
            _phsTable.Add("RedXIII", _img_redxiii_small);
            _phsTable.Add("Yuffie", _img_yuffie_small);
            _phsTable.Add("CaitSith", _img_caitsith_small);
            _phsTable.Add("Vincent", _img_vincent_small);
            _phsTable.Add("Cid", _img_cid_small);
            _phsTable.Add("Sephiroth", _img_sephiroth_small);
        }

        public static Pixbuf GetProfile(string characterName)
        {
            return _profileTable[characterName];
        }
        public static Pixbuf GetProfileSmall(string characterName)
        {
            return _phsTable[characterName];
        }

        private static void Draw()
        {
            _drawing = true;

            Gdk.GC gc = new Gdk.GC(_pixmap);
            _pixmap.DrawRectangle(gc, true, 0, 0, Globals.WIDTH, Globals.HEIGHT);



            #region Draw Grid
            Cairo.Context g = Gdk.CairoHelper.Create(_pixmap);
            g.Color = new Cairo.Color(.8, .8, .8);
            for (int i = 1; i < 8; i++)
            {
                g.MoveTo(i * Globals.WIDTH / 8, 0);
                g.LineTo(i * Globals.WIDTH / 8, Globals.HEIGHT);
                g.Stroke();
            }
            for (int j = 1; j < 7; j++)
            {
                g.MoveTo(0, j * Globals.HEIGHT / 7);
                g.LineTo(Globals.WIDTH, j * Globals.HEIGHT / 7);
                g.Stroke();
            }
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
            #endregion Draw Grid



            Game.State.Draw(_pixmap);

            _drawing = false;
        }

        internal static bool TimedDraw()
        {
            if (!IsDrawing)
            {
                if (!_firstDraw)
                    _drawThread.Join();
                _drawThread = new Thread(new ThreadStart(Draw));
                _drawThread.Start();
            }

            Game.Window.QueueDrawArea(0, 0, Globals.WIDTH, Globals.HEIGHT);

            _firstDraw = false;

            return true;
        }




        #region Utility

        public static void ShadowedText(Cairo.Context g, string text, double x, double y)
        {
            ShadowedText(g, new Cairo.Color(1, 1, 1), text, x, y);
        }

        public static void ShadowedText(Cairo.Context g, Cairo.Color c, string text, double x, double y)
        {
            g.Save();

            g.MoveTo(x + Graphics.SHADOW_SPACING, y + Graphics.SHADOW_SPACING);
            g.Color = new Cairo.Color(0, 0, 0);
            g.ShowText(text);

            g.MoveTo(x, y);
            g.Color = c;
            g.ShowText(text);

            g.Restore();
        }

        public static void RenderLine(Cairo.Context g, Cairo.Color c, double lw, double x0, double y0, double x1, double y1)
        {
            g.Save();
            g.Color = c;
            g.LineWidth = 3;
            g.MoveTo(x0, y0);
            g.LineTo(x1, y1);
            g.Stroke();
            g.Restore();
        }
        public static void RenderCircle(Cairo.Context g, Cairo.Color c, double r, double x, double y)
        {
            g.Save();
            g.Color = c;
            g.MoveTo(x, y);
            g.Arc(x, y, r, 0, 6.28);
            g.LineWidth = 3;
            g.ClosePath();
            g.Fill();
            g.Restore();
        }


        public static void RenderCursor(Cairo.Context g, double x, double y)
        {
            RenderCircle(g, new Cairo.Color(1, 1, 1), 5, x, y);
        }
        public static void RenderCursor(Cairo.Context g, Cairo.Color c, double x, double y)
        {
            RenderCircle(g, c, 5, x, y);
        }


        public static void DrawProfile(Gdk.Drawable d, Gdk.GC gc, string name, int x, int y)
        {
            d.DrawPixbuf(gc, GetProfile(name), 0, 0, x, y,
                PROFILE_WIDTH, PROFILE_HEIGHT,
                Gdk.RgbDither.None, 0, 0);
        }
        public static void DrawProfileSmall(Gdk.Drawable d, Gdk.GC gc, string name, int x, int y)
        {
            d.DrawPixbuf(gc, GetProfileSmall(name), 0, 0, x, y,
                PROFILE_WIDTH_SMALL, PROFILE_HEIGHT_SMALL,
                Gdk.RgbDither.None, 0, 0);
        }

        public unsafe static void RenderTriangle(Pixbuf pb, ColoredPoint cp0, ColoredPoint cp1, ColoredPoint cp2)
        {
            int x0 = cp0.X;
            int y0 = cp0.Y;
            int x1 = cp1.X;
            int y1 = cp1.Y;
            int x2 = cp2.X;
            int y2 = cp2.Y;

            // Fij(x, y, xi, xj, yi, yj) = (yi - yj) * x + (xj - xi) * y + xi * yj - xj * yi

            float d_a = (y1 - y2) * x0 + (x1 - x2) * y0 + x1 * y2 - x2 * y1;
            float d_b = (y2 - y0) * x1 + (x0 - x2) * y1 + x2 * y0 - x0 * y2;
            float d_c = (y0 - y1) * x2 + (x1 - x0) * y2 + x0 * y1 - x1 * y0;

            float a_i = y1 - y2;
            float a_j = x2 - x1;
            float a_k = x1 * y2 - x2 * y1;
            float b_i = y2 - y0;
            float b_j = x0 - x2;
            float b_k = x2 * y0 - x0 * y2;
            float c_i = y0 - y1;
            float c_j = x1 - x0;
            float c_k = x0 * y1 - x1 * y0;

            int ymin = Math.Min(Math.Min(y0, y1), y2);
            int ymax = Math.Max(Math.Max(y0, y1), y2);
            int xmin = Math.Min(Math.Min(x0, x1), x2);
            int xmax = Math.Max(Math.Max(x0, x1), x2);

            byte* bptr = (byte*)pb.Pixels.ToPointer();
            byte* p;

            float alpha, beta, gamma;

            for (int y = ymin; y < ymax; y++)
                for (int x = xmin; x < xmax; x++)
                {
                    alpha = (a_i * x + a_j * y + a_k) / d_a;
                    beta = (b_i * x + b_j * y + b_k) / d_b;
                    gamma = (c_i * x + c_j * y + c_k) / d_c;

                    if (alpha >= 0 && beta >= 0 && gamma >= 0)
                    {
                        p = bptr + y * pb.Rowstride + x * pb.NChannels;

                        p[0] = (byte)((cp0.R * alpha + cp1.R * beta + cp2.R * gamma) * 255); // r
                        p[1] = (byte)((cp0.G * alpha + cp1.G * beta + cp2.G * gamma) * 255); // g
                        p[2] = (byte)((cp0.B * alpha + cp1.B * beta + cp2.B * gamma) * 255); // b
                        p[3] = (byte)((cp0.A * alpha + cp1.A * beta + cp2.A * gamma) * 255); // a
                    }
                }
        }

        #endregion Utility




        #region Properties

        public static bool IsDrawing
        { get { return _drawing; } }

        public static Pixmap Pixmap
        {
            get { return _pixmap; }
            set { _pixmap = value; }
        }

        public static Thread DrawingThread
        { get { return _drawThread; } }

        #endregion Properties
    }
}
