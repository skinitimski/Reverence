using System;

namespace Atmosphere.Reverence.Graphics
{
    public struct ColoredPoint
    {
        public int X;
        public int Y;
        public float R;
        public float G;
        public float B;
        public float A;
        
        public ColoredPoint(int x, int y, float r, float g, float b, float a)
        {
            X = x;
            Y = y;
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}

