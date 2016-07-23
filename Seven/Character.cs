using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Reflection;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven
{
    internal class Character
    {
        private static readonly Random RANDOM = new Random();

        public const int MAX_HP = 9999;
        public const int MAX_MP = 999;

        public const int MAX_LEVEL = 99;
        public const int MIN_LEVEL = 1;

        public const int PROFILE_WIDTH = 120;
        public const int PROFILE_HEIGHT = 136;
        public const int PROFILE_WIDTH_SMALL = 89;
        public const int PROFILE_HEIGHT_SMALL = 101;
        public const int PROFILE_WIDTH_TINY = 36;
        public const int PROFILE_HEIGHT_TINY = 41;


        
        private int _level;
        private int _strength_base;
        private int _dexterity_base;
        private int _vitality_base;
        private int _magic_base;
        private int _spirit_base;
        private int _luck_base;
        private Weapon _weapon;
        private Armor _armor;
        private Accessory _accessory;
        private int _hp;
        private int _mp;
        private int _maxhp;
        private int _maxmp;
        private int _limitlvl;
        private List<Element> _halve;
        private List<Element> _void;
        private List<Element> _absorb;
        private List<Status> _immune;
        private Sex _sex;
        private int _exp;
        
        /// <summary>Quadratic modifiers for levelling up, indexed by level bracket.</summary>
        private int[] _qvals;
        /// <summary>Luck Gradient values, indexed by level bracket.</summary>
        private int[] _lck_gradient;
        /// <summary>Luck Base values, indexed by level bracket.</summary>
        private int[] _lck_base;
        /// <summary>HP Gradient values, indexed by level bracket.</summary>
        private int[] _hp_gradient;
        /// <summary>HP Base values, indexed by level bracket.</summary>
        private int[] _hp_base;
        /// <summary>MP Gradient values, indexed by level bracket.</summary>
        private int[] _mp_gradient;
        /// <summary>MP Base values, indexed by level bracket.</summary>
        private int[] _mp_base;
        /// <summary>Rank values for stat gains, indexed as follows: str(0), vit(1), mag(2), spi(3), dex(4).</summary>
        private int[] _stat_ranks;




        internal class Metrics
        {        
            /// <summary>Aggregate for stat gradient arrays. Indexed by level bracket.</summary>
            private List<int[]> _stat_gradient;
            /// <summary>Aggregate for stat base arrays. Indexed by level bracket.</summary>
            private List<int[]> _stat_base;

            
            
            /// <summary>
            /// Initializes tables used in stat gains
            /// </summary>
            public Metrics(Assembly assembly)
            {
                using (StreamReader reader = Resource.GetStreamReaderFromResource("data.diffs.table", assembly))
                {
                    reader.ReadLine(); // [stat_gradient]

                    _stat_gradient = new List<int[]>
                    {
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine())
                    };
                    
                    reader.ReadLine(); // [stat_base]

                    _stat_base = new List<int[]>
                    {
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine()),
                        InitTable(reader.ReadLine())
                    };
                

                    reader.ReadLine(); // [diff_gain]

                    DiffGain = InitTable(reader.ReadLine());

                    reader.ReadLine(); // [diff_gain_hp]

                    DiffGainHP = InitTable(reader.ReadLine());

                    reader.ReadLine(); // [diff_gain_mp]

                    DiffGainMP = InitTable(reader.ReadLine());
                }
            }


            
            /// <summary>
            /// Gets the "base" stat for calculating stat gains on level ups.
            /// </summary>
            /// <param name="level">Level you want the base modifier for.</param>
            /// <param name="rank">Rank of the modifier (0-29 incl).</param>
            /// <remarks>Applies to Strength, Vitality, Magic, Spirit, and Dexterity.</remarks>
            /// <returns>The "base" modifier for a given rank and level.</returns>
            public int StatBase(int level, int rank)
            {
                return _stat_base[LevelBracket(level)][rank];
            }
            
            /// <summary>
            /// Gets the "gradient" for calculating stat gains on level ups.
            /// </summary>
            /// <param name="level">Level you want the base modifier for.</param>
            /// <param name="rank">Rank of the modifier (0-29 incl).</param>
            /// <remarks>Applies to Strength, Vitality, Magic, Spirit, and Dexterity.</remarks>
            /// <returns>The "gradient" modifier for a given rank and level.</returns>
            public int StatGradient(int level, int rank)
            {
                return _stat_gradient[LevelBracket(level)][rank];
            }
            
            /// <summary>Lookup for stat gains, indexed by difference.</summary>
            public int[] DiffGain { get; private set; }
            /// <summary>Lookup for HP percent gains, indexed by difference.</summary>
            public int[] DiffGainHP { get; private set; }
            /// <summary>Lookup for MP percent gains, indexed by difference.</summary>
            public int[] DiffGainMP { get; private set; }
        }
        
        /// <summary>
        /// Populates an array with the values in given line, splitting on sep.
        /// </summary>
        /// <param name="table">Array to populate</param>
        /// <param name="line">Data to put into the array</param>
        /// <param name="sep">Used to split the line parameter into array parts</param>
        protected static int[] InitTable(string line)
        {
            return line.Split(new char[] { '\t', ' ', ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => Int32.Parse(x)).ToArray();
        }





        private Character()
        {
        }

        /// <summary>
        /// Creates a character from scratch with initial stats/equipment.
        /// </summary>
        /// <param name="dataxmlstring"></param>
        public Character(XmlNode dataxml, DataStore data)
            : this(dataxml, dataxml, data)
        {
        }
        
        /// <summary>
        /// Loads a saved character.
        /// </summary>
        /// <param name="savexmlstring"></param>
        /// <param name="dataxmlstring"></param>
        public Character(XmlNode savexml, XmlNode dataxml, DataStore data)
            : this()
        {
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();

                       
            Name = savexml.Name;

            
            // Stats
            _strength_base = Int32.Parse(savexml.SelectSingleNode("./stats/str").InnerText);
            _dexterity_base = Int32.Parse(savexml.SelectSingleNode("./stats/dex").InnerText);
            _vitality_base = Int32.Parse(savexml.SelectSingleNode("./stats/vit").InnerText);
            _magic_base = Int32.Parse(savexml.SelectSingleNode("./stats/mag").InnerText);
            _spirit_base = Int32.Parse(savexml.SelectSingleNode("./stats/spi").InnerText);
            _luck_base = Int32.Parse(savexml.SelectSingleNode("./stats/lck").InnerText);
            _level = Int32.Parse(savexml.SelectSingleNode("./stats/lvl").InnerText);
            
            // Experience and Level
            _exp = Int32.Parse(savexml.SelectSingleNode("./exp").InnerText);
            _limitlvl = Int32.Parse(savexml.SelectSingleNode("./limitlvl").InnerText);
            
            // HP
            _hp = Int32.Parse(savexml.SelectSingleNode("./hp").InnerText);
            _maxhp = Int32.Parse(savexml.SelectSingleNode("./maxhp").InnerText);
            if (_hp > _maxhp)
            {
                throw new SaveStateException("HP > MAXHP for " + Name);
            }
            //_death = (_hp == 0);
            
            // MP
            _mp = Int32.Parse(savexml.SelectSingleNode("./mp").InnerText);
            _maxmp = Int32.Parse(savexml.SelectSingleNode("./maxmp").InnerText);
            if (_mp > _maxmp)
            {
                throw new SaveStateException("MP > MAXMP for " + Name);
            }
            
            // Fury/Sadness
            Sadness = Boolean.Parse(savexml.SelectSingleNode("./sadness").InnerText);
            Fury = Boolean.Parse(savexml.SelectSingleNode("./fury").InnerText);
            if (Sadness && Fury)
            {
                throw new SaveStateException("Can't be both sad and furious");
            }
            
            // Sex
            _sex = (Sex)Enum.Parse(typeof(Sex), dataxml.SelectSingleNode("./sex").InnerText);

            // Back row?
            BackRow = Boolean.Parse(savexml.SelectSingleNode("./backRow").InnerText);
            
            // Equipment
            _weapon = data.GetWeapon(savexml.SelectSingleNode("./weapon/name").InnerText);

            foreach (XmlNode orb in savexml.SelectNodes("./weapon/materia/orb"))
            {
                string id = orb.Attributes["id"].Value;
                int ap = Int32.Parse(orb.Attributes["ap"].Value);
                int slot = Int32.Parse(orb.Attributes["slot"].Value);
                if (slot >= _weapon.Slots.Length)
                {
                    throw new SaveStateException("Materia orb assigned to slot that doesnt exist on weapon.");
                }
                
                _weapon.Slots[slot] = data.GetMateria(id, ap);
            }
            
            _armor = data.GetArmor(savexml.SelectSingleNode("./armor/name").InnerText);

            foreach (XmlNode orb in savexml.SelectNodes("./armor/materia/orb"))
            {
                string id = orb.Attributes["id"].Value;
                int ap = Int32.Parse(orb.Attributes["ap"].Value);
                int slot = Int32.Parse(orb.Attributes["slot"].Value);
                if (slot >= _weapon.Slots.Length)
                {
                    throw new SaveStateException("Materia orb assigned to slot that doesnt exist on armor.");
                }
                
                _armor.Slots[slot] = data.GetMateria(id, ap);
            }
            
            string acc = savexml.SelectSingleNode("./accessory").InnerText;
            _accessory = String.IsNullOrEmpty(acc) ? null : data.GetAccessory(acc);
            
            
            // Q-Values
            _qvals = InitTable(dataxml.SelectSingleNode("./qvals").InnerText);
            
            // Luck base/gradient tables
            _lck_base = InitTable(dataxml.SelectSingleNode("./lbvals").InnerText);
            _lck_gradient = InitTable(dataxml.SelectSingleNode("./lgvals").InnerText);
            
            // HP base/gradient tables
            _hp_base = InitTable(dataxml.SelectSingleNode("./hpbvals").InnerText);
            _hp_gradient = InitTable(dataxml.SelectSingleNode("./hpgvals").InnerText);
            
            // MP base/gradient tables
            _mp_base = InitTable(dataxml.SelectSingleNode("./mpbvals").InnerText);
            _mp_gradient = InitTable(dataxml.SelectSingleNode("./mpgvals").InnerText);
            
            // Stat ranks
            _stat_ranks = InitTable(dataxml.SelectSingleNode("./ranks").InnerText);


            Profile = new Gdk.Pixbuf(data.Assembly, "charfull." + Name.ToLower() + ".jpg");
            ProfileSmall = new Gdk.Pixbuf(data.Assembly, "charsmall." + Name.ToLower() + ".jpg");
            ProfileTiny = new Gdk.Pixbuf(data.Assembly, "chartiny." + Name.ToLower() + ".jpg");


            // Sanity checks. Sometimes we'll load materia onto a save file which
            //    causes the max to drop, but we don't otherwise modify the current
            //    values. So we'll do that here.

            if (_hp > _maxhp)
            {
                _hp = _maxhp;
            }
            else if (_hp <= 0)
            {
                _hp = 0;
                InflictDeath();
            }

            if (_mp > _maxmp)
            {
                _mp = _maxmp;
            }
            else if (_mp < 0)
            {
                _mp = 0;
            }

            CharacterMetrics = data.CharacterMetrics;
        }
        
        
        #region Methods

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
                        
            using (XmlWriter writer = XmlWriter.Create(s, Resource.XmlWriterSettings))
            {
                WriteToXml(writer);
            }

            return s.ToString();
        }
        
        public void WriteToXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name);
            writer.WriteStartElement("stats");
            writer.WriteElementString("str", _strength_base.ToString());
            writer.WriteElementString("dex", _dexterity_base.ToString());
            writer.WriteElementString("vit", _vitality_base.ToString());
            writer.WriteElementString("mag", _magic_base.ToString());
            writer.WriteElementString("spi", _spirit_base.ToString());
            writer.WriteElementString("lck", _luck_base.ToString());
            writer.WriteElementString("lvl", _level.ToString());
            writer.WriteEndElement();//stats
            writer.WriteElementString("exp", _exp.ToString());
            writer.WriteElementString("hp", _hp.ToString());
            writer.WriteElementString("mp", _mp.ToString());
            writer.WriteElementString("maxhp", _maxhp.ToString());
            writer.WriteElementString("maxmp", _maxmp.ToString());
            writer.WriteElementString("limitlvl", _limitlvl.ToString());
            writer.WriteElementString("fury", Fury.ToString().ToLower());
            writer.WriteElementString("sadness", Sadness.ToString().ToLower());
            writer.WriteElementString("backRow", BackRow.ToString().ToLower());


            writer.WriteStartElement("weapon");
            writer.WriteElementString("name", Weapon.ID);
            writer.WriteStartElement("materia");
            for (int i = 0; i < Weapon.Slots.Length; i++)
            {
                MateriaOrb m = Weapon.Slots[i];

                if (m != null)
                {
                    writer.WriteStartElement("orb");
                    writer.WriteAttributeString("name", m.Name);
                    writer.WriteAttributeString("type", m.Type.ToString());
                    writer.WriteAttributeString("ap", m.AP.ToString());
                    writer.WriteAttributeString("slot", i.ToString());
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();


            writer.WriteStartElement("armor");
            writer.WriteElementString("name", Armor.ID);
            writer.WriteStartElement("materia");
            for (int i = 0; i < Armor.Slots.Length; i++)
            {
                MateriaOrb m = Armor.Slots[i];
                                    
                if (m != null)
                {
                    writer.WriteStartElement("orb");
                    writer.WriteAttributeString("name", m.Name);
                    writer.WriteAttributeString("type", m.Type.ToString());
                    writer.WriteAttributeString("ap", m.AP.ToString());
                    writer.WriteAttributeString("slot", i.ToString());
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();
            writer.WriteEndElement();


            writer.WriteElementString("accessory", Accessory == null ? "" : Accessory.ID);
            
            writer.WriteEndElement();
        }
        
        
        #region Table Lookups
        
        /// <summary>
        /// Determines which level bracket given level is part of.
        /// </summary>
        /// <param name="level">Level to determine the bracket of</param>
        /// <returns>The level bracket index (0-based, 0-7 incl.)</returns>
        private static int LevelBracket(int level)
        {
            if (level < 1)
            {
                throw new ImplementationException("Level cannot be less than 1 - not in any bracket.");
            }
            if (level < 12)
            {
                return 0;
            }
            else if (level < 22)
            {
                return 1;
            }
            else if (level < 32)
            {
                return 2;
            }
            else if (level < 42)
            {
                return 3;
            }
            else if (level < 52)
            {
                return 4;
            }
            else if (level < 62)
            {
                return 5;
            }
            else if (level < 82)
            {
                return 6;
            }
            else if (level < 100)
            {
                return 7;
            }
            else
            {
                throw new ImplementationException("Level cannot be greater than 99 - not in any bracket.");
            }
        }
        
        /// <summary>
        /// Each character has a quadratic multiplier for each level bracket which
        /// is used to gauge how much experience is required to get to a certain level.
        /// </summary>
        /// <param name="level">Level you want the modifier for.</param>
        /// <returns>The quadratic modifier for this character at the given level.</returns>
        private int Q(int level)
        {
            return _qvals[LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "base" for calculating luck gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <remarks>Applies only to Luck.</remarks>
        /// <returns>The "base" modifier for a given level.</returns>
        private int LuckBase(int level)
        {
            return _lck_base[LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "gradient" for calculating luck gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <remarks>Applies only to Luck.</remarks>
        /// <returns>The "gradient" modifier for a given level.</returns>
        private int LuckGradient(int level)
        {
            return _lck_gradient[LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "base" for calculating HP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "base" modifier for a given level.</returns>
        private int HPBase(int level)
        {
            return _hp_base[LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "gradient" for calculating HP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "gradient" modifier for a given level.</returns>
        private int HPGradient(int level)
        {
            return _hp_gradient[LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "base" for calculating MP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "base" modifier for a given level.</returns>
        private int MPBase(int level)
        {
            return _mp_base[LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "gradient" for calculating MP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "gradient" modifier for a given level.</returns>
        private int MPGradient(int level)
        {
            return _mp_gradient[LevelBracket(level)];
        }
        
        #endregion Table Lookups
        
        
        
        /// <summary>
        /// Calculates the "baseline" stat for Str, Vit, Mag, Spi, and Dex.
        /// </summary>
        /// <param name="rank">One of the following: str(0), vit(1), mag(2), spi(3), dex(4).</param>
        /// <returns>The baseline for given stat and current level.</returns>
        private int Baseline(int statrank)
        {
            int b = CharacterMetrics.StatBase(_level, _stat_ranks[statrank]);
            int g = CharacterMetrics.StatGradient(_level, _stat_ranks[statrank]);
            return b + (g * _level / 100);
        }
        
        
        /// <summary>
        /// Adds experience to this Character. Handles levelling up.
        /// </summary>
        /// <param name="gain">Amount of EXP points to gain.</param>
        public void GainExperience(int gain)
        {
            int exp_after = _exp + gain;

            while (Level < MAX_LEVEL && exp_after >= ExpNextLevel)
            {
                LevelUp();
            }

            _exp = exp_after;
        }
        
        /// <summary>
        /// Increases this Character's level by one. Performs stat gains.
        /// </summary>
        private void LevelUp()
        {
            _level++;
            
            //         0   1   2   3   4
            // ranks: str vit mag spi dex
            
            // Baseline Stat = Base + [Gradient * Level / 100]
            // Stat Difference = Rnd(1..8) + Baseline - Current Stat
            // Stat Gain = _diff_gain[Stat Difference]
            
            #region Str, Vit, Mag, Spi, Dex
            
            int diff_str = RANDOM.Next(1, 9) + Baseline(0) - _strength_base;
            if (diff_str < 0)
            {
                diff_str = 0;
            }
            if (diff_str > 11)
            {
                diff_str = 11;
            }
            _strength_base += CharacterMetrics.DiffGain[diff_str];
            
            
            int diff_vit = RANDOM.Next(1, 9) + Baseline(1) - _vitality_base;
            if (diff_vit < 0)
            {
                diff_vit = 0;
            }
            if (diff_vit > 11)
            {
                diff_vit = 11;
            }
            _vitality_base += CharacterMetrics.DiffGain[diff_vit];
            
            
            int diff_mag = RANDOM.Next(1, 9) + Baseline(2) - _magic_base;
            if (diff_mag < 0)
            {
                diff_mag = 0;
            }
            if (diff_mag > 11)
            {
                diff_mag = 11;
            }
            _magic_base += CharacterMetrics.DiffGain[diff_mag];
            
            
            int diff_spi = RANDOM.Next(1, 9) + Baseline(3) - _spirit_base;
            if (diff_spi < 0)
            {
                diff_spi = 0;
            }
            if (diff_spi > 11)
            {
                diff_spi = 11;
            }
            _spirit_base += CharacterMetrics.DiffGain[diff_spi];
            
            
            int diff_dex = RANDOM.Next(1, 9) + Baseline(4) - _dexterity_base;
            if (diff_dex < 0)
            {
                diff_dex = 0;
            }
            if (diff_dex > 11)
            {
                diff_dex = 11;
            }
            _dexterity_base += CharacterMetrics.DiffGain[diff_dex];
            
#endregion
            
            
            int lb = LuckBase(_level);
            int lg = LuckGradient(_level);
            int luck_baseline = lb + (lg * _level / 100);
            int diff_lck = RANDOM.Next(1, 9) + luck_baseline - _luck_base;
            if (diff_lck < 0)
            {
                diff_lck = 0;
            }
            if (diff_lck > 11)
            {
                diff_lck = 11;
            }
            _luck_base += CharacterMetrics.DiffGain[diff_lck];
            
            
            int hpb = HPBase(_level);
            int hpg = HPGradient(_level);
            int hp_baseline = hpb + (_level - 1) * hpg;
            int diff_hp = RANDOM.Next(1, 9) + (100 * hp_baseline / _maxhp) - 100;
            if (diff_hp < 0)
            {
                diff_hp = 0;
            }
            if (diff_hp > 11)
            {
                diff_hp = 11;
            }
            int base_hp_gain = HPGradient(_level);
            _maxhp += CharacterMetrics.DiffGainHP[diff_hp] * base_hp_gain / 100;
            
            
            int mpb = MPBase(_level);
            int mpg = MPGradient(_level);
            int mp_baseline = mpb + ((_level - 1) * mpg / 10);
            int diff_mp = RANDOM.Next(1, 9) + (100 * mp_baseline / _maxmp) - 100;
            if (diff_mp < 0)
            {
                diff_mp = 0;
            }
            if (diff_mp > 11)
            {
                diff_mp = 11;
            }
            int base_mp_gain = (_level * mpg / 10) - ((_level - 1) * mpg / 10);
            _maxmp += CharacterMetrics.DiffGainMP[diff_mp] * base_mp_gain / 100;
            
        }
        
        
        
        
        #region Status

        public void Kill()
        {
            _hp = 0;

            Fury = false;
            Sadness = false;
            Death = true;
        }
        
        public bool InflictFury()
        {
            bool inflicted = false;
            
            if (!Fury)
            {
                Sadness = false;
                Fury = true;
                inflicted = true;
            }
            
            return inflicted;
        }

        public bool InflictSadness()
        {
            bool inflicted = false;
            
            if (!Sadness)
            {
                Fury = false;
                Sadness = true;
                inflicted = true;
            }
            
            return inflicted;
        }

        public bool InflictDeath()
        {
            bool inflicted = false;

            if (!Death)
            {
                Kill();
                inflicted = true;
            }
            
            return inflicted;
        }

        public bool CureFury()
        {
            bool cured = false;
            
            if (Fury)
            {
                Fury = false;
                cured = true;
            }
            
            return cured;
        }

        public bool CureSadness()
        {
            bool cured = false;
            
            if (Sadness)
            {
                Sadness = false;
                cured = true;
            }
            
            Sadness = false;
            
            return cured;
        }
        
        public bool CureDeath()
        {
            bool cured = false;

            if (Death)
            {
                Death = false;
                cured = true;
            }

            return cured;
        }


//        public bool CureDeath()
//        {
//            if (!_death)
//            {
//                return false;
//            }
//            _death = false;
//            return true;
//        }
        
        #endregion Status
        
        
        #region Elements
        
        public void AddHalve(params Element[] e)
        {
            foreach (Element i in e)
            {
                _halve.Add(i);
            }
        }
        public void AddVoid(params Element[] e)
        {
            foreach (Element i in e)
            {
                _void.Add(i);
            }
        }

        public void AddAbsorb(params Element[] e)
        {
            foreach (Element i in e)
            {
                _absorb.Add(i);
            }
        }

        public void AddImmunity(Status s)
        {
            _immune.Add(s);
        }
        
        public void RemoveHalve(params Element[] e)
        {
            foreach (Element i in e)
            {
                _halve.Remove(i);
            }
        }

        public void RemoveVoid(params Element[] e)
        {
            foreach (Element i in e)
            {
                _void.Remove(i);
            }
        }

        public void RemoveAbsorb(params Element[] e)
        {
            foreach (Element i in e)
            {
                _absorb.Remove(i);
            }
        }

        public void RemoveImmunity(Status s)
        {
            _immune.Remove(s);
        }
        
        public bool Halves(Element e)
        {
            return _halve.Contains(e);
        }

        public bool Halves(IEnumerable<Element> e)
        {
            foreach (Element d in e)
            {
                if (_halve.Contains(d))
                {
                    return true;
                }
            }

            return false;
        }

        public bool Voids(Element e)
        {
            return _void.Contains(e);
        }

        public bool Voids(IEnumerable<Element> e)
        {
            foreach (Element d in e)
                if (_void.Contains(d))
                {
                    return true;
                }
            return false;
        }

        public bool Absorbs(Element e)
        {
            return _absorb.Contains(e);
        }

        public bool Absorbs(IEnumerable<Element> e)
        {
            foreach (Element d in e)
                if (_absorb.Contains(d))
                {
                    return true;
                }
            return false;
        }

        public bool Immune(Status s)
        {
            return _immune.Contains(s);
        }
        
        #endregion Elements
        
        
        
        public void IncrementStrength()
        {
            StrengthBonus++;
        }

        public void IncrementDexterity()
        {
            DexterityBonus++;
        }

        public void IncrementVitality()
        {
            VitalityBonus++;
        }

        public void IncrementMagic()
        {
            MagicBonus++;
        }

        public void IncrementSpirit()
        {
            SpiritBonus++;
        }

        public void IncrementLuck()
        {
            LuckBonus++;
        }



        
        public int TotalExpForLevel(int level)
        {    
            int exp = 0;
            
            if (level > MIN_LEVEL)
            {
                if (level > MAX_LEVEL)
                {
                    level = MAX_LEVEL;
                }
                
                int mod = Q(level);
                
                for (int i = MIN_LEVEL; i <= level - 1; i++)
                {
                    exp = exp + (mod * i * i / 10);
                }
            }
            
            return exp;
        }
        
        #endregion Methods
        
        
        #region Properties
        
        public int Strength 
        { get { return _strength_base + (StrengthPercentBonus * _strength_base / 100) + StrengthBonus; } }

        public int Dexterity
        { get { return _dexterity_base + (DexterityPercentBonus * _dexterity_base / 100) + DexterityBonus; } }

        public int Vitality
        { get { return _vitality_base + (VitalityPercentBonus * _vitality_base / 100) + VitalityBonus; } }

        public int Magic
        { get { return _magic_base + (MagicPercentBonus * _magic_base / 100) + MagicBonus; } }

        public int Spirit
        { get { return _spirit_base + (SpiritPercentBonus * _spirit_base / 100) + SpiritBonus; } }

        public int Luck
        { get { return _luck_base + (LuckPercentBonus * _luck_base / 100) + LuckBonus; } }
        
        public int StrengthBonus { get; set; }

        public int DexterityBonus { get; set; }

        public int VitalityBonus { get; set; }

        public int MagicBonus { get; set; }

        public int SpiritBonus { get; set; }

        public int LuckBonus { get; set; }

        public int StrengthPercentBonus { get; set; }

        public int DexterityPercentBonus { get; set; }

        public int VitalityPercentBonus { get; set; }

        public int MagicPercentBonus { get; set; }

        public int SpiritPercentBonus { get; set; }

        public int LuckPercentBonus { get; set; }

        public int HPPercentBonus { get; set; }

        public int MPPercentBonus { get; set; }
        
        public int Level { get { return _level; } }

        public int LimitLevel { get { return _limitlvl; } }

        public int Exp { get { return _exp; } }

        /// <summary>
        /// Returns the TOTAL experience required to get to the CURRENT level
        /// </summary>
        public int ExpNextLevel
        {
            get
            {
                return TotalExpForLevel(_level + 1);
            }
        }
        
        /// <summary>
        /// Returns the TOTAL experience required to get to the CURRENT level
        /// </summary>
        public int ExpCurrentLevel
        {
            get
            {
                return TotalExpForLevel(_level);
            }
        }
        
        /// <summary>
        /// Returns the REMAINING experience required to get to the next level
        /// </summary>
        public int ExpToNextLevel { get { return ExpNextLevel - _exp; } }

        public bool HPFull { get { return HP == MaxHP; } }
        
        public bool MPFull { get { return MP == MaxMP; } }
        
        public int HP
        {
            get
            {
                return _hp; 
            }
            set
            {
                _hp = value;

                if (_hp > MaxHP)
                {
                    _hp = MaxHP;
                }
                else if (_hp < 0)
                {
                    _hp = 0;
                }

                if (_hp == 0)
                {
                    Kill();
                }
            }
        }

        public int MP
        {
            get
            {
                if (_mp > MaxMP)
                {
                    _mp = MaxMP;
                }

                return _mp; 
            }
            set
            {
                _mp = value;

                if (_mp > MaxMP)
                {
                    _mp = MaxMP;
                }

                if (_mp < 0)
                {
                    _mp = 0;
                }
            }
        }

        public int MaxHP
        {
            get
            {
                int temp = _maxhp;

                foreach (MateriaOrb m in Weapon.Slots)
                {
                    if (m != null)
                    {
                        temp += temp * m.HPMod / 100;
                    }
                }
                foreach (MateriaOrb m in Armor.Slots)
                {
                    if (m != null)
                    {
                        temp += temp * m.HPMod / 100;
                    }
                }

                temp += temp * HPPercentBonus / 100;
                
                if (temp > MAX_HP)
                {
                    temp = MAX_HP;
                }

                return temp;
            }
        }

        public int MaxMP
        {
            get
            {
                int temp = _maxmp;

                foreach (MateriaOrb m in Weapon.Slots.Union(Armor.Slots))
                {
                    if (m != null)
                    {
                        temp += temp * m.MPMod / 100;
                    }
                }

                temp += temp * MPPercentBonus / 100;

                if (temp > MAX_MP)
                {
                    temp = MAX_MP;
                }

                return temp;
            }
        }

        public bool Sadness { get; private set; }

        public bool Fury { get; private set; }

        public bool NearDeath { get { return HP <= (MaxHP / 4); } }

        public bool Death { get; private set; }
        
        public Weapon Weapon
        {
            get { return _weapon; }
            set
            {
                if (value == null)
                {
                    throw new ImplementationException("Cannot remove weapon. Must replace with new weapon.");
                }
                _weapon.Detach(this);
                _weapon = value;
                _weapon.Attach(this);
            }
        }

        public Armor Armor
        {
            get { return _armor; }
            set
            {
                if (value == null)
                {
                    throw new ImplementationException("Cannot remove armor. Must replace with new armor.");
                }

                _armor.Detach(this);
                _armor = value;
                _armor.Attach(this);
            }
        }

        public Accessory Accessory
        {
            get { return _accessory; }
            set
            {
                if (_accessory != null)
                {
                    _accessory.Detach(this);
                }

                _accessory = value;
                
                if (_accessory != null)
                {
                    _accessory.Attach(this);
                }
            }
        }

        public IEnumerable<MateriaOrb> Materia { get { return Weapon.Slots.Union(Armor.Slots); } }
        
        public string Name { get; private set; }

        public Sex Sex { get { return _sex; } }
        
        #endregion Properties

        

        public bool BackRow { get; set; }
        
        public Gdk.Pixbuf Profile { get; private set; }
        
        public Gdk.Pixbuf ProfileSmall { get; private set; }
        
        public Gdk.Pixbuf ProfileTiny { get; private set; }

        private Metrics CharacterMetrics { get; set; }
    }
}
