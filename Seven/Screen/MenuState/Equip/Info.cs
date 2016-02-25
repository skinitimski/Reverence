using System;
using Cairo;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Info : Menu.Menu
    {
        public Info(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height * 7 / 20,
                screenState.Width - 10,
                screenState.Height / 15)
        {
            MenuState = menuState;
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, MenuState.EquipScreen.Control.Info, X + 20, Y + 27);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private SevenMenuState MenuState { get; set; }
    }
}

