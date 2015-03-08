using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class MateriaTop : ControlMenu
    {
        public static MateriaTop Instance;

        #region Layout

        const int x3 = 140;
        const int x4 = x3 + 65;
        const int x5 = x4 + 42;
        const int x6 = x5 + 65;
        const int x7 = 330; // wpn.
        const int x7a = x7 + 25;
        const int x8 = x7 + 65; // wpn name
        const int x9 = x8 + 40; // box

        const int y = 50; // top row
        const int ya = 30; // subrow 1
        const int yb = 55; // subrow 2
        const int yc = 80; // subrow 3

        const int yh = 43;      // weapon
        const int yi = yh + 40; //  -subrow
        const int yj = yi + 35; // armor
        const int yja = yj - 8; // check
        const int yk = yj + 40; //  -subrow
        const int yl = yk + 35; // end
        const int yla = yl - 8; // arrange

        const int xpic = 15;
        const int ypic = 15;

        const int xs = (yl - yk) * 9 / 8;
        const int ys = 13;
        const int zs = 5;

        #endregion

        private int optionX = 0;
        private int optionY = 0;
        private int cx = x9;
        private int cy = yi;

        static MateriaTop()
        {
            Instance = new MateriaTop();
        }
        private MateriaTop()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT * 3 / 10 - 6)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (optionY > 0) optionY--;
                    break;
                case Key.Down:
                    if (optionY < 1) optionY++;
                    break;
                case Key.Left:
                    if (optionX > -1) optionX--;
                    break;
                case Key.Right:
                    if (optionX < 7) optionX++;
                    break;
                case Key.X:
                    Game.MainMenu.ChangeScreen(MenuScreen.MainScreen);
                    break;
                case Key.Square:
                    Game.MainMenu.ChangeScreen(MenuScreen.EquipScreen);
                    break;
                case Key.Circle:
                    if (optionX == -1)
                        switch (optionY)
                        {
                            case 0: // check
                                break;
                            case 1: // arrange
                                MenuScreen.MateriaScreen.ChangeControl(MateriaArrange.Instance);
                                break;
                            default: break;
                        }
                    else
                        switch (optionY)
                        {
                            case 0:
                                if (optionX < Selected.Weapon.Slots.Length)
                                    MenuScreen.MateriaScreen.ChangeControl(MateriaList.Instance);
                                break;
                            case 1:
                                if (optionX < Selected.Armor.Slots.Length)
                                    MenuScreen.MateriaScreen.ChangeControl(MateriaList.Instance);
                                break;
                        }
                    break;
                case Key.Triangle:
                    if (optionX == -1)
                        break;
                    Materia orb;
                    switch (optionY)
                    {
                        case 0:
                            orb = Selected.Weapon.Slots[optionX];
                            if (orb != null)
                            {
                                Selected.Weapon.Slots[optionX] = null;
                                orb.Detach(Selected);
                                Materiatory.Put(orb);
                            }
                            break;
                        case 1:
                            orb = Selected.Armor.Slots[optionX];
                            if (orb != null)
                            {
                                Selected.Armor.Slots[optionX] = null;
                                orb.Detach(Selected);
                                Materiatory.Put(orb);
                            }
                            break;
                        default: break;
                    }
                    break;
                default:
                    break;
            }
            switch (optionY)
            {
                case 0: cy = yi; break;
                case 1: cy = yk; break;
                default: break;
            }
            if (optionX == -1)
                cx = x7;
            else cx = x9 + optionX * xs;
        }


        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string lvl, hp, hpm, mp, mpm;
            string weapon, armor;


            #region Slots

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

            #endregion

            #region Character Status

            d.DrawPixbuf(gc, Graphics.GetProfile(Selected.Name), 0, 0,
                X + xpic, Y + ypic,
                Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT,
                Gdk.RgbDither.None, 0, 0);

            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x3, Y + y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + y + yc);
            g.ShowText("MP");
            g.Color = new Color(1, 1, 1);

            Graphics.ShadowedText(g, Selected.Name, X + x3, Y + y);

            lvl = Selected.Level.ToString();
            hp = Selected.HP.ToString() + "/";
            hpm = Selected.MaxHP.ToString();
            mp = Selected.MP.ToString() + "/";
            mpm = Selected.MaxMP.ToString();

            te = g.TextExtents(lvl);
            Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y + ya);
            te = g.TextExtents(hp);
            Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y + yb);
            te = g.TextExtents(hpm);
            Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y + yb);
            te = g.TextExtents(mp);
            Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y + yc);
            te = g.TextExtents(mpm);
            Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y + yc);

            #endregion Status

            #region Equipment

            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x7, Y + yh);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.Color = new Color(1, 1, 1);

            Graphics.ShadowedText(g, "Check", x7a, yja);
            Graphics.ShadowedText(g, "Arr.", x7a, yla);

            weapon = Selected.Weapon.Name;
            armor = Selected.Armor.Name;

            te = g.TextExtents(weapon);
            Graphics.ShadowedText(g, weapon, X + x8, Y + yh); 
            te = g.TextExtents(armor);
            Graphics.ShadowedText(g, armor, X + x8, Y + yj);

            #endregion 


            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy - ys);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public int OptionX { get { return optionX; } }
        public int OptionY { get { return optionY; } }
        public override string Info
        {
            get
            {
                if (optionX == -1)
                    return "";
                switch (optionY)
                {
                    case 0:
                        if (optionX < Selected.Weapon.Slots.Length)
                            if (Selected.Weapon.Slots[optionX] != null)
                                return Selected.Weapon.Slots[optionX].Description;
                        return "";
                    case 1:
                        if (optionX < Selected.Armor.Slots.Length)
                        if (Selected.Armor.Slots[optionX] != null)
                            return Selected.Armor.Slots[optionX].Description;
                        return "";
                    default: return "";
                }
            }
        }
        public Materia Selection
        {
            get
            {
                if (IsControl)
                {
                    if (optionX == -1)
                        return null;
                    switch (optionY)
                    {
                        case 0:
                            if (optionX < Selected.Weapon.Slots.Length)
                                if (Selected.Weapon.Slots[optionX] != null)
                                    return Selected.Weapon.Slots[optionX];
                            return null;
                        case 1:
                            if (optionX < Selected.Armor.Slots.Length)
                                if (Selected.Armor.Slots[optionX] != null)
                                    return Selected.Armor.Slots[optionX];
                            return null;
                        default: return null;
                    }
                }
                else return null;
            }
        }
    }




    internal sealed class MateriaStats : Menu
    {
        public static MateriaStats Instance;

        #region Layout

        const int x0 = 10; // first column
        const int x2 = 60; // second column
        const int x1 = x2 - 20; // orb column
        const int x4 = 300; // equip eff
        const int x5 = x4 + 30; // ap
        const int x8 = 485; // end
        const int x7 = x8 - 15; // end
        const int x6 = x8 - 35; // star-slash

        const int y0 = 40; // top row
        const int yp = y0 - 7; // orb row
        const int y1 = y0 + 30; // AP
        const int y2 = y1 + 27; // nxt lvl
        const int y3 = y2 + 40; // titles
        const int y4 = y3 + 30; // first ability row
        const int ys = 27;


        #endregion

        static MateriaStats()
        {
            Instance = new MateriaStats();
        }
        private MateriaStats()
            : base(
                2,
                Globals.HEIGHT * 5 / 12,
                Globals.WIDTH * 5 / 8,
                Globals.HEIGHT * 8 / 15)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            Materia orb = MateriaTop.Instance.Selection;
            if (orb == null)
                orb = MateriaList.Instance.Selection;


            if (orb != null)
            {
                Graphics.RenderCircle(g, new Color(1, 1, 1), 9, X + x1, Y + yp);
                Graphics.RenderCircle(g, orb.Color, 7, X + x1, Y + yp);
                Graphics.ShadowedText(g, orb.Name, X + x2, Y + y0);

                Cairo.Color greenish = new Color(.3, .8, .8);
                Cairo.Color yellow = new Color(.8, .8, 0);
                Cairo.Color red = new Color(0.8, 0, 0);
                Cairo.Color gray = new Color(.4, .4, .4);
                Cairo.Color white = new Color(1, 1, 1);


                if (orb.ID == "enemyskill")
                {
                    EnemySkillMateria esm = (EnemySkillMateria)orb;

                    string mask = "";
                    for (int i = 0; i < EnemySkillMateria.TOTAL_ENEMY_SKILLS; i++)
                        mask += ((esm.AP >> i) & 1) > 0 ? "1" : "0";
                    Graphics.ShadowedText(g, mask, X + x1, Y + y3);
                }
                else
                {
                    string lvl = (orb.Level + 1).ToString() + "/";
                    te = g.TextExtents(lvl);
                    Graphics.ShadowedText(g, lvl, X + x6 - te.Width, Y + y0);

                    string lvls = orb.Tiers.Length.ToString();
                    te = g.TextExtents(lvls);
                    Graphics.ShadowedText(g, lvls, X + x7 - te.Width, Y + y0);

                    te = g.TextExtents("Level");
                    Graphics.ShadowedText(g, greenish, "Level", X + x5 - te.Width, Y + y0);


                    string ap;
                    if (orb.AP < orb.Tiers[orb.Tiers.Length - 1])
                        ap = orb.AP.ToString();
                    else ap = "MASTER";
                    te = g.TextExtents(ap);
                    Graphics.ShadowedText(g, ap, X + x7 - te.Width, Y + y1);
                    te = g.TextExtents("AP");
                    Graphics.ShadowedText(g, greenish, "AP", X + x5 - te.Width, Y + y1);


                    string nxt;
                    if (orb.Master)
                        nxt = "0";
                    else
                        nxt = (orb.Tiers[orb.Level + 1] - orb.AP).ToString();
                    te = g.TextExtents(nxt);
                    Graphics.ShadowedText(g, nxt, X + x7 - te.Width, Y + y2);
                    te = g.TextExtents("To next level");
                    Graphics.ShadowedText(g, greenish, "To next level", X + x5 - te.Width, Y + y2);


                    Graphics.ShadowedText(g, greenish, "Ability List", X + x0, Y + y3);

                    int k = 0;
                    foreach (string s in orb.AllAbilities)
                        if (!String.IsNullOrEmpty(s))
                        {
                            if (orb.Abilities.Contains(s))
                                Graphics.ShadowedText(g, s, X + x1, Y + y4 + (ys * k));
                            else
                                Graphics.ShadowedText(g, gray, s, X + x1, Y + y4 + (ys * k));
                            k++;
                        }

                    Graphics.ShadowedText(g, greenish, "Equip Effect", X + x4, Y + y3);

                    int i = 0;
                    string stat;

                    te = g.TextExtents("-");
                    double dashw = te.Width;

                    te = g.TextExtents("+");
                    double plusw = te.Width;

                    #region Strength
                    if (orb.StrengthMod != 0)
                    {
                        Graphics.ShadowedText(g, "Str", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.StrengthMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;

                        te = g.TextExtents(stat);
                        if (orb.StrengthMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Strength
                    #region Vitality
                    if (orb.VitalityMod != 0)
                    {
                        Graphics.ShadowedText(g, "Vit", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.VitalityMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;

                        te = g.TextExtents(stat);
                        if (orb.VitalityMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Vitality
                    #region Dexterity
                    if (orb.DexterityMod != 0)
                    {
                        Graphics.ShadowedText(g, "Dex", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.DexterityMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;

                        te = g.TextExtents(stat);
                        if (orb.DexterityMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Dexterity
                    #region Magic
                    if (orb.MagicMod != 0)
                    {
                        Graphics.ShadowedText(g, "Mag", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.MagicMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;

                        te = g.TextExtents(stat);
                        if (orb.MagicMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Magic
                    #region Spirit
                    if (orb.SpiritMod != 0)
                    {
                        Graphics.ShadowedText(g, "Spr", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.SpiritMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;

                        te = g.TextExtents(stat);
                        if (orb.SpiritMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Spirit
                    #region Luck
                    if (orb.LuckMod != 0)
                    {
                        Graphics.ShadowedText(g, "Lck", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.LuckMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;

                        te = g.TextExtents(stat);
                        if (orb.LuckMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x7 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x7 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x7 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion Luck
                    #region HP
                    if (orb.HPMod != 0)
                    {
                        Graphics.ShadowedText(g, "MaxHP", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.HPMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;
                        stat = stat + "%";
                        te = g.TextExtents(stat);
                        if (orb.HPMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x8 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x8 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion HP
                    #region MP
                    if (orb.MPMod != 0)
                    {
                        Graphics.ShadowedText(g, "MaxMP", X + x5, Y + y4 + (ys * i));

                        stat = Math.Abs(orb.MPMod).ToString();
                        if (stat.Length < 2) stat = "0" + stat;
                        stat = stat + "%";
                        te = g.TextExtents(stat);
                        if (orb.MPMod < 0)
                        {
                            Graphics.ShadowedText(g, "-", X + x8 - te.Width - dashw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, red, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        else
                        {
                            Graphics.ShadowedText(g, "+", X + x8 - te.Width - plusw, Y + y4 + (ys * i));
                            Graphics.ShadowedText(g, yellow, stat, X + x8 - te.Width, Y + y4 + (ys * i));
                        }
                        i++;
                    }
                    #endregion MP
                }
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }




    internal sealed class MateriaList : ControlMenu
    {
        public static MateriaList Instance;

        #region Layout

        const int x1 = 40; // orb
        const int x2 = 60; // name
        const int y = 29; // line spacing
        const int cx = 16;
        const int cy = 22;
        const int rows = 10;

        #endregion

        private int option = 0;
        private int topRow = 0;
        private bool trashing = false;

        static MateriaList()
        {
            Instance = new MateriaList();
        }
        private MateriaList()
            : base(
                Globals.WIDTH * 5 / 8,
                Globals.HEIGHT * 5 / 12,
                Globals.WIDTH * 3 / 8 - 8,
                Globals.HEIGHT * 8 / 15)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    if (topRow > option) topRow--;
                    break;
                case Key.Down:
                    if (option < Materiatory.MATERIATORY_SIZE - 1) option++;
                    if (topRow < option - rows + 1) topRow++;
                    break;
                case Key.X:
                    if (trashing)
                        MenuScreen.MateriaScreen.ChangeControl(MateriaArrange.Instance);
                    else
                        MenuScreen.MateriaScreen.ChangeToDefaultControl();
                    trashing = false;
                    break;
                case Key.Circle:
                    if (Trashing)
                    {
                        MenuScreen.MateriaScreen.ChangeControl(MateriaPrompt.Instance);
                        break;
                    }
                    Materia neworb = Materiatory.Get(option);
                    Materia oldorb;
                    switch (MateriaTop.Instance.OptionY)
                    {
                        case 0:
                            oldorb = Selected.Weapon.Slots[MateriaTop.Instance.OptionX];
                            if (oldorb != null)
                                oldorb.Detach(Selected);
                            Materiatory.Put(oldorb, option);
                            if (neworb != null)
                                neworb.Attach(Selected);
                            Selected.Weapon.AttachMateria(neworb, MateriaTop.Instance.OptionX);
                            MenuScreen.MateriaScreen.ChangeToDefaultControl();
                            break;
                        case 1:
                            oldorb = Selected.Armor.Slots[MateriaTop.Instance.OptionX];
                            if (oldorb != null)
                                oldorb.Detach(Selected);
                            Materiatory.Put(oldorb, option);
                            if (neworb != null)
                                neworb.Attach(Selected);
                            Selected.Armor.AttachMateria(neworb, MateriaTop.Instance.OptionX);
                            MenuScreen.MateriaScreen.ChangeToDefaultControl();
                            break;
                        default: break;
                    }
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

            TextExtents te;


            if (IsControl || MateriaArrange.Instance.IsControl || MateriaPrompt.Instance.IsControl)
            {
                int j = Math.Min(rows + topRow, Materiatory.MATERIATORY_SIZE);

                for (int i = topRow; i < j; i++)
                {
                    Materia orb = Materiatory.Get(i);
                    if (orb != null)
                    {
                        Graphics.RenderCircle(g, new Color(1, 1, 1), 9, X + x1, Y + cy + (i - topRow) * y);
                        Graphics.RenderCircle(g, orb.Color, 7, X + x1, Y + cy + (i - topRow) * y);

                        Graphics.ShadowedText(g, orb.Name, X + x2, Y + (i - topRow + 1) * y);
                    }
                }
            }

            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy + (option - topRow) * y);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override string Info
        {
            get
            {
                Materia o = Materiatory.Get(option);
                return (o == null) ? "" : o.Description;
            }
        }
        public int Option { get { return option; } }
        public bool Trashing
        {
            get { return trashing; }
            set { trashing = value; }
        }
        public Materia Selection
        {
            get
            {
                if (IsControl || MateriaPrompt.Instance.IsControl) return Materiatory.Get(option);
                else return null;
            }
        }

    }




    internal sealed class MateriaArrange : ControlMenu
    {
        public static MateriaArrange Instance;

        #region Layout

        const int x = 35;
        const int y = 30;
        const int cx = 20;
        const int cy = 24;

        #endregion Layout

        private int option = 0;

        static MateriaArrange()
        {
            Instance = new MateriaArrange();
        }
        private MateriaArrange()
            : base(
                Globals.WIDTH * 3 / 8,
                150,
                170,
                140)
        {
            Visible = false;
        }
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    break;
                case Key.Down:
                    if (option < 3) option++;
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case 0:
                            Materiatory.Sort();
                            break;
                        case 1:
                            break;
                        case 2:
                            for (int i = 0; i < Selected.Weapon.Slots.Length; i++)
                            {
                                Materia orb = Selected.Weapon.Slots[i];
                                if (orb != null)
                                {
                                    Selected.Weapon.Slots[i] = null;
                                    orb.Detach(Selected);
                                    Materiatory.Put(orb);
                                }
                            }
                            for (int j = 0; j < Selected.Armor.Slots.Length; j++)
                            {
                                Materia orb = Selected.Armor.Slots[j];
                                if (orb != null)
                                {
                                    Selected.Armor.Slots[j] = null;
                                    orb.Detach(Selected);
                                    Materiatory.Put(orb);
                                }
                            }
                            break;
                        case 3:
                            MenuScreen.MateriaScreen.ChangeControl(MateriaList.Instance);
                            MateriaList.Instance.Trashing = true;
                            break;
                        default: break;
                    }
                    break;
                case Key.X:
                    Visible = false;
                    MenuScreen.MateriaScreen.ChangeToDefaultControl();
                    break;
                default: break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy + (option * y));

            Graphics.ShadowedText(g, "Arrange", X + x, Y + (1 * y));
            Graphics.ShadowedText(g, "Exchange", X + x, Y + (2 * y));
            Graphics.ShadowedText(g, "Clear", X + x, Y + (3 * y));
            Graphics.ShadowedText(g, "Trash", X + x, Y + (4 * y));

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
        }


        public override string Info
        { get { return ""; } }

    }




    internal sealed class MateriaPrompt : ControlMenu
    {
        public static MateriaPrompt Instance;

        #region Layout

        const int x0 = 55; // yes
        const int x1 = 42; // question
        const int x2 = 180; // no
        const int y0 = 50;
        const int y1 = 100;
        const int cx = 15;
        const int cy = 15;


        #endregion Layout

        private int option = 0;

        static MateriaPrompt()
        {
            Instance = new MateriaPrompt();
        }
        private MateriaPrompt()
            : base(265, 200, 270, 150)
        {
            Visible = false;
        }
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Left:
                    if (option > 0) option--;
                    break;
                case Key.Right:
                    if (option < 1) option++;
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case 0:
                            Materiatory.Put(null, MateriaList.Instance.Option);
                            MenuScreen.MateriaScreen.ChangeControl(MateriaList.Instance);
                            break;
                        case 1:
                            MenuScreen.MateriaScreen.ChangeControl(MateriaList.Instance);
                            break;
                        default: break;
                    }
                    break;
                case Key.X:
                    MenuScreen.MateriaScreen.ChangeControl(MateriaList.Instance);
                    break;
                default: break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            switch (option)
            {
                case 0:
                    Graphics.RenderCursor(g, X + x0 - cx, Y + y1 - cy); 
                    break;
                case 1:
                    Graphics.RenderCursor(g, X + x2 - cx, Y + y1 - cy); 
                    break;
                default: break;
            }
            Graphics.ShadowedText(g, "Are you sure?", X + x1, Y + y0);
            Graphics.ShadowedText(g, "Yes", X + x0, Y + y1);
            Graphics.ShadowedText(g, "No", X + x2, Y + y1);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            option = 1;
            Visible = true;
        }

        public override void SetNotControl()
        {
            base.SetNotControl();
            Visible = false;
        }


        public override string Info
        { get { return ""; } }

    }




    internal sealed class MateriaInfo : Menu
    {
        public static MateriaInfo Instance;

        static MateriaInfo()
        {
            Instance = new MateriaInfo();
        }
        private MateriaInfo()
            : base(
                2,
                Globals.HEIGHT * 7 / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, MenuScreen.MateriaScreen.Control.Info, X + 20, Y + 26);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

    }

    


    internal sealed class MateriaLabel : Menu
    {
        public static MateriaLabel Instance;

        static MateriaLabel()
        {
            Instance = new MateriaLabel();
        }
        private MateriaLabel()
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

            Graphics.ShadowedText(g, "Materia", X + 20, Y + 25);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}
