using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Cairo;

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
                
        private Character _c;
        private List<MagicMenuEntry> _magicSpells;
        private List<SummonMenuEntry> _summons;
                
        #endregion Member Data

        private Ally()
            : base()
        {
        }
        
        public Ally(Character c, int x, int y, int e)
            : this()
        {
            _c = c;
            
            //_abilityState = new AbilityState();

            int vStep = Seven.Party.BattleSpeed;
            int tStep = (Dexterity + 50) * vStep / Seven.Party.NormalSpeed();
            
            C_Timer = new Clock();
            V_Timer = new Clock(vStep);
            TurnTimer = new Time.Timer(TURN_TIMER_TIMEOUT, tStep, e, false);
            
            _x = x;
            _y = y;
            
            GetMagicSpells();
            GetSummons();
            
            BattleMenu = new BattleMenu(this);

            if (BattleMenu.WMagic)
            {
                MagicMenu = new Screens.Magic.WMagic(MagicSpells);
            }
            else
            {
                MagicMenu = new Screens.Magic.Main(MagicSpells);
            }

            if (!MagicMenu.IsValid)
            {
                MagicMenu = null;
            }

            if (BattleMenu.WSummon)
            {
                SummonMenu = new Screens.Summon.WSummon(Summons);
            }
            else
            {
                SummonMenu = new Screens.Summon.Main(Summons);
            }

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
        
        private void GetMagicSpells(SlotHolder sh, List<MagicMenuEntry> list)
        {
            // First, add all the new magic spells.
            foreach (MateriaOrb m in sh.Slots)
            {
                if (m is MagicMateria)
                {
                    foreach (Spell s in ((MagicMateria)m).GetSpells)
                    {
                        if (!list.Any(x => x.ID == m.Name))
                        {
                            list.Add(new MagicMenuEntry(s));
                        }
                    }
                }
            }

            for (int i = 0; i < sh.Links; i++)
            {
                MateriaOrb left = sh.Slots[i * 2];
                MateriaOrb right = sh.Slots[(i * 2) + 1];
                
                if (left is MagicMateria && right is SupportMateria)
                {
                    MateriaOrb temp = left;
                    left = right;
                    right = temp;
                }

                if (right is MagicMateria && left is SupportMateria)
                {
                    foreach (Spell s in ((MagicMateria)right).GetSpells)
                    {
                        for (int j = 0; j < list.Count; j++)
                        {
                            if (list[j].ID == s.ID)
                            {
                                list[j].AddAbility((SupportMateria)left);
                            }
                        }
                    }
                }
            }
        }

        private void GetMagicSpells()
        {
            List<MagicMenuEntry> spells = new List<MagicMenuEntry>();
            
            GetMagicSpells(Weapon, spells);
            GetMagicSpells(Armor, spells);
            
            spells.Sort(MagicMenuEntry.Compare);
            _magicSpells = spells;
        }
        
        private void GetSummons(SlotHolder sh, List<SummonMenuEntry> list)
        {    
            // First, add all the new summons.
            foreach (MateriaOrb m in sh.Slots)
            {
                if (m is SummonMateria)
                {
                    if (!list.Any(x => x.Name == m.Name))
                    {
                        foreach (Spell s in ((SummonMateria)m).GetSpells)
                        {
                            list.Add(new SummonMenuEntry(m.Name, s));
                        }
                    }
                }
            }

            // Then, go through and attach any support abilities
            for (int i = 0; i < sh.Links; i++)
            {
                MateriaOrb left = sh.Slots[i * 2];
                MateriaOrb right = sh.Slots[(i * 2) + 1];

                // If we have a summon-support pair, swap 'em
                if (left is SummonMateria && right is SupportMateria)
                {
                    MateriaOrb temp = left;
                    left = right;
                    right = temp;
                }

                // Now if we have a support-summon pair (and, inclusively, if we previously had
                //     a summon-support pair), match 'em up
                if (right is SummonMateria && left is SupportMateria)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if (list[j].Name == right.Name)
                        {
                            list[j].AddAbility((SupportMateria)left);
                        }
                    }
                }
            }
        }

        private void GetSummons()
        {
            List<SummonMenuEntry> summons = new List<SummonMenuEntry>();
            
            GetSummons(Weapon, summons);
            
            GetSummons(Armor, summons);
            
            summons.Sort(SummonMenuEntry.Compare);
            _summons = summons;
        }
        
        private int GetEnemySkillMask()
        {
            int ap = 0;

            foreach (MateriaOrb m in Materia)
            {
                if (m is EnemySkillMateria)
                {
                    ap = ap | m.AP;
                }
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
            g.Rectangle(X - iconSize / 2, Y - iconSize / 2, iconSize, iconSize);
            g.Fill();

            Text.ShadowedText(g, NameColor, Name, X + iconSize, Y, Text.MONOSPACE_FONT, 18);
        }
        
        #endregion Methods
        



        public void Attack(Combatant target, bool resetTurnTimer = true)
        {
            PhysicalAttack(16, Atkp, target, new Element[] { Weapon.Element }, resetTurnTimer);
        }















        
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
            {
                return false;
            }
            if (DeathForce || Peerless || Petrify || Resist)
            {
                return false;
            }
            TurnTimer.Reset();
            PauseTimers();
            return _c.InflictDeath();
        }

        public override bool InflictFury()
        {
            if (_c.Immune(Status.Fury))
            {
                return false;
            }
            return _c.InflictFury();
        }

        public override bool InflictSadness()
        {
            if (_c.Immune(Status.Sadness))
            {
                return false;
            }
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
            return c.Magic + c.Weapon.Magic;
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

        public override bool Weak(Element e)
        {
            return false;
        }

        public override bool Weak(IEnumerable<Element> elements)
        {
            return false;
        }

        public override bool Halves(IEnumerable<Element> elements)
        {
            return _c.Halves(elements);
        }

        public override bool Halves(Element e)
        {
            return _c.Halves(e);
        }
       
        public override bool Voids(IEnumerable<Element> elements)
        {
            return _c.Voids(elements);
        }

        public override bool Voids(Element e)
        {
            return _c.Voids(e);
        }

        public override bool Absorbs(IEnumerable<Element> elements)
        {
            return _c.Absorbs(elements);
        }

        public override bool Absorbs(Element e)
        {
            return _c.Absorbs(e);
        }

        public override bool Immune(Status s)
        {
            return _c.Immune(s);
        }
        
        
        
        #region Properties
        
        public override int Atk { get { return Attack(_c) + (AttackMod * Attack(_c) / 100); } }

        public int Atkp
        {
            get
            {
                int atkp = AttackPercent(_c);
                if (Darkness)
                {
                    return atkp / 2;
                }
                else
                {
                    return atkp;
                }
            }
        }

        public override int Def { get { return Defense(_c) + (DefenseMod * Attack(_c) / 100); } }

        public override int Defp { get { return DefensePercent(_c) + (DefensePercentMod * Attack(_c) / 100); } }

        public override int Mat { get { return MagicAttack(_c) + (MagicAttackMod * Attack(_c) / 100); } }

        public override int MDef { get { return MagicDefense(_c) + (MagicDefenseMod * Attack(_c) / 100); } }

        public override int MDefp { get { return MagicDefensePercent(_c); } }
        
        public override int Level { get { return _c.Level; } }
        
        public override int HP { get { return _c.HP; } }

        public override int MP { get { return _c.MP; } }

        public override int MaxHP { get { return _c.MaxHP; } }

        public override int MaxMP { get { return _c.MaxMP; } }
        
        public int Strength { get { return _c.Strength; } }

        public int Vitality { get { return _c.Vitality; } }

        public override int Dexterity { get { return _c.Dexterity + (DexterityMod * _c.Dexterity / 100); } }

        public int Magic { get { return _c.Magic; } }

        public int Spirit { get { return _c.Spirit; } }

        public override int Luck { get { return _c.Luck; } }
        
        public int AttackMod  { get; set; }

        public int DefenseMod  { get; set; }

        public int DefensePercentMod  { get; set; }

        public int MagicAttackMod  { get; set; }

        public int MagicDefenseMod  { get; set; }

        public int DexterityMod { get; set; }
        
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
                    foreach (MateriaOrb m in Materia)
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
        
        public List<MagicMenuEntry> MagicSpells { get { return _magicSpells; } }

        public List<SummonMenuEntry> Summons { get { return _summons; } }
        
        public BattleMenu BattleMenu { get; private set; }

        public Screens.Magic.Main MagicMenu  { get; private set; }

        public Screens.Summon.Main SummonMenu  { get; private set; }

        public Screens.EnemySkill.Main EnemySkillMenu { get; private set; }

        public Weapon Weapon { get { return _c.Weapon; } }

        public Armor Armor { get { return _c.Armor; } }

        public IEnumerable<MateriaOrb> Materia { get { return _c.Materia; } }
        
        #endregion Properties
        
    }
}

