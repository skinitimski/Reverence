using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
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
    internal class AbilityFailEvent : CombatantActionEvent
    {
        private const int DURATION = 2000;
        
        public AbilityFailEvent(Combatant source, Ability ability, bool resetSourceTurnTimer)
            : base(source, resetSourceTurnTimer)
        {
            Ability = ability.Name;

            if (source is Ally)
            {
                Status = "Not enough MP for " + Ability + "!";
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
        
        public override string ToString()
        {
            return String.Format(" {0} fails to use {1}", Source.Name, Ability);
        }

        protected override int Duration { get { return DURATION; } }
        
        private string Status { get; set; }

        private string Ability { get; set; }
    }
}

