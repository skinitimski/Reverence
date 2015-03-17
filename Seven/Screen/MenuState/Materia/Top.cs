using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Materia
{  
    internal sealed class Top : ControlMenu
    {
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
                        optionY--;
                    break;
                case Key.Down:
                    if (optionY < 1)
                        optionY++;
                    break;
                case Key.Left:
                    if (optionX > -1)
                        optionX--;
                    break;
                case Key.Right:
                    if (optionX < 7)
                        optionX++;
                    break;
                case Key.X:
                    Seven.MenuState.ChangeScreen(Seven.MenuState.MainScreen);
                    break;
                case Key.Square:
                    Seven.MenuState.ChangeScreen(Seven.MenuState.EquipScreen);
                    break;
                case Key.Circle:
                    if (optionX == -1)
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
                    else
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
                    break;
                case Key.Triangle:
                    if (optionX == -1){
                        break;}
                    MateriaBase orb;
                    switch (optionY)
                    {
                        case 0:
                            orb = Seven.Party.Selected.Weapon.Slots [optionX];
                            if (orb != null)
                            {
                                Seven.Party.Selected.Weapon.Slots [optionX] = null;
                                orb.Detach(Seven.Party.Selected);
                                Seven.Party.Materiatory.Put(orb);
                            }
                            break;
                        case 1:
                            orb = Seven.Party.Selected.Armor.Slots [optionX];
                            if (orb != null)
                            {
                                Seven.Party.Selected.Armor.Slots [optionX] = null;
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
                    cy = yi;
                    break;
                case 1:
                    cy = yk;
                    break;
                default:
                    break;
            }
            if (optionX == -1)
                cx = x7;
            else
                cx = x9 + optionX * xs;
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
            
            slots = Seven.Party.Selected.Weapon.Slots.Length;
            links = Seven.Party.Selected.Weapon.Links;
            
            
            for (int j = 0; j < links; j++)
            {
                Shapes.RenderLine(g, gray2, 3,
                                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yi - ys - zs,
                                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yi - ys - zs);
                Shapes.RenderLine(g, gray2, 3,
                                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yi - ys,
                                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yi - ys);
                Shapes.RenderLine(g, gray2, 3,
                                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yi - ys + zs,
                                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yi - ys + zs);
            }
            for (int i = 0; i < slots; i++)
            {
                Shapes.RenderCircle(g, gray2, 14,
                                      X + x9 + (i * xs) + (xs / 2), Y + yi - ys);
                if (Seven.Party.Selected.Weapon.Slots [i] == null)
                    Shapes.RenderCircle(g, gray1, 10,
                                          X + x9 + (i * xs) + (xs / 2), Y + yi - ys);
                else
                    Shapes.RenderCircle(g, Seven.Party.Selected.Weapon.Slots [i].Color, 10,
                                          X + x9 + (i * xs) + (xs / 2), Y + yi - ys);
            }
            
            slots = Seven.Party.Selected.Armor.Slots.Length;
            links = Seven.Party.Selected.Armor.Links;
            
            for (int j = 0; j < links; j++)
            {
                Shapes.RenderLine(g, gray2, 3,
                                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yk - ys - zs,
                                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yk - ys - zs);
                Shapes.RenderLine(g, gray2, 3,
                                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yk - ys,
                                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yk - ys);
                Shapes.RenderLine(g, gray2, 3,
                                    X + x9 + (xs / 2) + (j * 2 * xs), Y + yk - ys + zs,
                                    X + x9 + (xs / 2) + ((j * 2 + 1) * xs), Y + yk - ys + zs);
            }
            for (int i = 0; i < slots; i++)
            {
                Shapes.RenderCircle(g, gray2, 14,
                                      X + x9 + (i * xs) + (xs / 2), Y + yk - ys);
                
                if (Seven.Party.Selected.Armor.Slots [i] == null)
                    Shapes.RenderCircle(g, gray1, 10,
                                          X + x9 + (i * xs) + (xs / 2), Y + yk - ys);
                else
                    Shapes.RenderCircle(g, Seven.Party.Selected.Armor.Slots [i].Color, 10,
                                          X + x9 + (i * xs) + (xs / 2), Y + yk - ys);
            }
            
#endregion
            
            #region Character Status
            
            d.DrawPixbuf(gc, Seven.Party.Selected.Profile, 0, 0,
                         X + xpic, Y + ypic,
                         Character.PROFILE_WIDTH, Character.PROFILE_HEIGHT,
                         Gdk.RgbDither.None, 0, 0);
            
            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x3, Y + y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + y + yc);
            g.ShowText("MP");
            g.Color = new Color(1, 1, 1);
            
            Text.ShadowedText(g, Seven.Party.Selected.Name, X + x3, Y + y);
            
            lvl = Seven.Party.Selected.Level.ToString();
            hp = Seven.Party.Selected.HP.ToString() + "/";
            hpm = Seven.Party.Selected.MaxHP.ToString();
            mp = Seven.Party.Selected.MP.ToString() + "/";
            mpm = Seven.Party.Selected.MaxMP.ToString();
            
            te = g.TextExtents(lvl);
            Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + y + ya);
            te = g.TextExtents(hp);
            Text.ShadowedText(g, hp, X + x5 - te.Width, Y + y + yb);
            te = g.TextExtents(hpm);
            Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + y + yb);
            te = g.TextExtents(mp);
            Text.ShadowedText(g, mp, X + x5 - te.Width, Y + y + yc);
            te = g.TextExtents(mpm);
            Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + y + yc);
            
            #endregion Status
            
            #region Equipment
            
            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x7, Y + yh);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.Color = new Color(1, 1, 1);
            
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
                Shapes.RenderCursor(g, X + cx, Y + cy - ys);
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
                    return "";
                }
                switch (optionY)
                {
                    case 0:
                        if (optionX < Seven.Party.Selected.Weapon.Slots.Length)
                        if (Seven.Party.Selected.Weapon.Slots [optionX] != null)
                            return Seven.Party.Selected.Weapon.Slots [optionX].Description;
                        return "";
                    case 1:
                        if (optionX < Seven.Party.Selected.Armor.Slots.Length)
                        if (Seven.Party.Selected.Armor.Slots [optionX] != null)
                            return Seven.Party.Selected.Armor.Slots [optionX].Description;
                        return "";
                    default:
                        return "";
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
                        return null;
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

