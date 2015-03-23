using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Graphics;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Selected : Menu.Menu
    {
        #region Layout
        
        const int x0 = 30;
        const int x1 = 150;
        
        const int ya = 40;
        const int yb = ya + 40;
        
        const int yh = 0; // first row (right)
        const int yi = yh + 40; // successive rows
        const int yj = yi + 37; //    |
        const int yk = yj + 40; //    | 
        const int yl = yk + 37; //    x
        
        const int xs = yl - yk;
        const int ysa = 7;
        const int ysb = 11;
        const int zs = 5;
        
        const int x2 = x1 + (8 * xs) / 2;
        
#endregion

        public Selected()
            : base(
                2,
                Config.Instance.WindowHeight * 5 / 12,
                Config.Instance.WindowWidth * 5 / 8,
                Config.Instance.WindowHeight / 6)
        {
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            if (SelectedEquipment != null)
            {
                g.Color = new Color(.1, .1, .2);
                g.Rectangle(X + x1, Y + ya - (yj - yi) + ysa, 8 * xs, yj - yi);
                g.Fill();

                                
                MateriaSlots.Render(g, SelectedEquipment, X + x1, Y + ya, false);

                Text.ShadowedText(g, COLOR_TEXT_TEAL, "Slot", X + x0, Y + ya);
                Text.ShadowedText(g, COLOR_TEXT_TEAL, "Growth", X + x0, Y + yb);
                
                string growth = SelectedEquipment.Growth.ToString();
                
                te = g.TextExtents(growth);
                Text.ShadowedText(g, growth, X + x2 - (te.Width / 2), Y + yb);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private SlotHolder SelectedEquipment
        {
            get
            {
                SlotHolder equipment = null;

                if (Seven.MenuState.EquipTop.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        // case 2 is accessory
                        case 0: 
                            equipment = Seven.Party.Selected.Weapon;
                            break;
                        case 1:
                            equipment = Seven.Party.Selected.Armor;
                            break;
                    }
                }
                else if (Seven.MenuState.EquipList.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        // case 2 is accessory
                        case 0: 
                        case 1: 
                            equipment = (SlotHolder)Seven.MenuState.EquipList.Selection;
                            break;
                    }
                }

                return equipment;
            }
        }               
        
    }
}

