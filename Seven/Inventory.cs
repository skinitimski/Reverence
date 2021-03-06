using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;

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
            private IInventoryItem _item = GameItem.EMPTY;
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

            public IInventoryItem Item
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
                int comparison = 0;

                if (that.IsEmpty)
                {
                    if (!this.IsEmpty)
                    {
                        return 1;
                    }
                    // else == 0
                }
                else
                {
                    comparison = this.ID.CompareTo(that.ID);
                }

                return comparison;
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

        public Inventory(DataStore data, XmlNode savegame)
            : this()
        {
            foreach (XmlNode node in savegame.SelectNodes("./inventory/*"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                               
                InventoryItemType type = (InventoryItemType)Enum.Parse(typeof(InventoryItemType), node.Name);
                string id = node.Attributes["id"].Value;
                int count = Int32.Parse(node.Attributes["count"].Value);
                int slot = Int32.Parse(node.Attributes["slot"].Value);
                
                _inventory[slot] = new Record(slot);
                _inventory[slot].Count = count;
                _inventory[slot].Item = data.GetInventoryItem(id, type);
            }
        }
        
        public List<Record> GetWeaponsOfType(WeaponType t)
        {
            List<Record> list = new List<Record>();

            foreach (Record ir in _inventory)
            {
                if (!ir.IsEmpty)
                {
                    Weapon weapon = ir.Item as Weapon;

                    if (weapon != null)
                    {
                        if (weapon.Wielder == t)
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
                    Armor armor = ir.Item as Armor;
                    
                    if (armor != null)
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
//            List<Record> list = new List<Record>();
            List<Record> list = _inventory.Where(x => !x.IsEmpty && x.Item is Accessory).ToList();

//            foreach (Record ir in _inventory)
//            {
//                if (!ir.IsEmpty)
//                {
//                    Accessory accessory = ir.Item as Accessory;
//
//                    if (Accessory != null)
//                    {
//                        list.Add(ir);
//                    }
//                }
//            }

            return list;
        }
        
        public IInventoryItem SwapOut(IInventoryItem newItem, int slot)
        {
            if (_inventory[slot].Count != 1)
            {
                throw new ImplementationException("Can't swap out an item unless it has a count of 1.");
            }

            IInventoryItem temp = _inventory[slot].Item;
            
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


        public void AddToInventory(IInventoryItem item)
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

                    if (j > _inventory.Length)
                    {
                        throw new ImplementationException("Inventory is full.");
                    }
                }

                _inventory[j].Item = item;
                _inventory[j].Count = 1;
            }
        }
        
        public int IndexOf(IInventoryItem item)
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

        public IInventoryItem GetItem(int slot)
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

            // HACK. We want nulls at the end, but for some reason they're not
            // getting passed to the comparison.
            int q = 0;
            while (_inventory[q].IsEmpty)
            {
                q++;
            }
            
            Record[] tempNull = new Record[q];
            Record[] tempNotNull = new Record[INVENTORY_SIZE - q];
            Array.Copy(_inventory, 0, tempNull, 0, q);
            Array.Copy(_inventory, q, tempNotNull, 0, INVENTORY_SIZE - q);
            Array.Copy(tempNull, 0, _inventory, INVENTORY_SIZE - q, q);
            Array.Copy(tempNotNull, 0, _inventory, 0, INVENTORY_SIZE - q);
            // END HACK
        }


        public void WriteToXml(XmlWriter writer)
        {
            writer.WriteStartElement(typeof(Inventory).Name.ToLower());

            foreach (Record record in _inventory)
            {
                if (!record.IsEmpty)
                {
                    writer.WriteStartElement(record.Item.GetType().Name.ToLower());

                    writer.WriteAttributeString("id", record.Item.ID);
                    writer.WriteAttributeString("count", record.Count.ToString());
                    writer.WriteAttributeString("slot", record.Slot.ToString());

                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement(); // inventory;
        }

        private Party Owners { get; set; }
    }
}
