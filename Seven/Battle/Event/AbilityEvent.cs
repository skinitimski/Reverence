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
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Battle.Event;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Battle.Event
{
    internal class AbilityEvent : CombatantActionEvent
    {
        public AbilityEvent(Ability ability, AbilityModifiers modifiers, Combatant source, Combatant[] targets)
            : base(source, modifiers.ResetTurnTimer)
        {         
            Hits = new bool[ability.Hits];
            Steals = new string[modifiers.StealAsWell ? targets.Count() : 0];
            TargetHitFlags = new bool[targets.Count()];
            Targets = targets;
            Ability = ability;
            Modifiers = modifiers;

            if (modifiers.StealAsWell)
            {
                if (!(source is Ally))
                {
                    throw new GameDataException("Non-Ally source '{0}' attempted to steal.", source.Name);
                }
                else if (targets.Any(x => !(x is Enemy)))
                {
                    throw new GameDataException("Source '{0}' attempted to steal from a non-Enemy target using ability '{1}'.", source.Name, ability.Name);
                }
            }
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
                            TargetHitFlags[i] = !TargetHitFlags[i] && whoWasHit[i];
                        }

                        CurrentHit++;
                    }
                }
                else if (CurrentSteal < Steals.Length)
                {
                    if (Steals[CurrentSteal] == null && elapsed >= AbilityDuration + StealEvent.DURATION * CurrentSteal)
                    {
                        Steals[CurrentSteal] = ((Enemy)Targets[CurrentSteal]).StealItem((Ally)Source) ?? String.Empty;

                        CurrentSteal++;
                    }
                }
            }
            
            if (lastIteration)
            {
                for (int i = 0; i < TargetHitFlags.Length; i++)
                {
                    if (TargetHitFlags[i])
                    {
                        Targets[i].Respond(Ability);
                    }
                }
            }
        }

        protected override void BeginHook()
        {
            // Check our target(s).

            switch (Ability.Target)
            {
                case BattleTarget.Ally:
                case BattleTarget.Enemy:
                case BattleTarget.Combatant:
                    
                    // If we're just targeting one thing, and that thing is dead, then we need a new target.

                    if (Targets[0].Death)
                    {
                        ChooseNewTarget();
                    }

                    break;
                    
                case BattleTarget.Allies:
                case BattleTarget.AlliesRandom:
                case BattleTarget.Enemies:
                case BattleTarget.EnemiesRandom:
                case BattleTarget.Group:
                case BattleTarget.GroupRandom:
                case BattleTarget.Area:
                case BattleTarget.AreaRandom:

                    // If we're targeting a group of things, then if any one of those is dead, then we can prune that target.

                    Targets = Targets.Where(t => !t.Death).ToArray();

                    break;
            }
        }

        private void ChooseNewTarget()
        {
            List<Combatant> possibleTargets = new List<Combatant>();
            
            if (Ability.Target == BattleTarget.Ally || Ability.Target == BattleTarget.Combatant)
            {
                foreach (Ally a in Source.CurrentBattle.Allies)
                {
                    if (a != null)
                    {
                        possibleTargets.Add(a);
                    }
                }
            }
            
            if (Ability.Target == BattleTarget.Enemy || Ability.Target == BattleTarget.Combatant)
            {                
                foreach (Enemy e in Source.CurrentBattle.EnemyList)
                {
                    possibleTargets.Add(e);
                }
            }

            int i = Source.CurrentBattle.Random.Next(possibleTargets.Count);

            Targets = new Combatant[] { possibleTargets[i] };
        }
        
        protected override string GetStatus(long elapsed)
        {
            string msg = String.Empty;

            if (elapsed < AbilityDuration)
            {
                msg = Ability.GetMessage(Source);
                
                if (Source.Confusion)
                {
                    msg += " (confused)";
                }
                else if (Source.Berserk)
                {
                    msg += " (berserk)";
                }
            }
            else if (Steals.Length > 0)
            {  
                int index = ((int)elapsed - AbilityDuration) / StealEvent.DURATION;

                if (Steals[index] != null)
                {
                    if (!Steals[index].Equals(String.Empty))
                    {
                        // TODO: this doesn't work
                        msg = "Stole " + Steals[index] + "!";
                    }
                    else
                    {
                        msg = "Couldn't steal anything...";
                    }
                }
                else
                {
                    msg = "Nothing to steal.";
                }
            }

            return msg;
        }

        protected override int Duration
        {
            get
            {
                int duration = AbilityDuration;

                if (Modifiers.StealAsWell)
                {
                    duration += Targets.Length + StealEvent.DURATION;
                }

                return duration;
            }
        }

        public override string ToString()
        {
            return " " + Ability.GetMessage(Source);
        }

        private int AbilityDuration { get { return Ability.PauseDuration + Ability.SpellDuration * Ability.Hits; } }
                
        private bool[] Hits { get; set; }

        private string[] Steals { get; set; }
                                
        private int CurrentHit { get; set; }
        
        private int CurrentSteal { get; set; }
        
        private Combatant[] Targets { get; set; }
        
        private bool[] TargetHitFlags { get; set; }
        
        private Ability Ability { get; set; }
        
        private AbilityModifiers Modifiers { get; set; }
    }
}

