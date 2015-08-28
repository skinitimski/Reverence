using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryLabel : ControlMenu
    {
        #region Layout
        
        const int xs = 20;
        const int ys = 28;
        
        #endregion Layout
        
        public VictoryLabel(Menu.ScreenState screenState)
            : base(
                2,
                screenState.Height / 20,
                screenState.Width - 10,
                screenState.Height / 12 - 6)
        { 
        }

        public override void ControlHandle(Key k) { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, "Gained Experience points and AP.", X + xs, Y + ys);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override string Info { get { return ""; } }
    }
}

