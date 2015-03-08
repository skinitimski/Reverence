using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class ItemMenu : ControlMenu, ISelectorUser
    {
        public static ItemMenu Instance;

        #region Layout

        const int x1 = 200;
        const int x2 = 500;
        const int y = 35;
        const int cx = 15;
        const int cy = 22;

        #endregion Layout

        private int _option;
        private int _topRow;
        private readonly int _rows = 3;

        static ItemMenu()
        {
            Instance = new ItemMenu();
        }
        private ItemMenu()
            : base(
                5,
                Globals.HEIGHT * 7 / 10 + 20,
                Globals.WIDTH - 11,
                (Globals.HEIGHT * 5 / 20) - 25)
        {
            Visible = false;
            _option = 0;
            _topRow = 0;
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
                    BattleScreen.Instance.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    IItem i = Inventory.GetItem(_option);
                    if (i != null)
                        if (i.Type == ItemType.Battle || i.Type == ItemType.Hybrid)
                            BattleScreen.Instance.GetSelection(TargetGroup.Area, ((Item)i).TargetType);
                    break;
            }
        }
        public void ActOnSelection()
        {
            int o = _option; // allocate to stack (option is on heap)

            Item i = (Item)Inventory.GetItem(_option);

            AbilityState state = Game.Battle.Commanding.Ability;
            switch (i.TargetType)
            {
                case TargetType.AllTar:
                case TargetType.AllTarNS:
                case TargetType.NTar:
                    state.Target = GroupSelector.Instance.Selected;
                    break;
                case TargetType.Field:
                    state.Target = FieldSelector.Instance.Selected;
                    break;
                case TargetType.OneTar:
                    state.Target = TargetSelector.Instance.Selected;
                    break;
                default: break;
            }

            state.Performer = Game.Battle.Commanding;

            state.Action += delegate() { Inventory.UseItem(o); };
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            int j = Math.Min(_rows + _topRow, Inventory.INVENTORY_SIZE);

            Color gray = new Color(.4, .4, .4);

            for (int i = _topRow; i < j; i++)
            {
                IItem item = Inventory.GetItem(i);
                if (item != null)
                {
                    int count = Inventory.GetCount(i);
                    te = g.TextExtents(count.ToString());
                    if (item.Type == ItemType.Battle || item.Type == ItemType.Hybrid)
                    {
                        Graphics.ShadowedText(g, item.Name,
                            X + x1, Y + (i - _topRow + 1) * y);
                        Graphics.ShadowedText(g, count.ToString(),
                            X + x2 - te.Width, Y + (i - _topRow + 1) * y);
                    }
                    else
                    {
                        Graphics.ShadowedText(g, gray, item.Name,
                            X + x1, Y + (i - _topRow + 1) * y);
                        Graphics.ShadowedText(g, gray, count.ToString(),
                            X + x2 - te.Width, Y + (i - _topRow + 1) * y);
                    }
                }
            }

            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy + (_option - _topRow) * y);


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
                IItem i = Inventory.GetItem(_option);
                return (i == null) ? "" : i.Description;
            }
        }
    }
    internal sealed class WItemMenu : ControlMenu, ISelectorUser
    {
        public static WItemMenu Instance;

        #region Layout

        const int x1 = 200;
        const int x2 = 500;
        const int y = 35;
        const int cx = 15;
        const int cy = 22;

        #endregion Layout

        private int _option;
        private int _topRow;
        private readonly int _rows = 3;
        private int _first = -1;

        static WItemMenu()
        {
            Instance = new WItemMenu();
        }
        private WItemMenu()
            : base(
                5,
                Globals.HEIGHT * 7 / 10 + 20,
                Globals.WIDTH - 11,
                (Globals.HEIGHT * 5 / 20) - 25)
        {
            Visible = false;
            _option = 0;
            _topRow = 0;
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
                    BattleScreen.Instance.PopControl();
                    Reset();
                    break;
                case Key.Circle:
                    IItem i = Inventory.GetItem(_option);
                    if (i != null)
                        if (i.Type == ItemType.Battle || i.Type == ItemType.Hybrid)
                        {
                            if (_first == -1)
                                Selector.DisableActionHook();
                            BattleScreen.Instance.GetSelection(TargetGroup.Area, ((Item)i).TargetType);
                        }
                    break;
            }
        }
        public void ActOnSelection()
        {
            if (_first != -1)
            {
                // allocate to stack (option is on heap)
                int first = _first;
                int second = _option;

                #region First
                Item i = (Item)Inventory.GetItem(first);

                AbilityState state = (AbilityState)Game.Battle.Commanding.Ability.Clone();
                switch (i.TargetType)
                {
                    case TargetType.AllTar:
                    case TargetType.AllTarNS:
                    case TargetType.NTar:
                        state.Target = GroupSelector.Instance.Selected;
                        break;
                    case TargetType.Field:
                        state.Target = FieldSelector.Instance.Selected;
                        break;
                    case TargetType.OneTar:
                        state.Target = TargetSelector.Instance.Selected;
                        break;
                    default: break;
                }

                state.Performer = Game.Battle.Commanding;

                state.Action += delegate() { Inventory.UseItem(first); };

                Game.Battle.EnqueueAction(state);

                #endregion First

                #region Second

                i = (Item)Inventory.GetItem(second);

                state = Game.Battle.Commanding.Ability;
                switch (i.TargetType)
                {
                    case TargetType.AllTar:
                    case TargetType.AllTarNS:
                    case TargetType.NTar:
                        state.Target = GroupSelector.Instance.Selected;
                        break;
                    case TargetType.Field:
                        state.Target = FieldSelector.Instance.Selected;
                        break;
                    case TargetType.OneTar:
                        state.Target = TargetSelector.Instance.Selected;
                        break;
                    default: break;
                }

                state.Performer = Game.Battle.Commanding;

                state.Action += delegate() { Inventory.UseItem(second); };
                
                // don't enqueue, will be picked up by Battle.ActionHook()

                #endregion Second
            }
            else
            {
                _first = _option;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;


            int j = Math.Min(_rows + _topRow, Inventory.INVENTORY_SIZE);

            Color gray = new Color(.4, .4, .4);

            for (int i = _topRow; i < j; i++)
            {
                IItem item = Inventory.GetItem(i);
                if (item != null)
                {
                    int count = Inventory.GetCount(i);
                    te = g.TextExtents(count.ToString());
                    if (item.Type == ItemType.Battle || item.Type == ItemType.Hybrid)
                    {
                        Graphics.ShadowedText(g, item.Name,
                            X + x1, Y + (i - _topRow + 1) * y);
                        Graphics.ShadowedText(g, count.ToString(),
                            X + x2 - te.Width, Y + (i - _topRow + 1) * y);
                    }
                    else
                    {
                        Graphics.ShadowedText(g, gray, item.Name,
                            X + x1, Y + (i - _topRow + 1) * y);
                        Graphics.ShadowedText(g, gray, count.ToString(),
                            X + x2 - te.Width, Y + (i - _topRow + 1) * y);
                    }
                }
            }

            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy + (_option - _topRow) * y);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void Reset()
        {
            _option = 0;
            _topRow = 0;
            _first = -1;
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
                IItem i = Inventory.GetItem(_option);
                return (i == null) ? "" : i.Description;
            }
        }
    }
}
