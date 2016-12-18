using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Hoard
{  
    internal sealed class Label : ControlMenu
    {
        #region Layout
        
        const int xs = 20;
        const int ys = 28;
        
        #endregion Layout
        
        public Label(Menu.ScreenState screenState)
            : base(
                2,
                screenState.Height / 20,
                screenState.Width - 10,
                screenState.Height / 12 - 6)
        { }

        public override void ControlHandle(Key k) { }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, "Gained gil and item(s).", X + xs, Y + ys);
        }

        public override string Info { get { return String.Empty; } }
    }
}

