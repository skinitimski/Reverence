using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Top : ControlMenu
    {
        #region Layout
        
        const int x3 = 140; // name(s)
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = 330; // equipment labels
        const int x8 = x7 + 60; // equipment names
        const int cx = x7 + 25;
        const int y = 50; // main row
        const int ya = 30; // subrow A1
        const int yb = 55; // subrow A2
        const int yc = 80; // subrow A3
        const int yi = 35; // subrow B1
        const int yj = yi + 50; // subrow B2
        const int yk = yj + 50; // subrow B3
        
        const int xpic = 15;
        const int ypic = 15;
        static int cy = yi;
        
        #endregion
        
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
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;
            string weapon, armor, acc;
            
            
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
            g.MoveTo(X + x7, Y + yi);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.MoveTo(X + x7, Y + yk);
            g.ShowText("Acc.");
            g.Color = new Color(1, 1, 1);
            
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

