using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal sealed class SelfSelector : Selector
    {
        private int _option;
        
        private List<Combatant> _targets;
        

        public SelfSelector()
        {
            _targets = new List<Combatant>();
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
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
        
        public override void SetAsControl()
        {
            _isControl = true;
            _targets = new List<Combatant>();
            _targets.Add(Seven.BattleState.Commanding);
            
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

