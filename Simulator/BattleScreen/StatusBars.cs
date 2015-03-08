using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class StatusBarLeft : Menu
    {
        public static StatusBarLeft Instance;

        #region Layout

        const int x1 = 30;
        const int y0 = 35;
        const int y1 = 80;
        const int y2 = 125;

        #endregion Layout

        static StatusBarLeft()
        {
            Instance = new StatusBarLeft();
        }
        private StatusBarLeft()
            : base(
                5,
                Globals.HEIGHT * 7 / 10,
                Globals.WIDTH * 2 / 5,
                (Globals.HEIGHT * 5 / 20) - 5)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(28);


            string name0 = (Game.Battle.Allies[0] == null) ? "" : Game.Battle.Allies[0].Name;
            string name1 = (Game.Battle.Allies[1] == null) ? "" : Game.Battle.Allies[1].Name;
            string name2 = (Game.Battle.Allies[2] == null) ? "" : Game.Battle.Allies[2].Name;

            if (name0.CompareTo("") != 0) Graphics.ShadowedText(g, name0, X + x1, Y + y0);
            if (name1.CompareTo("") != 0) Graphics.ShadowedText(g, name1, X + x1, Y + y1);
            if (name2.CompareTo("") != 0) Graphics.ShadowedText(g, name2, X + x1, Y + y2);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

    internal sealed class StatusBarRight : Menu
    {
        public static StatusBarRight Instance;

        const int x1 = 90; // hp
        const int x2 = 150; // hpm
        const int x3 = 245; // mp
        const int x4 = 290; // mpm
        const int x5 = 365; // limit
        const int x6 = 440; // time
        const int y0 = 30;
        const int y1 = 75;
        const int y2 = 120;

        static StatusBarRight()
        {
            Instance = new StatusBarRight();
        }
        private StatusBarRight()
            : base(
                Globals.WIDTH * 2 / 5 + 12,
                Globals.HEIGHT * 7 / 10,
                Globals.WIDTH * 3 / 5 - 17,
                Globals.HEIGHT * 5 / 20 - 5)
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

            hp = Game.Battle.Allies[0].HP.ToString() + "/";
            hpmax = Game.Battle.Allies[0].MaxHP.ToString();

            te = g.TextExtents(hp);
            Graphics.ShadowedText(g, hp, X + x1 - te.Width, Y + y0);
            te = g.TextExtents(hpmax);
            Graphics.ShadowedText(g, hpmax, X + x2 - te.Width, Y + y0);

            mp = Game.Battle.Allies[0].MP.ToString() + "/";
            mpmax = Game.Battle.Allies[0].MaxMP.ToString();

            te = g.TextExtents(mp);
            Graphics.ShadowedText(g, mp, X + x3 - te.Width, Y + y0);
            te = g.TextExtents(mpmax);
            Graphics.ShadowedText(g, mpmax, X + x4 - te.Width, Y + y0);

            limit = "0%";

            te = g.TextExtents(limit);
            Graphics.ShadowedText(g, limit,
                X + x5 - te.Width + te.XBearing,
                Y + y0);

            e = Game.Battle.Allies[0].TurnTimer.Elapsed;
            t = Game.Battle.Allies[0].TurnTimer.Timeout;

            if (e > t) time = "100%";
            else time = ((e * 100 / t)).ToString() + "%";

            te = g.TextExtents(time);
            Graphics.ShadowedText(g, time,
                X + x6 - te.Width + te.XBearing,
                Y + y0);

            #endregion Ally 1


            #region Ally 2
            if (Game.Battle.Allies[1] != null)
            {
                hp = Game.Battle.Allies[1].HP.ToString() + "/";
                hpmax = Game.Battle.Allies[1].MaxHP.ToString();

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x1 - te.Width, Y + y1);
                te = g.TextExtents(hpmax);
                Graphics.ShadowedText(g, hpmax, X + x2 - te.Width, Y + y1);

                mp = Game.Battle.Allies[1].MP.ToString() + "/";
                mpmax = Game.Battle.Allies[1].MaxMP.ToString();

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x3 - te.Width, Y + y1);
                te = g.TextExtents(mpmax);
                Graphics.ShadowedText(g, mpmax, X + x4 - te.Width, Y + y1);

                limit = "0%";

                te = g.TextExtents(limit);
                Graphics.ShadowedText(g, limit,
                    X + x5 - te.Width + te.XBearing,
                    Y + y1);

                e = Game.Battle.Allies[1].TurnTimer.Elapsed;
                t = Game.Battle.Allies[1].TurnTimer.Timeout;

                if (e > t) time = "100%";
                else time = ((e * 100 / t)).ToString() + "%";

                te = g.TextExtents(time);
                Graphics.ShadowedText(g, time,
                    X + x6 - te.Width + te.XBearing,
                    Y + y1);
            }
            #endregion Ally 2


            #region Ally 3
            if (Game.Battle.Allies[2] != null)
            {
                hp = Game.Battle.Allies[2].HP.ToString() + "/";
                hpmax = Game.Battle.Allies[2].MaxHP.ToString();

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x1 - te.Width, Y + y2);
                te = g.TextExtents(hpmax);
                Graphics.ShadowedText(g, hpmax, X + x2 - te.Width, Y + y2);

                mp = Game.Battle.Allies[2].MP.ToString() + "/";
                mpmax = Game.Battle.Allies[2].MaxMP.ToString();

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x3 - te.Width, Y + y2);
                te = g.TextExtents(mpmax);
                Graphics.ShadowedText(g, mpmax, X + x4 - te.Width, Y + y2);

                limit = "0%";

                te = g.TextExtents(limit);
                Graphics.ShadowedText(g, limit, X + x5 - te.Width + te.XBearing, Y + y2);

                e = Game.Battle.Allies[2].TurnTimer.Elapsed;
                t = Game.Battle.Allies[2].TurnTimer.Timeout;

                if (e > t) time = "100%";
                else time = ((e * 100 / t)).ToString() + "%";

                te = g.TextExtents(time);
                Graphics.ShadowedText(g, time,
                    X + x6 - te.Width + te.XBearing,
                    Y + y2);
            }
            #endregion Ally3


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}
