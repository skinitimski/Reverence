using System;

namespace Atmosphere.Reverence.Seven.Graphics
{
    internal static class Images
    {
        public static void RenderProfile(Gdk.Drawable d, int x, int y, Character c)
        {
            Gdk.GC gc = new Gdk.GC(d);

            d.DrawPixbuf(gc, c.Profile, 0, 0, x, y, c.Profile.Width, c.Profile.Height, Gdk.RgbDither.None, 0, 0);
        }
        
        public static void RenderProfileSmall(Gdk.Drawable d, int x, int y, Character c)
        {
            Gdk.GC gc = new Gdk.GC(d);

            d.DrawPixbuf(gc, c.ProfileSmall, 0, 0, x, y, c.ProfileSmall.Width, c.ProfileSmall.Height, Gdk.RgbDither.None, 0, 0);
        }
        
        public static void RenderProfileTiny(Gdk.Drawable d, int x, int y, Character c)
        {
            Gdk.GC gc = new Gdk.GC(d);

            d.DrawPixbuf(gc, c.ProfileTiny, 0, 0, x, y, c.ProfileTiny.Width, c.ProfileTiny.Height, Gdk.RgbDither.None, 0, 0);
        }
    }
}

