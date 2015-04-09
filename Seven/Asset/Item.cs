using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using NLua;

using Atmosphere.Reverence.Attributes;
using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Battle;

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
        
        private class BattleUsageRecord
        {
            public BattleTarget Target { get; set; }
            
            public LuaFunction Use { get; set; }

            public bool IntendedForEnemies { get; set; }
        }



        static Item()
        {
            _table = new Dictionary<string, Item>();

            EMPTY = new Item();
                        
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
                
                string canUse = String.Format("return function ({0}) {1} end", targetParameterName, field.SelectSingleNode("canUse").InnerText);
                string use = String.Format("return function ({0}) {1} end", targetParameterName, field.SelectSingleNode("use").InnerText);
                
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
            
            XmlNode battle = node.SelectSingleNode("battle");

            if (battle != null)
            {
                BattleUsage = new BattleUsageRecord();
                
                BattleUsage.Target = (BattleTarget)Enum.Parse(typeof(BattleTarget), battle.SelectSingleNode("@target").Value);

                XmlNode intendedForEnemiesNode = battle.SelectSingleNode("@intendedForEnemies");

                if (intendedForEnemiesNode != null)
                {
                    BattleUsage.IntendedForEnemies = Boolean.Parse(intendedForEnemiesNode.Value);
                }

                string use = String.Format("return function (c) {0} end", battle.SelectSingleNode("use").InnerText);
                
                try
                {
                    BattleUsage.Use = (LuaFunction)Seven.Lua.DoString(use).First();
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in item battle scripts; id = " + ID, e);
                }
            }
        }
        
        
        /// <summary>
        /// Uses an item in the field.
        /// </summary>
        [LuaFunctionCaller]
        public bool UseInField()
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
                    throw new ImplementationException("Error calling field script; id = " + ID, e);
                }
                
                return canUse;
            }
            else
            {
                throw new ImplementationException("Tried to use an item in the field that can't be used in the field.");
            }            
        }
        
        
        /// <summary>
        /// Uses an item in battle.
        /// </summary>
        [LuaFunctionCaller]
        public void UseInBattle(IEnumerable<Combatant> targets)
        {
            targets.Count();

            if (CanUseInBattle)
            {                
                try
                {
                    switch (BattleUsage.Target)
                    {
                        case BattleTarget.Combatant:
                        case BattleTarget.Ally:
                        case BattleTarget.Enemy:
                            BattleUsage.Use.Call(targets.First());
                            break;

                        case BattleTarget.Group:
                        case BattleTarget.Allies:
                        case BattleTarget.Enemies:
                            BattleUsage.Use.Call(targets.ToList());
                            break;
                    }
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error calling battle script; id = " + ID, e);
                }
            }
            else
            {
                throw new ImplementationException("Tried to use an item in battle that can't be used in battle.");
            }            
        }



        
        public static IInventoryItem GetItem(string id, InventoryItemType type)
        {
            IInventoryItem item = null;

            switch (type)
            {
                case InventoryItemType.item:
                    item = ItemTable[id];
                    break;
                case InventoryItemType.weapon:
                    item = Weapon.Get(id);
                    break;
                case InventoryItemType.armor:
                    item = Armor.Get(id);
                    break;
                case InventoryItemType.accessory:
                    item = Accessory.Get(id);
                    break;
            }

            return item;
        }

        public string Name { get; private set; }

        public string ID { get { return Resource.CreateID(Name); } }

        public string Description { get; private set; }

        public bool CanUseInField { get { return FieldUsage != null; } }
        
        public bool CanUseInBattle { get { return BattleUsage != null; } }
        
        public FieldTarget FieldTarget { get { return FieldUsage == null ? FieldTarget.None : FieldUsage.Target; } }
        
        public BattleTarget BattleTarget { get { return BattleUsage == null ? BattleTarget.None : BattleUsage.Target; } }
        
        private FieldUsageRecord FieldUsage { get; set; }

        private BattleUsageRecord BattleUsage { get; set; }

        public static Dictionary<string, Item> ItemTable { get { return _table; } }

        public bool IntendedForEnemies { get { return BattleUsage == null ? false : BattleUsage.IntendedForEnemies; } }
    }
}

