using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Config
{
    internal sealed class WindowColor : ControlMenu
    {
        public const int WIDTH = 250;
        public const int HEIGHT = 120;

        const int spacing = 15;

        const int x0 = spacing;
        const int x1 = WIDTH - spacing;
        const int y0 = spacing;
        const int y1 = HEIGHT - spacing;
        
        private int option_x;
        private int option_y;





        private class WindowCornerColor : Menu.Menu
        {
            const int x0 = 10;
            const int yr = 30;
            const int yg = yr + 38;
            const int yb = yg + 38;

            int x_spacing = (WIDTH - x0 * 2) / 13; // 13 comes from 10 options plus the color indicator plus two borders


            int corner;
            int[] indicies = new int[3];
            int[] indicies_orig = new int[3];

            int option_y;

            public WindowCornerColor(int x, int y, int corner, WindowColor owner)
                : base(x, y, WIDTH, HEIGHT)
            {
                Owner = owner;

                this.corner = corner;

                GetCornerColor(corner, out indicies[0], out indicies[1], out indicies[2]);
                GetCornerColor(corner, out indicies_orig[0], out indicies_orig[1], out indicies_orig[2]);
            }
            
            public void ControlHandle(Key k)
            {
                switch (k)
                {
                    case Key.Up: 
                        if (option_y > 0)
                        {
                            option_y--;
                        } 
                        break;
                    case Key.Down: 
                        if (option_y < 2)
                        {
                            option_y++;
                        }
                        break;
                    case Key.Left: 
                        if (indicies[option_y] > 0)
                        {
                            indicies[option_y]--;
                        }
                        break;
                    case Key.Right: 
                        if (indicies[option_y] < 10)
                        {
                            indicies[option_y]++;
                        }
                        break;
                    case Key.X:
                        Menu.Menu.SetCornerColor(corner, indicies_orig[0], indicies_orig[1], indicies_orig[2]);
                        Owner.ReturnFromSubMenu();
                        break;
                    case Key.Circle:
                        Owner.MenuState.UpdateAllBackgrounds();
                        Owner.ReturnFromSubMenu();
                        break;
                    default:
                        break;
                }
                
                Menu.Menu.SetCornerColor(corner, indicies[0], indicies[1], indicies[2]);

                // This triggers an update of the background
                Visible = Visible;
            }
            
            protected override void DrawContents(Gdk.Drawable d)
            {
                Cairo.Context g = Gdk.CairoHelper.Create(d);
                
                g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
                g.SetFontSize(24);

                Color cr, cg, cb;

                for (int i = 0; i < 11; i++)
                {
                    cr = i == indicies[0] ? Colors.WHITE : Colors.GRAY_4;
                    cg = i ==  indicies[1] ? Colors.WHITE : Colors.GRAY_4;
                    cb = i ==  indicies[2] ? Colors.WHITE : Colors.GRAY_4;

                    int x = X + x0 + x_spacing * (i + 1);
                    
                    Text.ShadowedText(g, cr, i.ToString(), x, Y + yr);
                    Text.ShadowedText(g, cg, i.ToString(), x, Y + yg);
                    Text.ShadowedText(g, cb, i.ToString(), x, Y + yb);
                }
                
                cr = option_y == 0 ? Colors.WHITE : Colors.GRAY_4;
                cg = option_y == 1 ? Colors.WHITE : Colors.GRAY_4;
                cb = option_y == 2 ? Colors.WHITE : Colors.GRAY_4;

                
                Text.ShadowedText(g, cr, "R", X + x0, Y + yr);
                Text.ShadowedText(g, cg, "G", X + x0, Y + yg);
                Text.ShadowedText(g, cb, "B", X + x0, Y + yb);


                
                ((IDisposable)g.Target).Dispose();
                ((IDisposable)g).Dispose();
            }
            
            private WindowColor Owner { get; set; }
        }









        
        
        public WindowColor(SevenMenuState menuState, int x, int y, Main owner)
            : base(x, y, WIDTH, HEIGHT)
        {
            Owner = owner;
            MenuState = menuState;
        }
        
        public override void ControlHandle(Key k)
        {
            if (SubMenu != null)
            {
                SubMenu.ControlHandle(k);
            }
            else
            {
                switch (k)
                {
                    case Key.Up: 
                        if (option_y > 0)
                        {
                            option_y--;
                        } 
                        break;
                    case Key.Down: 
                        if (option_y < 1)
                        {
                            option_y++;
                        }
                        break;
                    case Key.Left: 
                        if (option_x > 0)
                        {
                            option_x--;
                        } 
                        break;
                    case Key.Right: 
                        if (option_x < 1)
                        {
                            option_x++;
                        }
                        break;
                    case Key.X:
                        SetNotControl();
                        break;
                    case Key.Circle:
                        int corner = option_x;

                        if (option_y > 0)
                        {
                            if (option_x == 0)
                            {
                                corner = 3;
                            }
                            else
                            {
                                corner = 2;
                            }
                        }

                        SubMenu = new WindowCornerColor(X, Y, corner, this);
                        break;
                    default:
                        break;
                }
            }
        }


        private void ReturnFromSubMenu()
        {
            SubMenu = null;

            // This will trigger an update of the background            
            Visible = Visible;

            // This will trigger an update of the owner's background
            Owner.Visible = Owner.Visible;
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            if (SubMenu != null)
            {
                SubMenu.Draw(d);
            }
            else
            {
                Cairo.Context g = Gdk.CairoHelper.Create(d);
            
                g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
                g.SetFontSize(24);
                       
                int x_cursor = option_x == 0 ? x0 : x1;
                int y_cursor = option_y == 0 ? y0 : y1;
                
                if (IsControl)
                {
                    Shapes.RenderCursor(g, X + x_cursor, Y + y_cursor);
                }
                                        
                ((IDisposable)g.Target).Dispose();
                ((IDisposable)g).Dispose();
            }
        }
        
        public override void SetAsControl()
        {
            Owner.SetNotControl();
            base.SetAsControl();
            option_x = 0;
            option_y = 0;
        }

        public override void SetNotControl()
        {
            base.SetNotControl();
            Owner.SetAsControl();
        }
        
        public override string Info { get { return String.Empty; } }

        private WindowCornerColor SubMenu { get; set; }

        private Main Owner { get; set; }
        
        private SevenMenuState MenuState { get; set; }
    }
}

