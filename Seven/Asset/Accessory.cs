using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

using NLua;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Accessory : Equipment
    {        
        private static readonly Dictionary<string, Equipment.EquipmentData> _table;
        
        public static readonly Accessory EMPTY;

        static Accessory()
        {
            EMPTY = new Accessory(Equipment.EquipmentData.EMPTY);

            _table = new Dictionary<string, Equipment.EquipmentData>();

            XmlDocument gamedata = Resource.GetXmlFromResource("data.accessories.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//accessories/accessory"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                Equipment.EquipmentData a = new Equipment.EquipmentData(node);
                
                _table.Add(a.ID, a);
            }
        }


        private Accessory(Equipment.EquipmentData data)
            : base(data)
        {
        }
        
        
        public static Accessory Get(string id)
        {
            return new Accessory(_table[id]);
        }


        public InventoryItemType Type { get { return InventoryItemType.accessory; } }
    }
}

