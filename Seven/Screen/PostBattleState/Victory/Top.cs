using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using SevenPostBattleState = Atmosphere.Reverence.Seven.State.PostBattleState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{  
    internal sealed class Top : Info
    {
        public Top(SevenPostBattleState postBattleState, ScreenState screenState)
            : base(
                postBattleState,
                2,
                screenState.Height * 13 / 60,
                screenState.Width - 8,
                screenState.Height / 4 - 6,
                0)
        { }
    }
}

