using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Battle.Event;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class EventBar : GameMenu
    {
        public EventBar(SevenBattleState battleState, ScreenState screenState)
            : base(
                screenState.Width / 16,
                20,
                screenState.Width * 7 / 8,
                screenState.Height / 10)
        {
            BattleState = battleState;
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {                  
            BattleEvent e = BattleState.ActiveAbility;
            
            if (e != null)
            {
                g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
                g.SetFontSize(24);
                                
                string msg = e.GetStatus();
                
                TextExtents te = g.TextExtents(msg);
                
                Text.ShadowedText(g, msg, X + (W / 2) - (te.Width / 2), Y + 32);
            }
        }
        
        private SevenBattleState BattleState { get; set; }
    }
}
