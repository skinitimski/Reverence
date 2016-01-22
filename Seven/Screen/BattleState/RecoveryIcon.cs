using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;
using StateOfBattle = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class RecoveryIcon : BattleIcon
    {
        private const string RECOVERY = "Recovery";
        
        
        public RecoveryIcon(StateOfBattle battle, Combatant receiver)
            : base(battle, receiver)
        {
            Message = RECOVERY;
            Color = Colors.GREEN;
        }
    }
}

