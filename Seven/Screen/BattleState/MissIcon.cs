using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;
using StateOfBattle = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class MissIcon : BattleIcon
    {
        private const string MISS = "Miss";


        public MissIcon(StateOfBattle battle, Combatant receiver)
            : base(battle, receiver)
        {
            Message = MISS;
        }
    }
}

