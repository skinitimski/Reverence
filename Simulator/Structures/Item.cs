using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using NLua;

namespace Atmosphere.BattleSimulator
{
    public struct Item : IItem
    {
        #region Member Data

        string _name;
        string _desc;
        ItemType _type;
        TargetType _targetType;

        private static Dictionary<string, Item> _table;

        private static Character _target;

        #endregion Member Data


        public static void Init()
        {
            _table = new Dictionary<string, Item>();

            XmlDocument gamedata = Util.GetXmlFromResource("data.items.xml");

            foreach (XmlNode node in gamedata.SelectSingleNode("//items").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)));

                string name = xml.SelectSingleNode("//name").InnerText;
                string desc = xml.SelectSingleNode("//desc").InnerText;

                string id = Globals.CreateID(name);

                ItemType type = (ItemType)Enum.Parse(typeof(ItemType), xml.SelectSingleNode("//type").InnerText);
                TargetType target = (TargetType)Enum.Parse(typeof(TargetType), xml.SelectSingleNode("//target").InnerText);

                string field, battle;

                switch (type)
                {
                    case ItemType.Field:
                        field = xml.SelectSingleNode("//field").InnerText;
                        Game.Lua.DoString("usefield" + id + " = " + field);
                        break;
                    case ItemType.Battle:
                        battle = xml.SelectSingleNode("//battle").InnerText;
                        Game.Lua.DoString("usebattle" + id + " = " + battle);
                        break;
                    case ItemType.Hybrid:
                        field = xml.SelectSingleNode("//field").InnerText;
                        battle = xml.SelectSingleNode("//battle").InnerText;
                        Game.Lua.DoString("usefield" + id + " = " + field);
                        Game.Lua.DoString("usebattle" + id + " = " + battle);
                        break;
                }

                Item i = new Item(name, desc, type, target);

                _table.Add(id, i);
            }
        }

        public Item(string name, string desc, ItemType type, TargetType targetType)
        {
            _name = name;
            _desc = desc;
            _type = type;
            _targetType = targetType;
        }

        public bool Use()
        {

            if (Game.State == Game.MainMenu)
            {
                LuaFunction l = Game.Lua.GetFunction("usefield" + ID);
                bool success = false;
                try
                {
                    success = (bool)l.Call()[0];
                }
                catch (Exception e)
                {
                }
                return success;
            }
            else if (Type == ItemType.Battle || Type == ItemType.Hybrid)
            {
                LuaFunction l = Game.Lua.GetFunction("usebattle" + ID);
                try
                {
                    l.Call(Game.Battle.ActiveAbility);
                }
                catch (Exception e)
                {
                }
                return true;
            }
            else
                throw new GameImplementationException("Used a non-battle item while outside the field.");
        }


        public static IItem GetItem(string id, string type)
        {
            switch (type)
            {
                case "weapon":
                    return new Weapon(id);
                case "armor":
                    return new Armor(id);
                case "item":
                    return Atmosphere.BattleSimulator.Item.ItemTable[id];
                case "accessory":
                    return Accessory.AccessoryTable[id];
                default:
                    throw new ArgumentException("Item type not valid: " + type, "type");
            }
        }

        public ItemType Type { get { return _type; } }
        public TargetType TargetType { get { return _targetType; } }
        public string Name { get { return _name; } }
        public string ID { get { return Globals.CreateID(_name); } }
        public string Description { get { return _desc; } }
        public static Dictionary<string, Item> ItemTable { get { return _table; } }
        public static Character Target
        {
            get { return _target; }
            set { _target = value; }
        }
    }
}