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
        private class Choice
        {
            public int Slot;
            public IEnumerable<Combatant> Targets;
        }
            


        public WItemMenu()
            : base()
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
                        if (_option == FirstChoice.Slot && Seven.Party.Inventory.GetCount(_option) == 1)
                        {
                            cancel = true;
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
                UseItem(FirstChoice.Slot, FirstChoice.Targets, false);
                UseItem(_option, targets);

                return true;
            }
            else
            {
                FirstChoice = new Choice
                {
                    Slot = _option,
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

        private Choice FirstChoice { get; set; }
    }
}

