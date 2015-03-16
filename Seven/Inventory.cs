using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset;

using GameItem = Atmosphere.Reverence.Seven.Asset.Item;

namespace Atmosphere.Reverence.Seven
{
    internal class Inventory
    {        
        public const int INVENTORY_SIZE = 1000;
        private Record[] _inventory;




        #region Nested
        
        public class Record : IComparable<Record>
        {
            private IItem _item = GameItem.EMPTY;
            private int _count;

            public Record(int slot)
            {
                Slot = slot;
            }
            
            public string ID { get { return Item.ID; } }

            public int Slot { get; set; }

            public bool IsEmpty { get { return Item.Equals(GameItem.EMPTY); } }

            public int Count
            {
                get { return _count; }
                set
                {
                    _count = value;
                    
                    if (Count == 0)
                    {
                        Item = GameItem.EMPTY;
                    }
                }
            }

            public IItem Item
            {
                get { return _item; }
                set
                {
                    if (value == null)
                    {
                        throw new ImplementationException("We're not using null items anymore.");
                    }

                    _item = value;
                }
            }
            
            public int CompareTo(Record that)
            {
                if (this.IsEmpty && that.IsEmpty)
                {
                    return 0;
                }
                
                int typeCompare = this.Item.Type.CompareTo(that.Item.Type);
                
                if (typeCompare != 0)
                {
                    return typeCompare;
                }
                else
                {
                    return this.ID.CompareTo(that.ID);
                }
            }
        }

        #endregion Nested







        public Inventory()
        {
            _inventory = new Record[INVENTORY_SIZE];

            // fill all slots
            for (int i = 0; i < INVENTORY_SIZE; i++)
            {
                _inventory[i] = new Record(i);
            }
        }
        
        public Inventory(XmlDocument savegame)
            : this()
        {
            foreach (XmlNode node in savegame.SelectNodes("//Seven.Party.Inventory/*"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                               
                string type = node.Name;
                string id = node.Attributes["id"].Value;
                int count = Int32.Parse(node.Attributes["count"].Value);
                int slot = Int32.Parse(node.Attributes["slot"].Value);
                
                _inventory[slot] = new Record(slot);
                _inventory[slot].Count = count;
                _inventory[slot].Item = Item.GetItem(id, type);
            }
        }
        
        public List<Record> GetWeaponsOfType(WeaponType t)
        {
            List<Record> list = new List<Record>();

            foreach (Record ir in _inventory)
            {
                if (!ir.IsEmpty)
                {
                    if (ir.Item.Type == ItemType.Weapon)
                    {
                        if (((Weapon)ir.Item).Wielder == t)
                        {
                            list.Add(ir);
                        }
                    }
                }
            }

            return list;
        }

        public List<Record> GetArmor(Sex sex)
        {
            List<Record> list = new List<Record>();

            foreach (Record ir in _inventory)
            {
                if (!ir.IsEmpty)
                {
                    if (ir.Item.Type == ItemType.Armor)
                    {
                        if (ir.ID == "minervaband")
                        {
                            if (sex == Sex.Female)
                            {
                                list.Add(ir);
                            }
                        }
                        else if (ir.ID == "escortguard")
                        {
                            if (sex == Sex.Male)
                            {
                                list.Add(ir);
                            }
                        }
                        else
                        {
                            list.Add(ir);
                        }
                    }
                }
            }
            return list;
        }

        public List<Record> GetAccessories()
        {
            List<Record> list = new List<Record>();

            foreach (Record ir in _inventory)
            {
                if (!ir.IsEmpty)
                {
                    if (ir.Item.Type == ItemType.Accessory)
                    {
                        list.Add(ir);
                    }
                }
            }

            return list;
        }
        
        public IItem SwapOut(IItem newItem, int slot)
        {
            if (_inventory[slot].Count != 1)
            {
                throw new ImplementationException("Can't swap out an item unless it has a count of 1.");
            }

            IItem temp = _inventory[slot].Item;
            
            _inventory[slot].Item = newItem;
            
            return temp;
        }

        public void IncreaseCount(int slot)
        {
            _inventory[slot].Count++;
        }

        public void DecreaseCount(int slot)
        {
            _inventory[slot].Count--;
        }

        public void AddToInventory(IItem item)
        {
            int i = IndexOf(item);

            if (i >= 0)
            {
                IncreaseCount(i);
            }
            else
            {
                int j = 0;

                while (_inventory[j].IsEmpty)
                {
                    j++;
                }

                _inventory[j].Item = item;
                _inventory[j].Count = 1;
            }
        }
        
        public int IndexOf(IItem item)
        {
            int index = -1;

            foreach (Record ir in _inventory)
            {
                if (ir.ID == item.ID)
                {
                    index = ir.Slot;
                }
            }

            return index;
        }

        public IItem GetItem(int slot)
        {
            return _inventory[slot].Item;
        }

        public int GetCount(int slot)
        {
            return _inventory[slot].Count;
        }

        public void Sort()
        {
            Array.Sort<Record>(_inventory);
        }

        public bool UseItem(int slot)
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
