using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Main
{  
    internal sealed class Location : Menu.Menu
    {
        #region Layout
        
        int x = 35;
        int y = 35;
        
        #endregion

        public Location(Menu.ScreenState screenState)
            : base(
                screenState.Width / 2,
                screenState.Height * 17 / 20 + 6,
                screenState.Width / 2 - 10,
                screenState.Height / 10)
        { }
        
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            
            Text.ShadowedText(g, "FF7 Battle Arena", X + x, Y + y);
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

