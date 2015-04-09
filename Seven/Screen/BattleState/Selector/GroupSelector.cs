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
        private BattleTargetGroup _selectedGroup;

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
            }
            if (CanTargetEither)
            {
                switch (k)
                {
                    case Key.Left:
                        _selectedGroup = BattleTargetGroup.Enemies;
                        break;
                    case Key.Right:
                        _selectedGroup = BattleTargetGroup.Allies;
                        break;
                    default:
                        break;
                }   
            }
            
            base.ControlHandle(k);
        }
        
        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            if (IsControl)
            {
                switch (_selectedGroup)
                {
                    case BattleTargetGroup.Allies:
                        foreach (Combatant a in Seven.BattleState.Allies)
                        {
                            if (a != null)
                            {
                                Shapes.RenderCursor(g, a.X - CURSOR_SPACING, a.Y);
                            }
                        }
                        break;
                    case BattleTargetGroup.Enemies:
                        foreach (Combatant e in Seven.BattleState.EnemyList)
                        {
                            Shapes.RenderCursor(g, e.X - CURSOR_SPACING, e.Y);
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
                case BattleTargetGroup.Allies:
                    _selectedGroup = BattleTargetGroup.Allies;
                    break;
                case BattleTargetGroup.Enemies:
                    _selectedGroup = BattleTargetGroup.Enemies;
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

            DefaultSelection = BattleTargetGroup.Allies;
        }
        public void SelectOnlyEnemies()
        {
            CanTargetAllies = false;
            CanTargetEnemies = true;
            
            DefaultSelection = BattleTargetGroup.Enemies;
        }
        public void SelectEitherGroup(BattleTargetGroup defaultSelection)
        {
            CanTargetAllies = true;
            CanTargetEnemies = true;

            DefaultSelection = defaultSelection;
        }
        
        protected override IEnumerable<Combatant> Selected
        {
            get
            {
                IEnumerable<Combatant> selected = null;

                switch (_selectedGroup)
                {
                    case BattleTargetGroup.Allies:
                        selected =  Seven.BattleState.Allies.Cast<Combatant>();
                        break;
                    case BattleTargetGroup.Enemies:
                        selected =  Seven.BattleState.EnemyList.Cast<Combatant>();
                        break;
                }

                return selected;
            }
        }

        public override string Info
        {
            get
            {
                switch (_selectedGroup)
                {
                    case BattleTargetGroup.Allies:
                        return "All Allies";
                    case BattleTargetGroup.Enemies:
                        return "All enemies";
                    default:
                        return "";
                }
            }
        }

        private bool CanTargetAllies { get; set; }
        private bool CanTargetEnemies { get; set; }
        private bool CanTargetEither { get { return CanTargetAllies && CanTargetEnemies; } }
        private BattleTargetGroup DefaultSelection { get; set; }

    }
}

