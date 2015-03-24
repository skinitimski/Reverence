using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal sealed class GroupSelector : Selector
    {
        private TargetGroup _selectedGroup;

        public GroupSelector()
        {
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
//                case Key.R1:
//                    if (Seven.BattleState.Screen.AllAble)
//                    {
//                    if (Seven.BattleState.Commanding.MagicMenu.Selected.AllCount > 0)
//                    {
//                        Seven.BattleState.Screen.PopControl();
//                        Seven.BattleState.Screen.Type = TargetType.Combatant;
//                        Seven.BattleState.Screen.PushControl(Seven.BattleState.Screen.TargetSelector);
//                    }
//                    }
//                    break;
                case Key.Circle:
                    Seven.BattleState.Screen.User.ActOnSelection();
                    if (Seven.BattleState.Screen.RunActionHook)
                    {
                        Seven.BattleState.ActionHook();
                    }
                    else if (Seven.BattleState.Screen.RunClearControl)
                    {
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
            if (CanTargetEither)
            {
                return;
            }
            switch (k)
            {
                case Key.Left:
                    _selectedGroup = TargetGroup.Enemies;
                    break;
                case Key.Right:
                    _selectedGroup = TargetGroup.Allies;
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
                switch (_selectedGroup)
                {
                    case TargetGroup.Allies:
                        foreach (Combatant a in Seven.BattleState.Allies)
                        {
                            if (a != null)
                            {
                                Shapes.RenderCursor(g, a.X - 15, a.Y);
                            }
                        }
                        break;
                    case TargetGroup.Enemies:
                        foreach (Combatant e in Seven.BattleState.EnemyList)
                        {
                            Shapes.RenderCursor(g, e.X - 15, e.Y);
                        }
                        break;
                }
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            _isControl = true;
            switch (DefaultSelection)
            
            {
                case TargetGroup.Allies:
                    _selectedGroup = TargetGroup.Allies;
                    break;
                case TargetGroup.Enemies:
                    _selectedGroup = TargetGroup.Enemies;
                    break;
            }
        }

        public override void SetNotControl()
        {
            _isControl = false;
        }





        public void SelectOnlyAllies()
        {
            CanTargetAllies = true;
            CanTargetEnemies = false;

            DefaultSelection = TargetGroup.Allies;
        }
        public void SelectOnlyEnemies()
        {
            CanTargetAllies = false;
            CanTargetEnemies = true;
            
            DefaultSelection = TargetGroup.Enemies;
        }
        public void SelectEitherGroup(TargetGroup defaultSelection)
        {
            CanTargetAllies = true;
            CanTargetEnemies = true;

            DefaultSelection = defaultSelection;
        }
        
        public List<Combatant> Selected
        {
            get
            {
                switch (_selectedGroup)
                {
                    case TargetGroup.Allies:
                        return Seven.BattleState.Allies.Cast<Combatant>().ToList();
                    case TargetGroup.Enemies:
                        return Seven.BattleState.EnemyList.Cast<Combatant>().ToList();
                    default:
                        throw new ImplementationException(String.Format("GroupSelector targeting wrong group: {0}", _selectedGroup.ToString()));
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

        private bool CanTargetAllies { get; set; }
        private bool CanTargetEnemies { get; set; }
        private bool CanTargetEither { get { return CanTargetAllies && CanTargetEnemies; } }
        private TargetGroup DefaultSelection { get; set; }

    }
}

