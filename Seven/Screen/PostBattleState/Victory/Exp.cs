using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryEXP : GameMenu
    {
        #region Layout
        
        const int x1 = 100;
        const int x2 = 300;
        const int ys = 28;
        
#endregion

        public VictoryEXP(Menu.ScreenState screenState)
            : base(
                2,
                screenState.Height  * 2 / 15,
                screenState.Width / 2,
                screenState.Height / 12 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string exp = Seven.PostBattleState.Exp.ToString() + "p";
            te = g.TextExtents(exp);
            Text.ShadowedText(g, "EXP", X + x1, Y + ys);
            Text.ShadowedText(g, exp, X + x2 - te.Width, Y + ys);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

