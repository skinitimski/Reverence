using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thread = System.Threading.Thread;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Battle.Event;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class ItemMenu : ControlMenu, ISelectorUser
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





        public ItemMenu(SevenBattleState battleState, ScreenState screenState)
            : base(
                5,
                screenState.Height * 7 / 10 + 20,
                screenState.Width - 11,
                (screenState.Height * 5 / 20) - 25)
        {
            BattleState = battleState;
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
                    BattleState.Screen.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    IInventoryItem i = Selection;

                    if (i.CanUseInBattle)
                    {
                        Item item = (Item)i;

                        BattleState.Screen.ActivateSelector(item.BattleTarget, item.IntendedForEnemies);
                    }

                    break;
            }
        }


        public virtual bool ActOnSelection(IEnumerable<Combatant> targets)
        {
            int o = _option; // allocate to stack (option is on heap)

            UseItem(o, targets);

            return true;
        }

        protected void UseItem(int slot, IEnumerable<Combatant> targets, bool releaseAlly = true)
        {
            Item item = (Item)BattleState.Party.Inventory.GetItem(slot);
            Ally source = BattleState.Commanding;

            UseItemEvent e = new UseItemEvent(item, slot, source, targets, releaseAlly);
            
            BattleState.EnqueueAction(e);
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            int j = Math.Min(_rows + _topRow, Inventory.INVENTORY_SIZE);

            for (int i = _topRow; i < j; i++)
            {
                IInventoryItem item = BattleState.Party.Inventory.GetItem(i);

                if (item != null)
                {
                    int count = BattleState.Party.Inventory.GetCount(i);
                    te = g.TextExtents(count.ToString());

                    Color textColor = item.CanUseInBattle ? Colors.WHITE : Colors.GRAY_4;

                    Text.ShadowedText(g, textColor, item.Name,
                            X + x1, Y + (i - _topRow + 1) * y);
                    Text.ShadowedText(g, textColor, count.ToString(),
                            X + x2 - te.Width, Y + (i - _topRow + 1) * y);
                }
            }

            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + (_option - _topRow) * y);
            }
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
                IInventoryItem i = Selection;
                return i.Description;
            }
        }

        protected IInventoryItem Selection { get { return BattleState.Party.Inventory.GetItem(_option); } }
        
        private SevenBattleState BattleState { get; set; }
    }
}
