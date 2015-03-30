using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal abstract class Selector : IController
    {
        protected const int CURSOR_SPACING = 25;

        protected bool _isControl = false;
        
        public virtual void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:

                    bool clear = Seven.BattleState.Screen.User.ActOnSelection(Selected);

                    if (clear)
                    {
                        Seven.BattleState.Commanding.WaitingToResolve = true;
                        Seven.BattleState.ClearControl();
                    }
                    else
                    {
                        Seven.BattleState.Screen.PopControl();
                    }

                    break;

                case Key.X:
                    Seven.BattleState.ActionAbort();
                    break;
                default:
                    break;
            }
        }

        public abstract void Draw(Gdk.Drawable d);

        public abstract void SetAsControl();
        public abstract void SetNotControl();

        public bool IsControl { get { return _isControl; } }
        
        public void Reset() 
        {
//            AllAble = false;
        }
        
        public abstract string Info { get; }

        protected abstract IEnumerable<Combatant> Selected { get; }

    }

}