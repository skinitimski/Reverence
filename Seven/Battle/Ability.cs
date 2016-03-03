using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Thread = System.Threading.Thread;
using System.Xml;

using NLua;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal delegate int DamageFormula(Combatant source, Combatant target, AbilityModifiers modifiers);
    
    internal delegate bool HitFormula(Combatant source, Combatant target, AbilityModifiers modifiers);


    internal abstract class Ability
    {  

        protected class StatusChange
        {
            public enum Effect
            {
                Inflict,
                Cure,
                Toggle
            }
                      
            public bool Hits(Combatant source, Combatant target, AbilityModifiers modifiers)
            {
                // auto hit conditions
                
                if (Odds >= 100)
                {
                    return true;
                }            
                if (Statuses.Count() == 1 && Statuses.Contains(Status.Frog) && target.Frog)
                {
                    return true;
                }
                if (Statuses.Count() == 1 && Statuses.Contains(Status.Small) && target.Small)
                {
                    return true;
                }
                if (target is Ally && Statuses.Any(s => new Status[] {
                    Status.Haste,
                    Status.Berserk,
                    Status.Shield
                }.Contains(s)))
                {
                    return true;
                }
                
                Odds = MPTurbo(Odds, modifiers);
                
                Odds = Split(Odds, modifiers);
                
                Odds -= 1;
                
                return source.CurrentBattle.Random.Next(99) < Odds;
            }

            public IEnumerable<Status> Statuses { get; set; }

            public Effect ChangeType { get; set; }

            public int Odds { get; set; }
        }
        
        protected class UseAbilityEvent : CombatantActionEvent
        {
            protected UseAbilityEvent(Combatant source, bool resetSourceTurnTimer, int duration)
                : base(source, resetSourceTurnTimer, duration)
            {
            }

            public static UseAbilityEvent Create(Ability ability, AbilityModifiers modifiers, Combatant source, Combatant[] targets)
            {            
                string msg = ability.GetMessage(source);
                
                if (source.Confusion)
                {
                    msg += " (confused)";
                }
                else if (source.Berserk)
                {
                    msg += " (berserk)";
                }


                int duration = ability.PauseDuration + ability.SpellDuration * ability.Hits;

                UseAbilityEvent @event = new UseAbilityEvent(source, modifiers.ResetTurnTimer, duration)
                {
                    Hits = new bool[ability.Hits],
                    Records = new bool[targets.Count()],
                    Status = msg,
                    Targets = targets,
                    Ability = ability,
                    Modifiers = modifiers
                };

                return @event;
            }
            
            protected override void RunIteration(long elapsed, bool lastIteration)
            {
                if (elapsed >= Ability.PauseDuration)
                {
                    if (CurrentHit < Hits.Length)
                    {
                        if (!Hits[CurrentHit] && elapsed >= Ability.PauseDuration + Ability.SpellDuration * CurrentHit)
                        {
                            bool[] whoWasHit = Ability.Cast(Source, Targets, Modifiers);

                            for (int i = 0; i < whoWasHit.Length; i++)
                            {
                                if (!Records[i] && whoWasHit[i])
                                {
                                    Records[i] = true;
                                }
                            }

                            CurrentHit++;
                        }
                    }
                }

                if (lastIteration)
                {
                    for (int i = 0; i < Records.Length; i++)
                    {
                        if (Records[i])
                        {
                            Targets[i].Respond(Ability);
                        }
                    }
                }
            }
            
            protected override string GetStatus(long elapsed)
            {
                return Status;
            }

            private String Status { get; set; }

            private bool[] Hits { get; set; }

            private int CurrentHit { get; set; }

            private Combatant[] Targets { get; set; }

            private bool[] Records { get; set; }

            private Ability Ability { get; set; }

            private AbilityModifiers Modifiers { get; set; }
        }
        
        protected class UseAbilityFailEvent : CombatantActionEvent
        {
            private const int DURATION = 2000;

            public UseAbilityFailEvent(Combatant source, Ability ability, bool resetSourceTurnTimer)
                : base(source, resetSourceTurnTimer, DURATION)
            {
                if (source is Ally)
                {
                    Status = "Not enough MP for " + ability.Name + "!";
                }
                else // is Enemy
                {
                    Status = source.Name + "'s skill power is used up.";
                }
            }
            
            protected override void RunIteration(long elapsed, bool lastIteration)
            {
                
            }
            
            protected override string GetStatus(long elapsed)
            {
                return Status;
            }

            private string Status { get; set; }
        }


        protected Ability()
        {
            Elements = new Element[] { };
            Statuses = new StatusChange[] { };
            Hits = 1;
        }


        
        
        
        #region Hit Formulas
                
        public bool PhysicalHit(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int hitp;
            
            if (target.Absorbs(Elements) ||
                target.Voids(Elements) ||
                target.Death || 
                target.Sleep ||
                target.Confusion || 
                target.Stop ||
                target.Petrify || 
                target.Manipulate ||
                target.Paralysed || 
                target.Peerless)
            {
                hitp = 255;
            }
            else
            {
                hitp = (source.Dexterity / 4) + Hitp + source.Defp - target.Defp;
                
                if (source.Fury)
                {
                    hitp = hitp - hitp * 3 / 10;
                }
            }
            
            // Sanity
            if (hitp < 1)
            {
                hitp = 1;
            }
            
            int lucky = source.CurrentBattle.Random.Next(0, 100);
            
            // Lucky Hit
            if (lucky < Math.Floor(source.Luck / 4.0d))
            {
                hitp = 255;
            }
            // Lucky Evade
            else if (lucky < Math.Floor(target.Luck / 4.0d))
            {
                if (source is Ally && target is Enemy)
                {
                    hitp = 0;
                }
            }
            
            int r = source.CurrentBattle.Random.Next(65536) * 99 / 65536 + 1;
            
            return r < hitp;
        }
        
        public bool MagicHit(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            if (Hitp == 255 ||
                target.Absorbs(Elements) ||
                target.Voids(Elements))
            {
                return true;
            }
            if (target.Death ||
                target.Sleep ||
                target.Confusion ||
                target.Stop ||
                target.Petrify ||
                target.Paralysed ||
                target.Peerless ||
                target.Reflect)
            {
                return true;
            }
            
            if (source.Fury)
            {
                Hitp = Hitp - Hitp * 3 / 10;
            }
            
            // Magic Defense Percent
            if (source.CurrentBattle.Random.Next(1, 101) < target.MDefp)
            {
                return false;
            }
            
            int hitp = Hitp + source.Level - target.Level / 2 - 1;
            
            return source.CurrentBattle.Random.Next(100) < hitp;
        }
        
        #endregion

        #region Damage Formulas
        
        protected int PhysicalAttack(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int bd = PhysicalBase(source);
            int dam = PhysicalDamage(bd, Power, target);
            
            dam = Critical(dam, source, target);
            dam = Berserk(dam, source);
            dam = RowCheck(dam, source, target);
            dam = Frog(dam, target);
            dam = Sadness(dam, target);
            dam = Barrier(dam, target);
            dam = Mini(dam, source);
            dam = RandomVariation(source.CurrentBattle.Random, dam);

            return dam;
        }
        
        protected int MagicalAttack(Combatant source, Combatant target, AbilityModifiers modifiers)
        {                        
            int bd = MagicalBase(source);
            int dam = MagicalDamage(bd, Power, target);
            
            dam = Sadness(dam, target);
            dam = Split(dam, modifiers);
            dam = MBarrier(dam, target);
            dam = MPTurbo(dam, modifiers);
            dam = RandomVariation(source.CurrentBattle.Random, dam);
            
            return dam;
        }
        
        protected int Cure(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int bd = MagicalBase(source);
            int dam = bd + 22 * Power;
            
            dam = Split(dam, modifiers);
            dam = MBarrier(dam, target);
            dam = MPTurbo(dam, modifiers);
            dam = RandomVariation(source.CurrentBattle.Random, dam);
            
            return dam;
        }
        
        protected int HPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.HP * Power / 32;
            
            dam = QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        protected int MPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.MP * Power / 32;
            
            dam = QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        protected int MaxHPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.MaxHP * Power / 32;
            
            dam = QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        protected int MaxMPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.MaxMP * Power / 32;
            
            dam = QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        protected int Fixed(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            return Power;
        }

        #endregion Damage Formulas

        #region Other Formulas
        
        protected int PhysicalBase(Combatant er)
        {
            return er.Atk + ((er.Atk + er.Level) / 32) * (er.Atk * er.Level / 32);
        }
        
        protected int PhysicalDamage(int bd, int power, Combatant ee)
        {
            return (power * (512 - ee.Def) * bd) / (16 * 512);
        }
        
        protected int MagicalBase(Combatant er)
        {
            return 6 * (er.Mat + er.Level);
        }
        
        protected int MagicalDamage(int bd, int power, Combatant ee)
        {
            return (power * (512 - ee.Def) * bd) / (16 * 512);
        }
        
        protected int Critical(int dam, Combatant source, Combatant target)
        {
            Combatant ee = target;
            Ally er = source as Ally;
            
            if (er == null)
            {
                return dam;
            }
            
            int critp;
            
            if (er.LuckyGirl)
            {
                critp = 255;
            }
            else
            {
                critp = (er.Luck + er.Level - ee.Level) / 4;
                critp = critp + er.Weapon.CriticalPercent;
            }
            
            int r = (source.CurrentBattle.Random.Next(65536) * 99 / 65536) + 1;
            
            if (r <= critp)
            {
                dam = dam * 2;
            }
            
            return dam;
        }
        
        protected static int Berserk(int dam, Combatant source)
        {
            if (source.Berserk)
            {
                dam = dam * 15 / 10;
            }
            
            return dam;
        }
        
        protected static int RowCheck(int dam, Combatant source, Combatant target)
        {
            if (source.LongRange)
            {
                return dam;
            }
            if (source.BackRow || target.BackRow)
            {
                dam = dam / 2;
            }
            
            return dam;
        }
        
        protected static int Frog(int dam, Combatant source)
        {
            if (source.Frog)
            {
                dam = dam / 4;
            }
            
            return dam;
        }
        
        protected static int Sadness(int dam, Combatant ee)
        {
            if (ee.Sadness)
            {
                dam = dam * 7 / 10;
            }
            
            return dam;
        }
        
        protected static int Split(int dam, AbilityModifiers modifiers)
        {
            if (modifiers.QuadraMagic)
            {
                dam = dam / 2;
            }
            else if (modifiers.Alled && !modifiers.NoSplit)
            {
                dam = dam * 2 / 3;
            }
            
            return dam;
        }
        
        protected static int QuadraMagic(int dam, AbilityModifiers modifiers)
        {
            if (modifiers.QuadraMagic)
            {
                dam = dam / 2;
            }
            
            return dam;
        }
        
        protected static int Barrier(int dam, Combatant target)
        {
            if (target.Barrier)
            {
                dam = dam / 2;
            }
            
            return dam;
        }
        
        protected static int MBarrier(int dam, Combatant target)
        {
            if (target.MBarrier)
            {
                dam = dam / 2;
            }
            
            return dam;
        }
        
        protected static int MPTurbo(int dam, AbilityModifiers modifiers)
        {
            dam = dam * (10 + modifiers.MPTurboFactor) / 10;
            
            return dam;
        }
        
        protected static int Mini(int dam, Combatant source)
        {
            if (source.Small)
            {
                dam = 0;
            }
            
            return dam;
        }
        
        protected static int RandomVariation(Random random, int dam)
        {
            dam = dam * (3841 + random.Next(256)) / 4096;
            
            return dam;
        }
        
        protected static int RunElementalChecks(int dam, Combatant target, IEnumerable<Element> attackElements)
        {
            bool checksDone = false;
            
            if (attackElements.Contains(Element.Restorative))
            {
                dam = -dam;
            }
            
            foreach (Element e in attackElements)
            {
                if (target.Voids(e))
                {
                    dam = 0;
                    checksDone = true;
                    break;
                }
            }
            
            if (!checksDone)
            {
                foreach (Element e in attackElements)
                {
                    if (target.Absorbs(e))
                    {
                        dam = -dam;
                        checksDone = true;
                        break;
                    }
                }
            }
            
            if (!checksDone)
            {
                foreach (Element e in attackElements)
                {
                    if (target.Halves(e) && target.Weak(e))
                    {
                        continue;
                    }
                    else if (target.Halves(e))
                    {
                        dam = dam / 2;
                        break;
                    }
                    else if (target.Weak(e))
                    {
                        dam = dam * 2;
                        break;
                    }
                }
            }
            
            return dam;
        }
        
        protected static int LowerSanityCkeck(int dam)
        {
            if (dam == 0)
            {
                dam = 1;
            }
            
            return dam;
        }
        
        protected static int UpperSanityCheck(int dam)
        {
            if (dam > 9999)
            {
                dam = 9999;
            }
            
            return dam;
        }

        #endregion









        protected static IEnumerable<Element> GetElements(XmlNodeList elementNodes)
        {
            foreach (XmlNode node in elementNodes)
            {
                yield return (Element)Enum.Parse(typeof(Element), node.InnerText);
            }
        }
        
        protected static IEnumerable<StatusChange> GetStatusChanges(XmlNodeList inflictionNodes)
        {
            foreach (XmlNode node in inflictionNodes)
            {
                StatusChange change = new StatusChange
                {
                    Odds = Int32.Parse(node.SelectSingleNode("@odds").Value),
                    ChangeType = (StatusChange.Effect)Enum.Parse(typeof(StatusChange.Effect), node.SelectSingleNode("@type").Value),
                    Statuses = node.SelectSingleNode("@statuses").Value.Split(',').Select(s => (Status)Enum.Parse(typeof(Status), s))
                };

                yield return change;
            }
        }

        protected DamageFormula GetFormula(XmlNode formulaNode, Lua lua)
        {
            DamageFormula formula = null;

            if (formulaNode != null)
            {
                XmlNode codeNode = formulaNode.SelectSingleNode("custom");

                if (codeNode != null)
                {                    
                    string code = String.Format("return function (source, target, modifiers) {0} end", codeNode.InnerText);
                    LuaFunction customFormula;

                    try
                    {
                        customFormula = (LuaFunction)lua.DoString(code).First();
                    }
                    catch (Exception ex)
                    {
                        throw new ImplementationException("Error loading ability custom formula; name = " + Name, ex);
                    }

                    formula = delegate(Combatant source, Combatant target, AbilityModifiers modifiers)
                    {
                        object formulaResult = customFormula.Call(source, target, modifiers).First(); // it's a double

                        return Convert.ToInt32(formulaResult); 
                    };
                }
                else
                {
                    formula = (DamageFormula)Delegate.CreateDelegate(typeof(DamageFormula), this, formulaNode.InnerText);
                }
            }
            else
            {
                switch (Type)
                {
                    case AttackType.Magical:
                        formula = MagicalAttack;
                        break;
                    case AttackType.Physical:
                        formula = PhysicalAttack;
                        break;
                    default:
                        throw new GameDataException("Neither a damage formula nor an attack type defined -- ability '{0}'", Name);
                }
            }

            return formula;
        }

        protected HitFormula GetHitFormula(XmlNode hitFormulaNode, Lua lua)
        {
            HitFormula hitFormula = null;

            if (hitFormulaNode != null)
            {
                XmlNode codeNode = hitFormulaNode.SelectSingleNode("custom");
                
                if (codeNode != null)
                {                    
                    string code = String.Format("return function (source, target, modifiers) {0} end", codeNode.InnerText);
                    LuaFunction customFormula;
                    
                    try
                    {
                        customFormula = (LuaFunction)lua.DoString(code).First();
                    }
                    catch (Exception ex)
                    {
                        throw new ImplementationException("Error loading ability custom formula; name = " + Name, ex);
                    }
                    
                    hitFormula = delegate(Combatant source, Combatant target, AbilityModifiers modifiers)
                    {
                        object formulaResult = customFormula.Call(source, target, modifiers).First(); // it's a double
                        
                        return Convert.ToBoolean(formulaResult); 
                    };
                }
                else
                {
                    hitFormula = (HitFormula)Delegate.CreateDelegate(typeof(HitFormula), this, hitFormulaNode.InnerText);
                }
            }
            else
            {
                switch (Type)
                {
                    case AttackType.Magical:
                        hitFormula = PhysicalHit;
                        break;
                    case AttackType.Physical:
                        hitFormula = MagicHit;
                        break;
                    default:
                        throw new GameDataException("Neither a hit formula nor an attack type defined -- ability '{0}'", Name);
                }
            }

            return hitFormula;
        }
                
        protected abstract String GetMessage(Combatant source);



        /// <summary>
        /// Cast this ability. The <param name="source">source</param> is the <see cref="Combatant" /> which is responsible for casting the spell.
        /// The spell can have one or more <param name="targets">targets</param>.
        /// </summary>
        /// <param name='source'>
        /// Source.
        /// </param>
        /// <param name='targets'>
        /// Targets.
        /// </param>
        /// <param name='modifiers'>
        /// Modifiers.
        /// </param>
        /// <param name='resetTurnTimer'>
        /// If set to <c>true</c> reset turn timer.
        /// </param>
        public void Use(Combatant source, IEnumerable<Combatant> targets, AbilityModifiers modifiers)
        {            
            bool canUse = true;
            
            if (!modifiers.CostsNothing)
            {
                if (source.MP >= MPCost)
                {
                    source.UseMP(MPCost);
                }
                else
                {
                    canUse = false;
                }
            }

            BattleEvent e = null;
            
            if (canUse)
            {                 
                e = UseAbilityEvent.Create(this, modifiers, source, targets.ToArray());
            }
            else
            {   
                e = new UseAbilityFailEvent(source, this, modifiers.ResetTurnTimer);
            }

            if (modifiers.CounterAttack)
            {
                source.CurrentBattle.EnqueueCounterAction(e);
            }
            else
            {
                source.CurrentBattle.EnqueueAction(e);
            }
        }
        
        private bool[] Cast(Combatant source, Combatant[] targets, AbilityModifiers modifiers)
        {
            if (RandomTarget && targets.Count() > 1)
            {
                int index = source.CurrentBattle.Random.Next(targets.Count());
                Combatant newTarget = targets.ToList()[index];
                targets = new Combatant[] { newTarget };
            }

            bool[] hits = new bool[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                Combatant target = targets[i];

                bool hit = HitFormula(source, target, modifiers);

                if (hit)
                {       
                    if (Power > 0)
                    {
                        int dam = DamageFormula(source, target, modifiers);

                        dam = RunElementalChecks(dam, target, Elements);
                        dam = LowerSanityCkeck(dam);
                        dam = UpperSanityCheck(dam);
                                                
                        target.AcceptDamage(source, dam, Type);
                    }
                    
                    foreach (StatusChange statusChange in Statuses)
                    {
                        if (statusChange.Hits(source, target, modifiers))
                        {
                            switch (statusChange.ChangeType)
                            {
                                case StatusChange.Effect.Cure:

                                    foreach (Status status in statusChange.Statuses)
                                    {
                                        Console.WriteLine(status);

                                        target.GetType().GetMethod("Cure" + status).Invoke(target, new object[] { source });
                                    }

                                    break;

                                case StatusChange.Effect.Inflict:

                                    foreach (Status status in statusChange.Statuses)
                                    {
                                        Console.WriteLine(status);
                                        
                                        target.GetType().GetMethod("Inflict" + status).Invoke(target, new object[] { source });
                                    }

                                    break;

                                case StatusChange.Effect.Toggle:

                                    throw new NotImplementedException("Haven't implemented status change toggle");
                            }
                        }
                    }       

                    hits[i] = true;
                }
                else
                {
                    source.CurrentBattle.AddMissIcon(target);
                }
            }

            return hits;
        }


        
        public string Name { get; protected set; }

        public string Desc { get; protected set; }

        public AttackType Type { get; protected set; }

        public BattleTarget Target { get; protected set; }

        public int MPCost { get; protected set; }
        
        protected int Hits { get; set; }

        protected HitFormula HitFormula { get; set; }

        protected DamageFormula DamageFormula { get; set; }

        protected int Power { get; set; }

        protected int Hitp { get; set; }
        
        protected IEnumerable<Element> Elements { get; set; }

        protected IEnumerable<StatusChange> Statuses { get; set; }

        private bool RandomTarget { get { return Target == BattleTarget.GroupRandom || Target == BattleTarget.AlliesRandom || Target == BattleTarget.EnemiesRandom; } }

        private int PauseDuration
        {
            get
            {
                int pause = 0;

                switch (Type)
                {
                    case AttackType.Magical:
                        pause = 1500;
                        break;
                    case AttackType.Physical:
                        pause = 800;
                        break;
                }

                return pause;
            }
        }

        private int SpellDuration
        {
            get
            {
                int duration = BattleIcon.ANIMATION_DURATION;

                if (Hits > 1)
                {
                    duration = duration * 2 / 3;
                }

                return duration;
            }
        }
    }
}

