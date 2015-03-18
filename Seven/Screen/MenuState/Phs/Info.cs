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
        
        const int x1 = 30; // pic
        const int x3 = 170; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        
        const int yp = 15;
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
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;
            
            #region Character
            
            Character c = Seven.MenuState.PhsList.Selection;
            
            if (c != null)
            {
                d.DrawPixbuf(gc, c.ProfileSmall, 0, 0, X + x1, Y + yp,
                             Character.PROFILE_WIDTH_SMALL, Character.PROFILE_HEIGHT_SMALL,
                             Gdk.RgbDither.None, 0, 0);
                
                g.Color = COLOR_TEXT_TEAL;
                g.MoveTo(X + x3, Y + y0 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y0 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y0 + yc);
                g.ShowText("MP");
                g.Color = Colors.WHITE;
                
                Color namec = Colors.WHITE;

                if (c.Death)
                {
                    namec = COLOR_TEXT_RED;
                }
                else if (c.NearDeath)
                {
                    namec = COLOR_TEXT_YELLOW;
                }
                
                Text.ShadowedText(g, namec, c.Name, X + x3, Y + y0);
                
                lvl = c.Level.ToString();
                hp = c.HP.ToString() + "/";
                hpm = c.MaxHP.ToString();
                mp = c.MP.ToString() + "/";
                mpm = c.MaxMP.ToString();
                
                te = g.TextExtents(lvl);
                Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + y0 + ya);
                te = g.TextExtents(hp);
                Text.ShadowedText(g, hp, X + x5 - te.Width, Y + y0 + yb);
                te = g.TextExtents(hpm);
                Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + y0 + yb);
                te = g.TextExtents(mp);
                Text.ShadowedText(g, mp, X + x5 - te.Width, Y + y0 + yc);
                te = g.TextExtents(mpm);
                Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + y0 + yc);
            }
            #endregion Character
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

