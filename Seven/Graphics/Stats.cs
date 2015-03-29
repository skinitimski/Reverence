using System;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Graphics;

namespace Atmosphere.Reverence.Seven.Graphics
{
    internal static class Stats
    {        
        const int x3 = 0;        // name
        const int x4 = x3 + 65;  // lvl
        const int x5 = x4 + 60;  // hp
        const int x6 = x5 + 70;  // hpm
        const int x7 = x3 + 110; // fury/sadness
        const int ya = 30;      // subrow 1
        const int yb = ya + 25; // subrow 2
        const int yc = yb + 25; // subrow 3

        
        public static void RenderCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Cairo.Context g, Character c, int x, int y, bool showFurySadness = true)
        {   
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;

            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Normal);
            
            g.Color = Colors.TEXT_TEAL;
            g.MoveTo(x + x3, y + ya);
            g.ShowText("LV");
            g.MoveTo(x + x3, y + yb);
            g.ShowText("HP");
            g.MoveTo(x + x3, y + yc);
            g.ShowText("MP");
            g.Color = Colors.WHITE;

            if (showFurySadness)
            {
                if (c.Fury)
                {
                    Text.ShadowedText(g, Colors.TEXT_MAGENTA, "[Fury]", x + x7, y);
                }
                else if (c.Sadness)
                {
                    Text.ShadowedText(g, Colors.TEXT_MAGENTA, "[Sadness]", x + x7, y);
                }
            }
            
            Color namec = Colors.WHITE;

            if (c.Death)
            {
                namec = Colors.TEXT_RED;
            }
            else if (c.NearDeath)
            {
                namec = Colors.TEXT_YELLOW;
            }
            
            Text.ShadowedText(g, namec, c.Name, x + x3, y);

            
            lvl = c.Level.ToString();
            te = g.TextExtents(lvl);
            Text.ShadowedText(g, lvl, x + x4 - te.Width, y + ya);
            
            hp = c.HP.ToString() + "/";
            te = g.TextExtents(hp);
            Text.ShadowedText(g, hp, x + x5 - te.Width, y + yb);
            
            hpm = c.MaxHP.ToString();
            te = g.TextExtents(hpm);
            Text.ShadowedText(g, hpm, x + x6 - te.Width, y + yb);
            
            mp = c.MP.ToString() + "/";
            te = g.TextExtents(mp);
            Text.ShadowedText(g, mp, x + x5 - te.Width, y + yc);
            
            mpm = c.MaxMP.ToString();
            te = g.TextExtents(mpm);
            Text.ShadowedText(g, mpm, x + x6 - te.Width, y + yc);
        }
    }
}

