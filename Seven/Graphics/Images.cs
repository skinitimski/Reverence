using System;

namespace Atmosphere.Reverence.Seven.Graphics
{
    internal static class Images
    {
        public static void RenderProfile(Gdk.Drawable d, Gdk.GC gc, int x, int y, Character c)
        {
            d.DrawPixbuf(gc, c.Profile, 0, 0, x, y, Character.PROFILE_WIDTH, Character.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);
        }

        public static void RenderProfileSmall(Gdk.Drawable d, Gdk.GC gc, int x, int y, Character c)
        {
            d.DrawPixbuf(gc, c.ProfileSmall, 0, 0, x, y, Character.PROFILE_WIDTH_SMALL, Character.PROFILE_HEIGHT_SMALL, Gdk.RgbDither.None, 0, 0);
        }
    }
}

