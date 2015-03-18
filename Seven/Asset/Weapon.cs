using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Weapon : SlotHolder
    {
        private static Dictionary<string, Weapon> _table;


        
        public static void LoadWeapons()
        {
            _table = new Dictionary<string, Weapon>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.weapons.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("./weapons/weapon"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                Weapon weapon = new Weapon(node);
                
                _table.Add(weapon.ID, weapon);
            }
        }

        private Weapon()
            : base()
        {
        }

        private Weapon(XmlNode node)
            : base(node)
        {     
            Attack = Int32.Parse(node.SelectSingleNode("atk").InnerText);
            AttackPercent = Int32.Parse(node.SelectSingleNode("atkp").InnerText);
            Magic = Int32.Parse(node.SelectSingleNode("mag").InnerText);
            CriticalPercent = Int32.Parse(node.SelectSingleNode("critp").InnerText);
            LongRange = Boolean.Parse(node.SelectSingleNode("longrange").InnerText);
            
            Element = (Element)Enum.Parse(typeof(Element), node.SelectSingleNode("element").InnerText);
            Wielder = (WeaponType)Enum.Parse(typeof(WeaponType), node.SelectSingleNode("type").InnerText);
        }



        public static Weapon Get(string id)
        {
            return _table[id];
        }



        
        
        #region Methods
        
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(string.Format("Name: {0}", Name));
            b.AppendLine(string.Format("\tAttack: {0}", Attack.ToString()));
            b.AppendLine(string.Format("\tAttack%: {0}", AttackPercent.ToString()));
            b.AppendLine(string.Format("\tType: {0}", Type.ToString()));
            
            return b.ToString();
        }
        
        public void Attach(Character c)
        {
            c.MagicBonus += Magic;
            Seven.Lua.GetFunction("attach" + ID).Call(c);
        }

        public void Detach(Character c)
        {
            c.MagicBonus -= Magic;
            Seven.Lua.GetFunction("detach" + ID).Call(c);
        }
        
        public void AttachMateria(MateriaBase orb, int slot)
        {
            if (slot >= Slots.Length)
            {
                throw new ImplementationException("Tried to attach materia to a slot not on this Weapon");
            }

            Slots[slot] = orb;
        }
        
        public static void SwapMateria(Weapon before, Weapon after, Character c)
        {
            for (int i = 0; i < before.Slots.Length; i++)
            {
                MateriaBase m = before.Slots[i];

                if (m != null)
                {
                    if (i > after.Slots.Length)
                    {
                        m.Detach(c);
                        Seven.Party.Materiatory.Put(m);
                    }
                    else
                    {
                        after.Slots[i] = m;
                    }
                }

                before.Slots[i] = null;
            }
        }
        
        #endregion Methods
        
        
        #region Properties

        public override string Description
        {
            get
            {
                string desc = Desc;

                if (Seven.MenuState.ActiveLayer == Seven.MenuState.ItemScreen)
                {
                    return String.Format("A weapon for {0}", Wielder.ToString());
                }

                if (LongRange)
                {
                    desc += (Desc == "" ? "" : "; ") + "Long range weapon";
                }

                return desc;
            }
        }

        public int Attack { get; private set; }

        public int AttackPercent  { get; private set; }

        public int Magic  { get; private set; }

        public int CriticalPercent  { get; private set; }

        public bool LongRange { get; private set; }

        public WeaponType Wielder  { get; private set; }

        public ItemType Type { get { return ItemType.Weapon; } }

        public Element Element { get; private set; }
        
        #endregion Properties
    }
}

