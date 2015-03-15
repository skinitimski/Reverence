using System;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal enum ItemTarget
    {
        World,

        // Item-only
        Character,
        Party,

        // Battle-only
        Ally,
        Enemy,
        Combatant,
        AllAllies,
        AllEnemies,
        Battlefield,

        // Hybrid
        OneTarget,
        PartyAllies
    }
}

