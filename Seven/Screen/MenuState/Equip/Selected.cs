using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Selected : Menu.Menu
    {
        #region Layout
        
        const int x0 = 30;
        const int x1 = 150;
        
        const int ya = 40;
        const int yb = ya + 40;
        
        const int yh = 0; // first row (right)
        const int yi = yh + 40; // successive rows
        const int yj = yi + 37; //    |
        const int yk = yj + 40; //    | 
        const int yl = yk + 37; //    x
        
        const int xs = yl - yk;
        const int ysa = 7;
        const int ysb = 11;
        const int zs = 5;
        
        const int x2 = x1 + (8 * xs) / 2;
        
#endregion

        public Selected()
            : base(
                2,
                Config.Instance.WindowHeight * 5 / 12,
                Config.Instance.WindowWidth * 5 / 8,
                Config.Instance.WindowHeight / 6)
        {
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            if (Growth != "")
            {
                g.Color = new Color(.1, .1, .2);
                g.Rectangle(X + x1, Y + ya - (yj - yi) + ysa, 8 * xs, yj - yi);
                g.Fill();
                
                Cairo.Color gray1 = new Color(.2, .2, .2);
                Cairo.Color gray2 = new Color(.7, .7, .8);
                
                int links, slots;
                
                slots = Slots.Length;
                links = Links;
                
                
                for (int j = 0; j < links; j++)
                {
                    Shapes.RenderLine(g, gray2, 3,
                                        X + x1 + (xs / 2) + (j * 2 * xs), Y + ya - ysb - zs,
                                        X + x1 + (xs / 2) + ((j * 2 + 1) * xs), Y + ya - ysb - zs);
                    Shapes.RenderLine(g, gray2, 3,
                                        X + x1 + (xs / 2) + (j * 2 * xs), Y + ya - ysb,
                                        X + x1 + (xs / 2) + ((j * 2 + 1) * xs), Y + ya - ysb);
                    Shapes.RenderLine(g, gray2, 3,
                                        X + x1 + (xs / 2) + (j * 2 * xs), Y + ya - ysb + zs,
                                        X + x1 + (xs / 2) + ((j * 2 + 1) * xs), Y + ya - ysb + zs);
                }
                for (int i = 0; i < slots; i++)
                {
                    Shapes.RenderCircle(g, gray2, 14,
                                          X + x1 + (i * xs) + (xs / 2), Y + yi - ysb);
                    Shapes.RenderCircle(g, gray1, 10,
                                          X + x1 + (i * xs) + (xs / 2), Y + yi - ysb);
                }
                
                Text.ShadowedText(g, new Color(.3, .8, .8), "Slot", X + x0, Y + ya);
                Text.ShadowedText(g, new Color(.3, .8, .8), "Growth", X + x0, Y + yb);
                
                string growth = Growth;
                
                te = g.TextExtents(growth);
                Text.ShadowedText(g, growth, X + x2 - (te.Width / 2), Y + yb);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        private string Growth
        {
            get
            {
                if (EquipTop.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return Selected.Weapon.Growth.ToString();
                        case 1: return Selected.Armor.Growth.ToString();
                        default: return "";
                    }
                }
                else if (List.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return ((Weapon)List.Instance.Selection).Growth.ToString();
                        case 1: return ((Armor)List.Instance.Selection).Growth.ToString();
                        default: return "";
                    }
                }
                else return "";
            }
        }
        private MateriaBase[] Slots
        {
            get
            {
                if (EquipTop.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return Selected.Weapon.Slots;
                        case 1: return Selected.Armor.Slots;
                        default: return null;
                    }
                }
                else if (List.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return ((Weapon)List.Instance.Selection).Slots;
                        case 1: return ((Armor)List.Instance.Selection).Slots;
                        default: return null;
                    }
                }
                else return null;
            }
        }
        private int Links
        {
            get
            {
                if (EquipTop.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return Selected.Weapon.Links;
                        case 1: return Selected.Armor.Links;
                        default: return 0;
                    }
                }
                else if (List.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return ((Weapon)List.Instance.Selection).Links;
                        case 1: return ((Armor)List.Instance.Selection).Links;
                        default: return 0;
                    }
                }
                else return 0;
            }
        }
        
        
    }
}

