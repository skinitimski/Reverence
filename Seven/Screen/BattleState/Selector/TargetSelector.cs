using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal sealed class TargetSelector : Selector
    {
        private delegate bool HalfplanePredicate(int i);


        private int _option;
        private List<Combatant> _targets;

        public TargetSelector(SevenBattleState battleState)
            : base(battleState)
        {
            _targets = new List<Combatant>();
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.R1:
//                    if (AllAble)
//                    {
//                        if (BattleState.Commanding.MagicMenu.Selected.AllCount > 0)
//                        {
//                            BattleState.Screen.PopControl();
//                            BattleState.Screen.Type = TargetType.Group;
//                            BattleState.Screen.PushControl(BattleState.Screen.GroupSelector);
//                        }
//                    }
                    break;
                case Key.Up:
                    AdvanceOption(i => _targets[i].Y > _targets[_option].Y); 
                    break;
                case Key.Down:
                    AdvanceOption(i => _targets[i].Y < _targets[_option].Y); 
                    break;
                case Key.Left:
                    AdvanceOption(i => _targets[i].X > _targets[_option].X);
                    break;
                case Key.Right:
                    AdvanceOption(i => _targets[i].X < _targets[_option].X);
                    break;
                default:
                    break;
            }

            base.ControlHandle(k);
        }
        
        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, _targets[_option].X - CURSOR_SPACING, _targets[_option].Y);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        
        private void AdvanceOption(HalfplanePredicate ap)
        {
            int x0 = _targets[_option].X;
            int y0 = _targets[_option].Y;
            
            // length (squared) of the screen's diagonal (largest fathomable distance)
            int min = BattleState.Seven.Configuration.WindowHeight * BattleState.Seven.Configuration.WindowHeight + 
                BattleState.Seven.Configuration.WindowWidth * BattleState.Seven.Configuration.WindowWidth; 
            
            int pick = -1;
            for (int i = 0; i < _targets.Count; i++)
            {
                if (i == _option)
                { // skip current option
                    continue;
                }
                if (ap(i))
                { // skip options not in the desired half-plane
                    continue;
                }
                
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
            {
                _option = pick;
            }
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


        public override void SetAsControl()
        {
            _option = 0;
            _isControl = true;
            _targets = new List<Combatant>();

            int numAllies = 0;

            if (CanTargetAllies)
            {
                foreach (Ally a in BattleState.Allies)
                {
                    if (a != null)
                    {
                        _targets.Add(a);
                        numAllies++;
                    }
                }
            }

            if (CanTargetEnemies)
            {                
                foreach (Enemy e in BattleState.EnemyList)
                {
                    _targets.Add(e);
                }
            }

            if (CanTargetAllies && CanTargetEnemies)
            {
                switch (DefaultSelection)
                {
                    case BattleTargetGroup.Allies:
                        _option = 0;
                        break;
                    case BattleTargetGroup.Enemies:
                        _option = numAllies;
                        break;
                }
            }           
        }

        public override void SetNotControl()
        {
            _isControl = false;
        }
        
        protected override IEnumerable<Combatant> Selected
        {
            get
            {
                return new List<Combatant> { _targets[_option] };
            }
        }

        public override string Info { get { return _targets[_option].ToString(); } }
       

        public bool AllAble { get; private set; }
             
        private bool CanTargetAllies { get; set; }
        private bool CanTargetEnemies { get; set; }

        private BattleTargetGroup DefaultSelection { get; set; }
    }
}

