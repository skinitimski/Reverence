using System;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal enum FormationType
    {
        Normal,

        /// <summary>
        /// Allies are attacked from behind by enemies.
        /// </summary>
        Back,

        /// <summary>
        /// Enemies are attacked from behind by allies.
        /// </summary>
        PreEmptive,

        /// <summary>
        /// Allies are attacked from both sides by enemies.
        /// </summary>
        Pincer,
        
        /// <summary>
        /// Enemies are attacked from behind by allies.
        /// </summary>
        Side,
    }
}

