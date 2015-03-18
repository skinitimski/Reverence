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
    internal class Armor : Equipment, ISlotHolder
    {
        private static Dictionary<string, Armor> _table;
        
        public static void LoadArmor()
        {
            _table = new Dictionary<string, Armor>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.armour.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//armour/armor"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                Armor armor = new Armor(node);

                _table.Add(armor.ID, armor);
            }
        }

        private Armor()
            : base()
        {
        }
        
        private Armor(XmlNode node)
            : base(node)
        {
            Defense = Int32.Parse(node.SelectSingleNode("def").InnerText);
            DefensePercent = Int32.Parse(node.SelectSingleNode("defp").InnerText);
            MagicDefense = Int32.Parse(node.SelectSingleNode("mdf").InnerText);
            MagicDefensePercent = Int32.Parse(node.SelectSingleNode("mdfp").InnerText);
            
            Slots = new MateriaBase[Int32.Parse(node.SelectSingleNode("slots").InnerText)];

            Links = Int32.Parse(node.SelectSingleNode("links").InnerText);

            if (Links > Slots.Length / 2)
            {
                throw new GameDataException("Materia pairs greater than number of slots");
            }

            Growth = (Growth)Enum.Parse(typeof(Growth), node.SelectSingleNode("growth").InnerText);
        }

        
        public static Armor Get(string id)
        {
            return _table[id];
        }












        
        public void AttachMateria(MateriaBase orb, int slot)
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
        
        public int Defense { get; private set; }

        public int DefensePercent { get; private set; }

        public int MagicDefense { get; private set; }

        public int MagicDefensePercent { get; private set; }

        public ItemType Type { get { return ItemType.Armor; } }

        public Growth Growth { get; private set; }

        public MateriaBase[] Slots { get; private set; }

        public int Links { get; private set; }
    }
}

