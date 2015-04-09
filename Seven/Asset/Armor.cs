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
        private static readonly Dictionary<string, ArmorData> _table;


        private class ArmorData : SlotHolder.SlotHolderData
        {
            public ArmorData(XmlNode node)
                : base(node)
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
        
        static Armor()
        {
            _table = new Dictionary<string, ArmorData>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.armour.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//armour/armor"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                ArmorData armor = new ArmorData(node);

                _table.Add(armor.ID, armor);
            }
        }


        
        private Armor(ArmorData data)
            : base(data)
        {
            Defense = data.Defense;
            DefensePercent = data.DefensePercent;
            MagicDefense = data.MagicDefense;
            MagicDefensePercent = data.MagicDefensePercent;
        }

        
        public static Armor Get(string id)
        {
            return new Armor(_table[id]);
        }












        
        public void AttachMateria(MateriaOrb orb, int slot)
        {
            if (slot >= Slots.Length)
            {
                throw new ImplementationException("Tried to attach materia to a slot not on this Armor");
            }

            Slots[slot] = orb;
        }

        public static void SwapMateria(Armor before, Armor after, Character c)
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
        
        public int Defense { get; private set; }

        public int DefensePercent { get; private set; }

        public int MagicDefense { get; private set; }

        public int MagicDefensePercent { get; private set; }

        public InventoryItemType Type { get { return InventoryItemType.armor; } }
    }
}

