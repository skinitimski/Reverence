using System;

namespace Atmosphere.Reverence.Menu
{
    public class ScreenState
    {
        public ScreenState()
        {
        }

        public ScreenState(int w, int h, bool hasChanged)
        {
            Width = w;
            Height = h;
            HasChanged = hasChanged;
        }

        public int Width { get; set; }
        public int Height { get; set; }
        public bool HasChanged { get; set; }
    }
}

