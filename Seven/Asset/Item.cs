using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NLua;

using Atmosphere.Reverence.Attributes;
using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Item : IInventoryItem
    {       
        private static Dictionary<string, Item> _table;
        public static readonly Item EMPTY;




        private class FieldUsageRecord
        {
            public FieldTarget Target { get; set; }

            public LuaFunction CanUse { get; set; }

            public LuaFunction Use { get; set; }
        }



        static Item()
        {
            EMPTY = new Item();
        }
        
        public static void LoadItems()
        {
            _table = new Dictionary<string, Item>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.items.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//items/item"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                
                Item i = new Item(node);
                
                _table.Add(i.ID, i);
            }
        }

        private Item()
        {
            Name = String.Empty;
            Description = String.Empty;
        }
        
        [LuaFunctionCaller]
        private Item(XmlNode node)
            : this()
        {
            Name = node.SelectSingleNode("name").InnerText;
            Description = node.SelectSingleNode("desc").InnerText;
                        
            XmlNode field = node.SelectSingleNode("field");

            if (field != null)
            {
                FieldUsage = new FieldUsageRecord();

                FieldUsage.Target = (FieldTarget)Enum.Parse(typeof(FieldTarget), field.SelectSingleNode("@target").Value);

                char targetParameterName = Char.ToLower(FieldUsage.Target.ToString()[0]);

                string canUse = String.Format("return function ({0}) {1} end", targetParameterName, node.SelectSingleNode("field/canUse").InnerText);
                string use = String.Format("return function ({0}) {1} end", targetParameterName, node.SelectSingleNode("field/use").InnerText);

                try
                {
                    FieldUsage.CanUse = (LuaFunction)Seven.Lua.DoString(canUse).First();
                    FieldUsage.Use = (LuaFunction)Seven.Lua.DoString(use).First();
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in item field scripts; id = " + ID, e);
                }
            }
        }


        /// <summary>
        /// Uses an item in the field.
        /// </summary>
        /// <returns>
        /// <c>true</c>, if use was fielded, <c>false</c> otherwise.
        /// </returns>
        [LuaFunctionCaller]
        public bool UseItemInField()
        {
            if (CanUseInField)
            {
                bool canUse = false;
             
                try
                {
                    switch (FieldUsage.Target)
                    {
                        case FieldTarget.Character:
                            canUse = (bool)FieldUsage.CanUse.Call(Seven.Party.Selected).First();
                            break;
                        case FieldTarget.Party:
                            canUse = (bool)FieldUsage.CanUse.Call(Seven.Party).First();
                            break;
                        case FieldTarget.World:
                            canUse = (bool)FieldUsage.CanUse.Call().First();
                            break;
                    }
                
                    if (canUse)
                    {
                        switch (FieldUsage.Target)
                        {
                            case FieldTarget.Character:
                                FieldUsage.Use.Call(Seven.Party.Selected);
                                break;
                            case FieldTarget.Party:
                                FieldUsage.Use.Call(Seven.Party);
                                break;
                            case FieldTarget.World:
                                FieldUsage.Use.Call();
                                break;
                        }
                    }
                }
                catch (Exception e)
                {
                }

                return canUse;
            }
            else
            {
                throw new ImplementationException("Tried to use an item in the field that can't be used in the field.");
            }
            
        }
        
        public static IInventoryItem GetItem(string id, string type)
        {
            switch (type)
            {
                case "weapon":
                    return Weapon.Get(id);
                case "armor":
                    return Armor.Get(id);
                case "item":
                    return ItemTable[id];
                case "accessory":
                    return Accessory.AccessoryTable[id];
                default:
                    throw new ArgumentException("Item type not valid: " + type, "type");
            }
        }

        public string Name { get; private set; }

        public string ID { get { return Resource.CreateID(Name); } }

        public string Description { get; private set; }

        public bool CanUseInField { get { return FieldUsage != null; } }

        public FieldTarget FieldTarget { get { return FieldUsage == null ? FieldTarget.None : FieldUsage.Target; } }
        
        private FieldUsageRecord FieldUsage { get; set; }

        public static Dictionary<string, Item> ItemTable { get { return _table; } }
    }
}

