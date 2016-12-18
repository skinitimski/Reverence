using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenPostBattleState = Atmosphere.Reverence.Seven.State.PostBattleState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryEXP : GameMenu
    {
        #region Layout
        
        const int x1 = 100;
        const int x2 = 300;
        const int ys = 28;
        
#endregion

        public VictoryEXP(SevenPostBattleState postBattleState, ScreenState screenState)
            : base(
                2,
                screenState.Height  * 2 / 15,
                screenState.Width / 2,
                screenState.Height / 12 - 6)
        { 
            Exp = postBattleState.Exp + "p";
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;

            te = g.TextExtents(Exp);
            Text.ShadowedText(g, "EXP", X + x1, Y + ys);
            Text.ShadowedText(g, Exp, X + x2 - te.Width, Y + ys);
        }

        private string Exp { get; set; }
    }
}

