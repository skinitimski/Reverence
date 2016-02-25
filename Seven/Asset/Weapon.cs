using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset.Materia;
using NLua;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Weapon : SlotHolder
    {
        internal class WeaponData : SlotHolder.SlotHolderData
        {
            public WeaponData(XmlNode node, Lua lua)
                : base(node, lua)
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





        public Weapon(WeaponData data)
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
        
        #endregion Methods
        
        
        #region Properties

        public override string Description
        {
            get
            {
                string desc = Desc;

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

