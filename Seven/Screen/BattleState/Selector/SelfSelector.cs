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
        private Combatant _target;
        

        public SelfSelector(SevenBattleState battleState)
            : base(battleState)
        {
        }

        
        public override void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            if (IsControl)
            {
                Shapes.RenderCursor(g, _target.X - CURSOR_SPACING, _target.Y);
            }
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            _target = BattleState.Commanding;
        }

        protected override IEnumerable<Combatant> Selected
        {
            get
            {
                return new List<Combatant> { _target };
            }
        }

        public override string Info { get { return _target.ToString(); } }
    }
}

