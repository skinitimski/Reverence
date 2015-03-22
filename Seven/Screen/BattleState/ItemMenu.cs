using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class ItemMenu : ControlMenu, SelectorUser
    {
        #region Layout

        protected const int x1 = 200;
        protected const int x2 = 500;
        protected const int y = 35;
        protected const int cx = 15;
        protected const int cy = 22;

        #endregion Layout

        protected int _option;
        protected int _topRow;
        protected readonly int _rows = 3;

        public ItemMenu()
            : base(
                5,
                Config.Instance.WindowHeight * 7 / 10 + 20,
                Config.Instance.WindowWidth - 11,
                (Config.Instance.WindowHeight * 5 / 20) - 25)
        {
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (_option > 0) _option--;
                    if (_topRow > _option) _topRow--;
                    break;
                case Key.Down:
                    if (_option < Inventory.INVENTORY_SIZE - 1) _option++;
                    if (_topRow < _option - _rows + 1) _topRow++;
                    break;
                case Key.X:
                    Visible = false;
                    Seven.BattleState.Screen.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    IInventoryItem i = Seven.Party.Inventory.GetItem(_option);
                    if (i != null && i.CanUseInBattle)
                    {
                        Seven.BattleState.Screen.GetSelection(TargetGroup.Area, ((Item)i).BattleTarget);
                    }
                    break;
            }
        }
        public void ActOnSelection()
        {
            int o = _option; // allocate to stack (option is on heap)

            Seven.Party.Inventory.UseItemInBattle(o);
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            int j = Math.Min(_rows + _topRow, Inventory.INVENTORY_SIZE);

            for (int i = _topRow; i < j; i++)
            {
                IInventoryItem item = Seven.Party.Inventory.GetItem(i);

                if (item != null)
                {
                    int count = Seven.Party.Inventory.GetCount(i);
                    te = g.TextExtents(count.ToString());

                    Color textColor = item.CanUseInBattle ? Colors.WHITE : Colors.GRAY_4;

                    Text.ShadowedText(g, textColor, item.Name,
                            X + x1, Y + (i - _topRow + 1) * y);
                    Text.ShadowedText(g, textColor, count.ToString(),
                            X + x2 - te.Width, Y + (i - _topRow + 1) * y);
                }
            }

            if (IsControl)
                Shapes.RenderCursor(g, X + cx, Y + cy + (_option - _topRow) * y);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void Reset()
        {
            _option = 0;
            _topRow = 0;
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
        }

        public override string Info
        {
            get
            {
                IInventoryItem i = Seven.Party.Inventory.GetItem(_option);
                return i.Description;
            }
        }
    }
}
