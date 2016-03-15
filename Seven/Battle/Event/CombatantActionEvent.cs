using System;
using System.Text;

namespace Atmosphere.Reverence.Seven.Battle.Event
{
    internal abstract class CombatantActionEvent : BattleEvent
    {
        protected CombatantActionEvent(Combatant source, bool resetSourceTurnTimer)
            : base()
        {
            Source = source;
            ResetSourceTurnTimer = resetSourceTurnTimer;
        }

        protected override void CompleteHook()
        {            
            if (ResetSourceTurnTimer)
            {
                Source.TurnTimer.Reset();
                
                Source.WaitingToResolve = false;
            }
        }
        



        public Combatant Source { get; private set; }

        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Atmosphere.Reverence.Seven.Battle.CombatantActionEvent"/>
        /// should reset the turn timer of its source when it completes its action. Defaults to true.
        /// </summary>
        protected bool ResetSourceTurnTimer { get; set; }
    }
}

