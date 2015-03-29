using System;

using Gdk;

namespace Atmosphere.Reverence.Graphics
{
    public static class Shapes
    {    
        private static bool _cursorBlinkOn;


        public static void RenderBlinkingCursor(Cairo.Context g, double x, double y)
        {
            RenderBlinkingCursor(g, Colors.WHITE, x, y);
        }
        public static void RenderBlinkingCursor(Cairo.Context g, Cairo.Color c, double x, double y)
        {
            _cursorBlinkOn = !_cursorBlinkOn;

            if (_cursorBlinkOn)
            {
                RenderCircle(g, c, 5, x, y);
            }
        }


        public static void RenderCursor(Cairo.Context g, double x, double y)
        {
            RenderCursor(g, Colors.WHITE, x, y);
        }
        public static void RenderCursor(Cairo.Context g, Cairo.Color c, double x, double y)
        {
            RenderCircle(g, c, 5, x, y);
        }



        public unsafe static void RenderTriangle(Pixbuf pb, ColoredPoint cp0, ColoredPoint cp1, ColoredPoint cp2)
        {
            int x0 = cp0.X;
            int y0 = cp0.Y;
            int x1 = cp1.X;
            int y1 = cp1.Y;
            int x2 = cp2.X;
            int y2 = cp2.Y;
            
            // Fij(x, y, xi, xj, yi, yj) = (yi - yj) * x + (xj - xi) * y + xi * yj - xj * yi
            
            float d_a = (y1 - y2) * x0 + (x1 - x2) * y0 + x1 * y2 - x2 * y1;
            float d_b = (y2 - y0) * x1 + (x0 - x2) * y1 + x2 * y0 - x0 * y2;
            float d_c = (y0 - y1) * x2 + (x1 - x0) * y2 + x0 * y1 - x1 * y0;
            
            float a_i = y1 - y2;
            float a_j = x2 - x1;
            float a_k = x1 * y2 - x2 * y1;
            float b_i = y2 - y0;
            float b_j = x0 - x2;
            float b_k = x2 * y0 - x0 * y2;
            float c_i = y0 - y1;
            float c_j = x1 - x0;
            float c_k = x0 * y1 - x1 * y0;
            
            int ymin = Math.Min(Math.Min(y0, y1), y2);
            int ymax = Math.Max(Math.Max(y0, y1), y2);
            int xmin = Math.Min(Math.Min(x0, x1), x2);
            int xmax = Math.Max(Math.Max(x0, x1), x2);
            
            byte* bptr = (byte*)pb.Pixels.ToPointer();
            byte* p;
            
            float alpha, beta, gamma;
            
            for (int y = ymin; y < ymax; y++)
                for (int x = xmin; x < xmax; x++)
            {
                alpha = (a_i * x + a_j * y + a_k) / d_a;
                beta = (b_i * x + b_j * y + b_k) / d_b;
                gamma = (c_i * x + c_j * y + c_k) / d_c;
                
                if (alpha >= 0 && beta >= 0 && gamma >= 0)
                {
                    p = bptr + y * pb.Rowstride + x * pb.NChannels;
                    
                    p[0] = (byte)((cp0.R * alpha + cp1.R * beta + cp2.R * gamma) * 255); // r
                    p[1] = (byte)((cp0.G * alpha + cp1.G * beta + cp2.G * gamma) * 255); // g
                    p[2] = (byte)((cp0.B * alpha + cp1.B * beta + cp2.B * gamma) * 255); // b
                    p[3] = (byte)((cp0.A * alpha + cp1.A * beta + cp2.A * gamma) * 255); // a
                }
            }
        }

        public static void RenderInvertedTriangle(Cairo.Context g, Cairo.Color c, double x, double y, int side)
        {            
            g.Save();
            
            g.Color = c;
            g.MoveTo(x, y);
            g.LineTo(x - side / 2, y - side);
            g.LineTo(x + side / 2, y - side);
            g.LineTo(x, y);
            g.Fill();
            
            g.Restore();
        }

        
        
        public static void RenderLine(Cairo.Context g, Cairo.Color c, double lw, double x0, double y0, double x1, double y1)
        {
            g.Save();
            g.Color = c;
            g.LineWidth = 3;
            g.MoveTo(x0, y0);
            g.LineTo(x1, y1);
            g.Stroke();
            g.Restore();
        }
        public static void RenderCircle(Cairo.Context g, Cairo.Color c, double r, double x, double y)
        {
            g.Save();
            g.Color = c;
            g.MoveTo(x, y);
            g.Arc(x, y, r, 0, 6.28);
            g.LineWidth = 3;
            g.ClosePath();
            g.Fill();
            g.Restore();
        }
    }
}

