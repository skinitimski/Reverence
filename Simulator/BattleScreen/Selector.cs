using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

namespace Atmosphere.BattleSimulator
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

        public abstract ICombatant[] Selected { get; }

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



    internal sealed class SelfSelector : Selector
    {
        public static SelfSelector Instance;

        private int _option;

        private List<ICombatant> _targets;


        static SelfSelector()
        {
            Instance = new SelfSelector();
            Game.Lua["SelfSelector"] = Instance;
        }
        private SelfSelector()
        {
            _targets = new List<ICombatant>();
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:
                    Game.Battle.Commanding.Ability.Target = Selected;
                    User.ActOnSelection();
                    if (RunActionHook)
                        Game.Battle.ActionHook();
                    else if (RunClearControl)
                        Game.Battle.ClearControl();
                    else
                        Game.Battle.Screen.PopControl();
                    break;
                case Key.X:
                    Game.Battle.ActionAbort();
                    break;
                default:
                    break;
            }
        }

        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            if (IsControl)
                Graphics.RenderCursor(g,
                    _targets[_option].X - 15,
                    _targets[_option].Y);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            _isControl = true;
            _targets = new List<ICombatant>();
            _targets.Add(Game.Battle.Commanding);

            _option = 0;
        }
        public override void SetNotControl() { _isControl = false; }

        public override bool IsControl { get { return _isControl; } }

        public override ICombatant[] Selected
        {
            get
            {
                ICombatant[] ret = new ICombatant[1];
                ret[0] = _targets[_option];
                return ret;
            }
        }
        public override string Info
        { get { return _targets[_option].ToString(); } }
    }

    internal sealed class TargetSelector : Selector
    {
        public static TargetSelector Instance;

        private int _option;

        private List<ICombatant> _targets;


        static TargetSelector()
        {
            Instance = new TargetSelector();
            Game.Lua["TargetSelector"] = Instance;
        }
        private TargetSelector()
        {
            _targets = new List<ICombatant>();
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.R1:
                    if (AllAble)
                        if (Game.Battle.Commanding.MagicMenu.Selected.AllCount > 0)
                        {
                            Game.Battle.Screen.PopControl();
                            Selector.Type = TargetType.AllTar;
                            Game.Battle.Screen.PushControl(GroupSelector.Instance);
                        }
                    break;
                case Key.Up:
                    AdvanceOption(delegate(int i) { return _targets[i].Y > _targets[_option].Y; }); 
                    break;
                case Key.Down:
                    AdvanceOption(delegate(int i) { return _targets[i].Y < _targets[_option].Y; }); 
                    break;
                case Key.Left:
                    AdvanceOption(delegate(int i) { return _targets[i].X > _targets[_option].X; }); 
                    break;
                case Key.Right:
                    AdvanceOption(delegate(int i) { return _targets[i].X < _targets[_option].X; }); 
                    break;
                case Key.Circle:
                    Game.Battle.Commanding.Ability.Target = Selected;
                    User.ActOnSelection();
                    if (RunActionHook)
                        Game.Battle.ActionHook();
                    else if (RunClearControl)
                        Game.Battle.ClearControl();
                    else
                        Game.Battle.Screen.PopControl();
                    break;
                case Key.X:
                    Game.Battle.ActionAbort();
                    break;
                default:
                    break;
            }
        }

        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            if (IsControl)
                Graphics.RenderCursor(g,
                    _targets[_option].X - 15,
                    _targets[_option].Y);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private delegate bool HalfplanePredicate(int i);

        private void AdvanceOption(HalfplanePredicate ap)
        {
            int x0 = _targets[_option].X;
            int y0 = _targets[_option].Y;

            // length (squared) of the screen's diagonal (largest fathomable distance)
            int min = Globals.HEIGHT * Globals.HEIGHT + Globals.WIDTH * Globals.WIDTH; 

            int pick = -1;
            for (int i = 0; i < _targets.Count; i++)
            {
                if (i == _option) // skip current option
                    continue;
                if (ap(i)) // skip options not in the desired half-plane
                    continue;

                // calculate distance
                int dx = _targets[i].X - x0;
                int dy = _targets[i].Y - y0;
                int ds_sq = dx * dx + dy * dy;

                if (ds_sq < min)
                {
                    pick = i;
                    min = ds_sq;
                }
            }
            if (pick >= 0)
                _option = pick;
        }

        public override void SetAsControl()
        {
            _isControl = true;
            _targets = new List<ICombatant>();

            switch (Group)
            {
                case TargetGroup.Allies:
                    foreach (Ally a in Game.Battle.Allies)
                        if (a != null) _targets.Add(a);
                    break;
                case TargetGroup.Enemies:
                    foreach (Enemy e in Game.Battle.EnemyList)
                        _targets.Add(e);
                    break;
                case TargetGroup.Area:
                    foreach (Ally a in Game.Battle.Allies)
                        if (a != null) _targets.Add(a);
                    foreach (Enemy e in Game.Battle.EnemyList)
                        _targets.Add(e);
                    break;
            }

            _option = 0;
        }
        public override void SetNotControl() { _isControl = false; }

        public override bool IsControl { get { return _isControl; } }

        public override ICombatant[] Selected
        {
            get
            {
                ICombatant[] ret = new ICombatant[1];
                ret[0] = _targets[_option];
                return ret;
            }
        }
        public override string Info
        { get { return _targets[_option].ToString(); } }
    }

    internal sealed class GroupSelector : Selector
    {
        public static GroupSelector Instance;

        private TargetGroup _selectedGroup;


        static GroupSelector()
        {
            Instance = new GroupSelector();
            Game.Lua["GroupSelector"] = Instance;
        }
        private GroupSelector()
        {
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.R1:
                    if (AllAble)
                        if (Game.Battle.Commanding.MagicMenu.Selected.AllCount > 0)
                        {
                            Game.Battle.Screen.PopControl();
                            Selector.Type = TargetType.OneTar;
                            Game.Battle.Screen.PushControl(TargetSelector.Instance);
                        }
                    break;
                case Key.Circle:
                    Game.Battle.Commanding.Ability.Target = Selected;
                    User.ActOnSelection();
                    if (RunActionHook)
                        Game.Battle.ActionHook();
                    else if (RunClearControl)
                        Game.Battle.ClearControl();
                    else
                        Game.Battle.Screen.PopControl();
                    break;
                case Key.X:
                    Game.Battle.ActionAbort();
                    break;
                default: break;
            }
            if (Group != TargetGroup.Area)
                return;
            switch (k)
            {
                case Key.Left:
                    _selectedGroup = TargetGroup.Enemies;
                    break;
                case Key.Right:
                    _selectedGroup = TargetGroup.Allies;
                    break;
                default: break;
            }
        }

        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            if (IsControl)
                switch (_selectedGroup)
                {
                    case TargetGroup.Allies:
                        foreach (ICombatant a in Game.Battle.Allies)
                            if (a != null)
                                Graphics.RenderCursor(g, a.X - 15, a.Y);
                        break;
                    case TargetGroup.Enemies:
                        foreach (ICombatant e in Game.Battle.EnemyList)
                            Graphics.RenderCursor(g, e.X - 15, e.Y);
                        break;
                }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            _isControl = true;

            switch (Group)
            {
                case TargetGroup.Allies:
                    _selectedGroup = TargetGroup.Allies;
                    break;
                case TargetGroup.Enemies:
                case TargetGroup.Area:
                    _selectedGroup = TargetGroup.Enemies;
                    break;
            }
        }
        public override void SetNotControl() { _isControl = false; }

        public override bool IsControl { get { return _isControl; } }

        public override ICombatant[] Selected
        {
            get
            {
                switch (_selectedGroup)
                {
                    case TargetGroup.Allies:
                        return Game.Battle.Allies;
                    case TargetGroup.Enemies:
                        return Game.Battle.EnemyList.ToArray();
                    default: throw new GameImplementationException(String.Format("GroupSelector targeting wrong group: {0}", _selectedGroup.ToString()));
                }
            }
        }
        public override string Info
        {
            get
            {
                switch (_selectedGroup)
                {
                    case TargetGroup.Allies:
                        return "All Allies";
                    case TargetGroup.Enemies:
                        return "All enemies";
                    default:
                        return "";
                }
            }
        }
    }

    internal sealed class FieldSelector : Selector
    {
        public static FieldSelector Instance;

        private List<ICombatant> _targets;

        static FieldSelector()
        {
            Instance = new FieldSelector();
            Game.Lua["FieldSelector"] = Instance;
        }
        private FieldSelector()
        {
            _targets = new List<ICombatant>();
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:
                    Game.Battle.Commanding.Ability.Target = Selected;
                    User.ActOnSelection();
                    if (RunActionHook)
                        Game.Battle.ActionHook();
                    else if (RunClearControl)
                        Game.Battle.ClearControl();
                    else
                        Game.Battle.Screen.PopControl();
                    break;
                case Key.X:
                    Game.Battle.ActionAbort();
                    break;
                default:
                    break;
            }
        }

        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            if (IsControl)
            {
                foreach (ICombatant a in Game.Battle.Allies)
                    Graphics.RenderCursor(g, a.X - 15, a.Y);
                foreach (ICombatant e in Game.Battle.EnemyList)
                    Graphics.RenderCursor(g, e.X - 15, e.Y);
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl() { _isControl = true; }
        public override void SetNotControl() { _isControl = false; }

        public override bool IsControl { get { return _isControl; } }

        public override ICombatant[] Selected
        {
            get
            {
                int count = 0;
                foreach (ICombatant a in Game.Battle.Allies)
                    count++;
                count += Game.Battle.EnemyList.Count;
                ICombatant[] ret = new ICombatant[count];
                int i = 0;
                foreach (ICombatant a in Game.Battle.Allies)
                {
                    if (a != null) ret[i] = a;
                    i++;
                }
                foreach (ICombatant e in Game.Battle.EnemyList)
                {
                    ret[i] = e;
                    i++;
                }
                return ret;
            }
        }
        public override string Info
        { get { return "All combatants"; } }
    }

}