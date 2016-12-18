using System;
using System.Collections.Generic;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenPostBattleState = Atmosphere.Reverence.Seven.State.PostBattleState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Hoard
{  
    internal sealed class ItemRight : GameMenu
    {
        #region Layout
        
        const int x1 = 50;
        const int x2 = 100;
        const int x3 = 380;
        const int ys = 50;
        
#endregion
        
        public ItemRight(SevenPostBattleState postBattleState, ScreenState screenState)
            : base(
                screenState.Width / 2,
                screenState.Height * 13 / 60,
                screenState.Width / 2 - 8,
                screenState.Height * 3 / 4 - 6)
        { 
            HoardItemLeft = postBattleState.HoardItemLeft;
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            Text.ShadowedText(g, "Item", X + x1, Y + (ys * 1));
            
            List<Inventory.Record> taken = HoardItemLeft.Taken;
            
            for (int i = 0; i < taken.Count; i++)
            {
                if (taken[i].ID != "")
                {
                    string count = taken[i].Count.ToString();
                    te = g.TextExtents(count);
                    Text.ShadowedText(g, taken[i].Item.Name, X + x2, Y + (ys * (i + 2)));
                    Text.ShadowedText(g, count, X + x3 - te.Width, Y + (ys * (i + 2)));
                }
            }
        }

        private ItemLeft HoardItemLeft { get; set; }
    }
}

