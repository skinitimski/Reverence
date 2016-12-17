using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Gtk;
using Gdk;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class PausedBar : GameMenu
    {            
        const string PAUSED = "Paused!";
        
        const int x_text = 32;
        const int y_text = 34;
        
        public PausedBar(ScreenState screenState)
            : base(
                (screenState.Width * 2) / 5,
                (screenState.Height * 3) / 10,
                screenState.Width / 5,
                (screenState.Height / 10) - 7)
        {
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, PAUSED, X + x_text, Y + y_text);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

