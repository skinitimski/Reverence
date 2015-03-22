using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.XPath;

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
            
//            if (HP == 0)
//            {
//                InflictDeath();
//            }
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
        
        public override void AcceptDamage(Combatant attacker, int delta)
        {
            // limit shtuff goes here
            
//            if (Seven.BattleState.ActiveAbility.Type == AttackType.Physical)
//            {
//                if (Sleep)
//                    CureSleep();
//                if (Confusion)
//                    CureConfusion();
//            }
            
            
            _c.HP -= delta;
            if (_c.HP < 0)
                _c.HP = 0;
//            if (_c.HP == 0)
//                InflictDeath();
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
            if (Sleep)
                g.Color = new Cairo.Color(.05, .4, .05);
            else if (Poison)
                g.Color = new Cairo.Color(0, 1, .4);
            else
                g.Color = new Cairo.Color(1, 1, 1);
            g.Rectangle(X, Y, 20, 20);
            g.Fill();
        }
        
        
        #endregion Methods
        
        
        
        #region AI
        
        protected override void ConfuAI()
        {
            while (true)
            {
                if (TurnTimer.IsUp)
                {
                    Ally attackee;
                    int i = Seven.BattleState.Random.Next(3);
                    
                    while (Seven.BattleState.Allies[i] == null)
                        i = (i + 1) % 3;
                    attackee = Seven.BattleState.Allies[i];
                    
                    int bd = Formula.PhysicalBase(this);
                    int dam = Formula.PhysicalDamage(bd, 16, attackee);
                    
//                    _abilityState.LongRange = LongRange;
//                    _abilityState.QuadraMagic = false;
//                    _abilityState.Type = AttackType.Physical;
//                    _abilityState.Performer = this;
//                    _abilityState.Target = new Combatant[1];
//                    _abilityState.Target[0] = attackee;
//                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };
//                    
//                    Seven.BattleState.EnqueueAction(_abilityState);
                }
                else 
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
        
        protected override void BerserkAI()
        {
            while (true)
            {
                if (TurnTimer.IsUp)
                {
                    Enemy attackee;
                    int i = Seven.BattleState.Random.Next(Seven.BattleState.EnemyList.Count);
                    
                    attackee = Seven.BattleState.EnemyList[i];
                    
                    int bd = Formula.PhysicalBase(this);
                    int dam = Formula.PhysicalDamage(bd, 16, attackee);
                    
//                    _abilityState.LongRange = LongRange;
//                    _abilityState.QuadraMagic = false;
//                    _abilityState.Type = AttackType.Physical;
//                    _abilityState.Performer = this;
//                    _abilityState.Target = new Combatant[1];
//                    _abilityState.Target[0] = attackee;
//                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };
//                    
//                    Seven.BattleState.EnqueueAction(_abilityState);
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }
        
        #endregion AI
        
        
        
        #region Inflict Status
//        public override bool InflictDeath()
//        {
//            if (_c.Immune(Status.Death))
//                return false;
//            if (DeathForce || Peerless || Petrify || Resist)
//                return false;
//            TurnTimer.Reset();
//            PauseTimers();
//            return _c.InflictDeath();
//        }
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
        public override bool InflictSleep()
        {
            if (_c.Immune(Status.Sleep))
                return false;
            if (Sleep || Petrify || Peerless || Resist) return false;
            Sleep = true;
            _sleepTime = V_Timer.TotalMilliseconds;
            TurnTimer.Pause();
            return true;
        }
        public override bool InflictPoison()
        {
            if (_c.Immune(Status.Poison))
                return false;
            if (Poison) return false;
            Poison = true;
            PoisonTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictConfusion()
        {
            if (_c.Immune(Status.Confusion))
                return false;
            if (Confusion || Petrify || Peerless || Resist)
                return false;
            Confusion = true;
            _confuThread = new System.Threading.Thread(new ThreadStart(ConfuAI));
            _confuThread.Start();
            return true;
        }
        public override bool InflictSilence()
        {
            if (_c.Immune(Status.Silence))
                return false;
            if (Silence || Petrify || Peerless || Resist)
                return false;
            Silence = true;
            return true;
        }
        public override bool InflictHaste()
        {
            if (_c.Immune(Status.Haste) || _c.Immune(Status.Slow))
                return false;
            if (Haste || Petrify || Peerless || Resist)
                return false;
            if (Slow)
                CureSlow();
           Haste = true;
            DoubleTimers();
            return true;
        }
        public override bool InflictSlow()
        {
            if (_c.Immune(Status.Slow) || _c.Immune(Status.Haste))
                return false;
            if (Slow || Petrify || Peerless || Resist)
                return false;
            if (Haste)
                CureHaste();
            Slow = true;
            HalveTimers();
            return true;
        }
        public override bool InflictStop()
        {
            if (_c.Immune(Status.Stop))
                return false;
            if (Stop || Petrify || Peerless || Resist)
                return false;
           Stop = true;
            PauseTimers();
            return true;
        }
        public override bool InflictFrog()
        {
            if (_c.Immune(Status.Frog))
                return false;
            if (Frog || Petrify || Peerless || Resist)
                return false;
            Frog = true;
            return true;
        }
        public override bool InflictSmall()
        {
            if (_c.Immune(Status.Small))
                return false;
            if (Small || Petrify || Peerless || Resist)
                return false;
            Small = true;
            return true;
        }
        public override bool InflictSlowNumb()
        {
            if (_c.Immune(Status.SlowNumb))
                return false;
            if (SlowNumb || Petrify || Peerless || Resist)
                return false;
            SlowNumb = true;
            _slownumbTime = C_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictPetrify()
        {
            if (_c.Immune(Status.Petrify))
                return false;
            if (Petrify || Peerless || Resist)
                return false;
            Petrify = true;
            return true;
        }
        public override bool InflictRegen()
        {
            if (_c.Immune(Status.Regen))
                return false;
            if (Regen || Petrify || Peerless || Resist)
                return false;
            Regen = true;
            _regenTimeEnd = V_Timer.TotalMilliseconds;
            _regenTimeInt = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictBarrier()
        {
            if (_c.Immune(Status.Barrier))
                return false;
            if (Barrier || Petrify || Peerless || Resist)
                return false;
            Barrier = true;
            _barrierTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictMBarrier()
        {
            if (_c.Immune(Status.MBarrier))
                return false;
            if (MBarrier || Petrify || Peerless || Resist)
                return false;
            MBarrier = true;
            _mbarrierTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictReflect()
        {
            if (_c.Immune(Status.Reflect))
                return false;
            if (Reflect || Petrify || Peerless || Resist)
                return false;
            Reflect = true;
            return true;
        }
        public override bool InflictShield()
        {
            if (_c.Immune(Status.Shield))
                return false;
            if (Shield || Petrify || Resist)
                return false;
            Shield = true;
            _shieldTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictDeathSentence()
        {
            if (_c.Immune(Status.DeathSentence))
                return false;
            if (DeathSentence || Petrify || Peerless || Resist || DeathForce)
                return false;
            DeathSentence = true;
            _deathsentenceTime = C_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictManipulate()
        {
            return false;
        }
        public override bool InflictBerserk()
        {
            if (_c.Immune(Status.Berserk))
                return false;
            if (Berserk || Petrify || Peerless || Resist)
                return false;
            Berserk = true;
            _berserkThread = new Thread(new ThreadStart(BerserkAI));
            _berserkThread.Start();
            return true;
        }
        public override bool InflictPeerless()
        {
            if (_c.Immune(Status.Peerless))
                return false;
            if (Peerless || Petrify || Resist)
                return false;
            Peerless = true;
            _peerlessTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictParalyzed()
        {
            if (_c.Immune(Status.Paralyzed))
                return false;
            if (Paralysed || Petrify || Peerless || Resist)
                return false;
            Paralysed = true;
            _paralyzedTime = V_Timer.TotalMilliseconds;
            TurnTimer.Pause();
            return true;
        }
        public override bool InflictDarkness()
        {
            if (_c.Immune(Status.Darkness))
                return false;
            if (Darkness || Petrify || Peerless || Resist)
                return false;
            Darkness = true;
            return true;
        }
        public override bool InflictSeizure()
        {
            if (_c.Immune(Status.Seizure))
                return false;
            if (Seizure || Petrify || Peerless || Resist)
                return false;
            Seizure = true;
            _seizureTimeEnd = V_Timer.TotalMilliseconds;
            _seizureTimeInt = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictDeathForce()
        {
            if (_c.Immune(Status.DeathForce))
                return false;
            if (DeathForce || Petrify || Peerless || Resist)
                return false;
            DeathForce = true;
            return true;
        }
        public override bool InflictResist()
        {
            if (_c.Immune(Status.Resist))
                return false;
            if (Resist || Petrify || Peerless)
                return false;
            Resist = true;
            return true;
        }
        public override bool InflictLuckyGirl()
        {
            if (_c.Immune(Status.LuckyGirl))
                return false;
            if (LuckyGirl || Petrify || Peerless || Resist)
                return false;
            LuckyGirl = true;
            return true;
        }
        public override bool InflictImprisoned()
        {
            if (_c.Immune(Status.Imprisoned))
                return false;
            if (Imprisoned || Petrify)
                return false;
            Imprisoned = true;
            PauseTimers();
            return true;
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
        
        
//        public override bool CureDeath()
//        {
//            CureAll();
//            UnpauseTimers();
//            return _c.CureDeath();
//        }
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
            if (_confuThread != null && _confuThread.IsAlive)
                _confuThread.Abort();
            if (_berserkThread != null && _berserkThread.IsAlive)
                _berserkThread.Abort();
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
        public override int MP { get { return _c.MP; } set { _c.MP = value; } }
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
                if (Weapon.LongRange)
                    return true;
                foreach (MateriaBase m in Materia)
                    if (m != null && m.ID == "longrange")
                        return true;
                return false;
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

