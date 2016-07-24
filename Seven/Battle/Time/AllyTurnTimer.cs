using System;

using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven.Battle.Time
{
    internal class AllyTurnTimer : TurnTimer
    {
        public AllyTurnTimer(Ally ally, long elapsed)
            : base(ally, elapsed)
        {     
        }

        public override long IncreasePerGameTick 
        {
            get
            {
                return (Combatant.Dexterity + 50) * Combatant.V_Timer.IncreasePerGameTick / Combatant.CurrentBattle.Party.NormalSpeed;
            }
        }
    }
}
       