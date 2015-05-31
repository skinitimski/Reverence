using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class InfoBar : GameMenu
    {
        public InfoBar(Menu.ScreenState screenState)
            : base(
                5,
                screenState.Height * 6 / 10,
                screenState.Width - 10,
                (screenState.Height / 10) - 7,
                false)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string msg = "";

            if (Seven.BattleState.Screen.Control != null)
            {
                msg = Seven.BattleState.Screen.Control.Info;
            }

            te = g.TextExtents(msg);

            Text.ShadowedText(g, msg, X + (W / 2) - (te.Width / 2), Y + 32);



            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}
