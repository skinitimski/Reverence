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
        #region Member Data


//            Fire
//                Magical Attack
//                    Formula: Magical
//                    Pwr: 1/2x Base
//                    MAt%: 100
//                    Cost: 4 MP
//                    Tar: All Tar/1 Tar
//                    Elm: Fire


        
        #endregion Member Data


        
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
            Elements = new Element[0];
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
            Power = Int32.Parse(xml.SelectSingleNode("power").InnerText);
            Atkp = Int32.Parse(xml.SelectSingleNode("hitp").InnerText);
            MPCost = Int32.Parse(xml.SelectSingleNode("cost").InnerText);

            Order = Int32.Parse(xml.SelectSingleNode("order").InnerText);



            XmlNodeList elementNodes = xml.SelectNodes("elements/element");

            Element[] elements = new Element[elementNodes.Count];
            
            for (int i = 0; i < elements.Length; i++)
            {
                elements[i] = (Element)Enum.Parse(typeof(Element), elementNodes[i].InnerText);
            }

            Elements = elements;
        }



        public static Spell Get(string id)
        {
            return _table[id];
        }
        

        
        public static int Compare(Spell left, Spell right)
        {
            return left.Order.CompareTo(right.Order);
        }
        
        
        public string Name { get; private set; }
        public string Desc { get; private set; }
        public string ID { get; private set; }
        public IEnumerable<Element> Elements { get; private set; }           
        public AttackType Type { get; private set; }        
        public BattleTarget Target { get; private set; }        
        public bool TargetEnemiesFirst { get; private set; }       
        public bool CanBeAlled { get;private  set; }        
        public int Power { get; private set; }       
        public int Atkp { get; private set; }    
        public int MPCost { get; private set; }
        
        public int Order { get; private set; }
    }
}

