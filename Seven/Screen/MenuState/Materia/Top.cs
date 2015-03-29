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

        const int x7 = 330; // wpn.
        const int x7a = x7 + 25;
        const int x8 = x7 + 65; // wpn name
        const int x9 = x8 + 60; // box
        
        const int yh = 43;      // weapon
        const int yi = yh + 15; //  -subrow
        const int yj = yi + 65; // armor
        const int yja = yj - 8; // check
        const int yk = yj + 15; //  -subrow
        const int yl = yk + 65; // end
        const int yla = yl - 8; // arrange
        
        const int cy0 = yi + MateriaSlots.SLOT_RADIUS;
        const int cy1 = yk + MateriaSlots.SLOT_RADIUS;
        
        const int xpic = 15;
        const int ypic = 15;
        const int ys = 13;
        
        const int x_status = xpic + Character.PROFILE_WIDTH + 15;
        const int y_status = ypic + 35;
        
        #endregion
        
        private int optionX = 0;
        private int optionY = 0;
        private int cx = x9;
        private int cy = cy0;

        public Top()
            : base(
                2,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth - 10,
                Config.Instance.WindowHeight * 3 / 10 - 6)
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
                cx = x7;
            }
            else
            {
                cx = x9 + optionX * MateriaSlots.SLOT_SPACING;
            }
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;
            string weapon, armor;
            
            
            #region Slots
                        
            MateriaSlots.Render(g, Seven.Party.Selected.Weapon, X + x9, Y + yi);
            MateriaSlots.Render(g, Seven.Party.Selected.Armor, X + x9, Y + yk);
            
            #endregion
            
            #region Character Status
            
            Images.RenderProfile(d, gc, X + xpic, Y + ypic, Seven.Party.Selected);
            
            Graphics.Stats.RenderCharacterStatus(d, gc, g, Seven.Party.Selected, X + x_status, Y + y_status, false);
            
            #endregion Status
            
            #region Equipment
            
            g.Color = Colors.TEXT_TEAL;
            g.MoveTo(X + x7, Y + yh);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.Color = Colors.WHITE;
            
            Text.ShadowedText(g, "Check", x7a, yja);
            Text.ShadowedText(g, "Arr.", x7a, yla);
            
            weapon = Seven.Party.Selected.Weapon.Name;
            armor = Seven.Party.Selected.Armor.Name;
            
            te = g.TextExtents(weapon);
            Text.ShadowedText(g, weapon, X + x8, Y + yh); 
            te = g.TextExtents(armor);
            Text.ShadowedText(g, armor, X + x8, Y + yj);
            
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

