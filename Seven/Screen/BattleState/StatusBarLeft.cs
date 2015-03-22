using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

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

        public StatusBarLeft()
            : base(
                5,
                Config.Instance.WindowHeight * 7 / 10,
                Config.Instance.WindowWidth * 2 / 5,
                (Config.Instance.WindowHeight * 5 / 20) - 5)
        { }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(28);
            
            
            string name0 = (Seven.BattleState.Allies[0] == null) ? "" : Seven.BattleState.Allies[0].Name;
            string name1 = (Seven.BattleState.Allies[1] == null) ? "" : Seven.BattleState.Allies[1].Name;
            string name2 = (Seven.BattleState.Allies[2] == null) ? "" : Seven.BattleState.Allies[2].Name;
            
            if (name0.CompareTo("") != 0) Text.ShadowedText(g, name0, X + x1, Y + y0);
            if (name1.CompareTo("") != 0) Text.ShadowedText(g, name1, X + x1, Y + y1);
            if (name2.CompareTo("") != 0) Text.ShadowedText(g, name2, X + x1, Y + y2);
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

