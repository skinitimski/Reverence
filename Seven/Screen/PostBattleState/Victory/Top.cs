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
                Config.Instance.WindowHeight * 13 / 60,
                Config.Instance.WindowWidth - 8,
                Config.Instance.WindowHeight / 4 - 6)
        { }

        protected override int PartyIndex { get { return 0; } }
    }
}

