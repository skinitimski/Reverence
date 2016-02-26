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
        public Accessory(Equipment.EquipmentData data)
            : base(data)
        {
        }


        public InventoryItemType Type { get { return InventoryItemType.accessory; } }
    }
}

