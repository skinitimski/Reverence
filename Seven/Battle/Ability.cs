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
        
        protected int PhysicalAttack(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int bd = Formula.PhysicalBase(source);
            int dam = Formula.PhysicalDamage(bd, Power, target);
            
            dam = Formula.Critical(dam, source, target);
            dam = Formula.Berserk(dam, source);
            dam = Formula.RowCheck(dam, source, target);
            dam = Formula.Frog(dam, target);
            dam = Formula.Sadness(dam, target);
            dam = Formula.Barrier(dam, target);
            dam = Formula.Mini(dam, source);
            dam = Formula.RandomVariation(dam);

            return dam;
        }
        
        protected int MagicalAttack(Combatant source, Combatant target, AbilityModifiers modifiers)
        {                        
            int bd = Formula.MagicalBase(source);
            int dam = Formula.MagicalDamage(bd, Power, target);
            
            dam = Formula.Sadness(dam, target);
            dam = Formula.Split(dam, modifiers);
            dam = Formula.MBarrier(dam, target);
            dam = Formula.MPTurbo(dam, modifiers);
            dam = Formula.RandomVariation(dam);
            
            return dam;
        }
        
        protected int Cure(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            int bd = Formula.MagicalBase(source);
            int dam = bd + 22 * Power;
            
            dam = Formula.Split(dam, modifiers);
            dam = Formula.MBarrier(dam, target);
            dam = Formula.MPTurbo(dam, modifiers);
            dam = Formula.RandomVariation(dam);
            
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
        
        protected int Fixed(Combatant source, Combatant target, AbilityModifiers modifiers)
        {
            return Power;
        }

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
                        throw new GameDataException("Neither a formula nor an attack type defined -- ability '{0}'", Name);
                }
            }

            return formula;
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
                UseAbilityEvent e = UseAbilityEvent.Create(this, modifiers, source, targets.ToArray());
                
                source.CurrentBattle.EnqueueAction(e, modifiers.CounterAttack);
            }
            else
            {   
                UseAbilityFailEvent e = new UseAbilityFailEvent(source, this, modifiers.ResetTurnTimer);
                
                source.CurrentBattle.EnqueueAction(e, modifiers.CounterAttack);
            }
        }
        
        private bool[] Cast(Combatant source, Combatant[] targets, AbilityModifiers modifiers)
        {
            if (RandomTarget && targets.Count() > 1)
            {
                int index = _random.Next(targets.Count());
                Combatant newTarget = targets.ToList()[index];
                targets = new Combatant[] { newTarget };
            }

            bool[] hits = new bool[targets.Length];

            for (int i = 0; i < targets.Length; i++)
            {
                Combatant target = targets[i];

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

                        dam = Formula.RunElementalChecks(dam, target, Elements);
                        dam = Formula.LowerSanityCkeck(dam);
                        dam = Formula.UpperSanityCheck(dam);
                                                
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

