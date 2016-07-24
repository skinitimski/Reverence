using System;

using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven.Battle.Time
{
    internal class EnemyTurnTimer : TurnTimer
    {
        public EnemyTurnTimer(Enemy enemy, long elapsed)
            : base(enemy, elapsed)
        {     
        }

        public override long IncreasePerGameTick 
        {
            get
            {
                return Combatant.Dexterity * Combatant.V_Timer.IncreasePerGameTick / Combatant.CurrentBattle.Party.NormalSpeed;
            }
        }
    }
}
       