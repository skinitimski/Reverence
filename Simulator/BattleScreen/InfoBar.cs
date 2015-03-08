using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class InfoBar : Menu
    {
        public static InfoBar Instance;

        static InfoBar()
        {
            Instance = new InfoBar();
        }
        private InfoBar()
            : base(
                5,
                Globals.HEIGHT * 6 / 10,
                Globals.WIDTH - 10,
                (Globals.HEIGHT / 10) - 7,
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
                msg = BattleScreen.Instance.Control.Info;
            te = g.TextExtents(msg);

            Graphics.ShadowedText(g, msg, X + (W / 2) - (te.Width / 2), Y + 32);



            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}
