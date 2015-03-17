using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset
{
    public class Spell
    {        
        #region Member Data
        
        private string _name;
        private string _desc;
        private int _cost;
        private int _matp;
        private int _order;
        private Element[] _element;
        
        #endregion Member Data


        
        private static Dictionary<string, Spell> _table;


        
        public static void LoadSpells()
        {
            _table = new Dictionary<string, Spell>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.spells.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("/spells/spell"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                string name = node.SelectSingleNode("name").InnerText;
                string id = Resource.CreateID(name);
                string dispatch = node.SelectSingleNode("dispatch").InnerText;
                string action = node.SelectSingleNode("action").InnerText;
                if (dispatch == "")
                {
                    dispatch = "function () end";
                }
                if (action == "")
                {
                    action = "function (state) end";
                }
                
                if (id == "????")
                {
                    Seven.Lua.DoString("dispatchqqqq" + " = " + dispatch);
                    Seven.Lua.DoString("actionqqqq" + " = " + action);
                }
                else
                {
                    Seven.Lua.DoString("dispatch" + id + " = " + dispatch);
                    Seven.Lua.DoString("action" + id + " = " + action);
                }
                
                _table.Add(id, new Spell(node));
            }
            
        }
        
        public Spell(XmlNode xml)
        {
            _name = xml.SelectSingleNode("name").InnerText;
            _desc = xml.SelectSingleNode("desc").InnerText;
            _cost = Int32.Parse(xml.SelectSingleNode("cost").InnerText);
            _matp = Int32.Parse(xml.SelectSingleNode("matp").InnerText);
            _order = Int32.Parse(xml.SelectSingleNode("order").InnerText);
            
            XmlNodeList nodes = xml.SelectSingleNode("elements").ChildNodes;
            _element = new Element[nodes.Count];
            
            for (int i = 0;i < _element.Length;i++)
            {
                if (nodes[i].NodeType == XmlNodeType.Comment)
                {
                    throw new GameDataException("Remove XML comment from spell element list goddammit!!");
                }
                
                _element[i] = (Element)Enum.Parse(typeof(Element), nodes[i].InnerText);
                i++;
            }
        }
        
        
        public void Dispatch()
        {
            if (ID == "????")
            {
                Seven.Lua.GetFunction("dispatch" + "qqqq").Call();
            }
            else
            {
                Seven.Lua.GetFunction("dispatch" + ID).Call();
            }
        }
        public void Action()
        {
            if (ID == "????")
            {
                //Seven.Lua.GetFunction("action" + "qqqq").Call(Game.Battle.ActiveAbility);
            }
            else
            {
                //Seven.Lua.GetFunction("action" + ID).Call(Game.Battle.ActiveAbility);
            }
        }
        
        public static int Compare(Spell left, Spell right)
        {
            return left._order.CompareTo(right._order);
        }
        
        
        public string Name { get { return _name; } }
        public string Desc { get { return _desc; } }
        public string ID { get { return Resource.CreateID(_name == null ? "" : _name); } }
        public Element[] Element { get { return _element; } }
        public int Cost { get { return _cost; } }
        public int Matp { get { return _matp; } }
        public int Order { get { return _order; } }
        
        public static Dictionary<string, Spell> SpellTable { get { return _table; } }
    }
}

