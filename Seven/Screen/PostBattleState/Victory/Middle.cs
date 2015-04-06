using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryMiddle : Info
    {
        public VictoryMiddle()
            : base(
                2,
                Seven.Config.WindowHeight * 28 / 60,
                Seven.Config.WindowWidth - 8,
                Seven.Config.WindowHeight / 4 - 6)
        { }
        
        protected override int PartyIndex { get { return 1; } }
    }
}

