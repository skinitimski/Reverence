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
        
        public Label()
            : base(
                2,
                Seven.Config.WindowHeight / 20,
                Seven.Config.WindowWidth - 10,
                Seven.Config.WindowHeight / 12 - 6)
        { }
        public override void ControlHandle(Key k) { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, "Gained gil and item(s).", X + xs, Y + ys);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        public override string Info { get { return ""; } }
    }
}

