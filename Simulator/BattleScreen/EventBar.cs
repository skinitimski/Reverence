using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class EventBar : Menu
    {
        public static EventBar Instance;

        static EventBar()
        {
            Instance = new EventBar();
        }
        private EventBar()
            : base(
                Globals.WIDTH / 16,
                20,
                Globals.WIDTH * 7 / 8,
                Globals.HEIGHT / 10)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}
