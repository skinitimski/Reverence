using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenPostBattleState = Atmosphere.Reverence.Seven.State.PostBattleState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Hoard
{  
    internal sealed class HoardGilRight : GameMenu
    {
        #region Layout
        
        const int x1 = 100;
        const int x2 = 250;
        const int ys = 28;
#endregion
        
        public HoardGilRight(SevenPostBattleState postBattleState, ScreenState screenState)
            : base(
                screenState.Width / 2,
                screenState.Height * 2 / 15,
                screenState.Width / 2 - 8,
                screenState.Height / 12 - 6)
        { 
            Party = postBattleState.Party;
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string gil = Party.Gil.ToString() + "g";
            te = g.TextExtents(gil);
            Text.ShadowedText(g, "Gil", X + x1, Y + ys);
            Text.ShadowedText(g, gil, X + x2 - te.Width, Y + ys);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private Party Party { get; set; }
    }
}

