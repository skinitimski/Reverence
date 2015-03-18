using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Graphics;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Status
{      
    internal sealed class One : Menu.Menu
    {        
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
        
        public One()
            : base(
                2,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth - 10,
                Config.Instance.WindowHeight * 9 / 10)
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
            
            d.DrawPixbuf(gc, Seven.Party.Selected.Profile, 0, 0,
                         X + xpic, Y + ypic,
                         Character.PROFILE_WIDTH, Character.PROFILE_HEIGHT,
                         Gdk.RgbDither.None, 0, 0);
            
            g.Color = COLOR_TEXT_TEAL;
            g.MoveTo(X + x3, Y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + yc);
            g.ShowText("MP");
            g.Color = Colors.WHITE;
            
            Text.ShadowedText(g, Seven.Party.Selected.Name, X + x3, Y + y);
            
            lvl = Seven.Party.Selected.Level.ToString();
            hp = Seven.Party.Selected.HP.ToString() + "/";
            hpm = Seven.Party.Selected.MaxHP.ToString();
            mp = Seven.Party.Selected.MP.ToString() + "/";
            mpm = Seven.Party.Selected.MaxMP.ToString();
            exp = Seven.Party.Selected.Exp.ToString();
            next = Seven.Party.Selected.ToNextLevel.ToString();
            llvl = Seven.Party.Selected.LimitLevel.ToString();
            
            te = g.TextExtents(lvl);
            Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + ya);
            
            te = g.TextExtents(hp);
            Text.ShadowedText(g, hp, X + x5 - te.Width, Y + yb);
            
            te = g.TextExtents(hpm);
            Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + yb);
            
            te = g.TextExtents(mp);
            Text.ShadowedText(g, mp, X + x5 - te.Width, Y + yc);
            
            te = g.TextExtents(mpm);
            Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + yc);
            
            Text.ShadowedText(g, "Exp:", X + x8, Y + ya);
            Text.ShadowedText(g, "Next lvl:", X + x8, Y + yb);
            Text.ShadowedText(g, "Limit lvl:", X + x8, Y + yc);
            
            te = g.TextExtents(exp);
            Text.ShadowedText(g, exp, X + x12 - te.Width, Y + ya);
            te = g.TextExtents(next);
            Text.ShadowedText(g, next, X + x12 - te.Width, Y + yb);
            te = g.TextExtents(llvl);
            Text.ShadowedText(g, llvl, X + x11 - te.Width, Y + yc);
            
            #endregion Top
            
            
            #region Left
            
            string str, vit, dex, mag, spi, lck;
            string atk, atkp, def, defp, mat, mdf, mdfp;
            
            str = Seven.Party.Selected.Strength.ToString();
            vit = Seven.Party.Selected.Vitality.ToString();
            dex = Seven.Party.Selected.Dexterity.ToString();
            mag = Seven.Party.Selected.Magic.ToString();
            spi = Seven.Party.Selected.Spirit.ToString();
            lck = Seven.Party.Selected.Luck.ToString();
            
            atk = Ally.Attack(Seven.Party.Selected).ToString();
            atkp = Ally.AttackPercent(Seven.Party.Selected).ToString();
            def = Ally.Defense(Seven.Party.Selected).ToString();
            defp = Ally.DefensePercent(Seven.Party.Selected).ToString();
            mat = Ally.MagicAttack(Seven.Party.Selected).ToString();
            mdf = Ally.MagicDefense(Seven.Party.Selected).ToString();
            mdfp = Ally.MagicDefensePercent(Seven.Party.Selected).ToString();
            
            Cairo.Color greenish = COLOR_TEXT_TEAL;
            
            Text.ShadowedText(g, greenish, "Strength", X + x0, Y + yq + (line * 0));
            Text.ShadowedText(g, greenish, "Vitality", X + x0, Y + yq + (line * 1));
            Text.ShadowedText(g, greenish, "Dexterity", X + x0, Y + yq + (line * 2));
            Text.ShadowedText(g, greenish, "Magic", X + x0, Y + yq + (line * 3));
            Text.ShadowedText(g, greenish, "Spirit", X + x0, Y + yq + (line * 4));
            Text.ShadowedText(g, greenish, "Luck", X + x0, Y + yq + (line * 5));
            
            Text.ShadowedText(g, greenish, "Attack", X + x0, Y + yr + (line * 0));
            Text.ShadowedText(g, greenish, "Attack %", X + x0, Y + yr + (line * 1));
            Text.ShadowedText(g, greenish, "Defense", X + x0, Y + yr + (line * 2));
            Text.ShadowedText(g, greenish, "Defense %", X + x0, Y + yr + (line * 3));
            Text.ShadowedText(g, greenish, "Magic", X + x0, Y + yr + (line * 4));
            Text.ShadowedText(g, greenish, "Magic def", X + x0, Y + yr + (line * 5));
            Text.ShadowedText(g, greenish, "Magic def %", X + x0, Y + yr + (line * 6));
            
            te = g.TextExtents(str);
            Text.ShadowedText(g, str, X + x1 - te.Width, Y + yq + (line * 0));
            te = g.TextExtents(vit);
            Text.ShadowedText(g, vit, X + x1 - te.Width, Y + yq + (line * 1));
            te = g.TextExtents(dex);
            Text.ShadowedText(g, dex, X + x1 - te.Width, Y + yq + (line * 2));
            te = g.TextExtents(mag);
            Text.ShadowedText(g, mag, X + x1 - te.Width, Y + yq + (line * 3));
            te = g.TextExtents(spi);
            Text.ShadowedText(g, spi, X + x1 - te.Width, Y + yq + (line * 4));
            te = g.TextExtents(lck);
            Text.ShadowedText(g, lck, X + x1 - te.Width, Y + yq + (line * 5));
            
            te = g.TextExtents(atk);
            Text.ShadowedText(g, atk, X + x1 - te.Width, Y + yr + (line * 0));
            te = g.TextExtents(atkp);
            Text.ShadowedText(g, atkp, X + x1 - te.Width, Y + yr + (line * 1));
            te = g.TextExtents(def);
            Text.ShadowedText(g, def, X + x1 - te.Width, Y + yr + (line * 2));
            te = g.TextExtents(defp);
            Text.ShadowedText(g, defp, X + x1 - te.Width, Y + yr + (line * 3));
            te = g.TextExtents(mat);
            Text.ShadowedText(g, mat, X + x1 - te.Width, Y + yr + (line * 4));
            te = g.TextExtents(mdf);
            Text.ShadowedText(g, mdf, X + x1 - te.Width, Y + yr + (line * 5));
            te = g.TextExtents(mdfp);
            Text.ShadowedText(g, mdfp, X + x1 - te.Width, Y + yr + (line * 6));
            
            #endregion Left
            
            
            #region Right
            
            
            g.Color = new Color(.1, .1, .2);
            g.Rectangle(x9, yi, 8 * xs, yj - yi);
            g.Fill();
            g.Rectangle(x9, yk, 8 * xs, yl - yk);
            g.Fill();
            
            Cairo.Color gray1 = new Color(.2, .2, .2);
            Cairo.Color gray2 = new Color(.7, .7, .8);


            MateriaSlots.RenderMateriaSlots(g, Seven.Party.Selected.Weapon, X + x9, Y + yi);
            MateriaSlots.RenderMateriaSlots(g, Seven.Party.Selected.Armor, X + x9, Y + yk);

            
            
            g.Color = COLOR_TEXT_TEAL;
            g.MoveTo(X + x7, Y + yh);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.MoveTo(X + x7, Y + yl);
            g.ShowText("Acc.");
            g.Color = Colors.WHITE;
            
            Text.ShadowedText(g, Seven.Party.Selected.Weapon.Name, X + x8, Y + yh);
            Text.ShadowedText(g, Seven.Party.Selected.Armor.Name, X + x8, Y + yj);
            Text.ShadowedText(g, Seven.Party.Selected.Accessory.Name, X + x8, Y + yl);

            #endregion Right
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}

