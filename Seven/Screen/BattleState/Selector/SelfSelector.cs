using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
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
        
        public override void SetAsControl()
        {
            _isControl = true;
            _targets = new List<Combatant>();
            _targets.Add(Seven.BattleState.Commanding);
            
            _option = 0;
        }

        public override void SetNotControl() { _isControl = false; }
        
        public Combatant Selected { get { return _targets[_option]; } }
        public override string Info
        { get { return _targets[_option].ToString(); } }
    }
}

