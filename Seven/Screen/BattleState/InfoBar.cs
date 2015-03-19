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
        public InfoBar()
            : base(
                5,
                Config.Instance.WindowHeight * 6 / 10,
                Config.Instance.WindowWidth - 10,
                (Config.Instance.WindowHeight / 10) - 7,
                false)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string msg = "";

            if (BattleScreen.Instance.Control != null)
            {
                msg = BattleScreen.Instance.Control.Info;
            }

            te = g.TextExtents(msg);

            Text.ShadowedText(g, msg, X + (W / 2) - (te.Width / 2), Y + 32);



            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}
