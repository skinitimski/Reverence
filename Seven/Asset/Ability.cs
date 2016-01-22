using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Thread = System.Threading.Thread;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal abstract class Ability
    {  
        private Random _random = new Random();

        protected class StatusChange
        {
            public enum Effect
            {
                Inflict,
                Cure,
                Toggle
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

            public static UseAbilityEvent Create(Ability ability, AbilityModifiers modifiers, Combatant source, IEnumerable<Combatant> targets)
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
                            if (Ability.Hits > 1 && Ability.Plural)
                            {
                                int index = Ability._random.Next(Targets.Count());
                                Combatant newTarget = Targets.ToList()[index];
                                Ability.Cast(Source, new List<Combatant>{ newTarget }, Modifiers);
                            }
                            else
                            {
                                Ability.Cast(Source, Targets, Modifiers);
                            }

                            CurrentHit++;
                        }
                    }

                
                    // timski: don't remember why I wrote this line, and it doesn't make
                    // any sense to me right now...
                    //if (Hits > 1) Thread.Sleep(spell_duration / 3);
                }
            }
            
            protected override string GetStatus(long elapsed)
            {
                return Status;
            }

            private String Status { get; set; }

            private bool[] Hits { get; set; }

            private int CurrentHit { get; set; }

            private IEnumerable<Combatant> Targets { get; set; }

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
        
        protected int PhysicalAttack(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int bd = Formula.PhysicalBase(source);
            int dam = Formula.PhysicalDamage(bd, Power, target);
            
            dam = Formula.RunPhysicalModifiers(dam, source, target, Elements);

            return dam;
        }
        
        protected int MagicalAttack(Combatant source, Combatant target, AbilityModifiers modifiers)
        {                        
            int bd = Formula.MagicalBase(source);
            int dam = Formula.MagicalDamage(bd, Power, target);
            
            dam = Formula.RunMagicModifiers(dam, target, Elements, modifiers);
            
            return dam;
        }
        
        protected int Cure(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int bd = Formula.MagicalBase(source);
            int dam = bd + 22 * Power;
            
            dam = Formula.RunCureModifiers(dam, target, Elements, modifiers);
            
            return dam;
        }
        
        protected int HPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.HP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        protected int MPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.MP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        protected int MaxHPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.MaxHP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }
        
        protected int MaxMPPercent(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int dam = target.MaxMP * Power / 32;
            
            dam = Formula.QuadraMagic(dam, modifiers);
            
            return dam;
        }

        protected IEnumerable<Element> GetElements(XmlNodeList elementNodes)
        {
            foreach (XmlNode node in elementNodes)
            {
                yield return (Element)Enum.Parse(typeof(Element), node.InnerText);
            }
        }
        
        protected IEnumerable<StatusChange> GetStatusChanges(XmlNodeList inflictionNodes)
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
            
            if (canUse)
            {                 
                UseAbilityEvent e = UseAbilityEvent.Create(this, modifiers, source, targets);
                
                Seven.BattleState.EnqueueAction(e, modifiers.CounterAttack);
            }
            else
            {   
                UseAbilityFailEvent e = new UseAbilityFailEvent(source, this, modifiers.ResetTurnTimer);
                
                Seven.BattleState.EnqueueAction(e, modifiers.CounterAttack);
            }
        }
        
        private void Cast(Combatant source, IEnumerable<Combatant> targets, AbilityModifiers modifiers)
        {
            foreach (Combatant target in targets)
            {
                bool hit;

                if (Type == AttackType.Physical)
                {
                    hit = Formula.PhysicalHit(Hitp, source, target, Elements);
                }
                else
                {
                    hit = Formula.MagicHit(source, target, Hitp, Elements);
                }

                if (hit)
                {       
                    if (Power > 0)
                    {
                        int dam = DamageFormula(source, target, modifiers);
                        
                        target.AcceptDamage(source, dam, Type);
                    }
                    
                    foreach (StatusChange statusChange in Statuses)
                    {
                        if (Formula.StatusHit(source, target, statusChange.Odds, statusChange.Statuses, modifiers))
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
                }
                else
                {
                    Seven.BattleState.AddMissIcon(target);
                }
            }
        }
        
        public string Name { get; protected set; }

        public string Desc { get; protected set; }

        public AttackType Type { get; protected set; }

        public BattleTarget Target { get; protected set; }

        public int MPCost { get; protected set; }
        
        protected int Hits { get; set; }

        protected DamageFormula DamageFormula { get; set; }

        protected int Power { get; set; }

        protected int Hitp { get; set; }
        
        protected IEnumerable<Element> Elements { get; set; }

        protected IEnumerable<StatusChange> Statuses { get; set; }

        private bool Plural { get { return Target == BattleTarget.AlliesPlural || Target == BattleTarget.EnemiesPlural || Target == BattleTarget.GroupPlural; } }

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

