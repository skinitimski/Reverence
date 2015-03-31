using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven.Screen.MenuState
{
    internal sealed class MessageBox : Menu.Menu
    {
        public const int TIMEOUT = 1000;


        const int w_padding = 100;
        const int h_padding = 50;

        public MessageBox(TimedDialogue message)
            : this(message, TIMEOUT)
        { 
        }

        public MessageBox(TimedDialogue message, int timeout)
            : base(0, 0, 0, 0)
        { 
            Message = message;
            Timer = new Timer(timeout);
        }


        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            string message = Message(Timer);

            TextExtents te = g.TextExtents(message);

            Move(100, 100);

            Width = (int)te.Width + w_padding * 2;
            Height = (int)te.Height + h_padding * 2;

            int x = Config.Instance.WindowWidth / 2 - Width / 2;
            int y = Config.Instance.WindowHeight / 2 - Height / 2;

            Move(x, y);

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            double x_text = X + Width / 2 - te.Width / 2 - te.Width;
            double y_text = Y + Height / 2 - te.Height / 2;

            Text.ShadowedText(g, message, x_text, y_text);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public Timer Timer { get; private set; }

        private TimedDialogue Message { get; set; }
    }
}

