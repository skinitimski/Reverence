using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class StatusBarLeft : GameMenu
    {
        #region Layout
        
        const int x1 = 30;
        const int y0 = 35;
        const int y1 = 80;
        const int y2 = 125;
        
        #endregion Layout

        public StatusBarLeft(SevenBattleState battleState, ScreenState screenState)
            : base(
                5,
                screenState.Height * 7 / 10,
                screenState.Width * 2 / 5,
                (screenState.Height * 5 / 20) - 5)
        {
            BattleState = battleState;
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(28);
            
            
            if (BattleState.Allies[0] != null)
            {
                DrawAllyStatus(g, BattleState.Allies[0], y0);
            }
            
            if (BattleState.Allies[1] != null)
            {
                DrawAllyStatus(g, BattleState.Allies[1], y1);
            }
            
            if (BattleState.Allies[2] != null)
            {
                DrawAllyStatus(g, BattleState.Allies[2], y2);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private void DrawAllyStatus(Context g, Ally a, int y)
        {                        
            Color nameColor = Colors.WHITE;
            
            if (a.Death)
            {
                nameColor = Colors.TEXT_RED;
            }
            else if (a.NearDeath)
            {
                nameColor = Colors.TEXT_YELLOW;
            }

            Text.ShadowedText(g, nameColor, a.Name, X + x1, Y + y);
        }
        
        private SevenBattleState BattleState { get; set; }
    }
}

