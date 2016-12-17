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
    internal class DeathAbility : Ability
    {
        private static readonly DeathAbility INSTANCE = new DeathAbility();
        private static readonly AbilityModifiers MODIFIERS = new AbilityModifiers();

#if DEBUG
        private const string STATUS = "(death)";
#else
        private const string STATUS = "";
#endif

        private DeathAbility()
        {
            Name = "Death";
            Desc = "Death as a result of Death Sentence";
            
            Type = AttackType.Physical;
            Target = BattleTarget.Combatant;
            
            Power = 0;
            Hitp = 255;
            
            HitFormula = PhysicalHit;
            
            Statuses = new StatusChange[] { new StatusChange(Status.Death, 100, StatusChange.Effect.Inflict) };
        }

        public static void Use(Combatant source, Combatant target)
        {
            source.CurrentBattle.EnqueueAction(new AbilityEvent(INSTANCE, MODIFIERS, source, new Combatant[] { target }));
        }

        public override int PauseDuration { get { return 0; } }

        public override string GetMessage(Combatant source)
        {
            return STATUS;
        }
    }
}

