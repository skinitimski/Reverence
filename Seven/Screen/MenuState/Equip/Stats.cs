using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

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

        public Stats(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height * 7 / 12,
                screenState.Width * 5 / 8,
                screenState.Height * 11 / 30)
        { 
            MenuState = menuState;
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            #region Left

            int atk, atkp, def, defp, mat, mdf, mdfp;

            atk = Ally.Attack(MenuState.Party.Selected);
            atkp = Ally.AttackPercent(MenuState.Party.Selected);
            def = Ally.Defense(MenuState.Party.Selected);
            defp = Ally.DefensePercent(MenuState.Party.Selected);
            mat = Ally.MagicAttack(MenuState.Party.Selected);
            mdf = Ally.MagicDefense(MenuState.Party.Selected);
            mdfp = Ally.MagicDefensePercent(MenuState.Party.Selected);
            
            Cairo.Color greenish = Colors.TEXT_TEAL;
            
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
            
            if (MenuState.EquipList.IsControl)
            {
                int t_atk, t_atkp, t_def, t_defp, t_mat, t_mdf, t_mdfp;
                
                IInventoryItem temp;
                
                switch (MenuState.EquipTop.Option)
                {
                    case 0:
                        temp = MenuState.Party.Selected.Weapon;
                        MenuState.Party.Selected.Weapon = (Weapon)MenuState.EquipList.Selection;
                        break;
                    case 1:
                        temp = MenuState.Party.Selected.Armor;
                        MenuState.Party.Selected.Armor = (Armor)MenuState.EquipList.Selection;
                        break;
                    case 2:
                        temp = MenuState.Party.Selected.Accessory;
                        MenuState.Party.Selected.Accessory = (Accessory)MenuState.EquipList.Selection;
                        break;
                    default:
                        temp = MenuState.Party.Selected.Weapon;
                        break;
                }
                
                t_atk = Ally.Attack(MenuState.Party.Selected);
                t_atkp = Ally.AttackPercent(MenuState.Party.Selected);
                t_def = Ally.Defense(MenuState.Party.Selected);
                t_defp = Ally.DefensePercent(MenuState.Party.Selected);
                t_mat = Ally.MagicAttack(MenuState.Party.Selected);
                t_mdf = Ally.MagicDefense(MenuState.Party.Selected);
                t_mdfp = Ally.MagicDefensePercent(MenuState.Party.Selected);
                
                switch (MenuState.EquipTop.Option)
                {
                    case 0:
                        MenuState.Party.Selected.Weapon = (Weapon)temp;
                        break;
                    case 1:
                        MenuState.Party.Selected.Armor = (Armor)temp;
                        break;
                    case 2:
                        MenuState.Party.Selected.Accessory = (Accessory)temp;
                        break;
                    default: break;
                }
                
                Color c;
                
                Text.ShadowedText(g, ">", X + x2, Y + yr + (line * 0));
                Text.ShadowedText(g, ">", X + x2, Y + yr + (line * 1));
                Text.ShadowedText(g, ">", X + x2, Y + yr + (line * 2));
                Text.ShadowedText(g, ">", X + x2, Y + yr + (line * 3));
                Text.ShadowedText(g, ">", X + x2, Y + yr + (line * 4));
                Text.ShadowedText(g, ">", X + x2, Y + yr + (line * 5));
                Text.ShadowedText(g, ">", X + x2, Y + yr + (line * 6));
                
                te = g.TextExtents(t_atk.ToString());
                if (t_atk < atk) c = Colors.TEXT_RED;
                else if (t_atk > atk) c = Colors.TEXT_YELLOW;
                else c = Colors.WHITE;
                Text.ShadowedText(g, c, t_atk.ToString(), X + x3 - te.Width, Y + yr + (line * 0));
                
                te = g.TextExtents(t_atkp.ToString());
                if (t_atkp < atkp) c = Colors.TEXT_RED;
                else if (t_atkp > atkp) c = Colors.TEXT_YELLOW;
                else c = Colors.WHITE;
                Text.ShadowedText(g, c, t_atkp.ToString(), X + x3 - te.Width, Y + yr + (line * 1));
                
                te = g.TextExtents(t_def.ToString());
                if (t_def < def) c = Colors.TEXT_RED;
                else if (t_def > def) c = Colors.TEXT_YELLOW;
                else c = Colors.WHITE;
                Text.ShadowedText(g, c, t_def.ToString(), X + x3 - te.Width, Y + yr + (line * 2));
                
                te = g.TextExtents(t_defp.ToString());
                if (t_defp < defp) c = Colors.TEXT_RED;
                else if (t_defp > defp) c = Colors.TEXT_YELLOW;
                else c = Colors.WHITE;
                Text.ShadowedText(g, c, t_defp.ToString(), X + x3 - te.Width, Y + yr + (line * 3));
                
                te = g.TextExtents(t_mat.ToString());
                if (t_mat < mat) c = Colors.TEXT_RED;
                else if (t_mat > mat) c = Colors.TEXT_YELLOW;
                else c = Colors.WHITE;
                Text.ShadowedText(g, c, t_mat.ToString(), X + x3 - te.Width, Y + yr + (line * 4));
                
                te = g.TextExtents(t_mdf.ToString());
                if (t_mdf < mdf) c = Colors.TEXT_RED;
                else if (t_mdf > mdf) c = Colors.TEXT_YELLOW;
                else c = Colors.WHITE;
                Text.ShadowedText(g, c, t_mdf.ToString(), X + x3 - te.Width, Y + yr + (line * 5));
                
                te = g.TextExtents(t_mdfp.ToString());
                if (t_mdfp < mdfp) c = Colors.TEXT_RED;
                else if (t_mdfp > mdfp) c = Colors.TEXT_YELLOW;
                else c = Colors.WHITE;
                Text.ShadowedText(g, c, t_mdfp.ToString(), X + x3 - te.Width, Y + yr + (line * 6));
                
            }
            
            #endregion Left
        }

        private SevenMenuState MenuState { get; set; }   
    }
}

