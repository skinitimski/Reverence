using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class DamageIcon : BattleIcon
    {
        public DamageIcon(int amount, Combatant receiver, bool mp = false)
           : base(receiver)
        {
            Message = amount.ToString();

            if (mp)
            {
                Message += " MP";
            }

            if (amount < 0)
            {
                Message = Message.Substring(1); // drop minus sign
                Color = Colors.GREEN;
            }
        }
    }
}

