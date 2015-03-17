using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NLua;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Item : IItem
    {       
        #region Member Data
        
        string _name;
        string _desc;
        ItemType _type;
        ItemTarget _targetType;
        
        #endregion Member Data



        private static Dictionary<string, Item> _table;

        public static Item EMPTY { get; private set; }
        
        public static void LoadItems()
        {
            _table = new Dictionary<string, Item>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.items.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//items/item"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }


                string name = node.SelectSingleNode("name").InnerText;
                string desc = node.SelectSingleNode("desc").InnerText;
                
                string id = Resource.CreateID(name);
                
                ItemType type = (ItemType)Enum.Parse(typeof(ItemType), node.SelectSingleNode("type").InnerText);
                ItemTarget target = (ItemTarget)Enum.Parse(typeof(ItemTarget), node.SelectSingleNode("target").InnerText);
                
                string field, battle;
                
                switch (type)
                {
                    case ItemType.Field:
                        field = node.SelectSingleNode("field").InnerText;
                        Seven.Lua.DoString("usefield" + id + " = " + field);
                        break;
                    case ItemType.Battle:
                        battle = node.SelectSingleNode("battle").InnerText;
                        Seven.Lua.DoString("usebattle" + id + " = " + battle);
                        break;
                    case ItemType.Hybrid:
                        field = node.SelectSingleNode("field").InnerText;
                        battle = node.SelectSingleNode("battle").InnerText;
                        Seven.Lua.DoString("usefield" + id + " = " + field);
                        Seven.Lua.DoString("usebattle" + id + " = " + battle);
                        break;
                }
                
                Item i = new Item(name, desc, type, target);
                
                _table.Add(id, i);
            }

            EMPTY = new Item("", "", ItemType.Nonfunctional, ItemTarget.None);
        }
        
        public Item(string name, string desc, ItemType type, ItemTarget targetType)
        {
            _name = name;
            _desc = desc;
            _type = type;
            _targetType = targetType;
        }
        
        public bool Use()
        {            
            if (Seven.CurrentState.Equals(Seven.MenuState))
            {
                LuaFunction l = Seven.Lua.GetFunction("usefield" + ID);
                bool success = false;
                try
                {
                    success = (bool)l.Call() [0];
                }
                catch (Exception e)
                {
                }
                return success;
            }
            else if (Type == ItemType.Battle || Type == ItemType.Hybrid)
            {
                LuaFunction l = Seven.Lua.GetFunction("usebattle" + ID);
                try
                {
                    //l.Call(Game.Battle.ActiveAbility);
                }
                catch (Exception e)
                {
                }
                return true;
            }
            else
            {
                throw new ImplementationException("Used a non-battle item while outside the field.");
            }
        }
        
        public static IItem GetItem(string id, string type)
        {
            switch (type)
            {
                case "weapon":
                    return Weapon.Get(id);
                case "armor":
                    return Armor.Get(id);
                case "item":
                    return ItemTable[id];
                case "accessory":
                    return Accessory.AccessoryTable[id];
                default:
                    throw new ArgumentException("Item type not valid: " + type, "type");
            }
        }
        
        public ItemType Type { get { return _type; } }

        public ItemTarget TargetType { get { return _targetType; } }

        public string Name { get { return _name; } }

        public string ID { get { return Resource.CreateID(_name); } }

        public string Description { get { return _desc; } }

        public static Dictionary<string, Item> ItemTable { get { return _table; } }
    }
}

