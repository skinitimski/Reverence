using System;
using Cairo;

using Atmosphere.Reverence.Graphics;

using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven.Graphics
{
    internal static class MateriaSlots
    {        
        private static readonly Color gray1 = new Color(.2, .2, .2);
        private static readonly Color gray2 = new Color(.7, .7, .8);

        private const int xs = 37;
        private const int ys = 13;
        const int zs = 5;
        
        public static void Render(Cairo.Context g, SlotHolder equipment, int x, int y)
        {
            Render(g, equipment, x, y, true);
        }

        public static void Render(Cairo.Context g, SlotHolder equipment, int x, int y, bool renderOrbs)
        {
            for (int j = 0; j < equipment.Links; j++)
            {
                Shapes.RenderLine(g, gray2, 3,
                                  x + (xs / 2) + (j * 2 * xs), y - ys - zs,
                                  x + (xs / 2) + ((j * 2 + 1) * xs), y - ys - zs);
                Shapes.RenderLine(g, gray2, 3,
                                  x + (xs / 2) + (j * 2 * xs), y - ys,
                                  x + (xs / 2) + ((j * 2 + 1) * xs), y - ys);
                Shapes.RenderLine(g, gray2, 3,
                                  x + (xs / 2) + (j * 2 * xs), y - ys + zs,
                                  x + (xs / 2) + ((j * 2 + 1) * xs), y - ys + zs);
            }

            for (int i = 0; i < equipment.Slots.Length; i++)
            {
                Shapes.RenderCircle(g, gray2, 14, x + (i * xs) + (xs / 2), y - ys);
                
                if (equipment.Slots [i] == null || !renderOrbs)
                {
                    Shapes.RenderCircle(g, gray1, 10, x + (i * xs) + (xs / 2), y - ys);
                }
                else
                {
                    Shapes.RenderCircle(g, equipment.Slots[i].Color, 10,
                                        x + (i * xs) + (xs / 2), y - ys);
                }
            }
        }

    }
}

