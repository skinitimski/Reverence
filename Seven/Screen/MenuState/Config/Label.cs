using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Config
{
    internal sealed class Label : Menu.Menu
    {
        public Label()
            : base(
                Seven.Config.WindowWidth * 5 / 6,
                Seven.Config.WindowHeight / 20,
                Seven.Config.WindowWidth / 6 - 10,
                Seven.Config.WindowHeight / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(Text.FONT_SIZE_LABEL);
            
            Text.ShadowedText(g, "Config", X + 20, Y + 25);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

