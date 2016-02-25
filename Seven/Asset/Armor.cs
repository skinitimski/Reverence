using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using NLua;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Armor : SlotHolder
    {
        internal class ArmorData : SlotHolder.SlotHolderData
        {
            public ArmorData(XmlNode node, Lua lua)
                : base(node, lua)
            {
                Defense = Int32.Parse(node.SelectSingleNode("def").InnerText);
                DefensePercent = Int32.Parse(node.SelectSingleNode("defp").InnerText);
                MagicDefense = Int32.Parse(node.SelectSingleNode("mdf").InnerText);
                MagicDefensePercent = Int32.Parse(node.SelectSingleNode("mdfp").InnerText);
            }

            public int Defense { get; private set; }
            
            public int DefensePercent { get; private set; }
            
            public int MagicDefense { get; private set; }
            
            public int MagicDefensePercent { get; private set; }
        }


        
        public Armor(ArmorData data)
            : base(data)
        {
            Defense = data.Defense;
            DefensePercent = data.DefensePercent;
            MagicDefense = data.MagicDefense;
            MagicDefensePercent = data.MagicDefensePercent;
        }












        
        public void AttachMateria(MateriaOrb orb, int slot)
        {
            if (slot >= Slots.Length)
            {
                throw new ImplementationException("Tried to attach materia to a slot not on this Armor");
            }

            Slots[slot] = orb;
        }
        
        public int Defense { get; private set; }

        public int DefensePercent { get; private set; }

        public int MagicDefense { get; private set; }

        public int MagicDefensePercent { get; private set; }

        public InventoryItemType Type { get { return InventoryItemType.armor; } }
    }
}

