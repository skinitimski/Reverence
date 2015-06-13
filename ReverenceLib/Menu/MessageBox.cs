using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Menu
{
    internal sealed class MessageBox : Menu
    {
        public const int TIMEOUT = 1000;


        const int w_padding = 100;
        const int h_padding = 50;

        public MessageBox(ScreenState screenState, TimedDialogue message)
            : this(screenState, message, TIMEOUT)
        { 
        }

        public MessageBox(ScreenState screenState, TimedDialogue message, int timeout)
            : base(0, 0, 0, 0)
        { 
            ScreenState = screenState;
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

            int x = ScreenState.Width / 2 - Width / 2;
            int y = ScreenState.Height / 2 - Height / 2;

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

        /// <remarks>
        /// this could potentially be updated in the future
        /// </remarks>            
        private ScreenState ScreenState { get; set; }
    }
}

