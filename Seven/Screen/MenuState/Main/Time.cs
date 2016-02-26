using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Time;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

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

        public Time(SevenMenuState menuState, Menu.ScreenState screenState)
            : base(
                screenState.Width * 3 / 4 - 10,
                screenState.Height * 7 / 10,
                screenState.Width / 4,
                screenState.Height * 3 / 20)
        {
            x2 = screenState.Width / 4 - 10;
            GameClock = menuState.Party.Clock;
            Gil = menuState.Party.Gil.ToString();
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
            s = GameClock.Seconds;
            m = GameClock.Minutes;
            h = GameClock.Hours;
            
            string time = String.Format("{0:D2}:{1:D2}:{2:D2}", h, m, s);

            te = g.TextExtents(time);
            Text.ShadowedText(g, time, X + x2 - te.Width, Y + y1);

            te = g.TextExtents(Gil);
            Text.ShadowedText(g, Gil, X + x2 - te.Width, Y + y2);
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private Clock GameClock { get; set; }

        private string Gil { get; set; }
    }

}

