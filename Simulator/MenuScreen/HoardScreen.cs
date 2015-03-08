using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class HoardLabel : ControlMenu
    {
        public static HoardLabel Instance;

        #region Layout

        const int xs = 20;
        const int ys = 28;

        #endregion Layout

        static HoardLabel()
        {
            Instance = new HoardLabel();
        }
        private HoardLabel()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT / 12 - 6)
        { }
        public override void ControlHandle(Key k) { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, "Gained gil and item(s).", X + xs, Y + ys);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        public override string Info { get { return ""; } }
    }




    internal sealed class HoardGilLeft : Menu
    {
        public static HoardGilLeft Instance;

        #region Layout

        const int x1 = 50;
        const int x2 = 350;
        const int ys = 28;

        #endregion

        static HoardGilLeft()
        {
            Instance = new HoardGilLeft();
        }
        private HoardGilLeft()
            : base(
                2,
                Globals.HEIGHT  * 2 / 15,
                Globals.WIDTH / 2,
                Globals.HEIGHT / 12 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string gil = Game.PostBattle.Gil.ToString() + "g";
            te = g.TextExtents(gil);
            Graphics.ShadowedText(g, "Gained Gil", X + x1, Y + ys);
            Graphics.ShadowedText(g, gil, X + x2 - te.Width, Y + ys);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }




    internal sealed class HoardGilRight : Menu
    {
        public static HoardGilRight Instance;

        #region Layout

        const int x1 = 100;
        const int x2 = 250;
        const int ys = 28;
        #endregion

        static HoardGilRight()
        {
            Instance = new HoardGilRight();
        }
        private HoardGilRight()
            : base(
                Globals.WIDTH / 2,
                Globals.HEIGHT * 2 / 15,
                Globals.WIDTH / 2 - 8,
                Globals.HEIGHT / 12 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            string gil = Globals.Gil.ToString() + "g";
            te = g.TextExtents(gil);
            Graphics.ShadowedText(g, "Gil", X + x1, Y + ys);
            Graphics.ShadowedText(g, gil, X + x2 - te.Width, Y + ys);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }




    internal sealed class HoardItemLeft : ControlMenu
    {
        public static HoardItemLeft Instance;

        #region Layout

        const int x1 = 50;
        const int x2 = 100;
        const int x3 = 380;
        const int ys = 50;
        const int cx = -15;
        const int cy = -8;

        #endregion

        private int _option = 0;
        private List<InventoryRecord> _hoard;
        private List<InventoryRecord> _taken;
        bool _takeEverything = false;

        static HoardItemLeft()
        {
            Instance = new HoardItemLeft();
        }
        private HoardItemLeft()
            : base(
                2,
                Globals.HEIGHT * 13 / 60,
                Globals.WIDTH / 2,
                Globals.HEIGHT * 3 / 4 - 6)
        { 
        }
        public override void ControlHandle(Key k) 
        {
            switch (k)
            {
                case Key.Up:
                    if (!_takeEverything)
                    {
                        if (_option > 0)
                            _option--;
                        if (_option > 0 && _option < 6)
                            while (_option > _hoard.Count)
                                _option--;
                    }
                    break;
                case Key.Down:
                    if (!_takeEverything)
                    {
                        if (_option < 6)
                            _option++;
                        if (_option > _hoard.Count && _option < 6)
                            _option = 6;
                    }
                    break;
                case Key.Circle:
                    if (_option == 6)
                        Exit();
                    if (_option == 0)
                        TakeEverything();
                    else
                        TakeItem();
                    break;
                default: 
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            Graphics.ShadowedText(g, "Take everything", X + x1, Y + (ys * 1));

            for (int i = 0; i < _hoard.Count; i++)
                if (_hoard[i].ID != "")
                {
                    string count = _hoard[i].Count.ToString();
                    te = g.TextExtents(count);
                    Graphics.ShadowedText(g, _hoard[i].Item.Name, X + x2, Y + (ys * (i + 2)));
                    Graphics.ShadowedText(g, count, X + x3 - te.Width, Y + (ys * (i + 2)));
                }
            
            Graphics.ShadowedText(g, "Exit", X + x1, Y + (ys * 7));


            if (IsControl)
                Graphics.RenderCursor(g, X + cx + x1, Y + cy + ys * (_option + 1));
            

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        private void Exit()
        {
            foreach (InventoryRecord item in _taken)
                for (int i = 0; i < item.Count; i++)
                    Inventory.AddToInventory(item.Item);
            Game.GoToMenuState();
        }
        private void TakeEverything() 
        {
            for (int i = 0; i < _hoard.Count; i++)
            {
                InventoryRecord take = _hoard[i];
                _hoard[i] = new InventoryRecord();
                _taken.Add(take);
            }
            _takeEverything = true;
            _option = 6;
        }
        private void TakeItem()
        {
            if (_option > _hoard.Count)
                return;
            InventoryRecord take = _hoard[_option - 1];
            if (take.ID == "")
                return;
            _hoard[_option - 1] = new InventoryRecord();
            _taken.Add(take);
        }
        public override void SetAsControl()
        {
            base.SetAsControl();
            _option = 0;
            _hoard = Game.PostBattle.Items;
            _taken = new List<InventoryRecord>();
        }
        public override void SetNotControl()
        {
            base.SetNotControl();
            _hoard = null;
            _taken = null;
        }
        public override string Info { get { return ""; } }
        public List<InventoryRecord> Taken { get { return _taken; } }
    }




    internal sealed class HoardItemRight : Menu
    {
        public static HoardItemRight Instance;

        #region Layout

        const int x1 = 50;
        const int x2 = 100;
        const int x3 = 380;
        const int ys = 50;

        #endregion

        static HoardItemRight()
        {
            Instance = new HoardItemRight();
        }
        private HoardItemRight()
            : base(
                Globals.WIDTH / 2,
                Globals.HEIGHT * 13 / 60,
                Globals.WIDTH / 2 - 8,
                Globals.HEIGHT * 3 / 4 - 6)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            Graphics.ShadowedText(g, "Item", X + x1, Y + (ys * 1));

            List<InventoryRecord> taken = HoardItemLeft.Instance.Taken;

            for (int i = 0; i < taken.Count; i++)
            if (taken[i].ID != "")
            {
                string count = taken[i].Count.ToString();
                te = g.TextExtents(count);
                Graphics.ShadowedText(g, taken[i].Item.Name, X + x2, Y + (ys * (i + 2)));
                Graphics.ShadowedText(g, count, X + x3 - te.Width, Y + (ys * (i + 2)));
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }

    
}