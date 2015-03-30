using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Spell
    {        
        private delegate void Action(Combatant source, IEnumerable<Combatant> targets, SpellModifiers modifiers);

        internal enum FormulaType
        {
            MagicalAttack,
        }
        
        private static Dictionary<string, Spell> _table;


        
        public static void LoadSpells()
        {
            _table = new Dictionary<string, Spell>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.spells.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("/spells/spell"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }

                Spell spell = new Spell(node);
                
                _table.Add(spell.ID, spell);
            }
            
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




            SetResolver(xml.SelectSingleNode("formula"));
        }




        private void SetResolver(XmlNode formulaNode)
        {
            FormulaType type = (FormulaType)Enum.Parse(typeof(FormulaType), formulaNode.SelectSingleNode("@type").Value);

            switch (type)
            {
                case FormulaType.MagicalAttack:

                    /*
                     * 
                            <formula type="MagicalAttack">
                                <power>8</power>
                                <hitp>100</hitp>
                                <cost>4</cost>
                                <order>9</order>
                                <elements>
                                    <element>Fire</element>
                                </elements>
                            </formula>
                     */

                    int power = Int32.Parse(formulaNode.SelectSingleNode("power").InnerText);
                    int hitp = Int32.Parse(formulaNode.SelectSingleNode("hitp").InnerText);
                    
                    IEnumerable<Element> elements = GetElements(formulaNode.SelectNodes("elements/element"));

                    Resolve = delegate (Combatant source, IEnumerable<Combatant> targets, SpellModifiers modifiers)
                    {
                        foreach (Combatant target in targets)
                        {
                            if (Formula.MagicHit(source, target, hitp, elements))
                            {            
                                bool restorative = false;
                                
                                int bd = Formula.MagicalBase(source);
                                int dam = Formula.MagicalDamage(bd, power, target);
                                
                                dam = Formula.RunMagicModifiers(dam, ref restorative, target, elements, modifiers);
                                
                                if (restorative)
                                {
                                    dam = -dam;
                                }
                                
                                target.AcceptDamage(dam, AttackType.Magical);
                            }
                            else
                            {
                                Seven.BattleState.AddMissIcon(target);
                            }
                        }
                    };
                    break;
            }
        }

        private IEnumerable<Element> GetElements(XmlNodeList elementNodes)
        {
            Element[] elements = new Element[elementNodes.Count];
            
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = (Element)Enum.Parse(typeof(Element), elementNodes[i].InnerText);
            }
            
            return elements;
        }

          



        public static Spell Get(string id)
        {
            return _table[id];
        }





        
        public static int Compare(Spell left, Spell right)
        {
            return left.Order.CompareTo(right.Order);
        }



        
        public void Cast(Combatant source, IEnumerable<Combatant> targets, SpellModifiers modifiers)
        {
            Resolve(source, targets, modifiers);
        }
        
        
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public string ID { get; private set; }  
        public AttackType Type { get; private set; }        
        public BattleTarget Target { get; private set; }        
        public bool TargetEnemiesFirst { get; private set; }       
        public bool CanBeAlled { get;private  set; }        
        public int MPCost { get; private set; }
        
        public int Order { get; private set; }

        private Action Resolve { get; set; }
    }
}

