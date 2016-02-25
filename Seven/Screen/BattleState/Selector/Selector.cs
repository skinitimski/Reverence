﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal abstract class Selector : IController
    {
        protected const int CURSOR_SPACING = 25;

        protected bool _isControl = false;

        protected Selector(SevenBattleState battleState)
        {
            BattleState = battleState;
        }
        
        public virtual void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:

                    bool clear = BattleState.Screen.User.ActOnSelection(Selected);

                    if (clear)
                    {
                        BattleState.Commanding.WaitingToResolve = true;
                        BattleState.ClearControl();
                    }
                    else
                    {
                        BattleState.Screen.PopControl();
                    }

                    break;

                case Key.X:
                    BattleState.ActionAbort();
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
        
        protected SevenBattleState BattleState { get; private set; }
    }
}