using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Weapon : IItem, ISlotHolder
    {
        #region Member Data
        
        private int _attack;
        private int _attackPercent;
        private int _magic;
        private int _criticalPercent;
        private bool _longRange;
        private string _name;
        private string _desc;
        private WeaponType _type;
        private Element _element;
        private MateriaBase[] _slots;
        private int _links;
        private Growth _growth;
        
        private static Dictionary<string, string> _table;
        
        #endregion Member Data
        
        
        public static void LoadWeapons()
        {
            _table = new Dictionary<string, string>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.weapons.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//weapons/weapon"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                                
                string name = node.SelectSingleNode("name").InnerText;
                string id = Resource.CreateID(name);
                //string attach = node.SelectSingleNode("attach").InnerText;
                //string detach = node.SelectSingleNode("detach").InnerText;
                
                //Game.Lua.DoString("attach" + id + " = " + attach);
                //Game.Lua.DoString("detach" + id + " = " + detach);
                
                _table.Add(id, node.OuterXml);
            }
        }

        public Weapon(string id)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(_table [id])));
            
            _name = xml.SelectSingleNode("//name").InnerText;
            _desc = xml.SelectSingleNode("//desc").InnerText;
            
            _attack = Int32.Parse(xml.SelectSingleNode("//atk").InnerText);
            _attackPercent = Int32.Parse(xml.SelectSingleNode("//atkp").InnerText);
            _magic = Int32.Parse(xml.SelectSingleNode("//mag").InnerText);
            _criticalPercent = Int32.Parse(xml.SelectSingleNode("//critp").InnerText);
            _longRange = Boolean.Parse(xml.SelectSingleNode("//longrange").InnerText);
            
            _element = (Element)Enum.Parse(typeof(Element), xml.SelectSingleNode("//element").InnerText);
            _type = (WeaponType)Enum.Parse(typeof(WeaponType), xml.SelectSingleNode("//type").InnerText);
            
            _slots = new MateriaBase[Int32.Parse(xml.SelectSingleNode("//slots").InnerText)];
            _links = Int32.Parse(xml.SelectSingleNode("//links").InnerText);

            if (_links > _slots.Length / 2)
            {
                throw new GameDataException("Materia pairs greater than number of slots");
            }
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
            //Game.Lua.GetFunction("attach" + ID).Call(c);
        }
        public void Detach(Character c)
        {
            c.MagicBonus -= _magic;
            //Game.Lua.GetFunction("detach" + ID).Call(c);
        }
        
        public void AttachMateria(MateriaBase orb, int slot)
        {
            if (slot >= _slots.Length)
            {
                throw new ImplementationException("Tried to attach materia to a slot not on this Weapon");
            }

            _slots[slot] = orb;
        }
        
        
        public static void SwapMateria(Weapon before, Weapon after, Character c)
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
        
        #endregion Methods
        
        
        #region Properties
        
        public string ID { get { return Resource.CreateID(_name); } }
        public string Name { get { return _name; } }
        public string Description
        {
            get
            {
                string desc = _desc;

//                if (Seven.MenuState.Layer == Seven.MenuState.ItemScreen)
//                    return String.Format("A weapon for {0}", Wielder.ToString());

                if (LongRange) 
                {
                    desc += (_desc == "" ? "" : "; ") + "Long range weapon";
                }

                return desc;
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
        public MateriaBase[] Slots { get { return _slots; } }
        public int Links { get { return _links; } }
        
        #endregion Properties
    }
}

