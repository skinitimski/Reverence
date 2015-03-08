using System;
using System.Collections.Generic;
using System.Xml;
using System.Text;
using System.IO;
using System.Reflection;
using NLua;


namespace Atmosphere.BattleSimulator
{

	public abstract class Materia
    {

        #region Nested
        public struct MateriaRecord
        {
            private string _name;
            private string _desc;

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

            public MateriaRecord(string xmlstring)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)));

                _name = xml.SelectSingleNode("//name").InnerText;
                _desc = xml.SelectSingleNode("//desc").InnerText;

                hpp = Int32.Parse(xml.SelectSingleNode("//hpp").InnerText);
                mpp = Int32.Parse(xml.SelectSingleNode("//mpp").InnerText);
                str = Int32.Parse(xml.SelectSingleNode("//str").InnerText);
                vit = Int32.Parse(xml.SelectSingleNode("//vit").InnerText);
                dex = Int32.Parse(xml.SelectSingleNode("//dex").InnerText);
                mag = Int32.Parse(xml.SelectSingleNode("//mag").InnerText);
                spr = Int32.Parse(xml.SelectSingleNode("//spr").InnerText);
                lck = Int32.Parse(xml.SelectSingleNode("//lck").InnerText);

                XmlNodeList tlist = xml.SelectSingleNode("//tiers").ChildNodes;
                tiers = new int[tlist.Count];
                int t = 0;

                type = (MateriaType)Enum.Parse(typeof(MateriaType), xml.SelectSingleNode("//type").InnerText);

                foreach (XmlNode node in tlist)
                {
                    if (node.NodeType == XmlNodeType.Comment)
                        continue;

                    tiers[t] = Int32.Parse(node.InnerText);
                    t++;
                }

                order = Int32.Parse(xml.SelectSingleNode("//order").InnerText);

                abilities = xml.SelectSingleNode("//abilities").InnerText.Split(
                    new char[] { ',' }, StringSplitOptions.None);

                XmlNode attachNode = xml.SelectSingleNode("//attach");
                if (attachNode != null)
                    Game.Lua.DoString("attach" + ID + " = " + attachNode.InnerText);
                XmlNode detachNode = xml.SelectSingleNode("//detach");
                if (detachNode != null)
                    Game.Lua.DoString("detach" + ID + " = " + detachNode.InnerText);
            }

            public string Name { get { return _name; } }
            public string ID { get { return Globals.CreateID(_name); } }
            public string Description { get { return _desc; } }

        }
        #endregion



        #region Member Data

        public const string DATAFILE = @"Data\materia.xml";

        protected string _name;
        protected string _desc;

        protected int _ap;
        protected int _level;

        protected int _hpp;
        protected int _mpp;
        protected int _str;
        protected int _vit;
        protected int _dex;
        protected int _mag;
        protected int _spr;
        protected int _lck;

        protected int[] _tiers;

        protected string[] _abilities;

        protected int _order;

        protected static Dictionary<string, MateriaRecord> _data;
        private static Dictionary<string, Materia> _masterTable;

        #endregion Member Data


        
        public Materia(string name, int ap)
        {
            MateriaRecord rec = _data[name];
            _name = rec.Name;
            _desc = rec.Description;
            _hpp = rec.hpp;
            _mpp = rec.mpp;
            _str = rec.str;
            _vit = rec.vit;
            _dex = rec.dex;
            _mag = rec.mag;
            _spr = rec.spr;
            _lck = rec.lck;

            _tiers = rec.tiers;

            _abilities = rec.abilities;

            _order = rec.order;

            _level = 0;
            _ap = 0;

            AddAP(ap);
        }


        public static void Init()
        {
            _masterTable = new Dictionary<string, Materia>();
            _data = new Dictionary<string, MateriaRecord>();

            XmlDocument gamedata = new XmlDocument();
            gamedata.Load(DATAFILE);

            foreach (XmlNode node in gamedata.SelectSingleNode("//materiadata").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                MateriaRecord rec = new MateriaRecord(node.OuterXml);

                _data.Add(Globals.CreateID(rec.Name), rec);

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
        }

        public static Materia Create(string id, int ap, MateriaType type)
        {
            if (id == "enemyskill")
                return new EnemySkillMateria(ap);

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
                default: throw new GameImplementationException("Materia type not supported.");
            }
        }

        #region Masters


        private static void AddMaster(Materia m)
        {
            _masterTable.Add(m.Name, m);
        }

        public static Materia GetMaster(string name)
        {
            return _masterTable[name];
        }

        #endregion Masters



        public static int CompareByType(Materia left, Materia right)
        {
            if (left == null && right == null)
                return 0;
            else if (left == null) return 1;
            else if (right == null) return -1;
            else return left.TypeOrder.CompareTo(right.TypeOrder);
        }

        public static int CompareByOrder(Materia left, Materia right)
        {
            if (left == null && right == null)
            {
                return 0;
            }
            else if (left == null)
            {
                return 1;
            }
            else if (right == null)
            {
                return -1;
            }
            else
            {
                int comp = left._order.CompareTo(right._order);
                if (comp == 0)
                {
                    return 0 - left.AP.CompareTo(right.AP);
                }
                else
                {
                    return comp;
                }
            }
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
            c.StrengthBonus += _str;
            c.VitalityBonus += _vit;
            c.DexterityBonus += _dex;
            c.MagicBonus += _mag;
            c.SpiritBonus += _spr;
            c.LuckBonus += _lck;

            LuaFunction f = Game.Lua.GetFunction("attach" + ID);

            if (f != null) f.Call(c, Level + 1);
        }
        public virtual void Detach(Character c)
        {
            c.StrengthBonus -= _str;
            c.VitalityBonus -= _vit;
            c.DexterityBonus -= _dex;
            c.MagicBonus -= _mag;
            c.SpiritBonus -= _spr;
            c.LuckBonus -= _lck;

            LuaFunction f = Game.Lua.GetFunction("detach" + ID);

            if (f != null) f.Call(c, Level + 1);
        }


        public abstract Cairo.Color Color { get; }

        public string Name { get { return _name;} }
        public string Description { get { return _desc; } }
        public string ID { get { return Globals.CreateID(_name); } }
        public int AP { get { return _ap; } }
        public int Level { get { return _level; } }
        public int[] Tiers { get { return _tiers; } }
        public bool Master { get { return _level == Tiers.Length - 1; } }

        public virtual int StrengthMod { get { return _str; } }
        public virtual int VitalityMod { get { return _vit; } }
        public virtual int DexterityMod { get { return _dex; } }
        public virtual int MagicMod { get { return _mag; } }
        public virtual int SpiritMod { get { return _spr; } }
        public virtual int LuckMod { get { return _lck; } }
        public virtual int HPMod { get { return _hpp; } }
        public virtual int MPMod { get { return _mpp; } }

        public abstract MateriaType Type { get; }

        public abstract List<string> Abilities { get; }
        public virtual string[] AllAbilities { get { return _abilities; } }

        protected abstract int TypeOrder { get; }
	}



}
