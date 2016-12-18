using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class InfoBar : GameMenu
    {
        public InfoBar(SevenBattleState battleState, ScreenState screenState)
            : base(
                5,
                screenState.Height * 6 / 10,
                screenState.Width - 10,
                (screenState.Height / 10) - 7,
                false)
        {
            BattleState = battleState;
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string msg = "";

            if (BattleState.Screen.Control != null)
            {
                msg = BattleState.Screen.Control.Info;
            }

            te = g.TextExtents(msg);

            Text.ShadowedText(g, msg, X + (W / 2) - (te.Width / 2), Y + 32);
        }
        
        private SevenBattleState BattleState { get; set; }
    }
}
