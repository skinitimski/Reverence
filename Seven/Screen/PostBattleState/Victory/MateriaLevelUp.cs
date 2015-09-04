using System;

using ScreenState = Atmosphere.Reverence.Menu.ScreenState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{
    internal class MateriaLevelUp : MateriaInfo
    {
        public MateriaLevelUp(ScreenState state, int partyIndex)
            : base((state.Width / 2) - (width / 2), 
                   (partyIndex * (state.Height / 4 - 6)) + 190)
        {
        }

        protected override string Message
        {
            get
            {
                return " level up";
            }
        }

        protected override int MillisecondsPerMateria
        {
            get
            {
                return State.PostBattleState.MS_PER_BAR_FILL / 2;
            }
        }
    }
}

