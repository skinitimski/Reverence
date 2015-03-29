using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Materia
{  
    internal sealed class Label : Menu.Menu
    {
        public Label()
            : base(
                Config.Instance.WindowWidth * 4 / 5,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth / 5 - 10,
                Config.Instance.WindowHeight / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(Text.FONT_SIZE_LABEL);
            
            Text.ShadowedText(g, "Materia", X + 20, Y + 25);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}

