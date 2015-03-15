using System;
using Cairo;

namespace Atmosphere.Reverence.Graphics
{
    public static class Text
    {        
        private static readonly Color BLACK = new Color(0, 0, 0);
        private static readonly Color WHITE = new Color(1, 1, 1);


        public const int SHADOW_SPACING = 3;


        public static void ShadowedText(Cairo.Context g, string text, double x, double y)
        {
            ShadowedText(g, WHITE, text, x, y);
        }
        
        public static void ShadowedText(Cairo.Context g, Cairo.Color c, string text, double x, double y)
        {
            g.Save();
            
            g.MoveTo(x + SHADOW_SPACING, y + SHADOW_SPACING);
            g.Color = BLACK;
            g.ShowText(text);
            
            g.MoveTo(x, y);
            g.Color = c;
            g.ShowText(text);
            
            g.Restore();
        }
    }
}

