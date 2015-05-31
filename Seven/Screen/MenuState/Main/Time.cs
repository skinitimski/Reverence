using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Main
{    
    internal sealed class Time : Menu.Menu
    {
        #region Layout
        
        int x1 = 10;
        int x2;
        int y1 = 30;
        int y2 = 65;
        
        #endregion

        public Time(Menu.ScreenState screenState)
            : base(
                screenState.Width * 3 / 4 - 10,
                screenState.Height * 7 / 10,
                screenState.Width / 4,
                screenState.Height * 3 / 20)
        {
            x2 = screenState.Width / 4 - 10;
        }
        
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            
            TextExtents te;
            
            Text.ShadowedText(g, "Time", X + x1, Y + y1);
            Text.ShadowedText(g, "Gil", X + x1, Y + y2);
            
            g.SelectFontFace("Courier New", FontSlant.Normal, FontWeight.Bold);
            
            long s, m, h;
            s = Seven.Party.Clock.Seconds;
            m = Seven.Party.Clock.Minutes;
            h = Seven.Party.Clock.Hours;
            
            string time = String.Format("{0}{1}:{2}{3}:{4}{5}",
                                        h < 10 ? "0" : "", h,
                                        m < 10 ? "0" : "", m,
                                        s < 10 ? "0" : "", s);
            
            te = g.TextExtents(time);
            Text.ShadowedText(g, time, X + x2 - te.Width, Y + y1);
            
            string gil = Seven.Party.Gil.ToString();
            te = g.TextExtents(gil);
            Text.ShadowedText(g, gil, X + x2 - te.Width, Y + y2);
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

}

