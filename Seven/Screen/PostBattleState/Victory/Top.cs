using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryTop : Info
    {
        public VictoryTop()
            : base(
                2,
                Seven.Config.WindowHeight * 13 / 60,
                Seven.Config.WindowWidth - 8,
                Seven.Config.WindowHeight / 4 - 6)
        { }

        protected override int PartyIndex { get { return 0; } }
    }
}

