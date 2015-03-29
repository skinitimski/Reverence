using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class WItemMenu : ItemMenu
    {
        private int _first = -1;
               
        public WItemMenu()
            : base()
        {
        }


        public override void ActOnSelection()
        {
            if (_first != -1)
            {
                // allocate to stack (option is on heap)
                int first = _first;
                int second = _option;
                                
                UseItem(first, false);
                UseItem(second);
            }
            else
            {
                _first = _option;
            }
        }
        
        public override void Reset()
        {
            base.Reset();
            _first = -1;
        }
    }
}

