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


            lock (_controllerStack)
            {
                if (stack)
                {
                    foreach (IController c in _controllerStack)
                    {
                        c.Draw(d);
                    }
                }
            }
        }
        

        public void PushControl(IController to)
        {
            lock (_controllerStack)
            {
                if (_controller != null)
                {
                    _controller.SetNotControl();
                }

                _controllerStack.Add(to);
                _controller = _controllerStack.Last();
                _controller.SetAsControl();
            }
        }
        public void PopControl()
        {
            if (_controller == null)
            {
                throw new ImplementationException("Hmm...no _control and you're Pop()ping?");
            }

            lock (_controllerStack)
            {
                _controller.SetNotControl();
                _controller.Reset();
                _controllerStack.RemoveAt(_controllerStack.Count - 1);
                _controller = _controllerStack.Last();
                _controller.SetAsControl();
            }
        }
        public void ClearControl()
        {
            if (_controller == null)
            {
                throw new ImplementationException("Hmm...no _control and you're Clear()ing?");
            }

            lock (_controllerStack)
            {
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
            }
        }





        public void GetSelection(BattleTarget target, bool targetEnemiesFirst = false)
        {
            switch (target)
            {
                case BattleTarget.Self:
                    SelectSelf();
                    break;
                case BattleTarget.Combatant:
                    SelectCombatant(targetEnemiesFirst ? BattleTargetGroup.Enemies : BattleTargetGroup.Allies);
                    break;
                case BattleTarget.Ally:
                    SelectAlly();
                    break;
                case BattleTarget.Enemy:
                    Seven.BattleState.Screen.SelectEnemy();
                    break;
                case BattleTarget.Group:
                    Seven.BattleState.Screen.SelectEitherGroup(targetEnemiesFirst ? BattleTargetGroup.Enemies : BattleTargetGroup.Allies);
                    break;
                case BattleTarget.Allies:
                    Seven.BattleState.Screen.SelectAllies();
                    break;
                case BattleTarget.Enemies:
                    Seven.BattleState.Screen.SelectEnemies();
                    break;
                case BattleTarget.Area:
                    Seven.BattleState.Screen.SelectArea();
                    break;
            }
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

        public void SelectCombatant(BattleTargetGroup defaultGroup)
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

        public void SelectEitherGroup(BattleTargetGroup defaultSelection)
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

            lock (_controllerStack)
            {
                User = (ISelectorUser)_controller;

                PushControl(selector);
            }
        }



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
