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

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Summon
{       
    internal class WSummon : Main
    {        
        private class Choice
        {
            public int Option;
            public IEnumerable<Combatant> Targets;
        }
        
        public WSummon(IEnumerable<SummonMenuEntry> spells, Menu.ScreenState screenState)
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
                        if (_summons[_option] != null)
                        {
                            Ability spell = _summons[_option].Spell;
                        
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
                UseSpell(FirstChoice.Option, FirstChoice.Targets, false);
                UseSpell(_option, targets);

                return true;
            }
            else
            {
                FirstChoice = new Choice
                {
                    Option = _option,
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
                    mp -= _summons[FirstChoice.Option].Spell.MPCost;
                }

                return mp; 
            }
        }

        private Choice FirstChoice { get; set; }
    }    
}
