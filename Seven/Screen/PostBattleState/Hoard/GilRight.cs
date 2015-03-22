using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Hoard
{  
    internal sealed class HoardGilRight : GameMenu
    {
        #region Layout
        
        const int x1 = 100;
        const int x2 = 250;
        const int ys = 28;
#endregion
        
        public HoardGilRight()
            : base(
                Config.Instance.WindowWidth / 2,
                Config.Instance.WindowHeight * 2 / 15,
                Config.Instance.WindowWidth / 2 - 8,
                Config.Instance.WindowHeight / 12 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string gil = Seven.Party.Gil.ToString() + "g";
            te = g.TextExtents(gil);
            Text.ShadowedText(g, "Gil", X + x1, Y + ys);
            Text.ShadowedText(g, gil, X + x2 - te.Width, Y + ys);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

