using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Materia
{  
    internal sealed class Top : ControlMenu
    {
        #region Layout

        const int x_labels = 370; // wpn.
        const int x_names = x_labels + 65; // wpn name
        
        const int y_weapon = 35;     
        const int y_check = y_weapon + 38;
        const int y_armor = y_check + 42;
        const int y_arrange = y_armor + 38;
                
        const int xpic = 15;
        const int ypic = 15;
        
        const int x_status = xpic + Character.PROFILE_WIDTH + 15;
        const int y_status = ypic + 35;

        const int x_slots = x_names + 20;

        const int y_slots_weapon = y_weapon + 15;
        const int y_slots_armor = y_armor + 15;

        
        const int cy0 = y_slots_weapon + MateriaSlots.SLOT_RADIUS;
        const int cy1 = y_slots_armor + MateriaSlots.SLOT_RADIUS;

        #endregion
        
        private int optionX = 0;
        private int optionY = 0;
        private int cx = x_slots;
        private int cy = cy0;

        public Top()
            : base(
                2,
                Seven.Config.WindowHeight / 20,
                Seven.Config.WindowWidth - 10,
                Seven.Config.WindowHeight * 3 / 10 - 6)
        {
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (optionY > 0)
                    {
                        optionY--;
                    }
                    break;
                case Key.Down:
                    if (optionY < 1)
                    {
                        optionY++;
                    }
                    break;
                case Key.Left:
                    if (optionX > -1)
                    {
                        optionX--;
                    }
                    break;
                case Key.Right:
                    if (optionX < 7)
                    {
                        optionX++;
                    }
                    break;
                case Key.X:
                    Seven.MenuState.ChangeScreen(Seven.MenuState.MainScreen);
                    break;
                case Key.Square:
                    Seven.MenuState.ChangeScreen(Seven.MenuState.EquipScreen);
                    break;
                case Key.Circle:
                    if (optionX == -1)
                    {
                        switch (optionY)
                        {
                            case 0: // check
                                break;
                            case 1: // arrange
                                Seven.MenuState.MateriaScreen.ChangeControl(Seven.MenuState.MateriaArrange);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (optionY)
                        {
                            case 0:
                                if (optionX < Seven.Party.Selected.Weapon.Slots.Length)
                                {
                                    Seven.MenuState.MateriaScreen.ChangeControl(Seven.MenuState.MateriaList);
                                }
                                break;
                            case 1:
                                if (optionX < Seven.Party.Selected.Armor.Slots.Length)
                                {
                                    Seven.MenuState.MateriaScreen.ChangeControl(Seven.MenuState.MateriaList);
                                }
                                break;
                        }
                    }
                    break;
                case Key.Triangle:
                    if (optionX == -1)
                    {
                        break;
                    }
                    MateriaBase orb;
                    switch (optionY)
                    {
                        case 0:
                            orb = Seven.Party.Selected.Weapon.Slots[optionX];
                            if (orb != null)
                            {
                                Seven.Party.Selected.Weapon.Slots[optionX] = null;
                                orb.Detach(Seven.Party.Selected);
                                Seven.Party.Materiatory.Put(orb);
                            }
                            break;
                        case 1:
                            orb = Seven.Party.Selected.Armor.Slots[optionX];
                            if (orb != null)
                            {
                                Seven.Party.Selected.Armor.Slots[optionX] = null;
                                orb.Detach(Seven.Party.Selected);
                                Seven.Party.Materiatory.Put(orb);
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            switch (optionY)
            {
                case 0:
                    cy = cy0;
                    break;
                case 1:
                    cy = cy1;
                    break;
                default:
                    break;
            }
            if (optionX == -1)
            {
                cx = x_labels;
            }
            else
            {
                cx = x_slots + optionX * MateriaSlots.SLOT_SPACING;
            }
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            
            
            #region Slots
                        
            MateriaSlots.Render(g, Seven.Party.Selected.Weapon, X + x_slots, Y + y_slots_weapon);
            MateriaSlots.Render(g, Seven.Party.Selected.Armor, X + x_slots, Y + y_slots_armor);
            
            #endregion
            
            #region Character Status
            
            Images.RenderProfile(d, gc, X + xpic, Y + ypic, Seven.Party.Selected);
            
            Graphics.Stats.RenderCharacterStatus(d, gc, g, Seven.Party.Selected, X + x_status, Y + y_status, false);
            
            #endregion Status
            
            #region Equipment
            
            g.Color = Colors.TEXT_TEAL;
            g.MoveTo(X + x_labels, Y + y_weapon);
            g.ShowText("Wpn.");
            g.MoveTo(X + x_labels, Y + y_armor);
            g.ShowText("Arm.");
            g.Color = Colors.WHITE;
            
            Text.ShadowedText(g, "Check", X + x_labels, Y + y_check);
            Text.ShadowedText(g, "Arr.", X + x_labels, Y + y_arrange);
            
            string weapon, armor;
            weapon = Seven.Party.Selected.Weapon.Name;
            armor = Seven.Party.Selected.Armor.Name;

            Text.ShadowedText(g, Seven.Party.Selected.Weapon.Name, X + x_names, Y + y_weapon); 
            Text.ShadowedText(g, Seven.Party.Selected.Armor.Name, X + x_names, Y + y_armor);
            
            #endregion 
            
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy);
            }


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
                {
                    return String.Empty;
                }
                switch (optionY)
                {
                    case 0:
                        if (optionX < Seven.Party.Selected.Weapon.Slots.Length)
                        if (Seven.Party.Selected.Weapon.Slots [optionX] != null)
                            return Seven.Party.Selected.Weapon.Slots [optionX].Description;
                        return String.Empty;
                    case 1:
                        if (optionX < Seven.Party.Selected.Armor.Slots.Length)
                        if (Seven.Party.Selected.Armor.Slots [optionX] != null)
                            return Seven.Party.Selected.Armor.Slots [optionX].Description;
                        return String.Empty;
                    default:
                        return String.Empty;
                }
            }
        }

        public MateriaBase Selection
        {
            get
            {
                if (IsControl)
                {
                    if (optionX == -1)
                    {
                        return null;
                    }

                    switch (optionY)
                    {
                        case 0:
                            if (optionX < Seven.Party.Selected.Weapon.Slots.Length)
                            if (Seven.Party.Selected.Weapon.Slots [optionX] != null)
                                return Seven.Party.Selected.Weapon.Slots [optionX];
                            return null;
                        case 1:
                            if (optionX < Seven.Party.Selected.Armor.Slots.Length)
                            if (Seven.Party.Selected.Armor.Slots [optionX] != null)
                                return Seven.Party.Selected.Armor.Slots [optionX];
                            return null;
                        default:
                            return null;
                    }
                }
                else
                    return null;
            }
        }
    }

}

