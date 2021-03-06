using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Status
{      
    internal sealed class Three : StatusBase
    {
        const int x0 = 50; // column 1
        const int x1 = 275; // column 2
        const int x3 = 140; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = 330; // fury/sadness
        
        const int y = 50; // name row
        const int ya = y + 30;  // next row 
        const int yb = ya + 25; //    "
        const int yc = yb + 25; //    "


        public Three(SevenMenuState menuState, ScreenState screenState)
            : base(menuState, screenState)
        {
            Visible = false;
        }
        
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            
            TextExtents te;
            

            #region Character Status
            
            DrawCharacterStatus(d, g);
            
            #endregion Status
        }
    }
}

