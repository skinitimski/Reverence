using System;
using System.Collections.Generic;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

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
        
        public ItemRight()
            : base(
                Config.Instance.WindowWidth / 2,
                Config.Instance.WindowHeight * 13 / 60,
                Config.Instance.WindowWidth / 2 - 8,
                Config.Instance.WindowHeight * 3 / 4 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;

            Text.ShadowedText(g, "Item", X + x1, Y + (ys * 1));
            
            List<Inventory.Record> taken = Seven.PostBattleState.HoardItemLeft.Taken;
            
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
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}

