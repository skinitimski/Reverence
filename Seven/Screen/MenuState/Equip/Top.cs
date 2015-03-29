using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Graphics;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Top : ControlMenu
    {
        #region Layout

        const int x7 = 370; // equipment labels
        const int x8 = x7 + 60; // equipment names
        const int cx = x7 + 25;

        const int yi = 35; // subrow B1
        const int yj = yi + 50; // subrow B2
        const int yk = yj + 50; // subrow B3
        
        const int xpic = 15;
        const int ypic = 15;
        
        const int x_status = xpic + Character.PROFILE_WIDTH + 15;
        const int y_status = ypic + 35;
                
        #endregion
        
        private int cy = yi;
        private int _option = 0;

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
                    if (_option > 0)
                    {
                        _option--;
                    }
                    break;
                case Key.Down:
                    if (_option < 2)
                    {
                        _option++;
                    }
                    break;
                case Key.X:
                    if (Changed)
                    {
                        Seven.MenuState.ChangeScreen(Seven.MenuState.MateriaScreen);
                    }
                    else
                    {
                        Seven.MenuState.ChangeScreen(Seven.MenuState.MainScreen);
                    }
                    Changed = false;
                    break;
                case Key.Square:
                    Seven.MenuState.ChangeScreen(Seven.MenuState.MateriaScreen);
                    break;
                case Key.Circle:
                    Seven.MenuState.EquipScreen.ChangeControl(Seven.MenuState.EquipList);
                    break;
                case Key.Triangle:
                    if (_option == 2)
                    {
                        if (Seven.Party.Selected.Accessory != Accessory.EMPTY)
                        {
                            Seven.Party.Inventory.AddToInventory(Seven.Party.Selected.Accessory);
                            Seven.Party.Selected.Accessory = Accessory.EMPTY;
                        }
                    }
                    break;
                default:
                    break;
            }
            switch (_option)
            {
                case 0:
                    cy = yi;
                    break;
                case 1:
                    cy = yj;
                    break;
                case 2:
                    cy = yk;
                    break;
                default:
                    break;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;

            string weapon, armor, acc;
            
            
            #region Character Status
            
            Images.RenderProfile(d, gc, X + xpic, Y + ypic, Seven.Party.Selected);

            Graphics.Stats.RenderCharacterStatus(d, gc, g, Seven.Party.Selected, X + x_status, Y + y_status, false);

            #endregion Status
            
            #region Equipment
            
            g.Color = Colors.TEXT_TEAL;
            g.MoveTo(X + x7, Y + yi);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.MoveTo(X + x7, Y + yk);
            g.ShowText("Acc.");
            g.Color = Colors.WHITE;
            
            weapon = Seven.Party.Selected.Weapon.Name;
            armor = Seven.Party.Selected.Armor.Name;
            acc = Seven.Party.Selected.Accessory.Name;
            
            te = g.TextExtents(weapon);
            Text.ShadowedText(g, weapon, X + x8, Y + yi);
            te = g.TextExtents(armor);
            Text.ShadowedText(g, armor, X + x8, Y + yj);
            te = g.TextExtents(acc);
            Text.ShadowedText(g, acc, X + x8, Y + yk);
            
            #endregion Equipment
            
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy - 10);
            }
                    
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public int Option { get { return _option; } }

        public bool Changed { get; set; }

        public override string Info
        {
            get
            {
                switch (_option)
                {
                    case 0:
                        return Seven.Party.Selected.Weapon.Description;
                    case 1:
                        return Seven.Party.Selected.Armor.Description;
                    case 2:
                        return Seven.Party.Selected.Accessory.Description;
                    default:
                        return "";
                }
            }
        }
    }
}

