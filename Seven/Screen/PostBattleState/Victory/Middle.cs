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
                Config.Instance.WindowHeight * 28 / 60,
                Config.Instance.WindowWidth - 8,
                Config.Instance.WindowHeight / 4 - 6)
        { }
        
        protected override int PartyIndex { get { return 1; } }
    }
}

