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

        public Location()
            : base(
                Config.Instance.WindowWidth / 2,
                Config.Instance.WindowHeight * 17 / 20 + 6,
                Config.Instance.WindowWidth / 2 - 10,
                Config.Instance.WindowHeight / 10)
        { }
        
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            
            Text.ShadowedText(g, "FF7 Battle Arena", X + x, Y + y);
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

