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
    internal abstract class SlotHolder : Equipment
    {
        internal class SlotHolderData : Equipment.EquipmentData
        {   
            public SlotHolderData(XmlNode node, Lua lua)
                : base(node, lua)
            {     
                Slots = Int32.Parse(node.SelectSingleNode("slots").InnerText);
                Links = Int32.Parse(node.SelectSingleNode("links").InnerText);
                
                if (Links > Slots / 2)
                {
                    throw new GameDataException("Materia pairs greater than number of slots; id = " + ID);
                }
                
                Growth = (Growth)Enum.Parse(typeof(Growth), node.SelectSingleNode("growth").InnerText);
            }       
            
            
            
            public Growth Growth { get; private set; }
            
            public int Slots  { get; private set; }
            
            public int Links  { get; private set; }
        }

        protected SlotHolder(SlotHolderData data)
            : base(data)
        {            
            Slots = new MateriaOrb[data.Slots];
            Links = data.Links;
            Growth = data.Growth;
        }


        
        
        public Growth Growth { get; private set; }
        
        public MateriaOrb[] Slots { get; private set; }
        
        public int Links  { get; private set; }
    }
}

