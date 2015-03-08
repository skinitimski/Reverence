using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Atmosphere.BattleSimulator
{
    public struct Accessory : IItem
    {
        #region Member Data

        public const string DATAFILE = @"Data\accessories.xml";

        string _name;
        string _desc;

        private static Dictionary<string, Accessory> _table;

        #endregion Member Data


        public static void Init()
        {
            _table = new Dictionary<string, Accessory>();

            XmlDocument gamedata = new XmlDocument();
            gamedata.Load(DATAFILE);

            foreach (XmlNode node in gamedata.SelectSingleNode("//accessories").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)));

                string name = xml.SelectSingleNode("//name").InnerText;
                string desc = xml.SelectSingleNode("//desc").InnerText;
                string attach = xml.SelectSingleNode("//attach").InnerText;
                string detach = xml.SelectSingleNode("//detach").InnerText;

                string id = Globals.CreateID(name);

                Game.Lua.DoString("attach" + id + " = " + attach);
                Game.Lua.DoString("detach" + id + " = " + detach);

                Accessory a = new Accessory(name, desc);

                _table.Add(id, a);
            }
        }

        public Accessory(string name, string desc)
        {
            _name = name;
            _desc = desc;
        }

        private static void Add(Accessory a)
        {
            _table.Add(Globals.CreateID(a.Name), a);
        }

        public void Attach(Character c)
        {
            Game.Lua.GetFunction("attach" + ID).Call(c);
        }
        public void Detach(Character c)
        {
            Game.Lua.GetFunction("detach" + ID).Call(c);
        }

        public ItemType Type { get { return ItemType.Accessory; } }
        public string Name { get { return _name; } }
        public string ID { get { return Globals.CreateID(_name); } }
        public string Description { get { return _desc; } }

        public static Dictionary<string, Accessory> AccessoryTable { get { return _table; } }
    }
    
}
