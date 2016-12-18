using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Graphics;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

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

        public Selected(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height * 5 / 12,
                screenState.Width * 5 / 8,
                screenState.Height / 6)
        {
            MenuState = menuState;
        }
        
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
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
        }

        private SlotHolder SelectedEquipment
        {
            get
            {
                SlotHolder equipment = null;

                if (MenuState.EquipTop.IsControl)
                {
                    switch (MenuState.EquipTop.Option)
                    {
                        case 0: 
                            equipment = MenuState.Party.Selected.Weapon;
                            break;
                        case 1:
                            equipment = MenuState.Party.Selected.Armor;
                            break;
                        // case 2 is accessory
                    }
                }
                else if (MenuState.EquipList.IsControl)
                {
                    switch (MenuState.EquipTop.Option)
                    {
                        case 0: 
                        case 1: 
                            equipment = (SlotHolder)MenuState.EquipList.Selection;
                            break;
                        // case 2 is accessory
                    }
                }

                return equipment;
            }
        }  
        
        private SevenMenuState MenuState { get; set; }            
        
    }
}

