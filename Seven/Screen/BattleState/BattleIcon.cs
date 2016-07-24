using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;
using StateOfBattle = Atmosphere.Reverence.Seven.State.BattleState;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal abstract class BattleIcon
    {
        public const int ANIMATION_DURATION = 1500;
        private const int Q1 = ANIMATION_DURATION / 4;
        private const int Q2 = Q1 * 2;
        private const int Q3 = Q1 * 3;
        
        private const int MAX = 5;
        private const int MIN = 3;
        
        private const int STEPS_ABOVE = Q1 / MAX;
        private const int STEPS_BELOW = Q1 / MIN;


        protected BattleIcon(StateOfBattle battle, Combatant receiver)
        {            
            X = receiver.X;
            Y = receiver.Y;

            Color = Colors.WHITE;
            AnimationTimer = new Timer(ANIMATION_DURATION);
        }

        
        public void Draw(Gdk.Drawable d)
        {
            Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(20);
            
            TextExtents te = g.TextExtents(Message);
            
            Text.ShadowedText(g, Color, Message, X - (te.Width / 2), Y - (te.Height / 2) + GetCurrentDisplacement());
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        private int GetCurrentDisplacement()
        {
            int d = 0;
            
            int t = (int)AnimationTimer.TotalMilliseconds;
            
            if (t < Q1)
            {
                d = -(t / STEPS_ABOVE);
            }
            else if (t < Q2)
            {
                d = -MAX + ((t - Q1) / STEPS_ABOVE);
            }
            else if (t < Q3)
            {
                d = (t - Q2) / STEPS_BELOW;
            }
            else
            {
                d = MIN - ((t - Q3) / STEPS_ABOVE);
            }
                        
            return d;
        }

        protected Color Color { get; set; }
        
        protected string Message { get; set; }
        
        public Timer AnimationTimer { get; private set; }
        
        private int X { get; set; }
        private int Y { get; set; }
        
        public bool IsDone { get { return AnimationTimer.IsUp; } }
    }
}

