using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Phs
{      
    internal sealed class Info : Menu.Menu
    {
        #region Layout

        const int xpic = 30;
        const int ypic = 20;
        
        const int x_stats = xpic + Character.PROFILE_WIDTH + 15;
        const int y_stats = ypic + 20;

        const int y0 = 60;
        const int ya = 30;
        const int yb = 55;
        const int yc = 80;
        
        #endregion Layout

        public Info(SevenMenuState menuState, Menu.ScreenState screenState)
            : base(
                screenState.Width / 2,
                screenState.Height * 7 / 60,
                screenState.Width / 2 - 9,
                screenState.Height / 4)
        { 
            PhsList = menuState.PhsList;
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            if (PhsList.IsControl)
            {
                Gdk.GC gc = new Gdk.GC(d);
                Cairo.Context g = Gdk.CairoHelper.Create(d);
            
                g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
                g.SetFontSize(24);
            
            
                #region Character
            
                Character c = PhsList.Selection;
            
                if (c != null)
                {
                    Images.RenderProfileSmall(d, gc, X + xpic, Y + ypic, c);

                    Graphics.Stats.RenderCharacterStatus(d, gc, g, c, X + x_stats, Y + y_stats, false);

                }
                #endregion Character
            
            
                ((IDisposable)g.Target).Dispose();
                ((IDisposable)g).Dispose();
            }
        }

        private List PhsList { get; set; }
    }
}

