using System;
using System.Collections.Generic;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class List : ControlMenu
    {
        #region Layout
        
        const int x = 35;
        const int y = 30;
        const int cx = 20;
        const int cy = 22;
        const int rows = 10;
        
        #endregion 
        
        int _option = 0;
        int _topRow = 0;
        
        private List<InventoryRecord> _equipment;

        public List()
            : base(
                Config.Instance.WindowWidth * 5 / 8,
                Config.Instance.WindowHeight * 5 / 12,
                Config.Instance.WindowWidth * 3 / 8 - 8,
                Config.Instance.WindowHeight * 8 / 15)
        {
            _equipment = new List<InventoryRecord>();
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
                    if (_option < _equipment.Count - 1) _option++;
                    if (_topRow < _option - rows + 1) _topRow++;
                    break;
                case Key.Circle:
                    int slot = _equipment[_option].Slot;
                    EquipTop.Instance.Changed = true;
                    switch (EquipTop.Instance.Option)
                    {
                        case 0:
                            Weapon.SwapMateria(Seven.Party.Selected.Weapon, (Weapon)_equipment[_option].Item, Seven.Party.Selected);
                            Seven.Party.Selected.Weapon = (Weapon)Inventory.SwapOut(Seven.Party.Selected.Weapon, slot);
                            break;
                        case 1:
                            Armor.SwapMateria(Seven.Party.Selected.Armor, (Armor)_equipment[_option].Item, Seven.Party.Selected);
                            Inventory.AddToInventory(Seven.Party.Selected.Armor);
                            Inventory.DecreaseCount(slot);
                            Seven.Party.Selected.Armor = (Armor)_equipment[_option].Item;
                            break;
                        case 2:
                            if (Seven.Party.Selected.Accessory.Name != null)
                                Inventory.AddToInventory(Seven.Party.Selected.Accessory);
                            Inventory.DecreaseCount(slot);
                            Seven.Party.Selected.Accessory = (Accessory)_equipment[_option].Item;
                            break;
                    }
                    MenuScreen.EquipScreen.ChangeToDefaultControl();
                    break;
                case Key.X:
                    MenuScreen.EquipScreen.ChangeToDefaultControl();
                    break;
                default:
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.Color = new Color(1, 1, 1);
            g.SetFontSize(24);
            
            int j = Math.Min(rows + _topRow, _equipment.Count);
            
            for (int i = _topRow; i < j; i++)
            {
                Shapes.ShadowedText(g, _equipment [i].Item.Name, 
                                    X + x, Y + (i - _topRow + 1) * y);
            }
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + (_option - _topRow) * y);
            }
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            switch (EquipTop.Instance.Option)
            {
                case 0:
                    Update(ItemType.Weapon);
                    break;
                case 1:
                    Update(ItemType.Armor);
                    break;
                case 2:
                    Update(ItemType.Accessory);
                    break;
            }
            if (_equipment.Count == 0)
                MenuScreen.EquipScreen.ChangeToDefaultControl();
        }

        public override void SetNotControl()
        {
            base.SetNotControl();
            _equipment.Clear();
            _option = 0;
            _topRow = 0;
        }
        
        private void Update(ItemType it)
        {
            switch (it)
            {
                case ItemType.Weapon:
                    _equipment = Inventory.GetWeaponsOfType(Seven.Party.Selected.Weapon.Wielder);
                    break;
                case ItemType.Armor:
                    _equipment = Inventory.GetArmor(Seven.Party.Selected.Sex);
                    break;
                case ItemType.Accessory:
                    _equipment = Inventory.GetAccessories();
                    break;
                default: break;
            }
        }
        
        public IItem Selection { get { return _equipment[_option].Item; } }
        
        public override string Info
        { get { return Selection.Description; } }
    }
}

