using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class VictoryLabel : ControlMenu
    {
        public static VictoryLabel Instance;

        #region Layout

        const int xs = 20;
        const int ys = 28;

        #endregion Layout

        static VictoryLabel()
        {
            Instance = new VictoryLabel();
        }
        private VictoryLabel()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT / 12 - 6)
        { }
        public override void ControlHandle(Key k) { switch (k) { default: break; } }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, "Gained Experience points and AP.", X + xs, Y + ys);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        public override string Info { get { return ""; } }
    }



    internal sealed class VictoryEXP : Menu
    {
        public static VictoryEXP Instance;

        #region Layout

        const int x1 = 100;
        const int x2 = 300;
        const int ys = 28;

        #endregion

        static VictoryEXP()
        {
            Instance = new VictoryEXP();
        }
        private VictoryEXP()
            : base(
                2,
                Globals.HEIGHT  * 2 / 15,
                Globals.WIDTH / 2,
                Globals.HEIGHT / 12 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string exp = Game.PostBattle.Exp.ToString() + "p";
            te = g.TextExtents(exp);
            Graphics.ShadowedText(g, "EXP", X + x1, Y + ys);
            Graphics.ShadowedText(g, exp, X + x2 - te.Width, Y + ys);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }



    internal sealed class VictoryAP : Menu
    {
        public static VictoryAP Instance;

        #region Layout

        const int x1 = 100;
        const int x2 = 250;
        const int ys = 28;
        #endregion

        static VictoryAP()
        {
            Instance = new VictoryAP();
        }
        private VictoryAP()
            : base(
                Globals.WIDTH / 2,
                Globals.HEIGHT * 2 / 15,
                Globals.WIDTH / 2 - 8,
                Globals.HEIGHT / 12 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string ap = Game.PostBattle.AP.ToString() + "p";
            te = g.TextExtents(ap);
            Graphics.ShadowedText(g, "AP", X + x1, Y + ys);
            Graphics.ShadowedText(g, ap, X + x2 - te.Width, Y + ys);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }



    internal sealed class VictoryTop : Menu
    {
        public static VictoryTop Instance;

        #region Layout

        const int x1 = 140;
        const int x2 = 170;
        const int x3 = x2 + 120;
        const int x4 = 600;
        const int x5 = x4 + 180;
        const int y0 = 50;
        const int y1 = 100;

        const int xpic = 15;
        const int ypic = 15;

        #endregion

        static VictoryTop()
        {
            Instance = new VictoryTop();
        }
        private VictoryTop()
            : base(
                2,
                Globals.HEIGHT * 13 / 60,
                Globals.WIDTH - 8,
                Globals.HEIGHT / 4 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            Character c = Globals.Party[0];
            if (c != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfileSmall(c.Name), 0, 0,
                    X + xpic, Y + ypic,
                    Graphics.PROFILE_WIDTH_SMALL, Graphics.PROFILE_HEIGHT_SMALL,
                    Gdk.RgbDither.None, 0, 0);

                Graphics.ShadowedText(g, c.Name, X + x1, Y + y0);
                Graphics.ShadowedText(g, "Level:", X + x2, Y + y1);

                string lvl = c.Level.ToString();
                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x3 - te.Width, Y + y1);

                string temp = "Exp:";
                te = g.TextExtents(temp);
                Graphics.ShadowedText(g, temp, X + x4 - te.Width, Y + y0);

                temp = "For level up:";
                te = g.TextExtents(temp);
                Graphics.ShadowedText(g, temp, X + x4 - te.Width, Y + y1);

                string exp = c.Exp.ToString() + "p";
                te = g.TextExtents(exp);
                Graphics.ShadowedText(g, exp, X + x5 - te.Width, Y + y0);

                string expNext = c.ToNextLevel.ToString() + "p";
                te = g.TextExtents(expNext);
                Graphics.ShadowedText(g, expNext, X + x5 - te.Width, Y + y1);
            }


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }



    internal sealed class VictoryMiddle : Menu
    {
        public static VictoryMiddle Instance;

        #region Layout

        const int x1 = 140;
        const int x2 = 170;
        const int x3 = x2 + 120;
        const int x4 = 600;
        const int x5 = x4 + 180;
        const int y0 = 50;
        const int y1 = 100;

        const int xpic = 15;
        const int ypic = 15;

        #endregion

        static VictoryMiddle()
        {
            Instance = new VictoryMiddle();
        }
        private VictoryMiddle()
            : base(
                2,
                Globals.HEIGHT * 28 / 60,
                Globals.WIDTH - 8,
                Globals.HEIGHT / 4 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            Character c = Globals.Party[1];
            if (c != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfileSmall(c.Name), 0, 0,
                    X + xpic, Y + ypic,
                    Graphics.PROFILE_WIDTH_SMALL, Graphics.PROFILE_HEIGHT_SMALL,
                    Gdk.RgbDither.None, 0, 0);

                Graphics.ShadowedText(g, c.Name, X + x1, Y + y0);
                Graphics.ShadowedText(g, "Level:", X + x2, Y + y1);

                string lvl = c.Level.ToString();
                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x3 - te.Width, Y + y1);

                string temp = "Exp:";
                te = g.TextExtents(temp);
                Graphics.ShadowedText(g, temp, X + x4 - te.Width, Y + y0);

                temp = "For level up:";
                te = g.TextExtents(temp);
                Graphics.ShadowedText(g, temp, X + x4 - te.Width, Y + y1);

                string exp = c.Exp.ToString() + "p";
                te = g.TextExtents(exp);
                Graphics.ShadowedText(g, exp, X + x5 - te.Width, Y + y0);

                string expNext = c.ToNextLevel.ToString() + "p";
                te = g.TextExtents(expNext);
                Graphics.ShadowedText(g, expNext, X + x5 - te.Width, Y + y1);
            }


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }



    internal sealed class VictoryBottom : Menu
    {
        public static VictoryBottom Instance;

        #region Layout

        const int x1 = 140;
        const int x2 = 170;
        const int x3 = x2 + 120;
        const int x4 = 600;
        const int x5 = x4 + 180;
        const int y0 = 50;
        const int y1 = 100;

        const int xpic = 15;
        const int ypic = 15;

        #endregion

        static VictoryBottom()
        {
            Instance = new VictoryBottom();
        }
        private VictoryBottom()
            : base(
                2,
                Globals.HEIGHT * 43 / 60,
                Globals.WIDTH - 8,
                Globals.HEIGHT / 4 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            Character c = Globals.Party[2];
            if (c != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfileSmall(c.Name), 0, 0,
                    X + xpic, Y + ypic,
                    Graphics.PROFILE_WIDTH_SMALL, Graphics.PROFILE_HEIGHT_SMALL,
                    Gdk.RgbDither.None, 0, 0);

                Graphics.ShadowedText(g, c.Name, X + x1, Y + y0);
                Graphics.ShadowedText(g, "Level:", X + x2, Y + y1);

                string lvl = c.Level.ToString();
                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x3 - te.Width, Y + y1);

                string temp = "Exp:";
                te = g.TextExtents(temp);
                Graphics.ShadowedText(g, temp, X + x4 - te.Width, Y + y0);

                temp = "For level up:";
                te = g.TextExtents(temp);
                Graphics.ShadowedText(g, temp, X + x4 - te.Width, Y + y1);

                string exp = c.Exp.ToString() + "p";
                te = g.TextExtents(exp);
                Graphics.ShadowedText(g, exp, X + x5 - te.Width, Y + y0);

                string expNext = c.ToNextLevel.ToString() + "p";
                te = g.TextExtents(expNext);
                Graphics.ShadowedText(g, expNext, X + x5 - te.Width, Y + y1);
            }


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}