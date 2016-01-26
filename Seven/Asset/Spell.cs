using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Thread = System.Threading.Thread;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Spell : Ability
    {
        protected Spell(XmlNode xml)
            : base()
        {
            Name = xml.SelectSingleNode("name").InnerText;
            Desc = xml.SelectSingleNode("desc").InnerText;
            
            Type = (AttackType)Enum.Parse(typeof(AttackType), xml.SelectSingleNode("type").InnerText);
            Target = (BattleTarget)Enum.Parse(typeof(BattleTarget), xml.SelectSingleNode("target").InnerText);
            
            XmlNode targetsFirstNode = xml.SelectSingleNode("targetEnemiesFirst");
            TargetEnemiesFirst = targetsFirstNode == null ? true : Boolean.Parse(targetsFirstNode.InnerText);
            
            XmlNode costNode = xml.SelectSingleNode("cost");            
            MPCost = costNode == null ? 0 : Int32.Parse(costNode.InnerText);
            
            XmlNode orderNode = xml.SelectSingleNode("order");
            Order = orderNode == null ? 0 : Int32.Parse(orderNode.InnerText);
            
            Elements = GetElements(xml.SelectNodes("elements/element")).ToArray();
            Statuses = GetStatusChanges(xml.SelectNodes("statusChange")).ToArray();
            
            Power = Int32.Parse(xml.SelectSingleNode("power").InnerText);
            Hitp = Int32.Parse(xml.SelectSingleNode("hitp").InnerText);
            
            XmlNode hitsNode = xml.SelectSingleNode("hits");
            Hits = hitsNode == null ? 1 : Int32.Parse(hitsNode.InnerText);

            DamageFormula = GetFormula(xml.SelectSingleNode("formula"));
        }



        
        public static Dictionary<string, Spell> LoadSpells(string resource)
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
                
                table.Add(spell.Name, spell);
            } 
            
            return table;
        }




        protected override string GetMessage(Combatant source)
        {
            return source.Name + " casts " + Name;
        }
        
        public static int Compare(Spell left, Spell right)
        {
            return left.Order.CompareTo(right.Order);
        }


        
        
        /// <summary>
        /// This value is used to determine whether or not the ability, when selected by 
        /// the player through a battle menu, will target the source's opponents first.
        /// This of course has no effect if the target doesn't peel over many brudish vowels.
        /// </summary>
        public bool TargetEnemiesFirst { get; private set; }
        
        /// ???
        public bool CanBeAlled { get; private set; }
        
        /// ???
        public int Order { get; private set; }
    }
}

