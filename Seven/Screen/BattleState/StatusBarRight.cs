using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
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
        { }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            string hp, hpmax, mp, mpmax, limit, time;
            long e, t;
            
            TextExtents te;
            
            
            #region Ally 1
            
            hp = Seven.BattleState.Allies[0].HP.ToString() + "/";
            hpmax = Seven.BattleState.Allies[0].MaxHP.ToString();
            
            te = g.TextExtents(hp);
            Text.ShadowedText(g, hp, X + x1 - te.Width, Y + y0);
            te = g.TextExtents(hpmax);
            Text.ShadowedText(g, hpmax, X + x2 - te.Width, Y + y0);
            
            mp = Seven.BattleState.Allies[0].MP.ToString() + "/";
            mpmax = Seven.BattleState.Allies[0].MaxMP.ToString();
            
            te = g.TextExtents(mp);
            Text.ShadowedText(g, mp, X + x3 - te.Width, Y + y0);
            te = g.TextExtents(mpmax);
            Text.ShadowedText(g, mpmax, X + x4 - te.Width, Y + y0);
            
            limit = "0%";
            
            te = g.TextExtents(limit);
            Text.ShadowedText(g, limit,
                              X + x5 - te.Width + te.XBearing,
                              Y + y0);
            
            e = Seven.BattleState.Allies[0].TurnTimer.TotalMilliseconds;
            t = Seven.BattleState.Allies[0].TurnTimer.Timeout;
            
            if (e > t) time = "100%";
            else time = ((e * 100 / t)).ToString() + "%";
            
            te = g.TextExtents(time);
            Text.ShadowedText(g, time,
                              X + x6 - te.Width + te.XBearing,
                              Y + y0);
            
            #endregion Ally 1
            
            
            #region Ally 2
            if (Seven.BattleState.Allies[1] != null)
            {
                hp = Seven.BattleState.Allies[1].HP.ToString() + "/";
                hpmax = Seven.BattleState.Allies[1].MaxHP.ToString();
                
                te = g.TextExtents(hp);
                Text.ShadowedText(g, hp, X + x1 - te.Width, Y + y1);
                te = g.TextExtents(hpmax);
                Text.ShadowedText(g, hpmax, X + x2 - te.Width, Y + y1);
                
                mp = Seven.BattleState.Allies[1].MP.ToString() + "/";
                mpmax = Seven.BattleState.Allies[1].MaxMP.ToString();
                
                te = g.TextExtents(mp);
                Text.ShadowedText(g, mp, X + x3 - te.Width, Y + y1);
                te = g.TextExtents(mpmax);
                Text.ShadowedText(g, mpmax, X + x4 - te.Width, Y + y1);
                
                limit = "0%";
                
                te = g.TextExtents(limit);
                Text.ShadowedText(g, limit,
                                  X + x5 - te.Width + te.XBearing,
                                  Y + y1);
                
                e = Seven.BattleState.Allies[1].TurnTimer.TotalMilliseconds;
                t = Seven.BattleState.Allies[1].TurnTimer.Timeout;
                
                if (e > t) time = "100%";
                else time = ((e * 100 / t)).ToString() + "%";
                
                te = g.TextExtents(time);
                Text.ShadowedText(g, time,
                                  X + x6 - te.Width + te.XBearing,
                                  Y + y1);
            }
            #endregion Ally 2
            
            
            #region Ally 3
            if (Seven.BattleState.Allies[2] != null)
            {
                hp = Seven.BattleState.Allies[2].HP.ToString() + "/";
                hpmax = Seven.BattleState.Allies[2].MaxHP.ToString();
                
                te = g.TextExtents(hp);
                Text.ShadowedText(g, hp, X + x1 - te.Width, Y + y2);
                te = g.TextExtents(hpmax);
                Text.ShadowedText(g, hpmax, X + x2 - te.Width, Y + y2);
                
                mp = Seven.BattleState.Allies[2].MP.ToString() + "/";
                mpmax = Seven.BattleState.Allies[2].MaxMP.ToString();
                
                te = g.TextExtents(mp);
                Text.ShadowedText(g, mp, X + x3 - te.Width, Y + y2);
                te = g.TextExtents(mpmax);
                Text.ShadowedText(g, mpmax, X + x4 - te.Width, Y + y2);
                
                limit = "0%";
                
                te = g.TextExtents(limit);
                Text.ShadowedText(g, limit, X + x5 - te.Width + te.XBearing, Y + y2);
                
                e = Seven.BattleState.Allies[2].TurnTimer.TotalMilliseconds;
                t = Seven.BattleState.Allies[2].TurnTimer.Timeout;
                
                if (e > t) time = "100%";
                else time = ((e * 100 / t)).ToString() + "%";
                
                te = g.TextExtents(time);
                Text.ShadowedText(g, time,
                                  X + x6 - te.Width + te.XBearing,
                                  Y + y2);
            }
            #endregion Ally3
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

