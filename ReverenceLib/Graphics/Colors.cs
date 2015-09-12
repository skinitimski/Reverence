using System;
using Cairo;

namespace Atmosphere.Reverence.Graphics
{
    public static class Colors
    {        
        public static readonly Color BLACK = new Color(0, 0, 0);
        public static readonly Color GRAY_1 = new Color(.1, .1, .1);
        public static readonly Color GRAY_4 = new Color(.4, .4, .4);
        public static readonly Color GRAY_8 = new Color(.8, .8, .8);
        public static readonly Color WHITE = new Color(1, 1, 1);
        
        public static readonly Color GREEN = new Color(.1, .8, .1);
        public static readonly Color YELLOW = new Color(.8, .8, 0);
        
        public static readonly Color TEXT_TEAL = new Color(.3, .8, .8);
        public static readonly Color TEXT_RED = new Color(0.8, 0, 0);
        public static readonly Color TEXT_YELLOW = new Color(.8, .8, 0);
        public static readonly Color TEXT_MAGENTA = new Color(.7, 0, .7);

        public static readonly Color ENEMY_RED = new Color(1.0, 0.6, 0.6);
    }
}

