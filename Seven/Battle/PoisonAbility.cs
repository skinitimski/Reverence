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
        private static readonly PoisonAbility INSTANCE = new PoisonAbility();
        private static readonly AbilityModifiers MODIFIERS = new AbilityModifiers();

#if DEBUG
        private const string STATUS = "(poison damage)";
#else
        private const string STATUS = String.Empty;
#endif

        private class PoisonEvent : AbilityEvent
        {
            public PoisonEvent(Combatant source, Combatant target)
                : base(INSTANCE, MODIFIERS, source, new Combatant[] { target })
            {
                Target = target;
            }

            protected override void CompleteHook()
            {
                Target.SetPoisonTime();
            }

            private Combatant Target { get; set; }
        }

        private PoisonAbility()
        {
            Name = "Poison";
            Desc = "Damage as a result of the Poison Status";
            
            Type = AttackType.Physical;
            Target = BattleTarget.Combatant;

            Elements = new Element[] { Element.Poison };
            
            Power = 1;
            Hitp = 255;
            Hits = 1;
            
            DamageFormula = HPPercent;
            HitFormula = PhysicalHit;
        }

        public static void Use(Combatant source, Combatant target)
        {
            source.CurrentBattle.EnqueueAction(new PoisonEvent(source, target));
        }

        public override int PauseDuration { get { return 0; } }

        public override string GetMessage(Combatant source)
        {
            return STATUS;
        }
    }
}

