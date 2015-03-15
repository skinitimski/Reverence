using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Armor : IItem, ISlotHolder
    {
        #region Member Data
        
        private int _defense;
        private int _defensePercent;
        private int _magicDefense;
        private int _magicDefensePercent;
        private string _name;
        private string _desc;
        private MateriaBase[] _slots;
        private int _links;
        private Growth _growth;
        
        private static Dictionary<string, string> _table;
        
        #endregion Member Data
        
        public static void LoadArmor()
        {
            _table = new Dictionary<string, string>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.armour.xml",typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//armour/armor"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                string name = node.SelectSingleNode("name").InnerText;
                string id = Resource.CreateID(name);
//                string attach = node.SelectSingleNode("attach").InnerText;
//                string detach = node.SelectSingleNode("detach").InnerText;
                
//                Game.Lua.DoString("attach" + id + " = " + attach);
//                Game.Lua.DoString("detach" + id + " = " + detach);
                
                _table.Add(id, node.OuterXml);
            }
        }
        
        public Armor(string id)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(_table [id])));
            
            _name = xml.SelectSingleNode("//name").InnerText;
            _desc = xml.SelectSingleNode("//desc").InnerText;
            _defense = Int32.Parse(xml.SelectSingleNode("//def").InnerText);
            _defensePercent = Int32.Parse(xml.SelectSingleNode("//defp").InnerText);
            _magicDefense = Int32.Parse(xml.SelectSingleNode("//mdf").InnerText);
            _magicDefensePercent = Int32.Parse(xml.SelectSingleNode("//mdfp").InnerText);
            
            _slots = new MateriaBase[Int32.Parse(xml.SelectSingleNode("//slots").InnerText)];

            _links = Int32.Parse(xml.SelectSingleNode("//links").InnerText);
            if (_links > _slots.Length / 2)
            {
                throw new GameDataException("Materia pairs greater than number of slots");
            }

            _growth = (Growth)Enum.Parse(typeof(Growth), xml.SelectSingleNode("//growth").InnerText);
        }
        
        public void AttachMateria(MateriaBase orb, int slot)
        {
            if (slot >= _slots.Length)
            {
                throw new ImplementationException("Tried to attach materia to a slot not on this Armor");
            }

            _slots[slot] = orb;
        }
        
//        public void Attach(Character c)
//        {
//            Game.Lua.GetFunction("attach" + ID).Call(c);
//        }
//        public void Detach(Character c)
//        {
//            Game.Lua.GetFunction("detach" + ID).Call(c);
//        }
        
        public static void SwapMateria(Armor before, Armor after, Character c)
        {
            for (int i = 0; i < before.Slots.Length; i++)
            {
                MateriaBase m = before.Slots[i];
                if (m != null)
                    if (i > after.Slots.Length)
                {
                    //m.Detach(c);
                    Seven.Party.Materiatory.Put(m);
                }
                else
                {
                    after.Slots[i] = m;
                }
                before.Slots[i] = null;
            }
        }
        
        public int Defense { get { return _defense; } }
        public int DefensePercent { get { return _defensePercent; } }
        public int MagicDefense { get { return _magicDefense; } }
        public int MagicDefensePercent { get { return _magicDefensePercent; } }
        public string Name { get { return _name; } }
        public string ID { get { return Resource.CreateID(_name); } }
        public string Description { get { return _desc; } }
        public ItemType Type { get { return ItemType.Armor; } }
        public Growth Growth { get { return _growth; } }
        public MateriaBase[] Slots { get { return _slots; } }
        public int Links { get { return _links; } }
    }
}

