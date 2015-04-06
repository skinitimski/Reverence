using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryAP : GameMenu
    {
        #region Layout
        
        const int x1 = 100;
        const int x2 = 250;
        const int ys = 28;
#endregion
        
        public VictoryAP()
            : base(
                Seven.Config.WindowWidth / 2,
                Seven.Config.WindowHeight * 2 / 15,
                Seven.Config.WindowWidth / 2 - 8,
                Seven.Config.WindowHeight / 12 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string ap = Seven.PostBattleState.AP.ToString() + "p";
            te = g.TextExtents(ap);
            Text.ShadowedText(g, "AP", X + x1, Y + ys);
            Text.ShadowedText(g, ap, X + x2 - te.Width, Y + ys);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

