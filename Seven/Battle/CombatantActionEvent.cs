using System;
using System.Text;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal abstract class CombatantActionEvent : BattleEvent
    {
        protected CombatantActionEvent(Combatant source, bool resetSourceTurnTimer, int duration)
            : base(source.CurrentBattle.TimeFactory, duration)
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
        
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" {0} : {1}{2}", GetType().Name, GetStatus(), Environment.NewLine);
            sb.AppendFormat("\tsource {0}", Source == null ? "" : Source.Name);
            sb.AppendLine();
            return sb.ToString();
        }



        protected Combatant Source { get; set; }

        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Atmosphere.Reverence.Seven.Battle.CombatantActionEvent"/>
        /// should reset the turn timer of its source when it completes its action. Defaults to true.
        /// </summary>
        protected bool ResetSourceTurnTimer { get; set; }
    }
}

