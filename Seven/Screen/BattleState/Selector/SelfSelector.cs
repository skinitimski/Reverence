using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Battle;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal sealed class SelfSelector : Selector
    {
        private int _option;
        
        private List<Combatant> _targets;
        

        public SelfSelector(SevenBattleState battleState)
            : base(battleState)
        {
            _targets = new List<Combatant>();
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
            _targets.Add(BattleState.Commanding);
            
            _option = 0;
        }

        public override void SetNotControl() { _isControl = false; }

        protected override IEnumerable<Combatant> Selected
        {
            get
            {
                return new List<Combatant> { _targets[_option] };
            }
        }

        public override string Info
        { get { return _targets[_option].ToString(); } }
    }
}

