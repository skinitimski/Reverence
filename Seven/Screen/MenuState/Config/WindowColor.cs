using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

using Atmosphere.Reverence.Seven;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Config
{
    internal sealed class WindowColor : ControlMenu
    {
        public const int WIDTH = 200;
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

            int x_spacing;

            int corner;
            int rIndex;
            int gIndex;
            int bIndex;

            int option_y;

            public WindowCornerColor(int x, int y, int corner, WindowColor owner)
                : base(x, y, WIDTH, HEIGHT)
            {
                x_spacing = (Width - x0 * 2) / 11;

                this.corner = corner;

                GetCornerColor(corner, ref rIndex, ref gIndex, ref bIndex);
                
                Owner = owner;
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
                        switch (option_y)
                        {
                            case 0:
                                rIndex--;
                                break;
                            case 1:
                                gIndex--;
                                break;
                            case 2:
                                bIndex--;
                                break;
                        }
                        break;
                    case Key.Right: 
                        switch (option_y)
                        {
                            case 0:
                                rIndex++;
                                break;
                            case 1:
                                gIndex++;
                                break;
                            case 2:
                                bIndex++;
                                break;
                        }
                        break;
                    case Key.X:
                        Menu.Menu.SetCornerColor(corner, rIndex, gIndex, bIndex);
                        Seven.MenuState.UpdateAllBackgrounds();
                        Owner.ReturnFromSubMenu();
                        break;
                    case Key.Circle:
                        break;
                    default:
                        break;
                }
            }
            
            protected override void DrawContents(Gdk.Drawable d)
            {
                Cairo.Context g = Gdk.CairoHelper.Create(d);
                
                g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
                g.SetFontSize(24);

                Color cr, cg, cb;

                for (int i = 0; i < 11; i++)
                {
                    cr = i == rIndex ? Colors.WHITE : Colors.GRAY_4;
                    cg = i == gIndex ? Colors.WHITE : Colors.GRAY_4;
                    cb = i == bIndex ? Colors.WHITE : Colors.GRAY_4;

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









        
        
        public WindowColor(int x, int y, ControlMenu owner)
            : base(x, y, WIDTH, HEIGHT)
        {
            Owner = owner;
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


        protected void ReturnFromSubMenu()
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
        
        public override string Info
        { get { return ""; } }


        private WindowCornerColor SubMenu { get; set; }

        private ControlMenu Owner { get; set; }


    }

}
