using System;

namespace Atmosphere.Reverence.Menu
{
    public class ScreenState
    {
        public ScreenState()
        {
        }

        public ScreenState(int w, int h)
        {
            Width = w;
            Height = h;
        }

        public int Width { get; set; }
        public int Height { get; set; }
    }
}

