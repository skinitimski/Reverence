using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Atmosphere.BattleSimulator
{
    public sealed class BattleScreen
    {
        private Menu[] _menus;

        private IController _controller;
        private List<IController> _controllerStack;

        private Mutex _mutex;


        public static BattleScreen Instance;


        static BattleScreen()
        {
            Instance = new BattleScreen();

            Game.Lua.RegisterFunction("GetSelection", Instance, Instance.GetType().GetMethod("GetSelection"));
        }
        private BattleScreen()
        {
            Init();
        }

        public void Init()
        {
            _menus = new Menu[4];
            _menus[0] = StatusBarLeft.Instance;
            _menus[1] = StatusBarRight.Instance;
            _menus[2] = InfoBar.Instance;
            _menus[3] = EventBar.Instance;

            _controllerStack = new List<IController>();
            _controller = null;

            _mutex = new Mutex();
        }

        public void Draw(Gdk.Drawable d)
        {
            Draw(d, true);
        }
        public void Draw(Gdk.Drawable d, bool stack)
        {
            foreach (Menu m in _menus)
                m.Draw(d);

            _mutex.WaitOne();
            if (stack)
                foreach (IController c in _controllerStack)
                    c.Draw(d);
            _mutex.ReleaseMutex();
        }
        

        public void PushControl(IController to)
        {
            _mutex.WaitOne();
            if (_controller != null)
                _controller.SetNotControl();

            _controllerStack.Add(to);
            _controller = _controllerStack.Last();
            _controller.SetAsControl();
            _mutex.ReleaseMutex();
        }
        public void PopControl()
        {
            if (_controller == null)
                throw new GameImplementationException("Hmm...no _control and you're Pop()ping?");

            _mutex.WaitOne();
            _controller.SetNotControl();
            _controller.Reset();
            _controllerStack.RemoveAt(_controllerStack.Count - 1);
            _controller = _controllerStack.Last();
            _controller.SetAsControl();
            _mutex.ReleaseMutex();
        }
        public void ClearControl()
        {
            if (_controller == null)
                throw new GameImplementationException("Hmm...no _control and you're Clear()ing?");

            _mutex.WaitOne();
            _controller.SetNotControl();
            while (_controllerStack.Count > 0)
            {
                if (_controllerStack.Last() is Menu)
                    ((Menu)_controllerStack.Last()).Visible = false;
                _controllerStack.Last().Reset();
                _controllerStack.RemoveAt(_controllerStack.Count - 1);
            }
            _controller = null;
            _mutex.ReleaseMutex();
        }
        public void GetSelection(TargetGroup tg, TargetType tt)
        {
            GetSelection(tg, tt, false);
        }

        /// <summary>Activates the Selector.</summary>
        /// <param name="tg">Valid TargetGroup for selection</param>
        /// <param name="tt">Valid TargetType for selection</param>
        /// <param name="allAble">Whether or not this ability can switch between one and all targets.</param>
        /// <remarks>If allAble, use TargetType.OneTar, since if the ability isn't attached to All,
        /// then this method will use the tt parameter.</remarks>
        public void GetSelection(TargetGroup tg, TargetType tt, bool allAble)
        {
            if (!(_controller is ISelectorUser))
                throw new GameImplementationException("Tried to switch control to the Selector from a non-ISelectorUser.");

            // If allAble, use TargetType.OneTar. If it's all-able, but not attached to All,
            //    then this must be able to use the tt parameter

            if (allAble && !(_controller is MagicMenu))
                throw new GameImplementationException("All-switch only implemented from MagicMenu.");

            _mutex.WaitOne();
            Selector.User = (ISelectorUser)_controller;
            Selector.Group = tg;
            Selector.Type = tt;
            Selector.AllAble = allAble && Game.Battle.Commanding.MagicMenu.Selected.AllCount > 0;
            if (Selector.AllAble)
                PushControl(GroupSelector.Instance);
            else
                switch (tt)
                {
                    case TargetType.Self:
                        PushControl(SelfSelector.Instance);
                        break;
                    case TargetType.OneTar:
                        PushControl(TargetSelector.Instance);
                        break;
                    case TargetType.AllTarNS:
                    case TargetType.AllTar:
                    case TargetType.NTar:
                        PushControl(GroupSelector.Instance);
                        break;
                    case TargetType.Field:
                        PushControl(FieldSelector.Instance);
                        break;
                }
            _mutex.ReleaseMutex();
        }



        public IController Control { get { return _controller; } }
        public Menu[] Menus { get { return _menus; } }

    }
}
