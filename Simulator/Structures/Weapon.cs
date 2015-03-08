using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Atmosphere.BattleSimulator
{
    public class Weapon : IItem, ISlotHolder
    {
        #region Member Data

        public const string DATAFILE = @"Data\weapons.xml";

        private int _attack;
        private int _attackPercent;
        private int _magic;
        private int _criticalPercent;
        private bool _longRange;
        private string _name;
        private string _desc;
        private WeaponType _type;
        private Element _element;
        private Materia[] _slots;
        private int _links;
        private Growth _growth;

        private static Dictionary<string, string> _table;

        #endregion Member Data


        public static void Init()
        {
            _table = new Dictionary<string, string>();

            XmlDocument gamedata = new XmlDocument();
            gamedata.Load(DATAFILE);

            foreach (XmlNode node in gamedata.SelectSingleNode("//weapons").ChildNodes)
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
        public Weapon(string id)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(_table[id])));

            _name = xml.SelectSingleNode("//name").InnerText;
            _desc = xml.SelectSingleNode("//desc").InnerText;

            _attack = Int32.Parse(xml.SelectSingleNode("//atk").InnerText);
            _attackPercent = Int32.Parse(xml.SelectSingleNode("//atkp").InnerText);
            _magic = Int32.Parse(xml.SelectSingleNode("//mag").InnerText);
            _criticalPercent = Int32.Parse(xml.SelectSingleNode("//critp").InnerText);
            _longRange = Boolean.Parse(xml.SelectSingleNode("//longrange").InnerText);

            _element = (Element)Enum.Parse(typeof(Element), xml.SelectSingleNode("//element").InnerText);
            _type = (WeaponType)Enum.Parse(typeof(WeaponType), xml.SelectSingleNode("//type").InnerText);

            _slots = new Materia[Int32.Parse(xml.SelectSingleNode("//slots").InnerText)];
            _links = Int32.Parse(xml.SelectSingleNode("//links").InnerText);
            if (_links > _slots.Length / 2)
                throw new GamedataException("Materia pairs greater than number of slots");
            _growth = (Growth)Enum.Parse(typeof(Growth), xml.SelectSingleNode("//growth").InnerText);
        }


        #region Methods

        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(string.Format("Name: {0}", _name));
            b.AppendLine(string.Format("\tAttack: {0}", _attack.ToString()));
            b.AppendLine(string.Format("\tAttack%: {0}", _attackPercent.ToString()));
            b.AppendLine(string.Format("\tType: {0}", _type.ToString()));

            return b.ToString();
        }

        public void Attach(Character c) 
        {
            c.MagicBonus += _magic;
            Game.Lua.GetFunction("attach" + ID).Call(c);
        }
        public void Detach(Character c)
        {
            c.MagicBonus -= _magic;
            Game.Lua.GetFunction("detach" + ID).Call(c);
        }

        public void AttachMateria(Materia orb, int slot)
        {
            if (slot >= _slots.Length)
                throw new GameImplementationException("Tried to attach materia to a slot not on this Weapon");
            _slots[slot] = orb;
        }


        public static void SwapMateria(Weapon before, Weapon after, Character c)
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

        #endregion Methods


        #region Properties

        public string ID { get { return Globals.CreateID(_name); } }
        public string Name { get { return _name; } }
        public string Description
        {
            get
            {
                if (Game.MainMenu.Layer == MenuScreen.ItemScreen)
                    return String.Format("A weapon for {0}", Wielder.ToString());
                else if (LongRange) 
                    return _desc + (_desc == "" ? "" : "; ") + "Long range weapon";
                else return _desc;
            }
        }
        public int Attack { get { return _attack; } }
        public int AttackPercent { get { return _attackPercent; } }
        public int Magic { get { return _magic; } }
        public int CriticalPercent { get { return _criticalPercent; } }
        public bool LongRange { get { return _longRange; } }
        public WeaponType Wielder { get { return _type; } }
        public ItemType Type { get { return ItemType.Weapon; } }
        public Element Element { get { return _element; } }
        public Growth Growth { get { return _growth; } }
        public Materia[] Slots { get { return _slots; } }
        public int Links { get { return _links; } }

        #endregion Properties

    }

    
    
}
