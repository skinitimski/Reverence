using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Save
{      
    internal sealed class Label : Menu.Menu
    {
        public Label()
            : base(
                Config.Instance.WindowWidth * 3 / 4,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth / 4 - 10,
                Config.Instance.WindowHeight / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, "Save", X + 20, Y + 25);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}

