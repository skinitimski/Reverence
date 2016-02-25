using System;
using System.Collections.Generic;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

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
        
        private List<Inventory.Record> _equipment;

        public List(SevenMenuState menuState, ScreenState screenState)
            : base(
                screenState.Width * 5 / 8,
                screenState.Height * 5 / 12,
                screenState.Width * 3 / 8 - 8,
                screenState.Height * 8 / 15)
        {
            _equipment = new List<Inventory.Record>();

            MenuState = menuState;
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
                    MenuState.EquipTop.Changed = true;
                    switch (MenuState.EquipTop.Option)
                    {
                        case 0:
                            MenuState.Party.Materiatory.Swap(MenuState.Party.Selected.Weapon, (Weapon)_equipment[_option].Item, MenuState.Party.Selected);
                            MenuState.Party.Selected.Weapon = (Weapon)MenuState.Party.Inventory.SwapOut(MenuState.Party.Selected.Weapon, slot);
                            break;
                        case 1:
                            MenuState.Party.Materiatory.Swap(MenuState.Party.Selected.Armor, (Armor)_equipment[_option].Item, MenuState.Party.Selected);
                            MenuState.Party.Inventory.AddToInventory(MenuState.Party.Selected.Armor);
                            MenuState.Party.Selected.Armor = (Armor)_equipment[_option].Item;
                            MenuState.Party.Inventory.DecreaseCount(slot);
                            break;
                        case 2:
                            MenuState.Party.Inventory.AddToInventory(MenuState.Party.Selected.Accessory);
                            MenuState.Party.Selected.Accessory = (Accessory)_equipment[_option].Item;
                            MenuState.Party.Inventory.DecreaseCount(slot);
                            break;
                    }
                    MenuState.EquipScreen.ChangeToDefaultControl();
                    break;
                case Key.X:
                    MenuState.EquipScreen.ChangeToDefaultControl();
                    break;
                default:
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.Color = Colors.WHITE;
            g.SetFontSize(24);
            
            int j = Math.Min(rows + _topRow, _equipment.Count);
            
            for (int i = _topRow; i < j; i++)
            {
                Text.ShadowedText(g, _equipment [i].Item.Name, 
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
            switch (MenuState.EquipTop.Option)
            {
                case 0:
                    Update(InventoryItemType.weapon);
                    break;
                case 1:
                    Update(InventoryItemType.armor);
                    break;
                case 2:
                    Update(InventoryItemType.accessory);
                    break;
            }
            if (_equipment.Count == 0)
            {
                MenuState.EquipScreen.ChangeToDefaultControl();
            }
        }

        public override void SetNotControl()
        {
            base.SetNotControl();
            _equipment.Clear();
            _option = 0;
            _topRow = 0;
        }
        
        private void Update(InventoryItemType it)
        {
            switch (it)
            {
                case InventoryItemType.weapon:
                    _equipment = MenuState.Party.Inventory.GetWeaponsOfType(MenuState.Party.Selected.Weapon.Wielder);
                    break;
                case InventoryItemType.armor:
                    _equipment = MenuState.Party.Inventory.GetArmor(MenuState.Party.Selected.Sex);
                    break;
                case InventoryItemType.accessory:
                    _equipment = MenuState.Party.Inventory.GetAccessories();
                    break;
                default: break;
            }
        }
        
        public IInventoryItem Selection { get { return _equipment[_option].Item; } }
        
        public override string Info { get { return Selection.Description; } }
        
        private SevenMenuState MenuState { get; set; }
    }
}

