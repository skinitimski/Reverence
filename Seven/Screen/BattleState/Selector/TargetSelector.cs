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
    internal sealed class TargetSelector : Selector
    {
        private int _option;
        
        private List<Combatant> _targets;
        

        public TargetSelector()
        {
            _targets = new List<Combatant>();
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.R1:
                    if (AllAble)
                        if (Seven.BattleState.Commanding.MagicMenu.Selected.AllCount > 0)
                    {
                        Seven.BattleState.Screen.PopControl();
                        Selector.Type = TargetType.AllTar;
                        Seven.BattleState.Screen.PushControl(GroupSelector.Instance);
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
                    Seven.BattleState.Commanding.Ability.Target = Selected;
                    User.ActOnSelection();
                    if (RunActionHook)
                        Seven.BattleState.ActionHook();
                    else if (RunClearControl)
                        Seven.BattleState.ClearControl();
                    else
                        Seven.BattleState.Screen.PopControl();
                    break;
                case Key.X:
                    Seven.BattleState.ActionAbort();
                    break;
                default:
                    break;
            }
        }
        
        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            if (IsControl)
                Shapes.RenderCursor(g,
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
            int min = Config.Instance.WindowHeight * Config.Instance.WindowHeight + Config.Instance.WindowWidth * Config.Instance.WindowWidth; 
            
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
            _targets = new List<Combatant>();
            
            switch (Group)
            {
                case TargetGroup.Allies:
                    foreach (Ally a in Seven.BattleState.Allies)
                        if (a != null) _targets.Add(a);
                    break;
                case TargetGroup.Enemies:
                    foreach (Enemy e in Seven.BattleState.EnemyList)
                        _targets.Add(e);
                    break;
                case TargetGroup.Area:
                    foreach (Ally a in Seven.BattleState.Allies)
                        if (a != null) _targets.Add(a);
                    foreach (Enemy e in Seven.BattleState.EnemyList)
                        _targets.Add(e);
                    break;
            }
            
            _option = 0;
        }
        public override void SetNotControl() { _isControl = false; }
        
        public override bool IsControl { get { return _isControl; } }
        
        public override Combatant[] Selected
        {
            get
            {
                Combatant[] ret = new Combatant[1];
                ret[0] = _targets[_option];
                return ret;
            }
        }
        public override string Info
        { get { return _targets[_option].ToString(); } }
    }
}

