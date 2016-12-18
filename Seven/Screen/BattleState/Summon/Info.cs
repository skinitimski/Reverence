using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Summon
{
    internal sealed class Info : GameMenu
    {
        #region Layout
        
        const int x0 = 15; // ability col
        const int x1 = 40; // mpneeded col
        const int x2 = 90; // slash col
        const int x3 = 130; // mptot col
        const int y0 = 30; // 
        const int ys = 30;
        
        #endregion Layout

        public Info(SevenBattleState battleState, ScreenState screenState)
            : base(
                screenState.Width * 3 / 4 + 12,
                screenState.Height * 7 / 10 + 20,
                screenState.Width / 4 - 17,
                (screenState.Height * 5 / 20) - 25)
        {
            Visible = false;
            BattleState = battleState;
        }
        
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            SummonMenuEntry s = BattleState.Commanding.SummonMenu.Selected;
            
            int row = 0;
            
            if (s != null)
            {
                string cost = s.Spell.MPCost.ToString();
                Text.ShadowedText(g, "MP Req", X + x1, Y + y0);
                
                row++;
                
                cost = cost + "/";
                te = g.TextExtents(cost);
                Text.ShadowedText(g, cost, X + x2 - te.Width, Y + y0 + (row * ys));
                
                string tot = BattleState.Commanding.MP.ToString();
                te = g.TextExtents(tot);
                Text.ShadowedText(g, tot, X + x3 - te.Width, Y + y0 + (row * ys));
                
                row++;
                
//                if (s.AddedAbility.Contains(AddedAbility.All))
//                {
//                    string msg = "All x";
//                    msg += s.AllCount.ToString();
//                    Text.ShadowedText(g, msg, X + x0, Y + y0 + (row * ys));
//                    row++;
//                }
                
//                if (s.AddedAbility.Contains(AddedAbility.QuadraMagic))
//                {
//                    string msg = "Q-Magic x";
//                    msg += s.QMagicCount.ToString();
//                    Text.ShadowedText(g, msg, X + x0, Y + y0 + (row * ys));
//                    row++;
//                }
                
            }
        }
        
        private SevenBattleState BattleState { get; set; }
    }
    
    
}

