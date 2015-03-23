using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Screen.BattleState
{
    internal class DamageIcon
    {
        private const int TIMEOUT_MS = 1500;
        private const int Q1 = TIMEOUT_MS / 4;
        private const int Q2 = Q1 * 2;
        private const int Q3 = Q1 * 3;

        private const int MAX = 5;
        private const int MIN = 3;

        private const int STEPS_ABOVE = Q1 / MAX;
        private const int STEPS_BELOW = Q1 / MIN;


        private DamageIcon()
        {
            Color = Colors.WHITE;
            AnimationTimer = new Timer(TIMEOUT_MS);
        }

        public DamageIcon(int amount, Combatant receiver, bool mp = false)
           : this()
        {
            Message = amount.ToString();

            if (mp)
            {
                Message += " MP";
            }

            if (amount < 0)
            {
                Message = Message.Substring(1); // drop minus sign
                Color = Colors.GREEN;
            }

            X = receiver.X;
            Y = receiver.Y;
        }

        public void Draw(Gdk.Drawable d)
        {
            Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(20);

            TextExtents te = g.TextExtents(Message);
            
            Text.ShadowedText(g, Color, Message, X - (te.Width / 2), Y - (te.Height / 2) + GetCurrentDisplacement());
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

        private Color Color { get; set; }

        private string Message { get; set; }

        private Timer AnimationTimer { get; set; }

        private int X { get; set; }
        private int Y { get; set; }

        public bool IsDone { get { return AnimationTimer.IsUp; } }
    }
}

