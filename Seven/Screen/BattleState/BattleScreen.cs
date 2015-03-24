using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using Screens = Atmosphere.Reverence.Seven.Screen.BattleState;
using Atmosphere.Reverence.Seven.Screen.BattleState.Selector;
using SelectorBase = Atmosphere.Reverence.Seven.Screen.BattleState.Selector.Selector;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal sealed class BattleScreen
    {
        private IController _controller;
        private List<IController> _controllerStack;

        private Mutex _mutex;





        public BattleScreen()
        {
            StatusBarLeft = new Screens.StatusBarLeft();
            StatusBarRight = new Screens.StatusBarRight();
            InfoBar = new Screens.InfoBar();
            EventBar = new Screens.EventBar();
            ItemMenu = new Screens.ItemMenu();
            WItemMenu = new Screens.WItemMenu();

            
            MagicInfo = new Screens.Magic.Info();  
            EnemySkillInfo = new Screens.EnemySkill.Info();  
            SummonMenuInfo = new Screens.Summon.Info();  
            
            SelfSelector = new Screens.Selector.SelfSelector ();
            TargetSelector = new Screens.Selector.TargetSelector();
            GroupSelector = new Screens.Selector.GroupSelector();
            AreaSelector = new Screens.Selector.AreaSelector();

            _controllerStack = new List<IController>();
            _controller = null;

            _mutex = new Mutex();

            RunActionHook = true;
        }

        public void Draw(Gdk.Drawable d)
        {
            Draw(d, true);
        }

        public void Draw(Gdk.Drawable d, bool stack)
        {
            StatusBarLeft.Draw(d);
            StatusBarRight.Draw(d);
            InfoBar.Draw(d);
            EventBar.Draw(d);


            _mutex.WaitOne();

            if (stack)
            {
                foreach (IController c in _controllerStack)
                {
                    c.Draw(d);
                }
            }

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
            {
                throw new ImplementationException("Hmm...no _control and you're Pop()ping?");
            }

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
            {
                throw new ImplementationException("Hmm...no _control and you're Clear()ing?");
            }

            _mutex.WaitOne();
            _controller.SetNotControl();
            while (_controllerStack.Count > 0)
            {
                if (_controllerStack.Last() is GameMenu)
                {
                    ((GameMenu)_controllerStack.Last()).Visible = false;
                }
                _controllerStack.Last().Reset();
                _controllerStack.RemoveAt(_controllerStack.Count - 1);
            }
            _controller = null;
            _mutex.ReleaseMutex();
        }









        public void SelectSelf()
        {
            PushSelector(SelfSelector);
        }

        public void SelectAlly()
        {
            TargetSelector.SelectOnlyAllies();
            
            SelectTarget();
        }

        public void SelectEnemy()
        {
            TargetSelector.SelectOnlyEnemies();
            
            SelectTarget();
        }

        public void SelectCombatant(TargetGroup defaultGroup)
        {
            TargetSelector.SelectEitherGroup(defaultGroup);

            SelectTarget();
        }

        private void SelectTarget()
        {            
//            if (allAble && !(_controller is Screens.Magic.Main))
//            {
//                throw new ImplementationException("All-switch only implemented from MagicMenu.");
//            }

            PushSelector(TargetSelector);
        }
        
        public void SelectAllies()
        {
            GroupSelector.SelectOnlyAllies();
            
            SelectGroup();
        }
        
        public void SelectEnemies()
        {
            GroupSelector.SelectOnlyAllies();
            
            SelectGroup();
        }

        public void SelectEitherGroup(TargetGroup defaultSelection)
        {
            GroupSelector.SelectEitherGroup(defaultSelection);

            SelectGroup();
        }

        private void SelectGroup()
        {            
            PushSelector(GroupSelector);
        }
        
        public void SelectArea()
        {
            PushSelector(AreaSelector);
        }

        private void PushSelector(SelectorBase selector)
        {
            if (!(_controller is ISelectorUser))
            {
                throw new ImplementationException("Tried to switch control to the Selector from a non-SelectorUser.");
            }

            User = (ISelectorUser)_controller;

            PushControl(selector);
        }

//        public void GetSelection(TargetGroup tg, TargetType tt)
//        {
//            GetSelection(tg, tt, false);
//                        
//            PushControl(Seven.BattleState.Screen.TargetSelector);
//        }
//
//        /// <summary>Activates the Selector.</summary>
//        /// <param name="tg">Valid TargetGroup for selection</param>
//        /// <param name="tt">Valid TargetType for selection</param>
//        /// <param name="allAble">Whether or not this ability can switch between one and all targets.</param>
//        /// <remarks>If allAble, use TargetType.OneTar, since if the ability isn't attached to All,
//        /// then this method will use the tt parameter.</remarks>
//        public void GetSelection(TargetGroup tg, TargetType tt, bool allAble)
//        {
//            if (!(_controller is ISelectorUser))
//            {
//                throw new ImplementationException("Tried to switch control to the Selector from a non-SelectorUser.");
//            }
//
//            // If allAble, use TargetType.OneTar. If it's all-able, but not attached to All,
//            //    then this must be able to use the tt parameter
//
//            if (allAble && !(_controller is Screens.Magic.Main))
//            {
//                throw new ImplementationException("All-switch only implemented from MagicMenu.");
//            }
//
//            _mutex.WaitOne();
//
//            User = (ISelectorUser)_controller;
//            //Group = tg;
//            //Type = tt;
//            //AllAble = allAble && Seven.BattleState.Commanding.MagicMenu.Selected.AllCount > 0;
//
////            if (AllAble)
////            {
////                PushControl(Seven.BattleState.Screen.GroupSelector);
////            }
////            else
////            {
//                switch (tt)
//                {
//                    case TargetType.Self:
//                        PushControl(Seven.BattleState.Screen.SelfSelector);
//                        break;
//                    case TargetType.Combatant:
//                        PushControl(Seven.BattleState.Screen.TargetSelector);
//                        break;
//                    case TargetType.GroupNS:
//                    case TargetType.Group:
//                    //case TargetType.NTar:
//                        PushControl(Seven.BattleState.Screen.GroupSelector);
//                        break;
//                    case TargetType.Area:
//                        PushControl(Seven.BattleState.Screen.AreaSelector);
//                        break;
//                }
////            }
//
//            _mutex.ReleaseMutex();
//        }



        public IController Control { get { return _controller; } }

        public Screens.StatusBarLeft StatusBarLeft { get; private set; }
        public Screens.StatusBarRight StatusBarRight { get; private set; }
        public Screens.InfoBar InfoBar { get; private set; }
        public Screens.EventBar EventBar { get; private set; }
        
        public Screens.ItemMenu ItemMenu { get; private set; }
        public Screens.WItemMenu WItemMenu { get; private set; }
        
        public Screens.Magic.Info MagicInfo { get; private set; }
        
        public Screens.Summon.Info SummonMenuInfo { get; private set; }
        
        public Screens.EnemySkill.Info EnemySkillInfo { get; private set; }
        
        public Screens.Selector.SelfSelector SelfSelector { get; private set; }
        public Screens.Selector.TargetSelector TargetSelector { get; private set; }
        public Screens.Selector.GroupSelector GroupSelector { get; private set; }
        public Screens.Selector.AreaSelector AreaSelector { get; private set; }










        //
        // Selector
        //

        
        private bool _runClear = false;
        
        public void DisableActionHook(bool clear)
        {
            RunActionHook = false;
            _runClear = clear;
        }
        public void DisableActionHook()
        {
            DisableActionHook(false);
        }
        
        public bool RunClearControl
        {
            get
            {
                // Clear is only relevant if we're not running the ActionHook().
                // The ActionHook runs clear anyway.
                if (!RunActionHook)
                {
                    return _runClear;
                }

                return false;
            }
        }
        public bool RunActionHook { get; private set; }
        
        
        public ISelectorUser User { get;  private set; }










    }
}
