using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Magic
{       
    internal class WMagic : Main
    {
        private int[] _first = new int[] { -1, -1 };
        
        public WMagic(IEnumerable<MagicSpell> spells)
            : base(spells)
        {
        }

        
        public void ActOnSelection()
        {
            if (_first[0] != -1)
            {
                UseSpell(_first[0], _first[1], false);
                UseSpell(_xopt, _yopt);

            }
            else
            {
                _first[0] = _xopt;
                _first[1] = _yopt;
            }
        }

        
        public override void Reset()
        {
            base.Reset();
            _first[0] = -1;
            _first[1] = -1;
        }

        
        
        protected virtual int CommandingAvailableMP 
        { 
            get
            {
                int mp = Seven.BattleState.Commanding.MP;

                if (_first[0] != -1)
                {
                    mp -= _spells[_first[0], _first[1]].Spell.MPCost;
                }

                return mp; 
            }
        }
    }    
}
