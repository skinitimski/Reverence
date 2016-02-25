using System;

using GameState = Atmosphere.Reverence.State;

namespace Atmosphere.Reverence.Seven.State
{
    internal abstract class State : GameState
    {
        protected State(Seven seven)
            : base(seven)
        {
            Seven = seven;
        }


        public Seven Seven { get; private set; }

        public Party Party { get { return Seven.Party; } }
    }
}

