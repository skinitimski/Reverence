using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class PHSLabel : Menu
    {
        public static PHSLabel Instance;

        static PHSLabel()
        {
            Instance = new PHSLabel();
        }
        private PHSLabel()
            : base(
                Globals.WIDTH * 3 / 4,
                Globals.HEIGHT / 20,
                Globals.WIDTH / 4 - 10,
                Globals.HEIGHT / 15)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, "PHS", X + 20, Y + 25);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }




    internal sealed class PHSTop : Menu
    {
        public static PHSTop Instance;

        static PHSTop()
        {
            Instance = new PHSTop();
        }
        private PHSTop()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, "Please Make a Party of Three", X + 50, Y + 25);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

    


    internal sealed class PHSStats : ControlMenu
    {
        public static PHSStats Instance;

        #region Layout

        const int x1 = 30; // pic
        const int x3 = 170; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = x3 + 110; // fury/sadness
        const int cx = 15;
        const int yp = 15;

        const int y0 = 60;
        const int y1 = 210;
        const int y2 = 360;
        const int ya = 30;
        const int yb = 55;
        const int yc = 80;

        #endregion Layout

        private int option;
        private int cy = y0;

        static PHSStats()
        {
            Instance = new PHSStats();
        }
        private PHSStats()
            : base(
                2,
                Globals.HEIGHT * 7 / 60,
                Globals.WIDTH / 2 - 6,
                Globals.HEIGHT * 5 / 6)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    break;
                case Key.Down:
                    if (option < 2) option++;
                    break;
                case Key.X:
                    Game.MainMenu.ChangeScreen(MenuScreen.MainScreen);
                    break;
                case Key.Circle:
                    MenuScreen.PhsScreen.ChangeControl(PHSList.Instance);
                    break;
            }
            switch (option)
            {
                case 0: cy = y0; break;
                case 1: cy = y1; break;
                case 2: cy = y2; break;
                default: break;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string lvl, hp, hpm, mp, mpm;


            #region Character 1

            if (Globals.Party[0] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[0].Name), 0, 0,
                    X + x1, Y + yp,
                    Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);
                
                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y0 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y0 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y0 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);

                Color namec = new Color(1, 1, 1);
                if (Globals.Party[0].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Globals.Party[0].NearDeath)
                    namec = new Color(.8, .8, 0);

                Graphics.ShadowedText(g, namec, Globals.Party[0].Name, X + x3, Y + y0);

                lvl = Globals.Party[0].Level.ToString();
                hp = Globals.Party[0].HP.ToString() + "/";
                hpm = Globals.Party[0].MaxHP.ToString();
                mp = Globals.Party[0].MP.ToString() + "/";
                mpm = Globals.Party[0].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y0 + ya);
                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y0 + yb);
                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y0 + yb);
                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y0 + yc);
                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y0 + yc);
            }
            #endregion Character 1


            #region Character 2

            if (Globals.Party[1] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[1].Name), 0, 0,
                    X + x1, Y + yp + (y1 - y0),
                    Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);

                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y1 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y1 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y1 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);

                Color namec = new Color(1, 1, 1);
                if (Globals.Party[1].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Globals.Party[1].NearDeath)
                    namec = new Color(.8, .8, 0);

                Graphics.ShadowedText(g, namec, Globals.Party[1].Name, X + x3, Y + y1);

                lvl = Globals.Party[1].Level.ToString();
                hp = Globals.Party[1].HP.ToString() + "/";
                hpm = Globals.Party[1].MaxHP.ToString();
                mp = Globals.Party[1].MP.ToString() + "/";
                mpm = Globals.Party[1].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y1 + ya);
                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y1 + yb);
                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y1 + yb);
                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y1 + yc);
                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y1 + yc);
            }

            #endregion Character 2


            #region Character 3

            if (Globals.Party[2] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[2].Name), 0, 0,
                    X + x1, Y + yp + (y2 - y0),
                    Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);

                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y2 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y2 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y2 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);

                Color namec = new Color(1, 1, 1);
                if (Globals.Party[2].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Globals.Party[2].NearDeath)
                    namec = new Color(.8, .8, 0);

                Graphics.ShadowedText(g, namec, Globals.Party[2].Name, X + x3, Y + y2);

                lvl = Globals.Party[2].Level.ToString();
                hp = Globals.Party[2].HP.ToString() + "/";
                hpm = Globals.Party[2].MaxHP.ToString();
                mp = Globals.Party[2].MP.ToString() + "/";
                mpm = Globals.Party[2].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y2 + ya);
                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y2 + yb);
                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y2 + yb);
                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y2 + yc);
                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y2 + yc);
            }

            #endregion Character 3


            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            PHSList.Instance.Update();
        }

        public int Option { get { return option; } }

        public override string Info
        {  get { return ""; } }
    }




    internal sealed class PHSList : ControlMenu
    {
        public static PHSList Instance;

        private Character[,] _characters;

        #region Layout

        const int x = 50;
        const int y = 10;
        const int xs = 100;
        const int ys = 110;
        static int cx = x - 10;
        static int cy = y + 50;

        #endregion

        private int optionX;
        private int optionY;


        static PHSList()
        {
            Instance = new PHSList();
        }
        private PHSList()
            : base(
                Globals.WIDTH / 2,
                Globals.HEIGHT * 22 / 60,
                Globals.WIDTH / 2 - 9,
                Globals.HEIGHT * 7 / 12)
        { 
            _characters = new Character[3,3];
        }
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (optionY > 0) optionY--;
                    break;
                case Key.Down:
                    if (optionY < 2) optionY++;
                    break;
                case Key.Left:
                    if (optionX > 0) optionX--;
                    break;
                case Key.Right:
                    if (optionX < 2) optionX++;
                    break;
                case Key.Circle:
                    Character temp = Globals.Reserves[optionY, optionX];
                    Globals.Reserves[optionY, optionX] = Globals.Party[PHSStats.Instance.Option];
                    Globals.Party[PHSStats.Instance.Option] = temp;
                    Update();
                    MenuScreen.PhsScreen.ChangeControl(PHSStats.Instance);
                    break;
                case Key.X:
                    MenuScreen.PhsScreen.ChangeControl(PHSStats.Instance);
                    break;
                default:
                    break;
            }
            switch (optionX)
            {
                case 0: cx = x - 10; break;
                case 1: cx = x - 10 + xs; break;
                case 2: cx = x - 10 + xs + xs; break;
            }
            switch (optionY)
            {
                case 0: cy = y + 50; break;
                case 1: cy = y + 50 + ys; break;
                case 2: cy = y + 50 + ys + ys; break;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            for (int a = 0; a <= 2; a++)
                for (int b = 0; b <= 2; b++)
                    if (Globals.Reserves[a,b] != null)
                        Graphics.DrawProfileSmall(d, gc, Globals.Reserves[a,b].Name,
                            X + x + b * xs, Y + y + a * ys);
            
            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy - 15);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public void Update()
        {
            for (int a = 0; a <= 2; a++)
                for (int b = 0; b <= 2; b++)
                    if (b + (3 * a) < Globals.Reserves.Length)
                        _characters[a, b] = Globals.Reserves[b, a];
            optionX = 0;
            optionY = 0;
            cx = x - 10;
            cy = y + 50;
        }

        public Character Selection
        { get { return Globals.Reserves[optionY, optionX]; } }

        public override string Info
        { get { return (Selection == null) ? "" : Selection.Name; } }
    }




    internal sealed class PHSInfo : Menu
    {
        public static PHSInfo Instance;

        #region Layout

        const int x1 = 30; // pic
        const int x3 = 170; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm

        const int yp = 15;
        const int y0 = 60;
        const int ya = 30;
        const int yb = 55;
        const int yc = 80;

        #endregion Layout

        static PHSInfo()
        {
            Instance = new PHSInfo();
        }
        private PHSInfo()
            : base(
                Globals.WIDTH / 2,
                Globals.HEIGHT * 7 / 60,
                Globals.WIDTH / 2 - 9,
                Globals.HEIGHT / 4)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string lvl, hp, hpm, mp, mpm;

            #region Character

            Character c = PHSList.Instance.Selection;

            if (c != null)
            {
                Graphics.DrawProfileSmall(d, gc, c.Name, X + x1, Y + yp);

                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y0 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y0 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y0 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);

                Color namec = new Color(1, 1, 1);
                if (c.Death)
                    namec = new Color(0.8, 0, 0);
                else if (c.NearDeath)
                    namec = new Color(.8, .8, 0);

                Graphics.ShadowedText(g, namec, c.Name, X + x3, Y + y0);

                lvl = c.Level.ToString();
                hp = c.HP.ToString() + "/";
                hpm = c.MaxHP.ToString();
                mp = c.MP.ToString() + "/";
                mpm = c.MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y0 + ya);
                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y0 + yb);
                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y0 + yb);
                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y0 + yc);
                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y0 + yc);
            }
            #endregion Character


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}