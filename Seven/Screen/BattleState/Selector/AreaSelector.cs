using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;
using SevenBattleState = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal sealed class AreaSelector : Selector
    {
        public AreaSelector(SevenBattleState battleState)
            : base(battleState)
        {
        }

        
        public override void Draw(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            if (IsControl)
            {
                foreach (Combatant a in BattleState.Allies)
                {
                    Shapes.RenderCursor(g, a.X - CURSOR_SPACING, a.Y);
                }
                foreach (Combatant e in BattleState.EnemyList)
                {
                    Shapes.RenderCursor(g, e.X - CURSOR_SPACING, e.Y);
                }
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl() { _isControl = true; }
        public override void SetNotControl() { _isControl = false; }
        
        protected override IEnumerable<Combatant> Selected
        {
            get
            {
                List<Combatant> selected = new List<Combatant>();

                selected.AddRange(BattleState.Allies);
                selected.AddRange(BattleState.EnemyList);

                return selected;
            }
        }
        public override string Info
        { get { return "All combatants"; } }
    }
}

