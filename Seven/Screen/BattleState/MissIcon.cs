using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class MissIcon : BattleIcon
    {
        private const string MISS = "Miss";


        public MissIcon(Combatant receiver)
            : base(receiver)
        {
            Message = MISS;
        }
    }
}

