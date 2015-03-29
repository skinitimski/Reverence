using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Magic
{       
    internal class WMagic : Main
    {        
        private class Choice
        {
            public int X;
            public int Y;
            public IEnumerable<Combatant> Targets;
        }
        
        public WMagic(IEnumerable<MagicSpell> spells)
            : base(spells)
        {
        }
        
        public override void ControlHandle(Key k)
        {
            bool cancel = false;

            switch (k)
            {
                case Key.Circle:

                    if (FirstChoice != null)
                    {
                        if (_spells[_yopt, _xopt] != null)
                        {
                            Spell spell = _spells[_yopt, _xopt].Spell;
                        
                            if (CommandingAvailableMP < spell.MPCost)
                            {
                                cancel = true;
                            }
                        }
                    }

                    break;
            }
            
            if (!cancel)
            {
                base.ControlHandle(k);
            }
        }


        
        public override bool ActOnSelection(IEnumerable<Combatant> targets)
        {
            if (FirstChoice != null)
            {
                UseSpell(FirstChoice.X, FirstChoice.Y, false);
                UseSpell(_xopt, _yopt);

                return true;
            }
            else
            {
                FirstChoice = new Choice
                {
                    X = _xopt,
                    Y = _yopt,
                    Targets = targets
                };

                return false;
            }
        }
        
        public override void Reset()
        {
            base.Reset();

            FirstChoice = null;
        }
        
        protected virtual int CommandingAvailableMP
        { 
            get
            {
                int mp = Seven.BattleState.Commanding.MP;

                if (FirstChoice != null)
                {
                    mp -= _spells[FirstChoice.X, FirstChoice.Y].Spell.MPCost;
                }

                return mp; 
            }
        }

        private Choice FirstChoice { get; set; }
    }    
}
