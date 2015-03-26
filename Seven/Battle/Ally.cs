using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Screen.BattleState;
using Screens = Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class Ally : Combatant
    {
        
        #region Member Data
        
        private int _attack_mod;
        private int _defense_mod;
        private int _magicAttack_mod;
        private int _magicDefense_mod;
        private int _defensePercent_mod;
        private int _dexterity_mod;
        
        private Character _c;
        
        private List<MagicSpell> _magicSpells;
        private List<Summon> _summons;
                
        #endregion Member Data
        
        
        public Ally(Character c, int x, int y, int e)
            : base()
        {
            _c = c;
            
            //_abilityState = new AbilityState();
            
            int vStep = Seven.Party.BattleSpeed;
            
            C_Timer = new Clock(Clock.TICKS_PER_MS);
            V_Timer = new Clock(vStep);
            TurnTimer = new Time.Timer(6000, Seven.Party.TurnTimerSpeed(c, vStep), e, true);
            
            _x = x;
            _y = y;
            
            GetMagicSpells();
            GetSummons();
            
            BattleMenu = new BattleMenu(this);
            
            MagicMenu = new Screens.Magic.Main(MagicSpells, BattleMenu.WMagic);
            if (!MagicMenu.IsValid)
            {
                MagicMenu = null;
            }
            
            SummonMenu = new Screens.Summon.Main(Summons, BattleMenu.WSummon);
            if (!SummonMenu.IsValid)
            {
                SummonMenu = null;
            }
            
            EnemySkillMateria m = new EnemySkillMateria(GetEnemySkillMask());
            if (m.AP > 0)
            {
                EnemySkillMenu = new Screens.EnemySkill.Main(m);
            }
            
            if (HP == 0)
            {
                InflictDeath();
            }
        }
        
        
        private void GetMagicSpells(SlotHolder sh, List<MagicSpell> list)
        {
            for (int i = 0; i < sh.Links; i++)
            {
                MateriaBase left = sh.Slots[i * 2];
                MateriaBase right = sh.Slots[(i * 2) + 1];
                
                if (left is MagicMateria && right is SupportMateria)
                {
                    MateriaBase temp = left;
                    left = right;
                    right = temp;
                }
                if (right is MagicMateria && left is SupportMateria)
                {
                    foreach (Spell s in ((MagicMateria)right).GetSpells)
                    {
                        bool replace = false;
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].ID == s.ID)
                            {
                                replace = true;
                                MagicSpell ms = list[j];
                                ms.AddAbility((SupportMateria)left);
                                list[j] = ms;
                            }
                        }
                        if (!replace)
                        {
                            MagicSpell t = new MagicSpell(s);
                            t.AddAbility((SupportMateria)left);
                            list.Add(t);
                        }
                    }
                }
                else
                {
                    if (right is MagicMateria)
                        foreach (Spell s in ((MagicMateria)right).GetSpells)
                            list.Add(new MagicSpell(s));
                    if (left is MagicMateria)
                        foreach (Spell s in ((MagicMateria)left).GetSpells)
                            list.Add(new MagicSpell(s));
                }
            }
            for (int j = sh.Links * 2; j < sh.Slots.Length; j++)
            {
                MateriaBase m = sh.Slots[j];
                if (m is MagicMateria)
                    foreach (Spell s in ((MagicMateria)m).GetSpells)
                        list.Add(new MagicSpell(s));
            }
        }
        private void GetMagicSpells()
        {
            List<MagicSpell> spells = new List<MagicSpell>();
            
            GetMagicSpells(Weapon, spells);
            GetMagicSpells(Armor, spells);
            
            spells.Sort(MagicSpell.Compare);
            _magicSpells = spells;
        }
        
        private void GetSummons(SlotHolder sh, List<Summon> list)
        {
            for (int i = 0; i < sh.Links; i++)
            {
                MateriaBase left = sh.Slots[i * 2];
                MateriaBase right = sh.Slots[(i * 2) + 1];
                
                if (left is SummonMateria && right is SupportMateria)
                {
                    MateriaBase temp = left;
                    left = right;
                    right = temp;
                }
                if (right is SummonMateria && left is SupportMateria)
                {
                    bool replace = false;
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].Name == right.Name)
                        {
                            replace = true;
                            Summon ss = list[j];
                            ss.AddAbility((SupportMateria)left);
                            list[j] = ss;
                        }
                    }
                    if (!replace)
                    {
                        Summon t = new Summon(right.Name, ((SummonMateria)right));
                        t.AddAbility((SupportMateria)left);
                        list.Add(t);
                    }
                }
                else
                {
                    if (right is SummonMateria)
                        list.Add(new Summon(right.Name, ((SummonMateria)right)));
                    if (left is SummonMateria)
                        list.Add(new Summon(left.Name, ((SummonMateria)left)));
                }
            }
            for (int j = sh.Links * 2; j < sh.Slots.Length; j++)
            {
                MateriaBase m = sh.Slots[j];
                if (m is SummonMateria)
                    list.Add(new Summon(m.Name, (SummonMateria)m));
            }
        }
        private void GetSummons()
        {
            List<Summon> summons = new List<Summon>();
            
            GetSummons(Weapon, summons);
            
            GetSummons(Armor, summons);
            
            summons.Sort(Summon.Compare);
            _summons = summons;
        }
        
        private int GetEnemySkillMask()
        {
            int ap = 0;
            foreach (MateriaBase m in Materia)
                if (m is EnemySkillMateria)
            {
                ap = ap | m.AP;
            }
            return ap;
        }
        

        
        #region Methods
        
        public override void AcceptDamage(int delta, AttackType type = AttackType.None)
        {
            Seven.BattleState.AddDamageIcon(delta, this);

            // limit shtuff goes here
            
            if (type == AttackType.Physical)
            {
                if (Sleep)
                {
                    CureSleep();
                }
                if (Confusion)
                {
                    CureConfusion();
                }
            }
                        
            _c.HP -= delta;

            if (HP == 0)
            {
                Kill();
            }
        }
        
        public override void AcceptMPLoss(int delta)
        {
            Seven.BattleState.AddDamageIcon(delta, this, true);

            _c.MP -= delta;
        }
        
        public override void UseMP(int amount)
        {
            if (_c.MP - amount < 0)
            {
                throw new ImplementationException("Used more MP than I had -- " + Name);
            }
            _c.MP -= amount;
        }
        
        public override string ToString()
        {
            return String.Format("{0} - HP:{1}/{2} MP:{3}/{4} - Time: {5}/{6}",
                                 Name, HP, MaxHP, MP, MaxMP, 
                                 (TurnTimer.IsUp) ? TurnTimer.Timeout : TurnTimer.TotalMilliseconds,
                                 TurnTimer.Timeout);
        }
        
        public override void Draw(Cairo.Context g)
        {
            Cairo.Color iconColor = Colors.GRAY_8;

            if (Sleep)
            {
                iconColor = new Cairo.Color(.05, .4, .05);
            }
            else if (Poison)
            {
                iconColor = new Cairo.Color(0, 1, .4);
            }

            int iconSize = 20;

            g.Color = iconColor;
            g.Rectangle(X - iconSize / 2, Y- iconSize / 2, iconSize, iconSize);
            g.Fill();
        }
        
        #endregion Methods
        
        
        
        #region AI
        
        public void RunAIConfu()
        {
            Ally target;
            int i = Seven.BattleState.Random.Next(3);
                    
            while (Seven.BattleState.Allies[i] == null)
            {
                i = (i + 1) % 3;
            }

            target = Seven.BattleState.Allies[i];

            PhysicalAttack(16, Atkp, target, new Element[] { Weapon.Element });
        }
        
        public void RunAIBerserk()
        {
            Enemy target;

            int i = Seven.BattleState.Random.Next(Seven.BattleState.EnemyList.Count);
                    
            target = Seven.BattleState.EnemyList[i];
            
            PhysicalAttack(16, Atkp, target, new Element[] { Weapon.Element });
        }
        
        #endregion AI
        

        private void Kill()
        {
            TurnTimer.Reset();
            PauseTimers();

            // if we're running kill, then we just set HP == 0, 
            //   so it's already been run on the character
            //_c.Kill();
        }


        #region Inflict Status

        public override bool InflictDeath()
        {
            if (_c.Immune(Status.Death))
                return false;
            if (DeathForce || Peerless || Petrify || Resist)
                return false;
            TurnTimer.Reset();
            PauseTimers();
            return _c.InflictDeath();
        }

        public override bool InflictFury()
        {
            if (_c.Immune(Status.Fury))
                return false;
            return _c.InflictFury();
        }

        public override bool InflictSadness()
        {
            if (_c.Immune(Status.Sadness))
                return false;
            return _c.InflictSadness();
        }
                
        public override bool InflictManipulate()
        {
            return false;
        }

        #endregion Inflict
        
        
        
        #region Cure Status
        
        private void CureAll()
        {
            CureFury();
            CureSadness();
            CureSleep();
            CurePoison();
            CureConfusion();
            CureSilence();
            CureHaste();
            CureSlow();
            CureStop();
            CureFrog();
            CureSmall();
            CureSlowNumb();
            CurePetrify();
            CureRegen();
            CureBarrier();
            CureMBarrier();
            CureReflect();
            CureShield();
            CureDeathSentence();
            CureManipulate();
            CureBerserk();
            CurePeerless();
            CureParalyzed();
            CureDarkness();
            CureSeizure();
            CureDeathForce();
            CureResist();
            CureLuckyGirl();
            CureImprisoned();
        }
        
        
        public override bool CureDeath()
        {
            CureAll();
            UnpauseTimers();
            return _c.CureDeath();
        }

        public override bool CureFury()
        {
            return _c.CureFury();
        }
        public override bool CureSadness()
        {
            return _c.CureSadness();
        }
        
        #endregion Cure Status
        
        
        
        public static int Attack(Character c)
        {
            return c.Strength + c.Weapon.Attack;
        }
        public static int AttackPercent(Character c)
        {
            return c.Weapon.AttackPercent;
        }
        public static int Defense(Character c)
        {
            return c.Vitality + c.Armor.Defense;
        }
        public static int DefensePercent(Character c)
        {
            return (c.Dexterity / 4) + c.Armor.DefensePercent;
        }
        public static int MagicAttack(Character c)
        {
            return c.Magic;
        }
        public static int MagicDefense(Character c)
        {
            return c.Spirit + c.Armor.MagicDefense;
        }
        public static int MagicDefensePercent(Character c)
        {
            return c.Armor.MagicDefensePercent;
        }
        
        
        public override void Dispose()
        {
        }
        
        public override bool Weak(params Element[] e)
        {
            return false;
        }
        public override bool Halves(params Element[] e)
        {
            return _c.Halves(e);
        }
        public override bool Voids(params Element[] e)
        {
            return _c.Voids(e);
        }
        public override bool Absorbs(params Element[] e)
        {
            return _c.Absorbs(e);
        }
        public override bool Immune(Status s)
        {
            return _c.Immune(s);
        }
        
        
        
        #region Properties
        
        public override int Atk { get { return Attack(_c) + (_attack_mod * Attack(_c) / 100); } }
        public int Atkp
        {
            get
            {
                int atkp = AttackPercent(_c);
                if (Darkness)
                    return atkp / 2;
                else return atkp;
            }
        }
        public override int Def { get { return Defense(_c) + (_defense_mod * Attack(_c) / 100); } }
        public override int Defp { get { return DefensePercent(_c) + (_defensePercent_mod * Attack(_c) / 100); } }
        public override int Mat { get { return MagicAttack(_c) + (_magicAttack_mod * Attack(_c) / 100); } }
        public override int MDef { get { return MagicDefense(_c) + (_magicDefense_mod * Attack(_c) / 100); } }
        public override int MDefp { get { return MagicDefensePercent(_c); } }
        
        public override int Level { get { return _c.Level; } }
        
        public override int HP { get { return _c.HP; } }
        public override int MP { get { return _c.MP; } }
        public override int MaxHP { get { return _c.MaxHP; } }
        public override int MaxMP { get { return _c.MaxMP; } }
        
        public int Strength { get { return _c.Strength; } }
        public int Vitality { get { return _c.Vitality; } }
        public override int Dexterity { get { return _c.Dexterity + (_dexterity_mod * _c.Dexterity / 100); } }
        public int Magic { get { return _c.Magic; } }
        public int Spirit { get { return _c.Spirit; } }
        public override int Luck { get { return _c.Luck; } }
        
        public int AttackMod { get { return _attack_mod; } set { _attack_mod = value; } }
        public int DefenseMod { get { return _defense_mod; } set { _defense_mod = value; } }
        public int DefensePercentMod { get { return _defensePercent_mod; } set { _defensePercent_mod = value; } }
        public int MagicAttackMod { get { return _magicAttack_mod; } set { _magicAttack_mod = value; } }
        public int MagicDefenseMod { get { return _magicDefense_mod; } set { _magicDefense_mod = value; } }
        public int DexterityMod { get { return _dexterity_mod; } set { _dexterity_mod = value; } }
        
        
        
        public override string Name { get { return _c.Name; } }
        
        public override bool LongRange
        {
            get
            {
                bool longRange = false;

                if (Weapon.LongRange)
                {
                    longRange = true;
                }
                else
                {
                    foreach (MateriaBase m in Materia)
                    {
                        if (m != null && m.ID == "longrange")
                        {
                            longRange = true;
                            break;
                        }
                    }
                }

                return longRange;
            }
        }
        public override bool Sensed { get { return true; } }
        public override bool BackRow { get { return _c.BackRow; } }
        public override bool Death { get { return _c.Death; } }
        public override bool NearDeath { get { return _c.NearDeath; } }
        public override bool Sadness { get { return _c.Sadness; } }
        public override bool Fury { get { return _c.Fury; } }
        public bool IsDead { get { return Petrify || Imprisoned || Death; } }
        public bool CannotAct { get { return IsDead || Sleep || Berserk || Confusion || Paralysed || Imprisoned; } }
        
        
        public List<MagicSpell> MagicSpells { get { return _magicSpells; } }
        public List<Summon> Summons { get { return _summons; } }
        
        public BattleMenu BattleMenu { get; private set; }
        public Screens.Magic.Main MagicMenu  { get; private set; }
        public Screens.Summon.Main SummonMenu  { get; private set; }
        public Screens.EnemySkill.Main EnemySkillMenu { get; private set; }

        public Weapon Weapon { get { return _c.Weapon; } }
        public Armor Armor { get { return _c.Armor; } }
        public IEnumerable<MateriaBase> Materia { get { return _c.Materia; } }
        
        #endregion Properties
        
    }
}

