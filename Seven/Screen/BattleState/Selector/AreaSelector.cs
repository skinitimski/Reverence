using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState.Selector
{
    internal sealed class AreaSelector : Selector
    {
        private List<Combatant> _targets;
        
        public AreaSelector()
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
                        Seven.BattleState.ActionHook();
                    else if (Seven.BattleState.Screen.RunClearControl)
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
            {
                foreach (Combatant a in Seven.BattleState.Allies)
                {
                    Shapes.RenderCursor(g, a.X - CURSOR_SPACING, a.Y);
                }
                foreach (Combatant e in Seven.BattleState.EnemyList)
                {
                    Shapes.RenderCursor(g, e.X - CURSOR_SPACING, e.Y);
                }
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl() { _isControl = true; }
        public override void SetNotControl() { _isControl = false; }
        
        public List<Combatant> Selected
        {
            get
            {
                List<Combatant> selected = new List<Combatant>();

                selected.AddRange(Seven.BattleState.Allies);
                selected.AddRange(Seven.BattleState.EnemyList);

                return selected;
            }
        }
        public override string Info
        { get { return "All combatants"; } }
    }
}

