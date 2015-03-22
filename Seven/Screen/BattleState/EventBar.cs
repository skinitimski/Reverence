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
            BattleEvent e = Seven.BattleState.ActiveAbility;
            
            if (e != null)
            {
                Cairo.Context g = Gdk.CairoHelper.Create(d);

                g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
                g.SetFontSize(24);
                                
                string msg = e.GetStatus();
                
                TextExtents te = g.TextExtents(msg);
                
                Text.ShadowedText(g, msg, X + (W / 2) - (te.Width / 2), Y + 32);
                
                ((IDisposable)g.Target).Dispose();
                ((IDisposable)g).Dispose();
            }
        }
    }
}
