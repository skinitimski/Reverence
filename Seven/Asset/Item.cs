using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static readonly Item EMPTY = new Item();



        
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



        private Item()
        {
            Name = String.Empty;
            Description = String.Empty;
        }
        
        [LuaFunctionCaller]
        public Item(XmlNode node, Lua lua)
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
                    FieldUsage.CanUse = (LuaFunction)lua.DoString(canUse).First();
                    FieldUsage.Use = (LuaFunction)lua.DoString(use).First();
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

                string use = String.Format("return function (s, t) {0} end", battle.SelectSingleNode("use").InnerText);
                
                try
                {
                    BattleUsage.Use = (LuaFunction)lua.DoString(use).First();
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
        public bool UseInField(Party party)
        {
            if (CanUseInField)
            {
                bool canUse = false;
                
                try
                {
                    switch (FieldUsage.Target)
                    {
                        case FieldTarget.Character:
                            canUse = (bool)FieldUsage.CanUse.Call(party.Selected).First();
                            break;
                        case FieldTarget.Party:
                            canUse = (bool)FieldUsage.CanUse.Call(party).First();
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
                                FieldUsage.Use.Call(party.Selected);
                                break;
                            case FieldTarget.Party:
                                FieldUsage.Use.Call(party);
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
        public void UseInBattle(Ally source, IEnumerable<Combatant> targets)
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
                            BattleUsage.Use.Call(source, targets.First());
                            break;
                            
                        case BattleTarget.Area:
                        case BattleTarget.AreaRandom:
                        case BattleTarget.Group:
                        case BattleTarget.GroupRandom:
                        case BattleTarget.Allies:
                        case BattleTarget.AlliesRandom:
                        case BattleTarget.Enemies:
                        case BattleTarget.EnemiesRandom:
                            BattleUsage.Use.Call(source, targets.ToList());
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





        public string Name { get; private set; }

        public string ID { get { return Resource.CreateID(Name); } }

        public string Description { get; private set; }

        public bool CanUseInField { get { return FieldUsage != null; } }
        
        public bool CanUseInBattle { get { return BattleUsage != null; } }
        
        public FieldTarget FieldTarget { get { return FieldUsage == null ? FieldTarget.None : FieldUsage.Target; } }
        
        public BattleTarget BattleTarget { get { return BattleUsage == null ? BattleTarget.None : BattleUsage.Target; } }
        
        private FieldUsageRecord FieldUsage { get; set; }

        private BattleUsageRecord BattleUsage { get; set; }

        public bool IntendedForEnemies { get { return BattleUsage == null ? false : BattleUsage.IntendedForEnemies; } }
    }
}

