using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Stats : Menu.Menu
    {
        #region Layout
        
        const int x0 = 50; // column 1
        const int x1 = x0 + 225; // column 2
        const int x2 = x1 + 40; // column 3
        const int x3 = x2 + 80; // column 3
        
        const int yr = 30; // 7th row (left)
        const int line = 28; // row spacing (left)
        
#endregion

        public Stats()
            : base(
                2,
                Config.Instance.WindowHeight * 7 / 12,
                Config.Instance.WindowWidth * 5 / 8,
                Config.Instance.WindowHeight * 11 / 30)
        { 
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            #region Left
            
            string str, vit, dex, mag, spi, lck;
            int atk, atkp, def, defp, mat, mdf, mdfp;
            
            str = Seven.Party.Selected.Strength.ToString();
            vit = Seven.Party.Selected.Vitality.ToString();
            dex = Seven.Party.Selected.Dexterity.ToString();
            mag = Seven.Party.Selected.Magic.ToString();
            spi = Seven.Party.Selected.Spirit.ToString();
            lck = Seven.Party.Selected.Luck.ToString();
            
            atk = Ally.Attack(Seven.Party.Selected);
            atkp = Ally.AttackPercent(Seven.Party.Selected);
            def = Ally.Defense(Seven.Party.Selected);
            defp = Ally.DefensePercent(Seven.Party.Selected);
            mat = Ally.MagicAttack(Seven.Party.Selected);
            mdf = Ally.MagicDefense(Seven.Party.Selected);
            mdfp = Ally.MagicDefensePercent(Seven.Party.Selected);
            
            Cairo.Color greenish = new Color(.3, .8, .8);
            
            Text.ShadowedText(g, greenish, "Attack", X + x0, Y + yr + (line * 0));
            Text.ShadowedText(g, greenish, "Attack %", X + x0, Y + yr + (line * 1));
            Text.ShadowedText(g, greenish, "Defense", X + x0, Y + yr + (line * 2));
            Text.ShadowedText(g, greenish, "Defense %", X + x0, Y + yr + (line * 3));
            Text.ShadowedText(g, greenish, "Magic", X + x0, Y + yr + (line * 4));
            Text.ShadowedText(g, greenish, "Magic def", X + x0, Y + yr + (line * 5));
            Text.ShadowedText(g, greenish, "Magic def %", X + x0, Y + yr + (line * 6));
            
            te = g.TextExtents(atk.ToString());
            Text.ShadowedText(g, atk.ToString(), X + x1 - te.Width, Y + yr + (line * 0));
            te = g.TextExtents(atkp.ToString());
            Text.ShadowedText(g, atkp.ToString(), X + x1 - te.Width, Y + yr + (line * 1));
            te = g.TextExtents(def.ToString());
            Text.ShadowedText(g, def.ToString(), X + x1 - te.Width, Y + yr + (line * 2));
            te = g.TextExtents(defp.ToString());
            Text.ShadowedText(g, defp.ToString(), X + x1 - te.Width, Y + yr + (line * 3));
            te = g.TextExtents(mat.ToString());
            Text.ShadowedText(g, mat.ToString(), X + x1 - te.Width, Y + yr + (line * 4));
            te = g.TextExtents(mdf.ToString());
            Text.ShadowedText(g, mdf.ToString(), X + x1 - te.Width, Y + yr + (line * 5));
            te = g.TextExtents(mdfp.ToString());
            Text.ShadowedText(g, mdfp.ToString(), X + x1 - te.Width, Y + yr + (line * 6));
            
            if (Seven.MenuState.EquipList.IsControl)
            {
                int t_atk, t_atkp, t_def, t_defp, t_mat, t_mdf, t_mdfp;
                
                IInventoryItem temp;
                
                switch (Seven.MenuState.EquipTop.Option)
                {
                    case 0:
                        temp = Seven.Party.Selected.Weapon;
                        Seven.Party.Selected.Weapon = (Weapon)Seven.MenuState.EquipList.Selection;
                        break;
                    case 1:
                        temp = Seven.Party.Selected.Armor;
                        Seven.Party.Selected.Armor = (Armor)Seven.MenuState.EquipList.Selection;
                        break;
                    case 2:
                        temp = Seven.Party.Selected.Accessory;
                        Seven.Party.Selected.Accessory = (Accessory)Seven.MenuState.EquipList.Selection;
                        break;
                    default:
                        temp = Seven.Party.Selected.Weapon;
                        break;
                }
                
                t_atk = Ally.Attack(Seven.Party.Selected);
                t_atkp = Ally.AttackPercent(Seven.Party.Selected);
                t_def = Ally.Defense(Seven.Party.Selected);
                t_defp = Ally.DefensePercent(Seven.Party.Selected);
                t_mat = Ally.MagicAttack(Seven.Party.Selected);
                t_mdf = Ally.MagicDefense(Seven.Party.Selected);
                t_mdfp = Ally.MagicDefensePercent(Seven.Party.Selected);
                
                switch (Seven.MenuState.EquipTop.Option)
                {
                    case 0:
                        Seven.Party.Selected.Weapon = (Weapon)temp;
                        break;
                    case 1:
                        Seven.Party.Selected.Armor = (Armor)temp;
                        break;
                    case 2:
                        Seven.Party.Selected.Accessory = (Accessory)temp;
                        break;
                    default: break;
                }
                
                Color c;
                Color yellow = new Color(.8, .8, 0);
                Color red = new Color(0.8, 0, 0);
                Color white = new Color(1, 1, 1);
                
                Text.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 0));
                Text.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 1));
                Text.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 2));
                Text.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 3));
                Text.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 4));
                Text.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 5));
                Text.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 6));
                
                te = g.TextExtents(t_atk.ToString());
                if (t_atk < atk) c = red;
                else if (t_atk > atk) c = yellow;
                else c = white;
                Text.ShadowedText(g, c, t_atk.ToString(), X + x3 - te.Width, Y + yr + (line * 0));
                
                te = g.TextExtents(t_atkp.ToString());
                if (t_atkp < atkp) c = red;
                else if (t_atkp > atkp) c = yellow;
                else c = white;
                Text.ShadowedText(g, c, t_atkp.ToString(), X + x3 - te.Width, Y + yr + (line * 1));
                
                te = g.TextExtents(t_def.ToString());
                if (t_def < def) c = red;
                else if (t_def > def) c = yellow;
                else c = white;
                Text.ShadowedText(g, c, t_def.ToString(), X + x3 - te.Width, Y + yr + (line * 2));
                
                te = g.TextExtents(t_defp.ToString());
                if (t_defp < defp) c = red;
                else if (t_defp > defp) c = yellow;
                else c = white;
                Text.ShadowedText(g, c, t_defp.ToString(), X + x3 - te.Width, Y + yr + (line * 3));
                
                te = g.TextExtents(t_mat.ToString());
                if (t_mat < mat) c = red;
                else if (t_mat > mat) c = yellow;
                else c = white;
                Text.ShadowedText(g, c, t_mat.ToString(), X + x3 - te.Width, Y + yr + (line * 4));
                
                te = g.TextExtents(t_mdf.ToString());
                if (t_mdf < mdf) c = red;
                else if (t_mdf > mdf) c = yellow;
                else c = white;
                Text.ShadowedText(g, c, t_mdf.ToString(), X + x3 - te.Width, Y + yr + (line * 5));
                
                te = g.TextExtents(t_mdfp.ToString());
                if (t_mdfp < mdfp) c = red;
                else if (t_mdfp > mdfp) c = yellow;
                else c = white;
                Text.ShadowedText(g, c, t_mdfp.ToString(), X + x3 - te.Width, Y + yr + (line * 6));
                
            }
            
            #endregion Left
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
    }
}

