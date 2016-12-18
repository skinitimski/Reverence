using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Save
{      
    internal sealed class Label : Menu.Menu
    {
        public Label(Menu.ScreenState screenState)
            : base(
                screenState.Width * 3 / 4,
                screenState.Height / 20,
                screenState.Width / 4 - 10,
                screenState.Height / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, "Save", X + 20, Y + 25);
        }
    }

}

