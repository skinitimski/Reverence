using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class RecoveryIcon : BattleIcon
    {
        private const string RECOVERY = "Recovery";
        
        
        public RecoveryIcon(Combatant receiver)
            : base(receiver)
        {
            Message = RECOVERY;
            Color = Colors.GREEN;
        }
    }
}

