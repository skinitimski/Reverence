using System;

using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven.Battle.Time
{
    internal abstract class TurnTimer : BattleClock
    {
        public const long TURN_TIMER_MAX_VALUE = 65535;

        public TurnTimer(Combatant combatant, long elapsed)
            : base(0, elapsed, false)
        {     
            Combatant = combatant;
        }

        protected Combatant Combatant { get; set; }

        public bool IsUp 
        { 
            get 
            { 
                return CurrentValue >= TURN_TIMER_MAX_VALUE; 
            } 
        }
                
        public int PercentComplete
        {
            get
            {
                int pe = 0;
                
                if (IsUp)
                {
                    pe = 100;
                }
                else
                {
                    pe = (int)(CurrentValue * 100L / TURN_TIMER_MAX_VALUE);
                }
                
                return pe;
            }
        }
    }
}
       