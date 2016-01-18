using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.EnemySkill
{
    internal sealed class Info : GameMenu
    {
        #region Layout

        const int x0 = 15; // ability col
        const int x1 = 40; // mpneeded col
        const int x2 = 90; // slash col
        const int x3 = 130; // mptot col
        const int y0 = 27; // 
        const int ys = 27;

        #endregion Layout

        public Info(Menu.ScreenState screenState)
            : base(
                screenState.Width * 3 / 4 + 12,
                screenState.Height * 7 / 10 + 20,
                screenState.Width / 4 - 17,
                (screenState.Height * 5 / 20) - 25)
        {
            Visible = false;
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            Ability s = Seven.BattleState.Commanding.EnemySkillMenu.Selected;

            int row = 0;

            if (!String.IsNullOrEmpty(s.Name))
            {
                string cost = s.MPCost.ToString();
                Text.ShadowedText(g, "MP Req", X + x1, Y + y0);

                row++;

                cost = cost + "/";
                te = g.TextExtents(cost);
                Text.ShadowedText(g, cost, X + x2 - te.Width, Y + y0 + (row * ys));

                string tot = Seven.BattleState.Commanding.MP.ToString();
                te = g.TextExtents(tot);
                Text.ShadowedText(g, tot, X + x3 - te.Width, Y + y0 + (row * ys));

                row++;
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }


}
