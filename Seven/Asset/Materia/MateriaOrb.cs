using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

using Cairo;
using NLua;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
	internal class MateriaOrb
    {
        private const string ATTACH_FUNCTION_FORMAT = "return function (c, l) {0} end";
        private const string DETACH_FUNCTION_FORMAT = ATTACH_FUNCTION_FORMAT;

        private static readonly Color ORB_COLOR_MAGIC = new Color(0, .7, .05);
        private static readonly Color ORB_COLOR_SUPPORT = new Color(.0, .6, .8);
        private static readonly Color ORB_COLOR_COMMAND = new Color(.8, .8, .2);
        private static readonly Color ORB_COLOR_INDEPENDENT = new Color(.8, .0, .8);
        private static readonly Color ORB_COLOR_SUMMON = new Color(.9, 0, 0);

        
        private string[] abilities;






        protected MateriaOrb()
        {            
        }

        public MateriaOrb(XmlNode node, Lua lua)
            : this()
        {
            Name = node.SelectSingleNode("name").InnerText;
            Description = node.SelectSingleNode("desc").InnerText;

            
            HPMod = node.SelectSingleNode("hpp") != null ? Int32.Parse(node.SelectSingleNode("hpp").InnerText) : 0;
            MPMod = node.SelectSingleNode("mpp") != null ? Int32.Parse(node.SelectSingleNode("mpp").InnerText) : 0;
            
            StrengthMod = node.SelectSingleNode("str") != null ? Int32.Parse(node.SelectSingleNode("str").InnerText) : 0;
            VitalityMod = node.SelectSingleNode("vit") != null ? Int32.Parse(node.SelectSingleNode("vit").InnerText) : 0;
            DexterityMod = node.SelectSingleNode("dex") != null ? Int32.Parse(node.SelectSingleNode("dex").InnerText) : 0;
            MagicMod = node.SelectSingleNode("mag") != null ? Int32.Parse(node.SelectSingleNode("mag").InnerText) : 0;
            SpiritMod = node.SelectSingleNode("spr") != null ? Int32.Parse(node.SelectSingleNode("spr").InnerText) : 0;
            LuckMod = node.SelectSingleNode("lck") != null ? Int32.Parse(node.SelectSingleNode("lck").InnerText) : 0;
            
            Type = (MateriaType)Enum.Parse(typeof(MateriaType), node.SelectSingleNode("type").InnerText);
            
            XmlNodeList tlist = node.SelectNodes("tiers/tier");
            Tiers = new int[tlist.Count];
            int t = 0;
            
            foreach (XmlNode child in tlist)
            {
                Tiers[t] = Int32.Parse(child.InnerText);
                t++;
            }
            
            Order = Int32.Parse(node.SelectSingleNode("order").InnerText);
            
            abilities = node.SelectSingleNode("abilities").InnerText.Split(new char[] { ',' }, StringSplitOptions.None);
            
            
            XmlNode attachNode = node.SelectSingleNode("attach");
            
            if (attachNode != null)
            {
                string attach = node.SelectSingleNode("attach").InnerText;
                string attachFunction = String.Format(ATTACH_FUNCTION_FORMAT, attach);
                
                try
                {
                    DoAttach = (LuaFunction)lua.DoString(attachFunction).First();
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in accessory attach script; name = " + Name, e);
                }
            }
            
            XmlNode detachNode = node.SelectSingleNode("detach");
            
            if (detachNode != null)
            {
                string detach = node.SelectSingleNode("detach").InnerText;
                string detachFunction = String.Format(DETACH_FUNCTION_FORMAT, detach);
                
                try
                {
                    DoDetach = (LuaFunction)lua.DoString(detachFunction).First();
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in accessory detach script; name = " + Name, e);
                }
            }
        }






        public static int CompareByType(MateriaOrb left, MateriaOrb right)
        {
            // Why aren't we seeing nulls?

            int comparison;

            if (left == null && right == null)
            {
                comparison = 0;
            }
            else if (left == null)
            {
                comparison =  1;
            }
            else if (right == null)
            {
                comparison =  -1;
            }
            else
            {
                comparison = left.TypeOrder.CompareTo(right.TypeOrder);
            }

            return comparison;
        }

        public static int CompareByOrder(MateriaOrb left, MateriaOrb right)
        {
            int comparison = 0;

            if (left == null)
            {
                if (right != null)
                {
                    comparison = 1;
                }
                // else, still 0
            }
            else
            {
                if (right == null)
                {
                    comparison = -1;
                }

                comparison = left.Order.CompareTo(right.Order);

                if (comparison == 0)
                {
                    comparison =  right.AP.CompareTo(left.AP);
                }
            }

            return comparison;
        }
                
        public virtual void AddAP(int delta)
        {
            AP += delta;

            // check for master
            //   master materia still builds ap at an unbounded rate
            //   but cannot level up any more
            if (Master) return;

            while (AP >= Tiers[Level + 1])
            {
                LevelUp();

                // check for master
                //   master materia still builds ap at an unbounded rate
                //   but cannot level up any more
                if (Master) return;
            }
        }

        private void LevelUp()
        {
            Level++;
        }


        public virtual void Attach(Character c)
        {
            c.StrengthBonus += StrengthMod;
            c.VitalityBonus += VitalityMod;
            c.DexterityBonus += DexterityMod;
            c.MagicBonus += MagicMod;
            c.SpiritBonus += SpiritMod;
            c.LuckBonus += LuckMod;

            if (DoAttach != null)
            {
                try
                {
                    DoAttach.Call(c, Level + 1);
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error calling materia attach script; name = " + Name, e);
                }
            }
        }
        public virtual void Detach(Character c)
        {
            c.StrengthBonus -= StrengthMod;
            c.VitalityBonus -= VitalityMod;
            c.DexterityBonus -= DexterityMod;
            c.MagicBonus -= MagicMod;
            c.SpiritBonus -= SpiritMod;
            c.LuckBonus -= LuckMod;
                     
            if (DoDetach != null)
            {
                try
                {
                    DoDetach.Call(c, Level + 1);
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error calling materia detach script; name = " + Name, e);
                }
            }
        }


        public Cairo.Color Color
        {
            get
            {
                switch (Type)
                {
                    case MateriaType.Magic:
                        return ORB_COLOR_MAGIC;
                    case MateriaType.Support:
                        return ORB_COLOR_SUPPORT;
                    case MateriaType.Command:
                        return ORB_COLOR_COMMAND;
                    case MateriaType.Independent:
                        return ORB_COLOR_INDEPENDENT;
                    case MateriaType.Summon:
                        return ORB_COLOR_SUPPORT;
                    default:
                        throw new GameDataException("Materia has no type, name " + Name);
                }
            }
        }

        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public int AP { get; protected set; }
        public int Level { get; private set; }
        public int[] Tiers { get; protected set; }
        public bool Master { get { return Level == Tiers.Length - 1; } }

        public int StrengthMod { get; protected set; }
        public int VitalityMod { get; protected set; }
        public int DexterityMod { get; protected set; }
        public int MagicMod { get; protected set; }
        public int SpiritMod { get; protected set; }
        public int LuckMod { get; protected set; }
        public int HPMod { get; protected set; }
        public int MPMod { get; protected set; }

        protected int Order { get; set; }

        public MateriaType Type { get; protected set; }

        
        public LuaFunction DoAttach { get; private set; }
        
        public LuaFunction DoDetach { get; private set; }

        public virtual IEnumerable<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();

                switch (Type)
                {
                    case MateriaType.Magic:                        
                    case MateriaType.Independent:
                        abilities.AddRange(AbilityDescriptions.TakeWhile((s, i) => i <= Level));
                        break;

                    case MateriaType.Support:
                    case MateriaType.Summon:
                        abilities.Add(AbilityDescriptions.First());
                        break;

                    case MateriaType.Command:
                        abilities.Add(AbilityDescriptions.ToArray()[Level >= AbilityDescriptions.Count() ? Level - 1 : Level]);
                        break;
                }

                return abilities;
            }
        }

        public virtual IEnumerable<string> AbilityDescriptions { get { return abilities; } }

        private int TypeOrder { get { return (int)Type; } }
	}



}
