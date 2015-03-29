using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Graphics;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Status
{      
    internal sealed class Two : StatusBase
    {
        const int x0 = 50; // labels
        const int x1 = 65; // row start
        
        const int ym = 200;
        const int yn = ym + 80;
        const int yo = yn + 80;
        const int yp = yo + 80;
        const int ys = 24;

        const int stop = 120;
                

        public Two()
            : base()
        {
            Visible = false;
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            

            
            string s;
            
            
            #region Character Status
            
            DrawCharacterStatus(d, gc, g);
            
            #endregion Status
            
            
            
            Text.ShadowedText(g, Colors.TEXT_TEAL, "Attack", X + x0, Y + ym);
            Text.ShadowedText(g, Colors.TEXT_TEAL, "Halve", X + x0, Y + yn);
            Text.ShadowedText(g, Colors.TEXT_TEAL, "Void", X + x0, Y + yo);
            Text.ShadowedText(g, Colors.TEXT_TEAL, "Absorb", X + x0, Y + yp);
            
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
                    Text.ShadowedText(g, Colors.WHITE, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, Colors.GRAY_4, e.ToString(), X + x, Y + r);
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
                    Text.ShadowedText(g, Colors.WHITE, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, Colors.GRAY_4, e.ToString(), X + x, Y + r);
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
                    Text.ShadowedText(g, Colors.WHITE, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, Colors.GRAY_4, e.ToString(), X + x, Y + r);
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
                    Text.ShadowedText(g, Colors.WHITE, e.ToString(), X + x, Y + r);
                else
                    Text.ShadowedText(g, Colors.GRAY_4, e.ToString(), X + x, Y + r);
                x += te.XAdvance;
            }
            
            
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}

