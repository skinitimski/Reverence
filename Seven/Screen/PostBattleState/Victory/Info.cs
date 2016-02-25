using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenPostBattleState = Atmosphere.Reverence.Seven.State.PostBattleState;

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
        
        const int xpic = 4;
        const int ypic = 4;
        
#endregion
        
        protected Info(SevenPostBattleState postBattleState, int x, int y, int w, int h, int index)
            : base(x, y, w, h)
        { 
            Character = postBattleState.Party[index];
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            


            Color textColor = Character.Death ? Colors.TEXT_RED : Colors.WHITE;



            if (Character != null)
            {
                Images.RenderProfile(d, gc, X + xpic, Y + ypic, Character);
                
                Text.ShadowedText(g, textColor,Character.Name, X + x1, Y + y0);
                Text.ShadowedText(g, textColor, "Level:", X + x2, Y + y1);
                
                string lvl = Character.Level.ToString();
                te = g.TextExtents(lvl);
                Text.ShadowedText(g, textColor, lvl, X + x3 - te.Width, Y + y1);
                
                string temp = "Exp:";
                te = g.TextExtents(temp);
                Text.ShadowedText(g, textColor, temp, X + x4 - te.Width, Y + y0);
                
                temp = "For level up:";
                te = g.TextExtents(temp);
                Text.ShadowedText(g, textColor, temp, X + x4 - te.Width, Y + y1);
                
                string exp = Character.Exp.ToString() + "p";
                te = g.TextExtents(exp);
                Text.ShadowedText(g, textColor, exp, X + x5 - te.Width, Y + y0);
                
                string expNext = Character.ExpToNextLevel.ToString() + "p";
                te = g.TextExtents(expNext);
                Text.ShadowedText(g, textColor, expNext, X + x5 - te.Width, Y + y1);
            }
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        protected Character Character { get; private set; }
    }
}

