using System;

using ScreenState = Atmosphere.Reverence.Menu.ScreenState;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{
    internal class Mastered : MateriaInfo
    {
        public Mastered(ScreenState state)
            : base((state.Width / 2) - (width / 2), 
                   10)
        {
        }
        
        protected override string Message
        {
            get
            {
                return " was born";
            }
        }
        
        protected override int MillisecondsPerMateria
        {
            get
            {
                return State.PostBattleState.MS_PER_BAR_FILL * 2 / 2;
            }
        }
    }
}

