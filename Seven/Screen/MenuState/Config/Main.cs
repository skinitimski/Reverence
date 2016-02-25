using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Config
{    
    internal sealed class Main : ControlMenu
    {
        #region Layout

        const int x_options = 30;                
        const int x_cursor = x_options - 15;
                
        const int y_displacement_cursor = -10;

        const int y_windowColor = 50;
        const int y_atb = y_windowColor + WindowColor.HEIGHT + 10;
        
        #endregion

        private class Option
        {
            public Option(int y, string description)
            {
                Y = y;
                Description = description;
            }

            public int Y { get; private  set; }
            public string Description { get; private set; }
        }

        private Option[] _options = new Option[]
        {
            new Option(y_windowColor, "Window Color"),
            new Option(y_atb, "ATB"),
        };

        
        private int option = 0;


        public Main(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height * 7 / 60,
                screenState.Width - 10,
                screenState.Height * 5 / 6)
        {
            MenuState = menuState;

            int wc_x = x_options + 300;
            int wc_y = Y + 15;

            WindowColorMenu = new WindowColor(menuState, wc_x, wc_y, this);
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
                        if (option > 0)
                        {
                            option--;
                        } 
                        break;
                    case Key.Down: 
                        if (option < _options.Length - 1)
                        {
                            option++;
                        }
                        break;
                    case Key.X:
                        option = 0;
                        MenuState.ChangeScreen(MenuState.MainScreen);
                        break;
                    case Key.Circle:
                        switch (option)
                        {
                            case 0:                             
                                SubMenu = WindowColorMenu;
                                SubMenu.SetAsControl();
                                break;
                            case 1:
                                break;
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {      
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            for (int i = 0; i < _options.Length; i++)
            {
                Text.ShadowedText(g, _options[i].Description, X + x_options, Y + _options[i].Y);
            }

            WindowColorMenu.Draw(d);
                                     
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + x_cursor, Y + _options[option].Y + y_displacement_cursor);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            if (SubMenu != null)
            {
                SubMenu = null;
            }

            base.SetAsControl();
        }
        
        public override string Info { get { return ""; } }

        private ControlMenu SubMenu { get; set; }

        private WindowColor WindowColorMenu { get; set; }

        private SevenMenuState MenuState { get; set; }
    }

}

