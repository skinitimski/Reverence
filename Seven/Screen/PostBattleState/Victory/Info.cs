using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal abstract class Info : GameMenu
    {
        #region Layout
        
        const int x1 = 140;
        const int x2 = 170;
        const int x3 = x2 + 120;
        const int x4 = 600;
        const int x5 = x4 + 180;
        const int y0 = 50;
        const int y1 = 100;
        
        const int xpic = 15;
        const int ypic = 15;
        
#endregion
        
        protected Info(int x, int y, int w, int h)
            : base(x, y, w, h)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            
            Character c = Seven.Party[PartyIndex];

            if (c != null)
            {
                Images.RenderProfile(d, gc, X + xpic, Y + ypic, c);
                
                Text.ShadowedText(g, c.Name, X + x1, Y + y0);
                Text.ShadowedText(g, "Level:", X + x2, Y + y1);
                
                string lvl = c.Level.ToString();
                te = g.TextExtents(lvl);
                Text.ShadowedText(g, lvl, X + x3 - te.Width, Y + y1);
                
                string temp = "Exp:";
                te = g.TextExtents(temp);
                Text.ShadowedText(g, temp, X + x4 - te.Width, Y + y0);
                
                temp = "For level up:";
                te = g.TextExtents(temp);
                Text.ShadowedText(g, temp, X + x4 - te.Width, Y + y1);
                
                string exp = c.Exp.ToString() + "p";
                te = g.TextExtents(exp);
                Text.ShadowedText(g, exp, X + x5 - te.Width, Y + y0);
                
                string expNext = c.ToNextLevel.ToString() + "p";
                te = g.TextExtents(expNext);
                Text.ShadowedText(g, expNext, X + x5 - te.Width, Y + y1);
            }
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        protected abstract int PartyIndex { get; }
    }
}

