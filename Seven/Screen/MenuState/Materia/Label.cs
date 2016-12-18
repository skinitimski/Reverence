using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Materia
{  
    internal sealed class Label : Menu.Menu
    {
        public Label(Menu.ScreenState screenState)
            : base(
                screenState.Width * 4 / 5,
                screenState.Height / 20,
                screenState.Width / 5 - 10,
                screenState.Height / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(Text.FONT_SIZE_LABEL);
            
            Text.ShadowedText(g, "Materia", X + 20, Y + 25);
        }
    }

}

