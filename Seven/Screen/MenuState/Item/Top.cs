using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Item
{
    internal sealed class Top : ControlMenu
    {
        #region Layout
        
        const int x1 = 50; // Use
        const int x2 = 150; // Arrange
        const int x3 = 300; // Key Items
        const int y = 25;
        
        const int xs = 15; // spacing for cursor
        static int cx = x1 - xs; // see? told ya
        const int cy = y - 10;
        
#endregion
        
        private int option = 0;
        
        
        public Top(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height / 20,
                screenState.Width - 10,
                screenState.Height / 15)
        { 
            MenuState = menuState;
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Left:
                    if (option > 0) option--;
                    break;
                case Key.Right:
                    if (option < 2) option++;
                    break;
                case Key.X:
                    MenuState.ChangeScreen(MenuState.MainScreen);
                    MenuState.ItemList.Reset();
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case 0:
                            MenuState.ItemScreen.ChangeControl(MenuState.ItemList);
                            break;
                        case 1:
                            MenuState.Party.Inventory.Sort();
                            break;
                        case 2:
                            break;
                    }
                    break;
                default:
                    break;
            }
            switch (option)
            {
                case 0: cx = x1 - xs; break;
                case 1: cx = x2 - xs; break;
                case 2: cx = x3 - xs; break;
                default: break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy);
            }

            Text.ShadowedText(g, "Use", X + x1, Y + y);
            Text.ShadowedText(g, "Arrange", X + x2, Y + y);
            Text.ShadowedText(g, "Key Items", X + x3, Y + y);
        }
        
        public override string Info
        {
            get
            {
                switch (option)
                {
                    case 0:
                        return "Use an item";
                    case 1:
                        return "Arrange by type/name";
                    case 2:
                        return "You have no key items, trust me";
                    default:
                        return "";
                }
            }
        }
        
        private SevenMenuState MenuState { get; set; }
    }

}

