using System;
using System.Collections.Generic;

using NLua;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Battle.Event;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class PoisonAbility : Ability
    {
        private static readonly AbilityModifiers MODIFIERS = new AbilityModifiers();
        
        private class PoisonEvent : AbilityEvent
        {
            public PoisonEvent(Combatant poisoner, Combatant poisonee)
                : base(new PoisonAbility(poisoner, poisonee), MODIFIERS, poisoner, new Combatant[] { poisonee })
            {
                Poisonee = poisonee;
            }
            
            protected override void CompleteHook()
            {
                if (!Poisonee.Death)
                {
                    Poisonee.SetPoisonTime();
                }
            }
            
            private Combatant Poisonee { get; set; }
        }
        
        private PoisonAbility(Combatant poisoner, Combatant poisonee)
        {
            Name = "Poison";
            Desc = "Damage as a result of the Poison Status";
            
            Type = AttackType.Physical;
            Target = BattleTarget.Combatant;
            
            Elements = new Element[] { Element.Poison };
            
            Power = 1;
            Hitp = 255;
            
            DamageFormula = MaxHPPercent;
            HitFormula = PhysicalHit;
            
#if DEBUG
            Message = String.Format("[ {0} had poisoned {1} ]", poisoner, poisonee);
#else
            Message = String.Empty;
#endif
        }

        public static void Use(Combatant poisoner, Combatant poisonee)
        {
            poisonee.CurrentBattle.EnqueueAction(new PoisonEvent(poisoner, poisonee));
        }
        
        public override int PauseDuration { get { return 0; } }
        
        public override int SpellDuration { get { return 200; } }
        
        public override string GetMessage(Combatant source)
        {
            return Message;
        }
        
        private string Message { get; set; }
    }
}

