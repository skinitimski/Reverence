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
                    Shapes.RenderCursor(g, a.X - 15, a.Y);
                foreach (Combatant e in Seven.BattleState.EnemyList)
                    Shapes.RenderCursor(g, e.X - 15, e.Y);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl() { _isControl = true; }
        public override void SetNotControl() { _isControl = false; }
        
        public override bool IsControl { get { return _isControl; } }
        
        public override Combatant[] Selected
        {
            get
            {
                int count = 0;
                foreach (Combatant a in Seven.BattleState.Allies)
                    count++;
                count += Seven.BattleState.EnemyList.Count;
                Combatant[] ret = new Combatant[count];
                int i = 0;
                foreach (Combatant a in Seven.BattleState.Allies)
                {
                    if (a != null) ret[i] = a;
                    i++;
                }
                foreach (Combatant e in Seven.BattleState.EnemyList)
                {
                    ret[i] = e;
                    i++;
                }
                return ret;
            }
        }
        public override string Info
        { get { return "All combatants"; } }
    }
}

