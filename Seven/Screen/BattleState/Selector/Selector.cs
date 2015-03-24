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
        protected bool _isControl = false;

        public abstract void ControlHandle(Key k);
        public abstract void Draw(Gdk.Drawable d);

        public abstract void SetAsControl();
        public abstract void SetNotControl();

        public bool IsControl { get { return _isControl; } }
        
        public virtual void Reset() 
        {
//            RunActionHook = true;
//            _runClear = false;
//            AllAble = false;
        }
        
        public abstract string Info { get; }

    }

}