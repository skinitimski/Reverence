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

        public const int SLOT_RADIUS = 14;
        public const int MATERIA_RADIUS = 10;        
        public const int SLOT_SPACING = 37;

        private const int ys = 13;
        private const int zs = 5;
        
        public static void Render(Cairo.Context g, SlotHolder equipment, int x, int y)
        {
            Render(g, equipment, x, y, true);
        }

        public static void Render(Cairo.Context g, SlotHolder equipment, int x, int y, bool renderOrbs, bool renderBackground = true)
        {
            if (renderBackground)
            {
                g.Color = Colors.GRAY_1;
                g.Rectangle(x, y, 8 * SLOT_SPACING, SLOT_RADIUS * 2);
                g.Fill();
            }

            for (int j = 0; j < equipment.Links; j++)
            {
                Shapes.RenderLine(g, gray2, 3,
                                  x + (SLOT_SPACING / 2) + (j * 2 * SLOT_SPACING), y + ys - zs,
                                  x + (SLOT_SPACING / 2) + ((j * 2 + 1) * SLOT_SPACING), y + ys - zs);
                Shapes.RenderLine(g, gray2, 3,
                                  x + (SLOT_SPACING / 2) + (j * 2 * SLOT_SPACING), y + ys,
                                  x + (SLOT_SPACING / 2) + ((j * 2 + 1) * SLOT_SPACING), y + ys);
                Shapes.RenderLine(g, gray2, 3,
                                  x + (SLOT_SPACING / 2) + (j * 2 * SLOT_SPACING), y + ys + zs,
                                  x + (SLOT_SPACING / 2) + ((j * 2 + 1) * SLOT_SPACING), y + ys + zs);
            }

            for (int i = 0; i < equipment.Slots.Length; i++)
            {
                Shapes.RenderCircle(g, gray2, SLOT_RADIUS, x + (i * SLOT_SPACING) + (SLOT_SPACING / 2), y + ys);
                
                if (equipment.Slots[i] == null || !renderOrbs)
                {
                    Shapes.RenderCircle(g, gray1, MATERIA_RADIUS, x + (i * SLOT_SPACING) + (SLOT_SPACING / 2), y + ys);
                }
                else
                {
                    Shapes.RenderCircle(g, equipment.Slots[i].Color, MATERIA_RADIUS,
                                        x + (i * SLOT_SPACING) + (SLOT_SPACING / 2), y + ys);
                }
            }

        }

    }
}

