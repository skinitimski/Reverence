using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Battle;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class StatusBarRight : GameMenu
    {
        const int x1 = 90; // hp
        const int x2 = 150; // hpm
        const int x3 = 245; // mp
        const int x4 = 290; // mpm
        const int x5 = 365; // limit
        const int x6 = 440; // time
        const int y0 = 30;
        const int y1 = 75;
        const int y2 = 120;

        public StatusBarRight()
            : base(
                Config.Instance.WindowWidth * 2 / 5 + 12,
                Config.Instance.WindowHeight * 7 / 10,
                Config.Instance.WindowWidth * 3 / 5 - 17,
                Config.Instance.WindowHeight * 5 / 20 - 5)
        {
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            if (Seven.BattleState.Allies[0] != null)
            {
                DrawAllyStatus(d, Seven.BattleState.Allies[0], y0);
            }
            if (Seven.BattleState.Allies[1] != null)
            {
                DrawAllyStatus(d, Seven.BattleState.Allies[1], y1);
            }
            if (Seven.BattleState.Allies[2] != null)
            {
                DrawAllyStatus(d, Seven.BattleState.Allies[2], y2);
            }
            

            

        }

        private void DrawAllyStatus(Gdk.Drawable d, Ally c, int y)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            string hp, hpmax, mp, mpmax, limit, time;
            
            TextExtents te;

            hp = c.HP.ToString() + "/";
            hpmax = c.MaxHP.ToString();
            
            te = g.TextExtents(hp);
            Text.ShadowedText(g, hp, X + x1 - te.Width, Y + y);
            te = g.TextExtents(hpmax);
            Text.ShadowedText(g, hpmax, X + x2 - te.Width, Y + y);
            
            mp = c.MP.ToString() + "/";
            mpmax = c.MaxMP.ToString();
            
            te = g.TextExtents(mp);
            Text.ShadowedText(g, mp, X + x3 - te.Width, Y + y);
            te = g.TextExtents(mpmax);
            Text.ShadowedText(g, mpmax, X + x4 - te.Width, Y + y);
            
            limit = "0%";
            
            te = g.TextExtents(limit);
            Text.ShadowedText(g, limit, X + x5 - te.Width + te.XBearing, Y + y);
            
            time = c.TurnTimer.PercentElapsed + "%";
            
            te = g.TextExtents(time);
            Text.ShadowedText(g, time, X + x6 - te.Width + te.XBearing, Y + y);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

