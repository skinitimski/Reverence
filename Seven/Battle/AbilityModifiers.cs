using System;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class AbilityModifiers
    {
        public AbilityModifiers()
        {
        }

        public bool QuadraMagic { get; set; }
        public bool Alled { get; set; }
        public int MPTurboFactor { get; set; }
        public bool CostsNothing { get; set; }
        public bool NoSplit { get; set; }
        public bool StealAsWell { get; set; }

        public bool CounterAttack { get; set; }
        public bool ResetTurnTimer { get; set; }
    }
}

