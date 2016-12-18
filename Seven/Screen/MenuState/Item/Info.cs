using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Item
{
    internal sealed class Info : Menu.Menu
    {
        public Info(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height / 20 + screenState.Height / 15,
                screenState.Width - 10,
                screenState.Height / 15)
        { 
            MenuState = menuState;
        }
        
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, MenuState.ItemScreen.Control.Info, X + 20, Y + 28);
        }
        
        private SevenMenuState MenuState { get; set; }
    }

}

