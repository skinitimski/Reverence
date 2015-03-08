using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Atmosphere.BattleSimulator
{
    public struct InventoryRecord : IComparable<InventoryRecord>
    {
        private int _slot;
        private int _count;
        private IItem _item;

        public InventoryRecord(int slot)
        {
            _slot = slot;
            _count = 0;
            _item = null;
        }

        public string ID { get { return _item == null ? "" : _item.ID; } }
        public int Slot { get { return _slot; } }
        public int Count { get { return _count; } set { _count = value; } }
        public IItem Item { get { return _item; } set { _item = value; } }

        public int CompareTo(InventoryRecord that)
        {
            if (this.Item == null)
            {
                if (that.Item == null)
                    return 0;
                return 1;
            }
            if (that.Item == null)
                return -1;

            int typeCompare = this.Item.Type.CompareTo(that.Item.Type);

            if (typeCompare != 0)
                return typeCompare;
            else
                return this.ID.CompareTo(that.ID);
        }
    }

    public static class Inventory
    {

        public const int INVENTORY_SIZE = 1000;

        private static InventoryRecord[] _inventory;

        public static void Init()
        {
            _inventory = new InventoryRecord[INVENTORY_SIZE];

            XmlDocument savegame = Globals.SaveGame;

            foreach (XmlNode node in savegame.SelectSingleNode("//inventory").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)));

                XmlNode itemNode = xml.DocumentElement;

                string type = itemNode.Name;
                string id = itemNode.Attributes["id"].Value;
                int count = Int32.Parse(itemNode.Attributes["count"].Value);
                int slot = Int32.Parse(itemNode.Attributes["slot"].Value);

                _inventory[slot] = new InventoryRecord(slot);
                _inventory[slot].Count = count;
                _inventory[slot].Item = Item.GetItem(id, type);
            }

            for (int i = 0; i < INVENTORY_SIZE; i++)
                if (_inventory[i].Item == null)
                    _inventory[i] = new InventoryRecord(i);
        }

        public static List<InventoryRecord> GetWeaponsOfType(WeaponType t)
        {
            List<InventoryRecord> list = new List<InventoryRecord>();
            foreach (InventoryRecord ir in _inventory)
                if (ir.Item != null)
                    if (ir.Item.Type == ItemType.Weapon)
                        if (((Weapon)ir.Item).Wielder == t)
                            list.Add(ir);
            return list;
        }
        public static List<InventoryRecord> GetArmor(Sex sex)
        {
            List<InventoryRecord> list = new List<InventoryRecord>();
            foreach (InventoryRecord ir in _inventory)
                if (ir.Item != null)
                    if (ir.Item.Type == ItemType.Armor)
                        if (ir.ID == "minervaband")
                        {
                            if (sex == Sex.Female)
                                list.Add(ir);
                        }
                        else if (ir.ID == "escortguard")
                        {
                            if (sex == Sex.Male)
                                list.Add(ir);
                        }
                        else
                            list.Add(ir);
            return list;
        }
        public static List<InventoryRecord> GetAccessories()
        {
            List<InventoryRecord> list = new List<InventoryRecord>();
            foreach (InventoryRecord ir in _inventory)
                if (ir.Item != null)
                    if (ir.Item.Type == ItemType.Accessory)
                        list.Add(ir);
            return list;
        }

        public static IItem SwapOut(IItem newItem, int slot)
        {
            if (_inventory[slot].Count != 1)
                throw new GameImplementationException("Can't swap out an item unless it has a count of 1.");

            IItem temp = _inventory[slot].Item;

            _inventory[slot].Item = newItem;

            return temp;
        }
        public static void IncreaseCount(int slot)
        {
            _inventory[slot].Count++;
        }
        public static void DecreaseCount(int slot)
        {
            _inventory[slot].Count--;
            if (_inventory[slot].Count == 0)
            {
                _inventory[slot].Item = null;
            }
        }
        public static void AddToInventory(IItem item)
        {
            int i = IndexOf(item);
            if (i >= 0)
                IncreaseCount(i);
            else
            {
                int j = 0;
                while (_inventory[j].Item != null)
                    j++;
                _inventory[j].Item = item;
                _inventory[j].Count = 1;
            }
        }

        public static int IndexOf(IItem item)
        {
            foreach (InventoryRecord ir in _inventory)
                if (ir.ID == item.ID)
                    return ir.Slot;
            return -1;
        }
        public static IItem GetItem(int slot)
        {
            return _inventory[slot].Item;
        }
        public static int GetCount(int slot)
        {
            return _inventory[slot].Count;
        }
        public static void Sort()
        {
            Array.Sort<InventoryRecord>(_inventory);
        }
        public static bool UseItem(int slot)
        {
            if (((Item)_inventory[slot].Item).Use())
            {
                DecreaseCount(slot);
                return true;
            }
            return false;
        }
    }
}
