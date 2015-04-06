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

        const int y_slots = ya - MateriaSlots.SLOT_RADIUS * 2;

        #endregion

        public Selected()
            : base(
                2,
                Seven.Config.WindowHeight * 5 / 12,
                Seven.Config.WindowWidth * 5 / 8,
                Seven.Config.WindowHeight / 6)
        {
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            if (SelectedEquipment != null)
            {                                
                MateriaSlots.Render(g, SelectedEquipment, X + x1, Y + y_slots, false);

                Text.ShadowedText(g, Colors.TEXT_TEAL, "Slot", X + x0, Y + ya);
                Text.ShadowedText(g, Colors.TEXT_TEAL, "Growth", X + x0, Y + yb);
                
                string growth = SelectedEquipment.Growth.ToString();
                
                te = g.TextExtents(growth);

                Text.ShadowedText(g, growth, X + x1, Y + yb);
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
                        case 0: 
                            equipment = Seven.Party.Selected.Weapon;
                            break;
                        case 1:
                            equipment = Seven.Party.Selected.Armor;
                            break;
                        // case 2 is accessory
                    }
                }
                else if (Seven.MenuState.EquipList.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        case 0: 
                        case 1: 
                            equipment = (SlotHolder)Seven.MenuState.EquipList.Selection;
                            break;
                        // case 2 is accessory
                    }
                }

                return equipment;
            }
        }               
        
    }
}

