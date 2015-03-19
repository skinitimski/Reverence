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
    public interface ISelectorUser : IController
    {
        void ActOnSelection();
    }

    public abstract class Selector : IController
    {
        public static ISelectorUser _user;
        public static TargetGroup _group;
        public static TargetType _type;

        private static bool _runHook = true;
        private static bool _runClear = false;
        private static bool _allAble = false;

        protected bool _isControl = false;

        public abstract void ControlHandle(Key k);
        public abstract void Draw(Gdk.Drawable d);

        public abstract void SetAsControl();
        public abstract void SetNotControl();

        public abstract bool IsControl { get; }

        public abstract Combatant[] Selected { get; }

        public static void DisableActionHook(bool clear)
        {
            _runHook = false;
            _runClear = clear;
        }
        public static void DisableActionHook()
        {
            _runHook = false;
            _runClear = false;
        }

        public virtual void Reset() 
        {
            _runHook = true;
            _runClear = false;
            _allAble = false;
        }

        protected static bool RunClearControl
        {
            get
            {
                // Clear is only relevant if we're not running the ActionHook().
                // The ActionHook runs clear anyway.
                if (!RunActionHook)
                    return _runClear;
                else return false;
            }
        }
        protected static bool RunActionHook { get { return _runHook; } }
        public static bool Selecting
        {
            get
            {
                return TargetSelector.Instance.IsControl ||
                    FieldSelector.Instance.IsControl ||
                    GroupSelector.Instance.IsControl;
            }
        }
        public abstract string Info { get; }
        public static ISelectorUser User { get { return _user; } set { _user = value; } }
        public static TargetGroup Group { get { return _group; } set { _group = value; } }
        public static TargetType Type { get { return _type; } set { _type = value; } }
        public static bool AllAble { get { return _allAble; } set { _allAble = value; } }
    }

}