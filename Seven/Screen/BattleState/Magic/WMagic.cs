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
        
        public WMagic(IEnumerable<MagicMenuEntry> spells, Menu.ScreenState screenState)
            : base(spells, screenState)
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
                UseSpell(FirstChoice.X, FirstChoice.Y, FirstChoice.Targets, false);
                UseSpell(_xopt, _yopt, targets);

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
        
        protected override int CommandingAvailableMP
        { 
            get
            {
                int mp = base.CommandingAvailableMP;

                if (FirstChoice != null)
                {
                    mp -= _spells[FirstChoice.Y, FirstChoice.X].Spell.MPCost;
                }

                return mp; 
            }
        }

        private Choice FirstChoice { get; set; }
    }    
}
