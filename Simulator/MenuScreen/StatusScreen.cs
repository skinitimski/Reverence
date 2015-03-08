using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{

    internal sealed class StatusLabel : ControlMenu
    {
        public static StatusLabel Instance;

        static StatusLabel()
        {
            Instance = new StatusLabel();
        }
        private StatusLabel()
            : base(
                Globals.WIDTH * 3 / 4,
                Globals.HEIGHT / 20,
                Globals.WIDTH / 4 - 10,
                Globals.HEIGHT / 15)
        { }
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:
                    if (StatusOne.Instance.Visible)
                    {
                        StatusOne.Instance.Visible = false;
                        StatusTwo.Instance.Visible = true;
                    }
                    else if (StatusTwo.Instance.Visible)
                    {
                        StatusTwo.Instance.Visible = false;
                        StatusThree.Instance.Visible = true;
                    }
                    else
                    {
                        StatusThree.Instance.Visible = false;
                        StatusOne.Instance.Visible = true;
                    }
                    break;
                case Key.X:
                    Game.MainMenu.ChangeScreen(MenuScreen.MainScreen);
                    break;
                default:
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, "Status", X + 20, Y + 25);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        public override void SetAsControl()
        {
            base.SetAsControl();

            StatusOne.Instance.Visible = true;
            StatusTwo.Instance.Visible = false;
            StatusThree.Instance.Visible = false;
        }

        public override string Info
        { get { return ""; } }
    }





    internal sealed class StatusOne : Menu
    {
        public static StatusOne Instance;

        const int x0 = 50; // column 1
        const int x1 = 275; // column 2
        const int x3 = 140; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = 330; // fury/sadness, label column (right)
        const int x8 = x7 + 60; // name column (right)
        const int x9 = x8 + 30; // black box column (right)
        const int x11 = 565;
        const int x12 = 670;

        const int y = 50; // name row
        const int ya = y + 30;  // next row 
        const int yb = ya + 25; //    "
        const int yc = yb + 25; //    "

        const int line = 28; // row spacing (left)
        const int yq = 175; // first row (left)
        const int yr = yq + (line * 6) + 12; // 7th row (left)

        const int yh = 365; // first row (right)
        const int yi = yh + 40; // successive rows
        const int yj = yi + 37; //    |
        const int yk = yj + 40; //    | 
        const int yl = yk + 37; //    x

        const int xs = yl - yk;
        const int ys = 13;
        const int zs = 5;

        const int xpic = 15;
        const int ypic = 15;

        static StatusOne()
        {
            Instance = new StatusOne();
        }
        private StatusOne()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT * 9 / 10)
        {
            Visible = true;
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);




            TextExtents te;

            string lvl, hp, hpm, mp, mpm, exp, next, llvl;


            #region Top Row

            d.DrawPixbuf(gc, Graphics.GetProfile(Selected.Name), 0, 0,
                X + xpic, Y + ypic,
                Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT,
                Gdk.RgbDither.None, 0, 0);

            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x3, Y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + yc);
            g.ShowText("MP");
            g.Color = new Color(1, 1, 1);

            Graphics.ShadowedText(g, Selected.Name, X + x3, Y + y);

            lvl = Selected.Level.ToString();
            hp = Selected.HP.ToString() + "/";
            hpm = Selected.MaxHP.ToString();
            mp = Selected.MP.ToString() + "/";
            mpm = Selected.MaxMP.ToString();
            exp = Selected.Exp.ToString();
            next = Selected.ToNextLevel.ToString();
            llvl = Selected.LimitLevel.ToString();

            te = g.TextExtents(lvl);
            Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + ya);

            te = g.TextExtents(hp);
            Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + yb);

            te = g.TextExtents(hpm);
            Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + yb);

            te = g.TextExtents(mp);
            Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + yc);

            te = g.TextExtents(mpm);
            Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + yc);

            Graphics.ShadowedText(g, "Exp:", X + x8, Y + ya);
            Graphics.ShadowedText(g, "Next lvl:", X + x8, Y + yb);
            Graphics.ShadowedText(g, "Limit lvl:", X + x8, Y + yc);

            te = g.TextExtents(exp);
            Graphics.ShadowedText(g, exp, X + x12 - te.Width, Y + ya);
            te = g.TextExtents(next);
            Graphics.ShadowedText(g, next, X + x12 - te.Width, Y + yb);
            te = g.TextExtents(llvl);
            Graphics.ShadowedText(g, llvl, X + x11 - te.Width, Y + yc);

            #endregion Top


            #region Left

            string str, vit, dex, mag, spi, lck;
            string atk, atkp, def, defp, mat, mdf, mdfp;

            str = Selected.Strength.ToString();
            vit = Selected.Vitality.ToString();
            dex = Selected.Dexterity.ToString();
            mag = Selected.Magic.ToString();
            spi = Selected.Spirit.ToString();
            lck = Selected.Luck.ToString();

            atk = Ally.Attack(Selected).ToString();
            atkp = Ally.AttackPercent(Selected).ToString();
            def = Ally.Defense(Selected).ToString();
            defp = Ally.DefensePercent(Selected).ToString();
            mat = Ally.MagicAttack(Selected).ToString();
            mdf = Ally.MagicDefense(Selected).ToString();
            mdfp = Ally.MagicDefensePercent(Selected).ToString();

            Cairo.Color greenish = new Color(.3, .8, .8);

            Graphics.ShadowedText(g, greenish, "Strength", X + x0, Y + yq + (line * 0));
            Graphics.ShadowedText(g, greenish, "Vitality", X + x0, Y + yq + (line * 1));
            Graphics.ShadowedText(g, greenish, "Dexterity", X + x0, Y + yq + (line * 2));
            Graphics.ShadowedText(g, greenish, "Magic", X + x0, Y + yq + (line * 3));
            Graphics.ShadowedText(g, greenish, "Spirit", X + x0, Y + yq + (line * 4));
            Graphics.ShadowedText(g, greenish, "Luck", X + x0, Y + yq + (line * 5));

            Graphics.ShadowedText(g, greenish, "Attack", X + x0, Y + yr + (line * 0));
            Graphics.ShadowedText(g, greenish, "Attack %", X + x0, Y + yr + (line * 1));
            Graphics.ShadowedText(g, greenish, "Defense", X + x0, Y + yr + (line * 2));
            Graphics.ShadowedText(g, greenish, "Defense %", X + x0, Y + yr + (line * 3));
            Graphics.ShadowedText(g, greenish, "Magic", X + x0, Y + yr + (line * 4));
            Graphics.ShadowedText(g, greenish, "Magic def", X + x0, Y + yr + (line * 5));
            Graphics.ShadowedText(g, greenish, "Magic def %", X + x0, Y + yr + (line * 6));

            te = g.TextExtents(str);
            Graphics.ShadowedText(g, str, X + x1 - te.Width, Y + yq + (line * 0));
            te = g.TextExtents(vit);
            Graphics.ShadowedText(g, vit, X + x1 - te.Width, Y + yq + (line * 1));
            te = g.TextExtents(dex);
            Graphics.ShadowedText(g, dex, X + x1 - te.Width, Y + yq + (line * 2));
            te = g.TextExtents(mag);
            Graphics.ShadowedText(g, mag, X + x1 - te.Width, Y + yq + (line * 3));
            te = g.TextExtents(spi);
            Graphics.ShadowedText(g, spi, X + x1 - te.Width, Y + yq + (line * 4));
            te = g.TextExtents(lck);
            Graphics.ShadowedText(g, lck, X + x1 - te.Width, Y + yq + (line * 5));

            te = g.TextExtents(atk);
            Graphics.ShadowedText(g, atk, X + x1 - te.Width, Y + yr + (line * 0));
            te = g.TextExtents(atkp);
            Graphics.ShadowedText(g, atkp, X + x1 - te.Width, Y + yr + (line * 1));
            te = g.TextExtents(def);
            Graphics.ShadowedText(g, def, X + x1 - te.Width, Y + yr + (line * 2));
            te = g.TextExtents(defp);
            Graphics.ShadowedText(g, defp, X + x1 - te.Width, Y + yr + (line * 3));
            te = g.TextExtents(mat);
            Graphics.ShadowedText(g, mat, X + x1 - te.Width, Y + yr + (line * 4));
            te = g.TextExtents(mdf);
            Graphics.ShadowedText(g, mdf, X + x1 - te.Width, Y + yr + (line * 5));
            te = g.TextExtents(mdfp);
            Graphics.ShadowedText(g, mdfp, X + x1 - te.Width, Y + yr + (line * 6));

            #endregion Left


            #region Right


            g.Color = new Color(.1, .1, .2);
            g.Rectangle(x9, yi, 8 * xs, yj - yi);
            g.Fill();
            g.Rectangle(x9, yk, 8 * xs, yl - yk);
            g.Fill();

            Cairo.Color gray1 = new Color(.2, .2, .2);
            Cairo.Color gray2 = new Color(.7, .7, .8);

            int links, slots;

            slots = Selected.Weapon.Slots.Length;
            links = Selected.Weapon.Links;


            for (int j = 0; j < links; j++)
            {
                Graphics.RenderLine(g, gray2, 3,
                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yi - ys - zs,
                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yi - ys - zs);
                Graphics.RenderLine(g, gray2, 3,
                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yi - ys,
                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yi - ys);
                Graphics.RenderLine(g, gray2, 3,
                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yi - ys + zs,
                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yi - ys + zs);
            }
            for (int i = 0; i < slots; i++)
            {
                Graphics.RenderCircle(g, gray2, 14,
                    X + x9 + (i * xs) + (xs / 2), Y + yi - ys);
                if (Selected.Weapon.Slots[i] == null)
                    Graphics.RenderCircle(g, gray1, 10,
                        X + x9 + (i * xs) + (xs / 2), Y + yi - ys);
                else
                    Graphics.RenderCircle(g, Selected.Weapon.Slots[i].Color, 10,
                        X + x9 + (i * xs) + (xs / 2), Y + yi - ys);
            }

            slots = Selected.Armor.Slots.Length;
            links = Selected.Armor.Links;

            for (int j = 0; j < links; j++)
            {
                Graphics.RenderLine(g, gray2, 3,
                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yk - ys - zs,
                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yk - ys - zs);
                Graphics.RenderLine(g, gray2, 3,
                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yk - ys,
                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yk - ys);
                Graphics.RenderLine(g, gray2, 3,
                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yk - ys + zs,
                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yk - ys + zs);
            }
            for (int i = 0; i < slots; i++)
            {
                Graphics.RenderCircle(g, gray2, 14,
                    X + x9 + (i * xs) + (xs / 2), Y + yk - ys);

                if (Selected.Armor.Slots[i] == null)
                    Graphics.RenderCircle(g, gray1, 10,
                        X + x9 + (i * xs) + (xs / 2), Y + yk - ys);
                else
                    Graphics.RenderCircle(g, Selected.Armor.Slots[i].Color, 10,
                        X + x9 + (i * xs) + (xs / 2), Y + yk - ys);
            }


            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x7, Y + yh);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.MoveTo(X + x7, Y + yl);
            g.ShowText("Acc.");
            g.Color = new Color(1, 1, 1);

            Graphics.ShadowedText(g, Selected.Weapon.Name, X + x8, Y + yh);
            Graphics.ShadowedText(g, Selected.Armor.Name, X + x8, Y + yj);
            Graphics.ShadowedText(g, Selected.Accessory.Name, X + x8, Y + yl);

            #endregion Right


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

    internal sealed class StatusTwo : Menu
    {
        public static StatusTwo Instance;

        const int x0 = 50; // labels
        const int x1 = 65; // row start

        const int x3 = 140; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = 330; // fury/sadness

        const int y = 50; // name row
        const int ya = y + 30;  // next row 
        const int yb = ya + 25; //    "
        const int yc = yb + 25; //    "

        const int ym = 200;
        const int yn = ym + 80;
        const int yo = yn + 80;
        const int yp = yo + 80;
        const int ys = 24;

        const int xpic = 15;
        const int ypic = 15;

        const int stop = 120;

        static StatusTwo()
        {
            Instance = new StatusTwo();
        }
        private StatusTwo()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT * 9 / 10)
        {
            Visible = false;
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            Cairo.Color greenish = new Color(.3, .8, .8);
            Cairo.Color gray = new Color(.4, .4, .4);
            Cairo.Color white = new Color(1, 1, 1);

            string lvl, hp, hpm, mp, mpm, s;


            #region Character Status

            d.DrawPixbuf(gc, Graphics.GetProfile(Selected.Name), 0, 0,
                X + xpic, Y + ypic,
                Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT,
                Gdk.RgbDither.None, 0, 0);

            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x3, Y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + yc);
            g.ShowText("MP");
            g.Color = new Color(1, 1, 1);

            Graphics.ShadowedText(g, Selected.Name, X + x3, Y + y);

            lvl = Selected.Level.ToString();
            hp = Selected.HP.ToString() + "/";
            hpm = Selected.MaxHP.ToString();
            mp = Selected.MP.ToString() + "/";
            mpm = Selected.MaxMP.ToString();

            te = g.TextExtents(lvl);
            Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + ya);

            te = g.TextExtents(hp);
            Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + yb);

            te = g.TextExtents(hpm);
            Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + yb);

            te = g.TextExtents(mp);
            Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + yc);

            te = g.TextExtents(mpm);
            Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + yc);

            #endregion Status
            


            Graphics.ShadowedText(g, greenish, "Attack", X + x0, Y + ym);
            Graphics.ShadowedText(g, greenish, "Halve", X + x0, Y + yn);
            Graphics.ShadowedText(g, greenish, "Void", X + x0, Y + yo);
            Graphics.ShadowedText(g, greenish, "Absorb", X + x0, Y + yp);

            g.SetFontSize(16);

            double x = (double)x1;
            double r = (double)(ym + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }

                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Selected.Halves(e))
                    Graphics.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Graphics.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }

            x = (double)x1;
            r = (double)(yn + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }

                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Selected.Halves(e))
                    Graphics.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Graphics.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }

            x = (double)x1;
            r = (double)(yo + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }

                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Selected.Voids(e))
                    Graphics.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Graphics.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }

            x = (double)x1;
            r = (double)(yp + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }

                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Selected.Absorbs(e))
                    Graphics.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Graphics.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }




            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

    internal sealed class StatusThree : Menu
    {
        public static StatusThree Instance;

        const int x0 = 50; // column 1
        const int x1 = 275; // column 2
        const int x3 = 140; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = 330; // fury/sadness

        const int y = 50; // name row
        const int ya = y + 30;  // next row 
        const int yb = ya + 25; //    "
        const int yc = yb + 25; //    "

        const int xpic = 15;
        const int ypic = 15;

        static StatusThree()
        {
            Instance = new StatusThree();
        }
        private StatusThree()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT * 9 / 10)
        {
            Visible = false;
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            TextExtents te;

            string lvl, hp, hpm, mp, mpm;


            #region Character Status

            d.DrawPixbuf(gc, Graphics.GetProfile(Selected.Name), 0, 0,
                X + xpic, Y + ypic,
                Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT,
                Gdk.RgbDither.None, 0, 0);

            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x3, Y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + yc);
            g.ShowText("MP");
            g.Color = new Color(1, 1, 1);

            Graphics.ShadowedText(g, Selected.Name, X + x3, Y + y);

            lvl = Selected.Level.ToString();
            hp = Selected.HP.ToString() + "/";
            hpm = Selected.MaxHP.ToString();
            mp = Selected.MP.ToString() + "/";
            mpm = Selected.MaxMP.ToString();

            te = g.TextExtents(lvl);
            Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + ya);

            te = g.TextExtents(hp);
            Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + yb);

            te = g.TextExtents(hpm);
            Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + yb);

            te = g.TextExtents(mp);
            Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + yc);

            te = g.TextExtents(mpm);
            Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + yc);

            #endregion Status



            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}
