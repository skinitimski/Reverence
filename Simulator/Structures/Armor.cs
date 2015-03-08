using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Atmosphere.BattleSimulator
{
    public class Armor : IItem, ISlotHolder
    {
        #region Member Data

        private int _defense;
        private int _defensePercent;
        private int _magicDefense;
        private int _magicDefensePercent;
        private string _name;
        private string _desc;
        private Materia[] _slots;
        private int _links;
        private Growth _growth;

        private static Dictionary<string, string> _table;

        #endregion Member Data

        public static void Init()
        {
            _table = new Dictionary<string, string>();

            XmlDocument gamedata = Util.GetXmlFromResource("data.armour.xml");

            foreach (XmlNode node in gamedata.SelectSingleNode("//armour").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)));

                string name = xml.SelectSingleNode("//name").InnerText;
                string id = Globals.CreateID(name);
                string attach = xml.SelectSingleNode("//attach").InnerText;
                string detach = xml.SelectSingleNode("//detach").InnerText;

                Game.Lua.DoString("attach" + id + " = " + attach);
                Game.Lua.DoString("detach" + id + " = " + detach);

                _table.Add(Globals.CreateID(name), node.OuterXml);
            }
        }

        public Armor(string id)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(_table[id])));

            _name = xml.SelectSingleNode("//name").InnerText;
            _desc = xml.SelectSingleNode("//desc").InnerText;
            _defense = Int32.Parse(xml.SelectSingleNode("//def").InnerText);
            _defensePercent = Int32.Parse(xml.SelectSingleNode("//defp").InnerText);
            _magicDefense = Int32.Parse(xml.SelectSingleNode("//mdf").InnerText);
            _magicDefensePercent = Int32.Parse(xml.SelectSingleNode("//mdfp").InnerText);

            _slots = new Materia[Int32.Parse(xml.SelectSingleNode("//slots").InnerText)];
            _links = Int32.Parse(xml.SelectSingleNode("//links").InnerText);
            if (_links > _slots.Length / 2)
                throw new GamedataException("Materia pairs greater than number of slots");
            _growth = (Growth)Enum.Parse(typeof(Growth), xml.SelectSingleNode("//growth").InnerText);
        }

        public void AttachMateria(Materia orb, int slot)
        {
            if (slot >= _slots.Length)
                throw new GameImplementationException("Tried to attach materia to a slot not on this Armor");
            _slots[slot] = orb;
        }

        public void Attach(Character c)
        {
            Game.Lua.GetFunction("attach" + ID).Call(c);
        }
        public void Detach(Character c)
        {
            Game.Lua.GetFunction("detach" + ID).Call(c);
        }

        public static void SwapMateria(Armor before, Armor after, Character c)
        {
            for (int i = 0; i < before.Slots.Length; i++)
            {
                Materia m = before.Slots[i];
                if (m != null)
                    if (i > after.Slots.Length)
                    {
                        m.Detach(c);
                        Materiatory.Put(m);
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
        public string ID { get { return Globals.CreateID(_name); } }
        public string Description { get { return _desc; } }
        public ItemType Type { get { return ItemType.Armor; } }
        public Growth Growth { get { return _growth; } }
        public Materia[] Slots { get { return _slots; } }
        public int Links { get { return _links; } }
    }

}
