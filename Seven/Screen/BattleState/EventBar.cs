using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class EventBar : GameMenu
    {
        public EventBar()
            : base(
                Config.Instance.WindowWidth / 16,
                20,
                Config.Instance.WindowWidth * 7 / 8,
                Config.Instance.WindowHeight / 10)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}
