using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal abstract class SlotHolder : Equipment
    {
        protected SlotHolder()
            : base()
        {
        }

        protected SlotHolder(XmlNode node)
            : base(node)
        {            
            Slots = new MateriaBase[Int32.Parse(node.SelectSingleNode("slots").InnerText)];
            Links = Int32.Parse(node.SelectSingleNode("links").InnerText);
            
            if (Links > Slots.Length / 2)
            {
                throw new GameDataException("Materia pairs greater than number of slots");
            }
            
            Growth = (Growth)Enum.Parse(typeof(Growth), node.SelectSingleNode("growth").InnerText);
        }


        
        
        public Growth Growth { get; private set; }
        
        public MateriaBase[] Slots  { get; private set; }
        
        public int Links  { get; private set; }
    }
}

