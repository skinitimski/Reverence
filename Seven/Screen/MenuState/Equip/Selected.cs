using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
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
                string growth = String.Empty;

                if (Seven.MenuState.EquipTop.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        case 0:
                            growth = Seven.Party.Selected.Weapon.Growth.ToString();
                            break;
                        case 1:
                            growth = Seven.Party.Selected.Armor.Growth.ToString();
                            break;
                    }
                }
                else if (Seven.MenuState.EquipList.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        case 0:
                            growth = ((Weapon)Seven.MenuState.EquipList.Selection).Growth.ToString();
                            break;
                        case 1:
                            growth = ((Armor)Seven.MenuState.EquipList.Selection).Growth.ToString();
                            break;
                    }
                }

                return growth;
            }
        }
        private MateriaBase[] Slots
        {
            get
            {
                if (Seven.MenuState.EquipTop.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        case 0: return Seven.Party.Selected.Weapon.Slots;
                        case 1: return Seven.Party.Selected.Armor.Slots;
                        default: return null;
                    }
                }
                else if (Seven.MenuState.EquipList.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        case 0: return ((Weapon)Seven.MenuState.EquipList.Selection).Slots;
                        case 1: return ((Armor)Seven.MenuState.EquipList.Selection).Slots;
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
                if (Seven.MenuState.EquipTop.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        case 0: return Seven.Party.Selected.Weapon.Links;
                        case 1: return Seven.Party.Selected.Armor.Links;
                        default: return 0;
                    }
                }
                else if (Seven.MenuState.EquipList.IsControl)
                {
                    switch (Seven.MenuState.EquipTop.Option)
                    {
                        case 0: return ((Weapon)Seven.MenuState.EquipList.Selection).Links;
                        case 1: return ((Armor)Seven.MenuState.EquipList.Selection).Links;
                        default: return 0;
                    }
                }
                else return 0;
            }
        }
        
        
    }
}

