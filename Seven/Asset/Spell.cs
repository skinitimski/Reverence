using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Spell
    {  
        private class StatusChange
        {
            public enum Effect
            {
                Inflict,
                Cure,
                Toggle
            }

            public IEnumerable<Status> Statuses { get; set; }

            public Effect ChangeType { get; set; }

            public int Odds { get; set; }
        }

        private static Dictionary<string, Spell> _magicSpells;
        private static Dictionary<string, Spell> _summonSpells;
        private static Dictionary<string, Spell> _enemySkillSpells;

        static Spell()
        {
            _magicSpells = LoadSpells("data.spells.magic.xml");
            _summonSpells = LoadSpells("data.spells.summon.xml");
            _enemySkillSpells = LoadSpells("data.spells.enemyskill.xml");
        }

        private static Dictionary<string, Spell> LoadSpells(string resource)
        {
            Dictionary<string, Spell> table = new Dictionary<string, Spell>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource(resource, typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("/spells/spell"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                Spell spell = new Spell(node);
                
                table.Add(spell.ID, spell);
            } 
            
            return table;
        }

        private Spell()
        {
            Type = AttackType.Magical;
            TargetEnemiesFirst = true;
        }
        
        public Spell(XmlNode xml)
            : this()
        {
            Name = xml.SelectSingleNode("name").InnerText;
            Desc = xml.SelectSingleNode("desc").InnerText;
            ID = Resource.CreateID(Name);
            
            Type = (AttackType)Enum.Parse(typeof(AttackType), xml.SelectSingleNode("type").InnerText);
            Target = (BattleTarget)Enum.Parse(typeof(BattleTarget), xml.SelectSingleNode("target").InnerText);
            TargetEnemiesFirst = Boolean.Parse(xml.SelectSingleNode("targetEnemiesFirst").InnerText);
            MPCost = Int32.Parse(xml.SelectSingleNode("cost").InnerText);

            Order = Int32.Parse(xml.SelectSingleNode("order").InnerText);
            
            Elements = GetElements(xml.SelectNodes("elements/element")).ToArray();
            Statuses = GetStatusChanges(xml.SelectNodes("statusChange")).ToArray();
            
            Power = Int32.Parse(xml.SelectSingleNode("power").InnerText);
            Hitp = Int32.Parse(xml.SelectSingleNode("hitp").InnerText);

            XmlNode formulaNode = xml.SelectSingleNode("formula");

            if (formulaNode != null)
            {
                DamageFormula = (DamageFormula)Delegate.CreateDelegate(typeof(DamageFormula), this, formulaNode.InnerText);
            }
            else
            {
                DamageFormula = MagicalAttack;
            }
        }
        
        private int MagicalAttack(Combatant source, Combatant target, SpellModifiers modifiers)
        {                        
            int bd = Formula.MagicalBase(source);
            int dam = Formula.MagicalDamage(bd, Power, target);
            
            dam = Formula.RunMagicModifiers(dam, target, Elements, modifiers);
            
            return dam;
        }
        
        private int Cure(Combatant source, Combatant target, SpellModifiers modifiers)
        {
            int bd = Formula.MagicalBase(source);
            int dam = bd + 22 * Power;
            
            dam = Formula.RunCureModifiers(dam, target, Elements, modifiers);
            
            return dam;
        }
        
        private int HPPercent(Combatant source, Combatant target, SpellModifiers modifiers)
        {
            int dam = target.HP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        private int MPPercent(Combatant source, Combatant target, SpellModifiers modifiers)
        {
            int dam = target.MP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        private int MaxHPPercent(Combatant source, Combatant target, SpellModifiers modifiers)
        {
            int dam = target.MaxHP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        private int MaxMPPercent(Combatant source, Combatant target, SpellModifiers modifiers)
        {
            int dam = target.MaxMP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }

        private IEnumerable<Element> GetElements(XmlNodeList elementNodes)
        {
            foreach (XmlNode node in elementNodes)
            {
                yield return (Element)Enum.Parse(typeof(Element), node.InnerText);
            }
        }
        
        private IEnumerable<StatusChange> GetStatusChanges(XmlNodeList inflictionNodes)
        {
            foreach (XmlNode node in inflictionNodes)
            {
                StatusChange change = new StatusChange
                {
                    Odds = Int32.Parse(node.SelectSingleNode("@odds").Value),
                    ChangeType = (StatusChange.Effect)Enum.Parse(typeof(StatusChange.Effect), node.SelectSingleNode("@type").Value),
                    Statuses = node.SelectSingleNode("@statuses").Value.Split(',').Select(s => (Status)Enum.Parse(typeof(Status), s))
                };

                yield return change;
            }
        }
        
        public static Spell GetMagicSpell(string id)
        {
            return _magicSpells[id];
        }

        public static IEnumerable<Spell> GetMagicSpells()
        {
            return _magicSpells.Values.ToList();
        }
        
        public static Spell GetSummonSpell(string id)
        {
            return _summonSpells[id];
        }
        
        public static Spell GetEnemySkillSpell(string id)
        {
            return _enemySkillSpells[id];
        }
        
        public static int Compare(Spell left, Spell right)
        {
            return left.Order.CompareTo(right.Order);
        }
        
        public void Cast(Combatant source, IEnumerable<Combatant> targets, SpellModifiers modifiers)
        {
            foreach (Combatant target in targets)
            {
                if (Formula.MagicHit(source, target, Hitp, Elements))
                {       
                    if (Power > 0)
                    {
                        int dam = DamageFormula(source, target, modifiers);
                        
                        target.AcceptDamage(dam, Type);
                    }
                    
                    foreach (StatusChange statusChange in Statuses)
                    {
                        if (Formula.StatusHit(source, target, statusChange.Odds, statusChange.Statuses, modifiers))
                        {
                            switch (statusChange.ChangeType)
                            {
                                case StatusChange.Effect.Cure:

                                    foreach (Status status in statusChange.Statuses)
                                    {
                                        Console.WriteLine(status);
                                        target.GetType().GetMethod("Cure" + status).Invoke(target, new object[0]);
                                    }

                                    break;

                                case StatusChange.Effect.Inflict:

                                    foreach (Status status in statusChange.Statuses)
                                    {
                                        Console.WriteLine(status);
                                        
                                        target.GetType().GetMethod("Inflict" + status).Invoke(target, new object[0]);
                                    }

                                    break;

                                case StatusChange.Effect.Toggle:

                                    throw new NotImplementedException("Haven't implemented status change toggle");
                            }
                        }
                    }       
                }
                else
                {
                    Seven.BattleState.AddMissIcon(target);
                }
            }
        }
        
        public string Name { get; private set; }

        public string Desc { get; private set; }

        public string ID { get; private set; }

        public AttackType Type { get; private set; }

        public BattleTarget Target { get; private set; }

        public bool TargetEnemiesFirst { get; private set; }

        public bool CanBeAlled { get; private  set; }

        public int MPCost { get; private set; }
        
        public int Order { get; private set; }

        private DamageFormula DamageFormula { get; set; }
        
        private int Power { get; set; }

        private int Hitp { get; set; }
        
        private IEnumerable<Element> Elements { get; set; }

        private IEnumerable<StatusChange> Statuses { get; set; }


        public static int MagicSpellCount { get { return _magicSpells.Count; } }
    }
}

