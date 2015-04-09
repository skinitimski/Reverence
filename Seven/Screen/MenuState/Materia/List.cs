using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Materia
{  
    internal sealed class List : ControlMenu
    {
        #region Layout
        
        const int x1 = 40; // orb
        const int x2 = 60; // name
        const int y = 29; // line spacing
        const int cx = 16;
        const int cy = 22;
        const int rows = 10;

        #endregion
        
        private int option = 0;
        private int topRow = 0;
        private bool trashing = false;
        
        public List()
            : base(
                Seven.Config.WindowWidth * 5 / 8,
                Seven.Config.WindowHeight * 5 / 12,
                Seven.Config.WindowWidth * 3 / 8 - 8,
                Seven.Config.WindowHeight * 8 / 15)
        { }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    if (topRow > option) topRow--;
                    break;
                case Key.Down:
                    if (option < Materiatory.MATERIATORY_SIZE - 1) option++;
                    if (topRow < option - rows + 1) topRow++;
                    break;
                case Key.X:
                    if (trashing)
                    {
                        Seven.MenuState.MateriaScreen.ChangeControl(Seven.MenuState.MateriaArrange);
                    }
                    else
                    {
                        Seven.MenuState.MateriaScreen.ChangeToDefaultControl();
                    }
                    trashing = false;
                    break;
                case Key.Circle:
                    if (Trashing)
                    {
                        Seven.MenuState.MateriaScreen.ChangeControl(Seven.MenuState.MateriaPrompt);
                        break;
                    }
                    MateriaOrb neworb = Seven.Party.Materiatory.Get(option);
                    MateriaOrb oldorb;
                    switch (Seven.MenuState.MateriaTop.OptionY)
                    {
                        case 0:
                            oldorb = Seven.Party.Selected.Weapon.Slots[Seven.MenuState.MateriaTop.OptionX];
                            if (oldorb != null)
                                oldorb.Detach(Seven.Party.Selected);
                            Seven.Party.Materiatory.Put(oldorb, option);
                            if (neworb != null)
                                neworb.Attach(Seven.Party.Selected);
                            Seven.Party.Selected.Weapon.AttachMateria(neworb, Seven.MenuState.MateriaTop.OptionX);
                            Seven.MenuState.MateriaScreen.ChangeToDefaultControl();
                            break;
                        case 1:
                            oldorb = Seven.Party.Selected.Armor.Slots[Seven.MenuState.MateriaTop.OptionX];
                            if (oldorb != null)
                                oldorb.Detach(Seven.Party.Selected);
                            Seven.Party.Materiatory.Put(oldorb, option);
                            if (neworb != null)
                                neworb.Attach(Seven.Party.Selected);
                            Seven.Party.Selected.Armor.AttachMateria(neworb, Seven.MenuState.MateriaTop.OptionX);
                            Seven.MenuState.MateriaScreen.ChangeToDefaultControl();
                            break;
                        default: break;
                    }
                    break;
                default:
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            
            if (IsControl || Seven.MenuState.MateriaArrange.IsControl || Seven.MenuState.MateriaPrompt.IsControl)
            {
                int j = Math.Min(rows + topRow, Materiatory.MATERIATORY_SIZE);
                
                for (int i = topRow; i < j; i++)
                {
                    MateriaOrb orb = Seven.Party.Materiatory.Get(i);
                    if (orb != null)
                    {
                        Shapes.RenderCircle(g, Colors.WHITE, 9, X + x1, Y + cy + (i - topRow) * y);
                        Shapes.RenderCircle(g, orb.Color, 7, X + x1, Y + cy + (i - topRow) * y);
                        
                        Text.ShadowedText(g, orb.Name, X + x2, Y + (i - topRow + 1) * y);
                    }
                }
            }
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + (option - topRow) * y);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override string Info
        {
            get
            {
                MateriaOrb o = Seven.Party.Materiatory.Get(option);
                return (o == null) ? "" : o.Description;
            }
        }
        public int Option { get { return option; } }
        public bool Trashing
        {
            get { return trashing; }
            set { trashing = value; }
        }
        public MateriaOrb Selection
        {
            get
            {
                if (IsControl || Seven.MenuState.MateriaPrompt.IsControl) return Seven.Party.Materiatory.Get(option);
                else return null;
            }
        }
        
    }

}

