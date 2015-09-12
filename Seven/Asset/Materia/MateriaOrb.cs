using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using NLua;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
	internal abstract class MateriaOrb
    {
        private const string ATTACH_FUNCTION_FORMAT = "return function (c, l) {0} end";
        private const string DETACH_FUNCTION_FORMAT = ATTACH_FUNCTION_FORMAT;

        private static readonly string ATTACH_FUNCTION_EMPTY = String.Format(ATTACH_FUNCTION_FORMAT, String.Empty);
        private static readonly string DETACH_FUNCTION_EMPTY = String.Format(DETACH_FUNCTION_FORMAT, String.Empty);

        #region Nested

        private class MateriaDefinition
        {
            public int hpp;
            public int mpp;
            public int str;
            public int vit;
            public int dex;
            public int mag;
            public int spr;
            public int lck;
            public int order;

            public MateriaType type;

            public int[] tiers;
            public string[] abilities;
            
            private MateriaDefinition() 
            {
                Name = String.Empty;
                Desc = String.Empty;
                
                tiers = new int[1];
                tiers[0] = 0;
                
                abilities = new string[0];
                
                DoAttach = (LuaFunction)Seven.Lua.DoString(ATTACH_FUNCTION_EMPTY).First();
                DoDetach = (LuaFunction)Seven.Lua.DoString(DETACH_FUNCTION_EMPTY).First();
            }
            
            public MateriaDefinition(string name, string desc, string ability) 
                : this()
            {
                Name = name;
                Desc = desc;
                ID = Resource.CreateID(Name);
                abilities = new string[] { ability };
            }

            public MateriaDefinition(XmlNode node)
                : this()
            {
                Name = node.SelectSingleNode("name").InnerText;
                Desc = node.SelectSingleNode("desc").InnerText;

                ID = Resource.CreateID(Name);

                hpp = node.SelectSingleNode("hpp") != null ? Int32.Parse(node.SelectSingleNode("hpp").InnerText) : 0;
                mpp = node.SelectSingleNode("mpp") != null ? Int32.Parse(node.SelectSingleNode("mpp").InnerText) : 0;

                str = node.SelectSingleNode("str") != null ? Int32.Parse(node.SelectSingleNode("str").InnerText) : 0;
                vit = node.SelectSingleNode("vit") != null ? Int32.Parse(node.SelectSingleNode("vit").InnerText) : 0;
                dex = node.SelectSingleNode("dex") != null ? Int32.Parse(node.SelectSingleNode("dex").InnerText) : 0;
                mag = node.SelectSingleNode("mag") != null ? Int32.Parse(node.SelectSingleNode("mag").InnerText) : 0;
                spr = node.SelectSingleNode("spr") != null ? Int32.Parse(node.SelectSingleNode("spr").InnerText) : 0;
                lck = node.SelectSingleNode("lck") != null ? Int32.Parse(node.SelectSingleNode("lck").InnerText) : 0;

                type = (MateriaType)Enum.Parse(typeof(MateriaType), node.SelectSingleNode("type").InnerText);

                XmlNodeList tlist = node.SelectNodes("tiers/tier");
                tiers = new int[tlist.Count];
                int t = 0;

                foreach (XmlNode child in tlist)
                {
                    tiers[t] = Int32.Parse(child.InnerText);
                    t++;
                }

                order = Int32.Parse(node.SelectSingleNode("order").InnerText);

                abilities = node.SelectSingleNode("abilities").InnerText.Split(new char[] { ',' }, StringSplitOptions.None);
                
                
                XmlNode attachNode = node.SelectSingleNode("attach");
                
                if (attachNode != null)
                {
                    string attach = node.SelectSingleNode("attach").InnerText;
                    string attachFunction = String.Format(ATTACH_FUNCTION_FORMAT, attach);
                    
                    try
                    {
                        DoAttach = (LuaFunction)Seven.Lua.DoString(attachFunction).First();
                    }
                    catch (Exception e)
                    {
                        throw new ImplementationException("Error in accessory attach script; id = " + ID, e);
                    }
                }
                
                XmlNode detachNode = node.SelectSingleNode("detach");
                
                if (detachNode != null)
                {
                    string detach = node.SelectSingleNode("detach").InnerText;
                    string detachFunction = String.Format(DETACH_FUNCTION_FORMAT, detach);
                    
                    try
                    {
                        DoDetach = (LuaFunction)Seven.Lua.DoString(detachFunction).First();
                    }
                    catch (Exception e)
                    {
                        throw new ImplementationException("Error in accessory detach script; id = " + ID, e);
                    }
                }
            }

            public string Name { get; private set; }
            public string ID { get; private set; }
            public string Desc { get; private set; }
                     
            
            public LuaFunction DoAttach { get; private set; }
            
            public LuaFunction DoDetach { get; private set; }

        }
        
        
        private class MasterMagicMateria : MagicMateria
        {        
            internal static readonly MateriaOrb.MateriaDefinition MasterMagic;
            
            static MasterMagicMateria()
            {
                MasterMagic = new MateriaOrb.MateriaDefinition("Master Magic", "Equips all magical spells", "All Magic");
            }
            
            public MasterMagicMateria() : base(MasterMagic.ID, 0) { }
            
            
            public override List<Spell> GetSpells
            {
                get
                {
                    List<Spell> sp = new List<Spell>();
                    
                    foreach (Spell s in Spell.GetMagicSpells())
                    {
                        sp.Add(s);
                    }
                    
                    return sp;
                }
            }
        }
        
        
        private class MasterSummonMateria : SummonMateria
        {        
            internal static readonly MateriaOrb.MateriaDefinition MasterMagic;
            
            static MasterSummonMateria()
            {
                MasterMagic = new MateriaOrb.MateriaDefinition("Master Summon", "Equips all summons", "All Summons");
            }
            
            public MasterSummonMateria() : base(MasterMagic.ID, 0) { }
            
            
            public override List<Spell> GetSpells
            {
                get
                {
                    List<Spell> sp = new List<Spell>();
                    
                    foreach (Spell s in Spell.GetSummonSpells())
                    {
                        sp.Add(s);
                    }
                    
                    return sp;
                }
            }
        }

        #endregion




        private static Dictionary<string, MateriaDefinition> _data;





        
        static MateriaOrb()
        {
            _data = new Dictionary<string, MateriaDefinition>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.materia.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//materiadata/materia"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                MateriaDefinition rec = new MateriaDefinition(node);
                
                _data.Add(rec.ID, rec);
            }
            
            _data.Add(MasterMagicMateria.MasterMagic.ID, MasterMagicMateria.MasterMagic);
        }


        private MateriaOrb()
        {
        }
                
        protected MateriaOrb(string name, int ap)
        {
            Record = _data[name];

            Level = 0;
            AP = 0;

            AddAP(ap);
        }



        public static MateriaOrb Create(string id, int ap, MateriaType type)
        {
            MateriaOrb materia = null;

            if (id == "enemyskill")
            {
                materia = new EnemySkillMateria(ap);
            }
            else if (id == MasterMagicMateria.MasterMagic.ID)
            {
                materia = new MasterMagicMateria();
            }
            else
            {
                switch (type)
                {
                    case MateriaType.Magic:
                        return new MagicMateria(id, ap);
                    case MateriaType.Support:
                        return new SupportMateria(id, ap);
                    case MateriaType.Command:
                        return new CommandMateria(id, ap);
                    case MateriaType.Independent:
                        return new IndependentMateria(id, ap);
                    case MateriaType.Summon:
                        return new SummonMateria(id, ap);
                }
            }

            return materia;
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
            
            try
            {
                Record.DoAttach.Call(c, Level + 1);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error calling materia attach script; id = " + ID, e);
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
                                    
            try
            {
                Record.DoDetach.Call(c, Level + 1);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error calling materia detach script; id = " + ID, e);
            }
        }


        public abstract Cairo.Color Color { get; }

        public string Name { get { return Record.Name;} }
        public string Description { get { return Record.Desc; } }
        public string ID { get { return Record.ID; } }
        public int AP { get; protected set; }
        public int Level { get; private set; }
        public int[] Tiers { get { return Record.tiers; } }
        public bool Master { get { return Level == Tiers.Length - 1; } }

        public int StrengthMod { get { return Record.str; } }
        public int VitalityMod { get { return Record.vit; } }
        public int DexterityMod { get { return Record.dex; } }
        public int MagicMod { get { return Record.mag; } }
        public int SpiritMod { get { return Record.spr; } }
        public int LuckMod { get { return Record.lck; } }
        public int HPMod { get { return Record.hpp; } }
        public int MPMod { get { return Record.mpp; } }

        public int Order { get { return Record.order; } }

        public abstract MateriaType Type { get; }

        public abstract List<string> Abilities { get; }

        public virtual string[] AllAbilities { get { return Record.abilities; } }

        protected abstract int TypeOrder { get; }

        private MateriaDefinition Record { get; set; }
	}



}
