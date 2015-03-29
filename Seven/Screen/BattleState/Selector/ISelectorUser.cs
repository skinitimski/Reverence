using System;
using System.Collections.Generic;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal interface ISelectorUser : IController
    {
        /// <summary>
        /// Acts on the selection.
        /// </summary>
        /// <returns>
        /// True if the control stack should be cleared, false otherwise.
        /// </returns>
        bool ActOnSelection(IEnumerable<Combatant> selected);
    }
}

