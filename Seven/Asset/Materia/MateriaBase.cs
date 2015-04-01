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
	internal abstract class MateriaBase
    {

        #region Nested

        internal class MateriaRecord
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
            
            private MateriaRecord() 
            {
                Name = String.Empty;
                Desc = String.Empty;
                
                tiers = new int[1];
                tiers[0] = 0;
                
                abilities = new string[0];
                
                DoAttach = (LuaFunction)Seven.Lua.DoString("return function (c, l) end").First();
                DoDetach = (LuaFunction)Seven.Lua.DoString("return function (c, l) end").First();
            }
            
            public MateriaRecord(string name, string desc, string ability) 
                : this()
            {
                Name = name;
                Desc = desc;
                ID = Resource.CreateID(Name);
                abilities = new string[] { ability };
            }

            public MateriaRecord(XmlNode node)
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
                    string attachFunction = String.Format("return function (c, l) {0} end", attach);
                    
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
                    string detachFunction = String.Format("return function (c, l) {0} end", detach);
                    
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

        #endregion



        #region Member Data

        protected int _ap;
        protected int _level;

        protected static Dictionary<string, MateriaRecord> _data;
        private static Dictionary<string, MateriaBase> _masterTable;

        #endregion Member Data




        
        static MateriaBase()
        {
            _masterTable = new Dictionary<string, MateriaBase>();
            _data = new Dictionary<string, MateriaRecord>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.materia.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//materiadata/materia"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                MateriaRecord rec = new MateriaRecord(node);
                
                _data.Add(rec.ID, rec);
                
                int ap = _data[rec.ID].tiers[_data[rec.ID].tiers.Length - 1];
                
                switch (rec.type)
                {
                    case MateriaType.Magic:
                        AddMaster(new MagicMateria(rec.ID, ap));
                        break;
                    case MateriaType.Support:
                        AddMaster(new SupportMateria(rec.ID, ap));
                        break;
                    case MateriaType.Command:
                        AddMaster(new CommandMateria(rec.ID, ap));
                        break;
                    case MateriaType.Independent:
                        AddMaster(new IndependentMateria(rec.ID, ap));
                        break;
                    case MateriaType.Summon:
                        AddMaster(new SummonMateria(rec.ID, ap));
                        break;
                }
            }
            
            _data.Add(MasterMagicMateria.MasterMagic.ID, MasterMagicMateria.MasterMagic);
        }


        private MateriaBase()
        {
        }
                
        protected MateriaBase(string name, int ap)
        {
            Record = _data[name];

            _level = 0;
            _ap = 0;

            AddAP(ap);
        }



        public static MateriaBase Create(string id, int ap, MateriaType type)
        {
            MateriaBase materia = null;

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

        #region Masters


        private static void AddMaster(MateriaBase m)
        {
            _masterTable.Add(m.Name, m);
        }

        public static MateriaBase GetMaster(string name)
        {
            return _masterTable[name];
        }

        #endregion Masters



        public static int CompareByType(MateriaBase left, MateriaBase right)
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

        public static int CompareByOrder(MateriaBase left, MateriaBase right)
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
            _ap += delta;

            // check for master
            //   master materia still builds ap at an unbounded rate
            //   but cannot level up any more
            if (Master) return;

            while (_ap >= Tiers[_level + 1])
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
            _level++;
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
        public string ID { get { return Resource.CreateID(Name); } }
        public int AP { get { return _ap; } }
        public int Level { get { return _level; } }
        public int[] Tiers { get { return Record.tiers; } }
        public bool Master { get { return _level == Tiers.Length - 1; } }

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

        private MateriaRecord Record { get; set; }
	}



}
