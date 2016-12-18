using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenPostBattleState = Atmosphere.Reverence.Seven.State.PostBattleState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Hoard
{  
    internal sealed class GilLeft : GameMenu
    {
        #region Layout
        
        const int x1 = 50;
        const int x2 = 350;
        const int ys = 28;
        
#endregion
        
        public GilLeft(SevenPostBattleState postBattleState, ScreenState screenState)
            : base(
                2,
                screenState.Height  * 2 / 15,
                screenState.Width / 2,
                screenState.Height / 12 - 6)
        {
            PostBattleState = postBattleState;
        }


        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string gil = PostBattleState.Gil.ToString() + "g";
            te = g.TextExtents(gil);
            Text.ShadowedText(g, "Gained Gil", X + x1, Y + ys);
            Text.ShadowedText(g, gil, X + x2 - te.Width, Y + ys);
        }
        
        private SevenPostBattleState PostBattleState { get; set; }
    }
}

