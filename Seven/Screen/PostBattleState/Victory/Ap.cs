using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenPostBattleState = Atmosphere.Reverence.Seven.State.PostBattleState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryAP : GameMenu
    {
        #region Layout
        
        const int x1 = 100;
        const int x2 = 250;
        const int ys = 28;
#endregion
        
        public VictoryAP(SevenPostBattleState postBattleState, ScreenState screenState)
            : base(
                screenState.Width / 2,
                screenState.Height * 2 / 15,
                screenState.Width / 2 - 8,
                screenState.Height / 12 - 6)
        { 
            AP = postBattleState.AP + "p";
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            te = g.TextExtents(AP);
            Text.ShadowedText(g, "AP", X + x1, Y + ys);
            Text.ShadowedText(g, AP, X + x2 - te.Width, Y + ys);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private string AP { get; set; }
    }
}

