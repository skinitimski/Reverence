using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Label : Menu.Menu
    {
        public Label()
            : base(
                Config.Instance.WindowWidth * 5 / 6,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth / 6 - 10,
                Config.Instance.WindowHeight / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(Text.FONT_SIZE_LABEL);
            
            Text.ShadowedText(g, "Equip", X + 20, Y + 25);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

