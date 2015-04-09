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
        private static readonly Dictionary<string, WeaponData> _table;


        private class WeaponData : SlotHolder.SlotHolderData
        {
            public WeaponData(XmlNode node)
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
            
            public string Description { get; private set; }
                        
            public int Attack { get; private set; }
            
            public int AttackPercent  { get; private set; }
            
            public int Magic  { get; private set; }
            
            public int CriticalPercent  { get; private set; }
            
            public bool LongRange { get; private set; }
            
            public WeaponType Wielder { get; private set; }
            
            public Element Element { get; private set; }
        }


        
        static Weapon()
        {
            _table = new Dictionary<string, WeaponData>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.weapons.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("./weapons/weapon"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                WeaponData weapon = new WeaponData(node);
                
                _table.Add(weapon.ID, weapon);
            }
        }



        private Weapon(WeaponData data)
            : base(data)
        {     
            Attack = data.Attack;
            AttackPercent = data.AttackPercent;
            Magic = data.Magic;
            CriticalPercent = data.CriticalPercent;
            LongRange = data.LongRange;
            
            Element = data.Element;
            Wielder = data.Wielder;
        }



        public static Weapon Get(string id)
        {
            return new Weapon(_table[id]);
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
        
        public void AttachMateria(MateriaOrb orb, int slot)
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
                MateriaOrb m = before.Slots[i];

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

        public InventoryItemType Type { get { return InventoryItemType.weapon; } }

        public Element Element { get; private set; }
        
        #endregion Properties
    }
}

