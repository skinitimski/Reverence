using System;
using Cairo;

namespace Atmosphere.Reverence.Graphics
{
    public static class Text
    {        
        public const string MONOSPACE_FONT = "Monospace";
        
        public const int SHADOW_SPACING = 3;

        public const int FONT_SIZE_LABEL = 24;


        public static void ShadowedText(Cairo.Context g, string text, double x, double y)
        {
            ShadowedText(g, Colors.WHITE, text, x, y);
        }
        
        public static void ShadowedText(Cairo.Context g, Cairo.Color c, string text, double x, double y)
        {
            g.Save();
            
            g.MoveTo(x + SHADOW_SPACING, y + SHADOW_SPACING);
            g.Color = Colors.BLACK;
            g.ShowText(text);
            
            g.MoveTo(x, y);
            g.Color = c;
            g.ShowText(text);
            
            g.Restore();
        }
    }
}

