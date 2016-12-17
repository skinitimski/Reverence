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
        private static readonly AbilityModifiers MODIFIERS = new AbilityModifiers();

        private DeathAbility(Combatant source, Combatant target)
        {
            Name = "Death";
            Desc = "Death as a result of Death Sentence";
            
            Type = AttackType.Physical;
            Target = BattleTarget.Combatant;
            
            Power = 0;
            Hitp = 255;
            
            HitFormula = PhysicalHit;
            
            Statuses = new StatusChange[] { new StatusChange(Status.Death, 100, StatusChange.Effect.Inflict) };
            
#if DEBUG
            Message = String.Format("[ {0} reaps {1} ]", source, target);
#else
            Message = String.Empty;
#endif
        }

        public static void Use(Combatant source, Combatant target)
        {
            DeathAbility ability = new DeathAbility(source, target);

            source.CurrentBattle.EnqueueAction(new AbilityEvent(ability, MODIFIERS, source, new Combatant[] { target }));
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

