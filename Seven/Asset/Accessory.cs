using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Accessory : IItem
    {
        #region Member Data
        
        string _name;
        string _desc;
        
        private static Dictionary<string, Accessory> _table;
        
        #endregion Member Data
        
        
        public static void LoadAccessories()
        {
            _table = new Dictionary<string, Accessory>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.accessories.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//accessories/accessory"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                string name = node.SelectSingleNode("name").InnerText;
                string desc = node.SelectSingleNode("desc").InnerText;
                string attach = node.SelectSingleNode("attach").InnerText;
                string detach = node.SelectSingleNode("detach").InnerText;
                
                string id = Resource.CreateID(name);
                
//                Game.Lua.DoString("attach" + id + " = " + attach);
//                Game.Lua.DoString("detach" + id + " = " + detach);
                
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
            _table.Add(Resource.CreateID(a.Name), a);
        }
        
//        public void Attach(Character c)
//        {
//            Game.Lua.GetFunction("attach" + ID).Call(c);
//        }
//        public void Detach(Character c)
//        {
//            Game.Lua.GetFunction("detach" + ID).Call(c);
//        }
        
        public ItemType Type { get { return ItemType.Accessory; } }
        public string Name { get { return _name; } }
        public string ID { get { return Resource.CreateID(_name); } }
        public string Description { get { return _desc; } }
        
        public static Dictionary<string, Accessory> AccessoryTable { get { return _table; } }
    }
}

