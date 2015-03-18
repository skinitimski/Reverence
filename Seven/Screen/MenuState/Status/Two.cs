using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Status
{      
    internal sealed class Two : Menu.Menu
    {
        const int x0 = 50; // labels
        const int x1 = 65; // row start
        
        const int x3 = 140; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = 330; // fury/sadness
        
        const int y = 50; // name row
        const int ya = y + 30;  // next row 
        const int yb = ya + 25; //    "
        const int yc = yb + 25; //    "
        
        const int ym = 200;
        const int yn = ym + 80;
        const int yo = yn + 80;
        const int yp = yo + 80;
        const int ys = 24;
        
        const int xpic = 15;
        const int ypic = 15;
        
        const int stop = 120;

        public Two()
            : base(
                2,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth - 10,
                Config.Instance.WindowHeight * 9 / 10)
        {
            Visible = false;
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            
            Cairo.Color greenish = COLOR_TEXT_TEAL;
            Cairo.Color gray = new Color(.4, .4, .4);
            Cairo.Color white = Colors.WHITE;
            
            string lvl, hp, hpm, mp, mpm, s;
            
            
            #region Character Status
            
            d.DrawPixbuf(gc, Seven.Party.Selected.Profile, 0, 0,
                         X + xpic, Y + ypic,
                         Character.PROFILE_WIDTH, Character.PROFILE_HEIGHT,
                         Gdk.RgbDither.None, 0, 0);
            
            g.Color = COLOR_TEXT_TEAL;
            g.MoveTo(X + x3, Y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + yc);
            g.ShowText("MP");
            g.Color = Colors.WHITE;
            
            Text.ShadowedText(g, Seven.Party.Selected.Name, X + x3, Y + y);
            
            lvl = Seven.Party.Selected.Level.ToString();
            hp = Seven.Party.Selected.HP.ToString() + "/";
            hpm = Seven.Party.Selected.MaxHP.ToString();
            mp = Seven.Party.Selected.MP.ToString() + "/";
            mpm = Seven.Party.Selected.MaxMP.ToString();
            
            te = g.TextExtents(lvl);
            Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + ya);
            
            te = g.TextExtents(hp);
            Text.ShadowedText(g, hp, X + x5 - te.Width, Y + yb);
            
            te = g.TextExtents(hpm);
            Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + yb);
            
            te = g.TextExtents(mp);
            Text.ShadowedText(g, mp, X + x5 - te.Width, Y + yc);
            
            te = g.TextExtents(mpm);
            Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + yc);
            
            #endregion Status
            
            
            
            Text.ShadowedText(g, greenish, "Attack", X + x0, Y + ym);
            Text.ShadowedText(g, greenish, "Halve", X + x0, Y + yn);
            Text.ShadowedText(g, greenish, "Void", X + x0, Y + yo);
            Text.ShadowedText(g, greenish, "Absorb", X + x0, Y + yp);
            
            g.SetFontSize(16);
            
            double x = (double)x1;
            double r = (double)(ym + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }
                
                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Seven.Party.Selected.Halves(e))
                    Text.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }
            
            x = (double)x1;
            r = (double)(yn + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }
                
                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Seven.Party.Selected.Halves(e))
                    Text.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }
            
            x = (double)x1;
            r = (double)(yo + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }
                
                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Seven.Party.Selected.Voids(e))
                    Text.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }
            
            x = (double)x1;
            r = (double)(yp + ys);
            foreach (Element e in Enum.GetValues(typeof(Element)))
            {
                if (x > W - stop)
                {
                    x = x1;
                    r = r + ys;
                }
                
                s = e.ToString() + " ";
                te = g.TextExtents(s);
                if (Seven.Party.Selected.Absorbs(e))
                    Text.ShadowedText(g, white, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, gray, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }
            
            
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}

