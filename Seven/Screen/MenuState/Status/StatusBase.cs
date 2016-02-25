using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Status
{
    internal abstract class StatusBase : Menu.Menu
    {
        const int xpic = 15;
        const int ypic = 15;
        
        const int x_status = xpic + Character.PROFILE_WIDTH + 15;
        const int y_status = ypic + 35;
        
        protected StatusBase(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height / 20,
                screenState.Width - 10,
                screenState.Height * 9 / 10)
        {
            Party = menuState.Party;
        }

        protected void DrawCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Context g)
        {
            Images.RenderProfile(d, gc, X + xpic, Y + ypic, Party.Selected);
            
            Stats.RenderCharacterStatus(d, gc, g, Party.Selected, X + x_status, Y + y_status);
        }
        
        protected Party Party { get; private set; }
    }
}

