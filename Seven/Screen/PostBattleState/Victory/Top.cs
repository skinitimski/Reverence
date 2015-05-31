using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class VictoryTop : Info
    {
        public VictoryTop(Menu.ScreenState screenState)
            : base(
                2,
                screenState.Height * 13 / 60,
                screenState.Width - 8,
                screenState.Height / 4 - 6)
        { }

        protected override int PartyIndex { get { return 0; } }
    }
}

