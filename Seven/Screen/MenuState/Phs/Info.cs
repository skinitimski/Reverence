using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Phs
{      
    internal sealed class Info : Menu.Menu
    {
        #region Layout

        const int xpic = 30;
        const int ypic = 20;
        
        const int x_stats = xpic + Character.PROFILE_WIDTH + 15;
        const int y_stats = ypic + 20;

        const int y0 = 60;
        const int ya = 30;
        const int yb = 55;
        const int yc = 80;
        
        #endregion Layout

        public Info()
            : base(
                Config.Instance.WindowWidth / 2,
                Config.Instance.WindowHeight * 7 / 60,
                Config.Instance.WindowWidth / 2 - 9,
                Config.Instance.WindowHeight / 4)
        { }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;
            
            #region Character
            
            Character c = Seven.MenuState.PhsList.Selection;
            
            if (c != null)
            {
                Images.RenderProfileSmall(d, gc, X + xpic, Y + ypic, c);

                Graphics.Stats.RenderCharacterStatus(d, gc, g, c, X + x_stats, Y + y_stats, false);

            }
            #endregion Character
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

