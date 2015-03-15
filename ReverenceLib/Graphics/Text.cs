using System;

namespace Atmosphere.Reverence.Graphics
{
    public static class Text
    {        
        public const int SHADOW_SPACING = 3;

        public static void ShadowedText(Cairo.Context g, string text, double x, double y)
        {
            ShadowedText(g, new Cairo.Color(1, 1, 1), text, x, y);
        }
        
        public static void ShadowedText(Cairo.Context g, Cairo.Color c, string text, double x, double y)
        {
            g.Save();
            
            g.MoveTo(x + SHADOW_SPACING, y + SHADOW_SPACING);
            g.Color = new Cairo.Color(0, 0, 0);
            g.ShowText(text);
            
            g.MoveTo(x, y);
            g.Color = c;
            g.ShowText(text);
            
            g.Restore();
        }
    }
}

