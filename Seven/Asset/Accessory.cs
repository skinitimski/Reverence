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
        public static readonly Dictionary<string, Accessory> AccessoryTable;

        public static readonly Accessory EMPTY;

        static Accessory()
        {
            EMPTY = new Accessory();
            AccessoryTable = new Dictionary<string, Accessory>();

            XmlDocument gamedata = Resource.GetXmlFromResource("data.accessories.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//accessories/accessory"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                Accessory a = new Accessory(node);
                
                AccessoryTable.Add(a.ID, a);
            }
        }

        private Accessory()
            : base()
        {
        }

        private Accessory(XmlNode node)
            : base(node)
        {
        }
        
        public InventoryItemType Type { get { return InventoryItemType.accessory; } }


    }
}

