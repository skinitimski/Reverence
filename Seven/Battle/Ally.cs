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
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle.Time;
using Atmosphere.Reverence.Seven.Graphics;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Screen.BattleState;
using Atmosphere.Reverence.Seven.State;
using Screens = Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class Ally : Combatant
    {
        private const int xs = 30;
        private const int X_FRONT_ROW = 550;
        private const int X_BACK_ROW = X_FRONT_ROW + xs;


        private const int y0 = 125;
        private const int ys = 250 / Party.PARTY_SIZE;

        private Character _c;
        private List<MagicMenuEntry> _magicSpells;
        private List<SummonMenuEntry> _summons;

        private int _x;
        private int _y;


        private class WeaponAttack : Ability
        {
            public WeaponAttack(Ally ally)
                : this(ally, 1)
            {
            }

            public WeaponAttack(Ally ally, int hits)
            {
                Name = "";
                Desc = "";
                
                Type = AttackType.Physical;

                if (hits > 2)
                {
                    Target = BattleTarget.GroupRandom;
                }
                else
                {
                    Target = BattleTarget.Combatant;
                }
                
                Elements = new Element[] { ally.Weapon.Element };
                
                Power = 16;
                Hitp = ally.Atkp;
                Hits = hits;

                DamageFormula = PhysicalAttack;
                HitFormula = PhysicalHit;
            }

            public override string GetMessage(Combatant source)
            {
                return source.Name + " attacks";
            }
        }
        
        public Ally(BattleState battle, int index, int e)
            : base(battle)
        {
            _c = battle.Party[index];
            
            // the (index * 1) is for separation for targeting purposes.
            _x = (index * 1) + (_c.BackRow ? X_BACK_ROW : X_FRONT_ROW);
            _y = (index * 1) + y0 + (index * ys);
                        
            GetMagicSpells();
            GetSummons();
                        
            if (HP == 0)
            {
                InflictDeath(null);
            }

            V_Timer = new BattleClock(battle.SpeedValue * 2);
            TurnTimer = new AllyTurnTimer(this, e);
            
            PrimaryAttack = new WeaponAttack(this);
            PrimaryAttackX2 = new WeaponAttack(this, 2);
            PrimaryAttackX4 = new WeaponAttack(this, 4);
        }

        internal void InitMenu(ScreenState state)
        {
            BattleMenu = new BattleMenu(this, state);
            
            if (BattleMenu.WMagic)
            {
                MagicMenu = new Screens.Magic.WMagic(CurrentBattle, MagicSpells, state);
            }
            else
            {
                MagicMenu = new Screens.Magic.Main(CurrentBattle, MagicSpells, state);
            }
            
            if (!MagicMenu.IsValid)
            {
                MagicMenu = null;
            }
            
            if (BattleMenu.WSummon)
            {
                SummonMenu = new Screens.Summon.WSummon(CurrentBattle, Summons, state);
            }
            else
            {
                SummonMenu = new Screens.Summon.Main(CurrentBattle, Summons, state);
            }
            
            if (!SummonMenu.IsValid)
            {
                SummonMenu = null;
            }
            
            EnemySkillMateria m = EnemySkillMateria.Merge(Materia.Where(x => x is EnemySkillMateria).Cast<EnemySkillMateria>());

            if (m.AP > 0)
            {
                EnemySkillMenu = new Screens.EnemySkill.Main(CurrentBattle, m, state);
            }
        }
        
        private void GetMagicSpells(SlotHolder sh, List<MagicMenuEntry> list)
        {
            // First, add all the new magic spells.
            foreach (MateriaOrb m in sh.Slots)
            {
                if (m != null && m.Type == MateriaType.Magic)
                {
                    foreach (string ability in m.Abilities)
                    {
                        Spell magicSpell = CurrentBattle.Seven.Data.GetMagicSpell(ability);

                        if (!list.Any(x => x.Name == magicSpell.Name))
                        {
                            list.Add(new MagicMenuEntry(magicSpell));
                        }
                    }
                }
            }

            for (int i = 0; i < sh.Links; i++)
            {
                MateriaOrb left = sh.Slots[i * 2];
                MateriaOrb right = sh.Slots[(i * 2) + 1];

                if (left != null && right != null)
                {
                    if (left.Type == MateriaType.Magic && right.Type == MateriaType.Support)
                    {
                        MateriaOrb temp = left;
                        left = right;
                        right = temp;
                    }

                    if (right.Type == MateriaType.Magic && left.Type == MateriaType.Support)
                    {
                        foreach (string ability in right.Abilities)
                        {
                            Spell magicSpell = CurrentBattle.Seven.Data.GetMagicSpell(ability);

                            foreach (MagicMenuEntry entry in list.Where(x => x.Name == magicSpell.Name))
                            {
                                entry.AddAbility(left);
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
                if (m != null && m.Type == MateriaType.Summon)
                {
                    foreach (string ability in m.Abilities)
                    {
                        Spell summon = CurrentBattle.Seven.Data.GetSummonSpell(ability);
                        
                        if (!list.Any(x => x.Name == summon.Name))
                        {
                            list.Add(new SummonMenuEntry(m.Name, summon));
                        }
                    }
                }
            }

            // Then, go through and attach any support abilities
            for (int i = 0; i < sh.Links; i++)
            {
                MateriaOrb left = sh.Slots[i * 2];
                MateriaOrb right = sh.Slots[(i * 2) + 1];
                
                
                if (left != null && right != null)
                {
                    if (left.Type == MateriaType.Summon && right.Type == MateriaType.Support)
                    {
                        MateriaOrb temp = left;
                        left = right;
                        right = temp;
                    }

                    if (right.Type == MateriaType.Summon && left.Type == MateriaType.Support)
                    {
                        foreach (string ability in right.Abilities)
                        {
                            Spell summon = CurrentBattle.Seven.Data.GetSummonSpell(ability);
                            
                            foreach (SummonMenuEntry entry in list.Where(x => x.Name == summon.Name))
                            {
                                entry.AddAbility(left);
                            }
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
        

        
        #region Methods
        
        public override void AcceptDamage(Combatant source, int delta, AttackType type = AttackType.None)
        {
            CurrentBattle.AddDamageIcon(delta, this);

            // limit shtuff goes here
            
            if (type == AttackType.Physical)
            {
                if (Sleep)
                {
                    CureSleep(source);
                }
                if (Confusion)
                {
                    CureConfusion(source);
                }
            }

            int hp = _c.HP - delta;

            if (hp < 0)
            {
                hp = 0;
            }
            else if (hp >= _c.MaxHP)
            {
                hp = _c.MaxHP;
            }
                        
            _c.HP = hp;

            if (HP == 0)
            {
                Kill(source);
            }

            if (source is Enemy)
            {
                LastAttacker = source;

                switch (type)
                {
                    case AttackType.Physical:
                        LastAttackerPhysical = source;
                        break;
                    case AttackType.Magical:
                        LastAttackerMagical = source;
                        break;
                }
            }
        }
        
        public override void AcceptMPLoss(Combatant source, int delta)
        {
            CurrentBattle.AddDamageIcon(delta, this, true);

            _c.MP -= delta;
            
            if (source is Enemy)
            {
                LastAttacker = source;
            }
        }

        public override void Recover(Combatant source)
        {
            CurrentBattle.AddRecoveryIcon(this);

            if (Death)
            {
                CureDeath(source);
            }

            _c.HP = MaxHP;
            _c.MP = MaxMP;
        }

        public override void Respond(Ability ability)
        {
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
            return String.Format("{0} - HP:{1}/{2} MP:{3}/{4} - Time: {5}%",
                                 Name, HP, MaxHP, MP, MaxMP, TurnTimer.PercentComplete);
        }
        
        protected override void DrawIcon(Gdk.Drawable d, Cairo.Context g)
        {
            Gdk.GC gc = new Gdk.GC(d);

            Images.RenderProfileTiny(d, gc, X - _icon_half_width, Y - _icon_half_height, _c);
        }
        
        #endregion Methods
        


        
        public void Attack(Combatant target, bool resetTurnTimer = true)
        {
            PrimaryAttack.Use(this, new Combatant[] { target }, new AbilityModifiers { ResetTurnTimer = resetTurnTimer});
        }
        
        public void AttackX2(Combatant target, bool resetTurnTimer = true)
        {
            PrimaryAttackX2.Use(this, new Combatant[] { target }, new AbilityModifiers { ResetTurnTimer = resetTurnTimer});
        }

        public void AttackX4(IEnumerable<Combatant> targets, bool resetTurnTimer = true)
        {
            PrimaryAttackX4.Use(this, targets, new AbilityModifiers { ResetTurnTimer = resetTurnTimer});
        }


        public void ChangeRow()
        {
            if (BackRow)
            {
                _x -= xs;
            }
            else
            {
                _x += xs;
            }

            _c.BackRow = !BackRow;
        }




        
        #region AI
        
        public void RunAIConfu()
        {
            Ally target;
            int i = CurrentBattle.Random.Next(Party.PARTY_SIZE);
                    
            while (CurrentBattle.Allies[i] == null)
            {
                i = (i + 1) % 3;
            }

            target = CurrentBattle.Allies[i];
            
            PrimaryAttack.Use(this, new Combatant[] { target }, new AbilityModifiers());
        }
        
        public void RunAIBerserk()
        {
            Enemy target;

            int i = CurrentBattle.Random.Next(CurrentBattle.EnemyList.Count);
                    
            target = CurrentBattle.EnemyList[i];
            
            PrimaryAttack.Use(this, new Combatant[] { target }, new AbilityModifiers());
        }
        
        #endregion AI


        #region Inflict Status

        public override bool InflictDeath(Combatant source)
        {
            bool inflicted = false;
            
            if (!Immune(Status.Death) && !(DeathForce || Peerless || Petrify || Resist))
            {
                if (DeathSentence)
                {
                    CureDeathSentence(source);
                }

                inflicted = _c.InflictDeath();

                if (inflicted)
                {                    
                    Kill(source);
                }
            }
            
            return inflicted;
        }

        public override bool InflictFury(Combatant source)
        {
            if (_c.Immune(Status.Fury))
            {
                return false;
            }
            return _c.InflictFury();
        }

        public override bool InflictSadness(Combatant source)
        {
            if (_c.Immune(Status.Sadness))
            {
                return false;
            }
            return _c.InflictSadness();
        }
                
        public override bool InflictManipulate(Combatant source)
        {
            return false;
        }

        protected override void Kill(Combatant source)
        {
            _c.HP = 0;

            CureAll(source);
            TurnTimer.Reset();
            PauseTimers();

            // FINAL ATTACK !!!
        }

        #endregion Inflict
        
        
        
        #region Cure Status
        
        public override bool CureDeath(Combatant source)
        {
            bool cured = _c.CureDeath();

            if (cured)
            {
                CureAll(source);
                UnpauseTimers();
            }

            return cured;
        }

        public override bool CureFury(Combatant source)
        {
            return _c.CureFury();
        }

        public override bool CureSadness(Combatant source)
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
        
        public override int X { get { return _x; } }
        
        public override int Y { get { return _y; } }

        public override IList<Element> Weaknesses
        {
            get
            {
                return new List<Element>();
            }
        }
        
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
                        if (m != null && m.Name == "Long Range")
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

        public override bool NearDeath { get { return _c.NearDeath && !Death; } }

        public override bool Sadness { get { return _c.Sadness; } }

        public override bool Fury { get { return _c.Fury; } }
        
        public List<MagicMenuEntry> MagicSpells { get { return _magicSpells; } }

        public List<SummonMenuEntry> Summons { get { return _summons; } }
        
        public BattleMenu BattleMenu { get; private set; }

        public Screens.Magic.Main MagicMenu  { get; private set; }

        public Screens.Summon.Main SummonMenu  { get; private set; }

        public Screens.EnemySkill.Main EnemySkillMenu { get; private set; }

        public Weapon Weapon { get { return _c.Weapon; } }

        public Armor Armor { get { return _c.Armor; } }

        public IEnumerable<MateriaOrb> Materia { get { return _c.Materia; } }
        
        public Ability PrimaryAttack { get; private set; }
        
        public Ability PrimaryAttackX2 { get; private set; }
        
        public Ability PrimaryAttackX4 { get; private set; }
        
        #endregion Properties
        
    }
}
