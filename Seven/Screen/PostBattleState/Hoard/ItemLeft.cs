using System;
using System.Collections.Generic;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Hoard
{  
    internal sealed class ItemLeft : ControlMenu
    {
        #region Layout
        
        const int x1 = 50;
        const int x2 = 100;
        const int x3 = 380;
        const int ys = 50;
        const int cx = -15;
        const int cy = -8;
        
#endregion
        
        private int _option = 0;
        private List<Inventory.Record> _hoard;
        private List<Inventory.Record> _taken;
        bool _takeEverything = false;
        
        public ItemLeft()
            : base(
                2,
                Seven.Config.WindowHeight * 13 / 60,
                Seven.Config.WindowWidth / 2,
                Seven.Config.WindowHeight * 3 / 4 - 6)
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
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            Text.ShadowedText(g, "Take everything", X + x1, Y + (ys * 1));
            
            for (int i = 0; i < _hoard.Count; i++)
            {
                if (_hoard[i].ID != "")
                {
                    string count = _hoard[i].Count.ToString();
                    te = g.TextExtents(count);
                    Text.ShadowedText(g, _hoard[i].Item.Name, X + x2, Y + (ys * (i + 2)));
                    Text.ShadowedText(g, count, X + x3 - te.Width, Y + (ys * (i + 2)));
                }
            }
            
            Text.ShadowedText(g, "Exit", X + x1, Y + (ys * 7));
            
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx + x1, Y + cy + ys * (_option + 1));
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private void Exit()
        {
            foreach (Inventory.Record item in _taken)
            {
                for (int i = 0; i < item.Count; i++)
                {
                    Seven.Party.Inventory.AddToInventory(item.Item);
                }
            }

            Seven.Instance.EndPostBattle();
        }

        private void TakeEverything() 
        {
            for (int i = 0; i < _hoard.Count; i++)
            {
                Inventory.Record take = _hoard[i];
                _hoard[i] = new Inventory.Record(0);
                _taken.Add(take);
            }
            _takeEverything = true;
            _option = 6;
        }

        private void TakeItem()
        {
            if (_option > _hoard.Count)
                return;
            Inventory.Record take = _hoard[_option - 1];
            if (take.ID == "")
                return;
            _hoard[_option - 1] = new Inventory.Record(0);
            _taken.Add(take);
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            _option = 0;
            _hoard = Seven.PostBattleState.Items;
            _taken = new List<Inventory.Record>();
        }

        public override void SetNotControl()
        {
            base.SetNotControl();
            _hoard = null;
            _taken = null;
        }

        public override string Info { get { return ""; } }
        public List<Inventory.Record> Taken { get { return _taken; } }
    }
}

