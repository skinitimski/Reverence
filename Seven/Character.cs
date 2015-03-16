using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;
using System.Reflection;

using Gdk;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven
{
    internal class Character
    {
        public const int PROFILE_WIDTH = 120;
        public const int PROFILE_HEIGHT = 136;
        public const int PROFILE_WIDTH_SMALL = 89;
        public const int PROFILE_HEIGHT_SMALL = 101;

        #region Member Data
        
        private int _level;
        private int _strength_base;
        private int _dexterity_base;
        private int _vitality_base;
        private int _magic_base;
        private int _spirit_base;
        private int _luck_base;
        private int _strength_bonus;
        private int _dexterity_bonus;
        private int _vitality_bonus;
        private int _magic_bonus;
        private int _spirit_bonus;
        private int _luck_bonus;
        private int _strength_p_bonus;
        private int _dexterity_p_bonus;
        private int _vitality_p_bonus;
        private int _magic_p_bonus;
        private int _spirit_p_bonus;
        private int _luck_p_bonus;
        private int _hp_p_bonus;
        private int _mp_p_bonus;
        private WeaponType _weaponType;
        private Weapon _weapon;
        private Armor _armor;
        private Accessory _accessory;
        private int _hp;
        private int _mp;
        private int _maxhp;
        private int _maxmp;
        private int _limitlvl;
        private bool _fury = false;
        private bool _sadness = false;
        private bool _death = false;
        
        private List<Element> _halve;
        private List<Element> _void;
        private List<Element> _absorb;
        private List<Status> _immune;
        
        private string _name;
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

        
        /// <summary>Aggregate for stat gradient arrays. Indexed by level bracket.</summary>
        private static List<int[]> _stat_gradient;
        /// <summary>Aggregate for stat base arrays. Indexed by level bracket.</summary>
        private static List<int[]> _stat_base;
        
        /// <summary>Gradient values for level bracket 0, indexed by rank.</summary>
        private static int[] _L2_11_gradient;
        /// <summary>Gradient values for level bracket 1, indexed by rank.</summary>
        private static int[] _L12_21_gradient;
        /// <summary>Gradient values for level bracket 2, indexed by rank.</summary>
        private static int[] _L22_31_gradient;
        /// <summary>Gradient values for level bracket 3, indexed by rank.</summary>
        private static int[] _L32_41_gradient;
        /// <summary>Gradient values for level bracket 4, indexed by rank.</summary>
        private static int[] _L42_51_gradient;
        /// <summary>Gradient values for level bracket 5, indexed by rank.</summary>
        private static int[] _L52_61_gradient;
        /// <summary>Gradient values for level bracket 6, indexed by rank.</summary>
        private static int[] _L62_81_gradient;
        /// <summary>Gradient values for level bracket 7, indexed by rank.</summary>
        private static int[] _L82_99_gradient;
        
        /// <summary>Base values for level bracket 0, indexed by rank.</summary>
        private static int[] _L2_11_base;
        /// <summary>Base values for level bracket 1, indexed by rank.</summary>
        private static int[] _L12_21_base;
        /// <summary>Base values for level bracket 2, indexed by rank.</summary>
        private static int[] _L22_31_base;
        /// <summary>Base values for level bracket 3, indexed by rank.</summary>
        private static int[] _L32_41_base;
        /// <summary>Base values for level bracket 4, indexed by rank.</summary>
        private static int[] _L42_51_base;
        /// <summary>Base values for level bracket 5, indexed by rank.</summary>
        private static int[] _L52_61_base;
        /// <summary>Base values for level bracket 6, indexed by rank.</summary>
        private static int[] _L62_81_base;
        /// <summary>Base values for level bracket 7, indexed by rank.</summary>
        private static int[] _L82_99_base;
        
        /// <summary>Lookup for stat gains, indexed by difference.</summary>
        private static int[] _diff_gain;
        /// <summary>Lookup for HP percent gains, indexed by difference.</summary>
        private static int[] _diff_gain_hp;
        /// <summary>Lookup for MP percent gains, indexed by difference.</summary>
        private static int[] _diff_gain_mp;
        private static Random _random;
        
        #endregion Member Data
        
        
        /// <summary>
        /// Creates tables and the game Characters
        /// </summary>
        static Character()
        {
            InitTables();
        }
        /// <summary>
        /// Initializes tables used in stat gains
        /// </summary>
        private static void InitTables()
        {
            using (StreamReader reader = Resource.GetStreamReaderFromResource("data.gradient.table", typeof(Seven).Assembly))
            {
                InitTable(ref _L2_11_gradient, reader.ReadLine(), '\t');
                InitTable(ref _L12_21_gradient, reader.ReadLine(), '\t');
                InitTable(ref _L22_31_gradient, reader.ReadLine(), '\t');
                InitTable(ref _L32_41_gradient, reader.ReadLine(), '\t');
                InitTable(ref _L42_51_gradient, reader.ReadLine(), '\t');
                InitTable(ref _L52_61_gradient, reader.ReadLine(), '\t');
                InitTable(ref _L62_81_gradient, reader.ReadLine(), '\t');
                InitTable(ref _L82_99_gradient, reader.ReadLine(), '\t');
            }
            
            using (StreamReader reader = Resource.GetStreamReaderFromResource("data.base.table", typeof(Seven).Assembly))
            {
                InitTable(ref _L2_11_base, reader.ReadLine(), '\t');
                InitTable(ref _L12_21_base, reader.ReadLine(), '\t');
                InitTable(ref _L22_31_base, reader.ReadLine(), '\t');
                InitTable(ref _L32_41_base, reader.ReadLine(), '\t');
                InitTable(ref _L42_51_base, reader.ReadLine(), '\t');
                InitTable(ref _L52_61_base, reader.ReadLine(), '\t');
                InitTable(ref _L62_81_base, reader.ReadLine(), '\t');
                InitTable(ref _L82_99_base, reader.ReadLine(), '\t');
            }
            
            using (StreamReader reader = Resource.GetStreamReaderFromResource("data.diffs.table", typeof(Seven).Assembly))
            {
                reader.ReadLine(); // [diff_gain]
                InitTable(ref _diff_gain, reader.ReadLine(), '\t');
                reader.ReadLine(); // [diff_gain_hp]
                InitTable(ref _diff_gain_hp, reader.ReadLine(), '\t');
                reader.ReadLine(); // [diff_gain_mp]
                InitTable(ref _diff_gain_mp, reader.ReadLine(), '\t');
            }
            
            _stat_base = new List<int[]>();
            _stat_gradient = new List<int[]>();
            
            _stat_base.Add(_L2_11_base);
            _stat_base.Add(_L12_21_base);
            _stat_base.Add(_L22_31_base);
            _stat_base.Add(_L32_41_base);
            _stat_base.Add(_L42_51_base);
            _stat_base.Add(_L52_61_base);
            _stat_base.Add(_L62_81_base);
            _stat_base.Add(_L82_99_base);
            
            _stat_gradient.Add(_L2_11_gradient);
            _stat_gradient.Add(_L12_21_gradient);
            _stat_gradient.Add(_L22_31_gradient);
            _stat_gradient.Add(_L32_41_gradient);
            _stat_gradient.Add(_L42_51_gradient);
            _stat_gradient.Add(_L52_61_gradient);
            _stat_gradient.Add(_L62_81_gradient);
            _stat_gradient.Add(_L82_99_gradient);
        }

        /// <summary>
        /// Populates an array with the values in given line, splitting on sep.
        /// </summary>
        /// <param name="table">Array to populate</param>
        /// <param name="line">Data to put into the array</param>
        /// <param name="sep">Used to split the line parameter into array parts</param>
        private static void InitTable(ref int[] table, string line, char sep)
        {
            string[] parts = line.Split(new char[] { sep }, StringSplitOptions.RemoveEmptyEntries);

            table = new int[parts.Length];

            for (int i = 0; i < table.Length; i++)
            {
                table [i] = Int32.Parse(parts [i]);
            }
        }

        /// <summary>
        /// Creates a character from scratch with initial stats/equipment.
        /// </summary>
        /// <param name="dataxmlstring"></param>
        public Character(string dataxmlstring)
        {
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();
            
            
            XmlDocument dataxml = new XmlDocument();
            dataxml.Load(new MemoryStream(Encoding.UTF8.GetBytes(dataxmlstring)));
            
            _name = dataxml.FirstChild.Name;
            
            // Old Way : xml.SelectSingleNode("/" + _name + "/stats/lvl").InnerText
            
            // Stats
            _strength_base = Int32.Parse(dataxml.SelectSingleNode("//str").InnerText);
            _dexterity_base = Int32.Parse(dataxml.SelectSingleNode("//dex").InnerText);
            _vitality_base = Int32.Parse(dataxml.SelectSingleNode("//vit").InnerText);
            _magic_base = Int32.Parse(dataxml.SelectSingleNode("//mag").InnerText);
            _spirit_base = Int32.Parse(dataxml.SelectSingleNode("//spi").InnerText);
            _luck_base = Int32.Parse(dataxml.SelectSingleNode("//lck").InnerText);
            
            // Experience and Level
            _exp = Int32.Parse(dataxml.SelectSingleNode("//exp").InnerText);
            _level = Int32.Parse(dataxml.SelectSingleNode("//lvl").InnerText);
            _limitlvl = 1;
            
            // HP
            _hp = Int32.Parse(dataxml.SelectSingleNode("//hp").InnerText);
            _maxhp = Int32.Parse(dataxml.SelectSingleNode("//hp").InnerText);
            if (_hp > _maxhp)
                throw new SaveStateException("HP > MAXHP for " + _name);
            
            // MP
            _mp = Int32.Parse(dataxml.SelectSingleNode("//mp").InnerText);
            _maxmp = Int32.Parse(dataxml.SelectSingleNode("//mp").InnerText);
            if (_mp > _maxmp)
                throw new SaveStateException("MP > MAXMP for " + _name);
            
            _sadness = false;
            _fury = false;
            _death = false;
            
            _sex = (Sex)Enum.Parse(typeof(Sex), dataxml.SelectSingleNode("//sex").InnerText);
            
            // Equipment
            _weaponType = (WeaponType)Enum.Parse(typeof(WeaponType), _name.Replace(" ", ""));
            _weapon = new Weapon(dataxml.SelectSingleNode("//weapon/name").InnerText);

//            foreach (XmlNode orb in dataxml.SelectNodes("//weapon/materia/orb"))
//            {
//                string id = orb.Attributes["id"].Value;
//                MateriaType type = (MateriaType)Enum.Parse(typeof(MateriaType), orb.Attributes["type"].Value);
//                int ap = Int32.Parse(orb.Attributes["ap"].Value);
//                int slot = Int32.Parse(orb.Attributes["slot"].Value);
//                if (slot >= _weapon.Slots.Length)
//                    throw new SavegameException("Materia orb assigned to slot that doesnt exist on weapon.");
//                
//                _weapon.Slots[slot] = Atmosphere.BattleSimulator.MateriaBase.Create(id, ap, type);
//            }
            
            _armor = new Armor(dataxml.SelectSingleNode("//armor/name").InnerText);

//            foreach (XmlNode orb in dataxml.SelectNodes("//armor/materia/orb"))
//            {
//                string id = orb.Attributes["id"].Value;
//                MateriaType type = (MateriaType)Enum.Parse(typeof(MateriaType), orb.Attributes["type"].Value);
//                int ap = Int32.Parse(orb.Attributes["ap"].Value);
//                int slot = Int32.Parse(orb.Attributes["slot"].Value);
//                if (slot >= _weapon.Slots.Length)
//                    throw new SavegameException("Materia orb assigned to slot that doesnt exist on armor.");
//                
//                _armor.Slots[slot] = Atmosphere.BattleSimulator.MateriaBase.Create(id, ap, type);
//            }
            
            string acc = dataxml.SelectSingleNode("//accessory").InnerText;
            _accessory = String.IsNullOrEmpty(acc) ? Accessory.EMPTY : Accessory.AccessoryTable [acc];
            
            // Q-Values
            InitTable(ref _qvals, dataxml.SelectSingleNode("//qvals").InnerText, ',');
            
            // Luck base/gradient tables
            InitTable(ref _lck_base, dataxml.SelectSingleNode("//lbvals").InnerText, ',');
            InitTable(ref _lck_gradient, dataxml.SelectSingleNode("//lgvals").InnerText, ',');
            
            // HP base/gradient tables
            InitTable(ref _hp_base, dataxml.SelectSingleNode("//hpbvals").InnerText, ',');
            InitTable(ref _hp_gradient, dataxml.SelectSingleNode("//hpgvals").InnerText, ',');
            
            // MP base/gradient tables
            InitTable(ref _mp_base, dataxml.SelectSingleNode("//mpbvals").InnerText, ',');
            InitTable(ref _mp_gradient, dataxml.SelectSingleNode("//mpgvals").InnerText, ',');
            
            // Stat ranks
            InitTable(ref _stat_ranks, dataxml.SelectSingleNode("//ranks").InnerText, ',');
            
            
            Profile = new Pixbuf(typeof(Seven).Assembly, "charfull." + _name.ToLower() + ".jpg");
            ProfileSmall = new Pixbuf(typeof(Seven).Assembly, "charsmall." + _name.ToLower() + ".jpg");
        }
        
        /// <summary>
        /// Loads a saved character.
        /// </summary>
        /// <param name="savexmlstring"></param>
        /// <param name="dataxmlstring"></param>
        public Character(string savexmlstring, string dataxmlstring)
        {
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();


            
            XmlDocument savexml = new XmlDocument();
            XmlDocument dataxml = new XmlDocument();
            savexml.Load(new MemoryStream(Encoding.UTF8.GetBytes(savexmlstring)));
            dataxml.Load(new MemoryStream(Encoding.UTF8.GetBytes(dataxmlstring)));
            
            _name = savexml.FirstChild.Name;
            
            // Old Way : xml.SelectSingleNode("/" + _name + "/stats/lvl").InnerText
            
            // Stats
            _strength_base = Int32.Parse(savexml.SelectSingleNode("//str").InnerText);
            _dexterity_base = Int32.Parse(savexml.SelectSingleNode("//dex").InnerText);
            _vitality_base = Int32.Parse(savexml.SelectSingleNode("//vit").InnerText);
            _magic_base = Int32.Parse(savexml.SelectSingleNode("//mag").InnerText);
            _spirit_base = Int32.Parse(savexml.SelectSingleNode("//spi").InnerText);
            _luck_base = Int32.Parse(savexml.SelectSingleNode("//lck").InnerText);
            
            // Experience and Level
            _exp = Int32.Parse(savexml.SelectSingleNode("//exp").InnerText);
            _level = Int32.Parse(savexml.SelectSingleNode("//lvl").InnerText);
            _limitlvl = Int32.Parse(savexml.SelectSingleNode("//limitlvl").InnerText);
            
            // HP
            _hp = Int32.Parse(savexml.SelectSingleNode("//hp").InnerText);
            _maxhp = Int32.Parse(savexml.SelectSingleNode("//maxhp").InnerText);
            if (_hp > _maxhp)
                throw new SaveStateException("HP > MAXHP for " + _name);
            _death = (_hp == 0);
            
            // MP
            _mp = Int32.Parse(savexml.SelectSingleNode("//mp").InnerText);
            _maxmp = Int32.Parse(savexml.SelectSingleNode("//maxmp").InnerText);
            if (_mp > _maxmp)
                throw new SaveStateException("MP > MAXMP for " + _name);
            
            // Fury/Sadness
            _sadness = Boolean.Parse(savexml.SelectSingleNode("//sadness").InnerText);
            _fury = Boolean.Parse(savexml.SelectSingleNode("//fury").InnerText);
            if (_sadness && _fury)
                throw new SaveStateException("Can't be both sad and furious");
            
            // Sex
            _sex = (Sex)Enum.Parse(typeof(Sex), dataxml.SelectSingleNode("//sex").InnerText);
            
            // Equipment
            _weaponType = (WeaponType)Enum.Parse(typeof(WeaponType), _name.Replace(" ", ""));
            
            _weapon = new Weapon(savexml.SelectSingleNode("//weapon/name").InnerText);

//            foreach (XmlNode orb in savexml.SelectNodes("//weapon/materia/orb"))
//            {
//                string id = orb.Attributes["id"].Value;
//                MateriaType type = (MateriaType)Enum.Parse(typeof(MateriaType), orb.Attributes["type"].Value);
//                int ap = Int32.Parse(orb.Attributes["ap"].Value);
//                int slot = Int32.Parse(orb.Attributes["slot"].Value);
//                if (slot >= _weapon.Slots.Length)
//                    throw new SavegameException("Materia orb assigned to slot that doesnt exist on weapon.");
//                
//                _weapon.Slots[slot] = Atmosphere.BattleSimulator.MateriaBase.Create(id, ap, type);
//            }
//            
            _armor = new Armor(savexml.SelectSingleNode("//armor/name").InnerText);

//            foreach (XmlNode orb in savexml.SelectNodes("//armor/materia/orb"))
//            {
//                string id = orb.Attributes["id"].Value;
//                MateriaType type = (MateriaType)Enum.Parse(typeof(MateriaType), orb.Attributes["type"].Value);
//                int ap = Int32.Parse(orb.Attributes["ap"].Value);
//                int slot = Int32.Parse(orb.Attributes["slot"].Value);
//                if (slot >= _weapon.Slots.Length)
//                    throw new SavegameException("Materia orb assigned to slot that doesnt exist on armor.");
//                
//                _armor.Slots[slot] = Atmosphere.BattleSimulator.MateriaBase.Create(id, ap, type);
//            }
            
            string acc = savexml.SelectSingleNode("//accessory").InnerText;
            _accessory = String.IsNullOrEmpty(acc) ? Accessory.EMPTY: Accessory.AccessoryTable [acc];
            
            
            // Q-Values
            InitTable(ref _qvals, dataxml.SelectSingleNode("//qvals").InnerText, ',');
            
            // Luck base/gradient tables
            InitTable(ref _lck_base, dataxml.SelectSingleNode("//lbvals").InnerText, ',');
            InitTable(ref _lck_gradient, dataxml.SelectSingleNode("//lgvals").InnerText, ',');
            
            // HP base/gradient tables
            InitTable(ref _hp_base, dataxml.SelectSingleNode("//hpbvals").InnerText, ',');
            InitTable(ref _hp_gradient, dataxml.SelectSingleNode("//hpgvals").InnerText, ',');
            
            // MP base/gradient tables
            InitTable(ref _mp_base, dataxml.SelectSingleNode("//mpbvals").InnerText, ',');
            InitTable(ref _mp_gradient, dataxml.SelectSingleNode("//mpgvals").InnerText, ',');
            
            // Stat ranks
            InitTable(ref _stat_ranks, dataxml.SelectSingleNode("//ranks").InnerText, ',');


            Profile = new Pixbuf(typeof(Seven).Assembly, "charfull." + _name.ToLower() + ".jpg");
            ProfileSmall = new Pixbuf(typeof(Seven).Assembly, "charsmall." + _name.ToLower() + ".jpg");
        }
        
        
        #region Methods
        
        public override string ToString()
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine(String.Format("Name: {0}", _name));
            b.AppendLine(String.Format("\tLevel: {0}", _level.ToString()));
            b.AppendLine(String.Format("\tExperience: {0}", _exp));
            b.AppendLine(String.Format("\tHP: {0}", _maxhp));
            b.AppendLine(String.Format("\tMP: {0}", _maxmp));
            b.AppendLine(String.Format("\tStrength: {0}", _strength_base.ToString()));
            b.AppendLine(String.Format("\tDexterity: {0}", _dexterity_base.ToString()));
            b.AppendLine(String.Format("\tVitality: {0}", _vitality_base.ToString()));
            b.AppendLine(String.Format("\tMagic: {0}", _magic_base.ToString()));
            b.AppendLine(String.Format("\tSpirit: {0}", _spirit_base.ToString()));
            b.AppendLine(String.Format("\tLuck: {0}", _luck_base.ToString()));
            
            return b.ToString();
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
                throw new ImplementationException("Level cannot be less than 1 - not in any bracket.");
            if (level < 12)
                return 0;
            else if (level < 22)
                return 1;
            else if (level < 32)
                return 2;
            else if (level < 42)
                return 3;
            else if (level < 52)
                return 4;
            else if (level < 62)
                return 5;
            else if (level < 82)
                return 6;
            else if (level < 100)
                return 7;
            else
                throw new ImplementationException("Level cannot be greater than 99 - not in any bracket.");
        }
        
        /// <summary>
        /// Each character has a quadratic multiplier for each level bracket which
        /// is used to gauge how much experience is required to get to a certain level.
        /// </summary>
        /// <param name="level">Level you want the modifier for.</param>
        /// <returns>The quadratic modifier for this character at the given level.</returns>
        private int Q(int level)
        {
            return _qvals [LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "base" stat for calculating stat gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <param name="rank">Rank of the modifier (0-29 incl).</param>
        /// <remarks>Applies to Strength, Vitality, Magic, Spirit, and Dexterity.</remarks>
        /// <returns>The "base" modifier for a given rank and level.</returns>
        private static int StatBase(int level, int rank)
        {
            return _stat_base [LevelBracket(level)] [rank];
        }
        
        /// <summary>
        /// Gets the "gradient" for calculating stat gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <param name="rank">Rank of the modifier (0-29 incl).</param>
        /// <remarks>Applies to Strength, Vitality, Magic, Spirit, and Dexterity.</remarks>
        /// <returns>The "gradient" modifier for a given rank and level.</returns>
        private static int StatGradient(int level, int rank)
        {
            return _stat_gradient [LevelBracket(level)] [rank];
        }
        
        /// <summary>
        /// Gets the "base" for calculating luck gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <remarks>Applies only to Luck.</remarks>
        /// <returns>The "base" modifier for a given level.</returns>
        private int LuckBase(int level)
        {
            return _lck_base [LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "gradient" for calculating luck gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <remarks>Applies only to Luck.</remarks>
        /// <returns>The "gradient" modifier for a given level.</returns>
        private int LuckGradient(int level)
        {
            return _lck_gradient [LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "base" for calculating HP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "base" modifier for a given level.</returns>
        private int HPBase(int level)
        {
            return _hp_base [LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "gradient" for calculating HP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "gradient" modifier for a given level.</returns>
        private int HPGradient(int level)
        {
            return _hp_gradient [LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "base" for calculating MP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "base" modifier for a given level.</returns>
        private int MPBase(int level)
        {
            return _mp_base [LevelBracket(level)];
        }
        
        /// <summary>
        /// Gets the "gradient" for calculating MP gains on level ups.
        /// </summary>
        /// <param name="level">Level you want the base modifier for.</param>
        /// <returns>The "gradient" modifier for a given level.</returns>
        private int MPGradient(int level)
        {
            return _mp_gradient [LevelBracket(level)];
        }
        
        #endregion Table Lookups
        
        
        
        /// <summary>
        /// Calculates the "baseline" stat for Str, Vit, Mag, Spi, and Dex.
        /// </summary>
        /// <param name="rank">One of the following: str(0), vit(1), mag(2), spi(3), dex(4).</param>
        /// <returns>The baseline for given stat and current level.</returns>
        private int Baseline(int statrank)
        {
            int b = StatBase(_level, _stat_ranks [statrank]);
            int g = StatGradient(_level, _stat_ranks [statrank]);
            return b + (g * _level / 100);
        }
        
        
        /// <summary>
        /// Adds experience to this Character. Handles levelling up.
        /// </summary>
        /// <param name="gain">Amount of EXP points to gain.</param>
        public void GainExperience(int gain)
        {
            while (_exp + gain >= NextLevel)
                LevelUp();
            _exp += gain;
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
            
            int diff_str = _random.Next(1, 9) + Baseline(0) - _strength_base;
            if (diff_str < 0)
                diff_str = 0;
            if (diff_str > 11)
                diff_str = 11;
            _strength_base += _diff_gain [diff_str];
            
            
            int diff_vit = _random.Next(1, 9) + Baseline(1) - _vitality_base;
            if (diff_vit < 0)
                diff_vit = 0;
            if (diff_vit > 11)
                diff_vit = 11;
            _vitality_base += _diff_gain [diff_vit];
            
            
            int diff_mag = _random.Next(1, 9) + Baseline(2) - _magic_base;
            if (diff_mag < 0)
                diff_mag = 0;
            if (diff_mag > 11)
                diff_mag = 11;
            _magic_base += _diff_gain [diff_mag];
            
            
            int diff_spi = _random.Next(1, 9) + Baseline(3) - _spirit_base;
            if (diff_spi < 0)
                diff_spi = 0;
            if (diff_spi > 11)
                diff_spi = 11;
            _spirit_base += _diff_gain [diff_spi];
            
            
            int diff_dex = _random.Next(1, 9) + Baseline(4) - _dexterity_base;
            if (diff_dex < 0)
                diff_dex = 0;
            if (diff_dex > 11)
                diff_dex = 11;
            _dexterity_base += _diff_gain [diff_dex];
            
#endregion
            
            
            int lb = LuckBase(_level);
            int lg = LuckGradient(_level);
            int luck_baseline = lb + (lg * _level / 100);
            int diff_lck = _random.Next(1, 9) + luck_baseline - _luck_base;
            if (diff_lck < 0)
                diff_lck = 0;
            if (diff_lck > 11)
                diff_lck = 11;
            _luck_base += _diff_gain [diff_lck];
            
            
            int hpb = HPBase(_level);
            int hpg = HPGradient(_level);
            int hp_baseline = hpb + (_level - 1) * hpg;
            int diff_hp = _random.Next(1, 9) + (100 * hp_baseline / _maxhp) - 100;
            if (diff_hp < 0)
                diff_hp = 0;
            if (diff_hp > 11)
                diff_hp = 11;
            int base_hp_gain = HPGradient(_level);
            _maxhp += _diff_gain_hp [diff_hp] * base_hp_gain / 100;
            
            
            int mpb = MPBase(_level);
            int mpg = MPGradient(_level);
            int mp_baseline = mpb + ((_level - 1) * mpg / 10);
            int diff_mp = _random.Next(1, 9) + (100 * mp_baseline / _maxmp) - 100;
            if (diff_mp < 0)
                diff_mp = 0;
            if (diff_mp > 11)
                diff_mp = 11;
            int base_mp_gain = (_level * mpg / 10) - ((_level - 1) * mpg / 10);
            _maxmp += _diff_gain_mp [diff_mp] * base_mp_gain / 100;
            
        }
        
        
        
        
        #region Status
        
        public bool InflictFury()
        {
            if (_fury)
                return false;
            _sadness = false;
            _fury = true;
            return true;
        }

        public bool InflictSadness()
        {
            if (_sadness)
                return false;
            _fury = false;
            _sadness = true;
            return true;
        }

        public bool InflictDeath()
        {
            if (_death)
                return false;
            _death = true;
            _hp = 0;
            return true;
        }

        public bool CureFury()
        {
            if (!_fury)
                return false;
            _fury = false;
            return true;
        }

        public bool CureSadness()
        {
            if (!_sadness)
                return false;
            _sadness = false;
            return true;
        }

        public bool CureDeath()
        {
            if (!_death)
                return false;
            _death = false;
            return true;
        }
        
        #endregion Status
        
        
        #region Elements
        
        public void AddHalve(params Element[] e)
        {
            foreach (Element i in e)
                _halve.Add(i);
        }
        public void AddHalve(Element e)
        {
            _halve.Add(e);
        }
        public void AddVoid(params Element[] e)
        {
            foreach (Element i in e)
                _void.Add(i);
        }
        public void AddVoid(Element e)
        {
            _void.Add(e);
        }
        public void AddAbsorb(params Element[] e)
        {
            foreach (Element i in e)
                _absorb.Add(i);
        }
        public void AddAbsorb(Element e)
        {
            _absorb.Add(e);
        }
        public void AddImmunity(Status s)
        {
            _immune.Add(s);
        }
        
        public void RemoveHalve(params Element[] e)
        {
            foreach (Element i in e)
                _halve.Remove(i);
        }
        public void RemoveHalve(Element e)
        {
            _halve.Remove(e);
        }
        public void RemoveVoid(params Element[] e)
        {
            foreach (Element i in e)
                _void.Remove(i);
        }
        public void RemoveVoid(Element e)
        {
            _void.Remove(e);
        }
        public void RemoveAbsorb(params Element[] e)
        {
            foreach (Element i in e)
                _absorb.Remove(i);
        }
        public void RemoveAbsorb(Element e)
        {
            _absorb.Remove(e);
        }
        public void RemoveImmunity(Status s)
        {
            _immune.Remove(s);
        }
        
        public bool Halves(Element e)
        {
            return _halve.Contains(e);
        }
        public bool Halves(params Element[] e)
        {
            foreach (Element d in e)
                if (_halve.Contains(d))
                    return true;
            return false;
        }
        public bool Voids(Element e)
        {
            return _void.Contains(e);
        }
        public bool Voids(params Element[] e)
        {
            foreach (Element d in e)
                if (_void.Contains(d))
                    return true;
            return false;
        }
        public bool Absorbs(Element e)
        {
            return _absorb.Contains(e);
        }
        public bool Absorbs(params Element[] e)
        {
            foreach (Element d in e)
                if (_absorb.Contains(d))
                    return true;
            return false;
        }
        public bool Immune(Status s)
        {
            return _immune.Contains(s);
        }
        
        #endregion Elements
        
        
        
        public void IncrementStrength()
        {
            _strength_bonus++;
        }

        public void IncrementDexterity()
        {
            _dexterity_bonus++;
        }

        public void IncrementVitality()
        {
            _vitality_bonus++;
        }

        public void IncrementMagic()
        {
            _magic_bonus++;
        }

        public void IncrementSpirit()
        {
            _spirit_bonus++;
        }

        public void IncrementLuck()
        {
            _luck_bonus++;
        }
        
        #endregion Methods
        
        
        #region Properties
        
        public int Strength
        { get { return _strength_base + (_strength_p_bonus * _strength_base / 100) + _strength_bonus; } }

        public int Dexterity
        { get { return _dexterity_base + (_dexterity_p_bonus * _dexterity_base / 100) + _dexterity_bonus; } }

        public int Vitality
        { get { return _vitality_base + (_vitality_p_bonus * _vitality_base / 100) + _vitality_bonus; } }

        public int Magic
        { get { return _magic_base + (_magic_p_bonus * _magic_base / 100) + _magic_bonus; } }

        public int Spirit
        { get { return _spirit_base + (_spirit_p_bonus * _spirit_base / 100) + _spirit_bonus; } }

        public int Luck
        { get { return _luck_base + (_luck_p_bonus * _luck_base / 100) + _luck_bonus; } }
        
        public int StrengthBonus
        {
            get { return _strength_bonus; }
            set { _strength_bonus = value; }
        }

        public int DexterityBonus
        {
            get { return _dexterity_bonus; }
            set { _dexterity_bonus = value; }
        }

        public int VitalityBonus
        {
            get { return _vitality_bonus; }
            set { _vitality_bonus = value; }
        }

        public int MagicBonus
        {
            get { return _magic_bonus; }
            set { _magic_bonus = value; }
        }

        public int SpiritBonus
        {
            get { return _spirit_bonus; }
            set { _spirit_bonus = value; }
        }

        public int LuckBonus
        {
            get { return _luck_bonus; }
            set { _luck_bonus = value; }
        }
        
        public int StrengthPercentBonus
        {
            get { return _strength_p_bonus; }
            set { _strength_p_bonus = value; }
        }

        public int DexterityPercentBonus
        {
            get { return _dexterity_p_bonus; }
            set { _dexterity_p_bonus = value; }
        }

        public int VitalityPercentBonus
        {
            get { return _vitality_p_bonus; }
            set { _vitality_p_bonus = value; }
        }

        public int MagicPercentBonus
        {
            get { return _magic_p_bonus; }
            set { _magic_p_bonus = value; }
        }

        public int SpiritPercentBonus
        {
            get { return _spirit_p_bonus; }
            set { _spirit_p_bonus = value; }
        }

        public int LuckPercentBonus
        {
            get { return _luck_p_bonus; }
            set { _luck_p_bonus = value; }
        }

        public int HPPercentBonus
        {
            get { return _hp_p_bonus; }
            set { _hp_p_bonus = value; }
        }

        public int MPPercentBonus
        {
            get { return _mp_p_bonus; }
            set { _mp_p_bonus = value; }
        }
        
        public int Level { get { return _level; } }

        public int LimitLevel { get { return _limitlvl; } }

        public int Exp { get { return _exp; } }
        /// <summary>Returns the TOTAL experience required to get to the next level</summary>
        public int NextLevel
        {
            get
            {
                int mod = Q(_level + 1);
                int xp = 0;
                for (int i = 1; i <= _level; i++)
                    xp = xp + (mod * i * i / 10);
                return xp;
            }
        }
        /// <summary>Returns the REMAINING experience required to get to the next level</summary>
        public int ToNextLevel
        { get { return NextLevel - _exp; } }
        
        public int HP
        {
            get
            {
                if (_hp > MaxHP)
                    _hp = MaxHP;
                return _hp; 
            }
            set
            {
                _hp = value;
                if (_hp > MaxHP)
                    _hp = MaxHP;
                if (_hp < 0)
                    _hp = 0;
            }
        }

        public int MP
        {
            get
            {
                if (_mp > MaxMP)
                    _mp = MaxMP;
                return _mp; 
            }
            set
            {
                _mp = value;
                if (_mp > MaxMP)
                    _mp = MaxMP;
                if (_mp < 0)
                    _mp = 0;
            }
        }

        public int MaxHP
        {
            get
            {
                int temp = _maxhp;
//                foreach (Materia m in Weapon.Slots)
//                    if (m != null)
//                        temp += temp * m.HPMod / 100;
//                foreach (Materia m in Armor.Slots)
//                    if (m != null)
//                        temp += temp * m.HPMod / 100;
                temp += temp * _hp_p_bonus / 100;
                return temp;
            }
        }

        public int MaxMP
        {
            get
            {
                int temp = _maxmp;
//                foreach (Materia m in Weapon.Slots)
//                    if (m != null)
//                        temp += temp * m.MPMod / 100;
//                foreach (Materia m in Armor.Slots)
//                    if (m != null)
//                        temp += temp * m.MPMod / 100;
                temp += temp * _mp_p_bonus / 100;
                return temp;
            }
        }

        public bool Sadness { get { return _sadness; } }

        public bool Fury { get { return _fury; } }

        public bool NearDeath { get { return HP <= (MaxHP / 4); } }

        public bool Death { get { return _death; } }
        
        public Weapon Weapon
        {
            get { return _weapon; }
            set
            {
                if (value == null)
                {
                    throw new ImplementationException("Cannot remove weapon. Must replace with new weapon.");
                }
                //_weapon.Detach(this);
                _weapon = value;
                //_weapon.Attach(this);
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
                //_armor.Detach(this);
                _armor = value;
                //_armor.Attach(this);
            }
        }

        public Accessory Accessory
        {
            get { return _accessory; }
            set
            {
//                if (_accessory  != null)
//                    _accessory.Detach(this);
                _accessory = value;
//                if (_accessory  != null)
//                    _accessory.Attach(this);
            }
        }
        //public IEnumerable<Materia> Materia { get { return Weapon.Slots.Union(Armor.Slots); } }
        
        public string Name { get { return _name; } }

        public Sex Sex { get { return _sex; } }
        
        #endregion Properties

        

        public bool BackRow { get; set; }
        
        public Pixbuf Profile { get; private set; }
        
        public Pixbuf ProfileSmall { get; private set; }
    }
}
