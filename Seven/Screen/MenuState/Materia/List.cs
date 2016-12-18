using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset.Materia;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

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
        
        public List(SevenMenuState menuState, ScreenState screenState)
            : base(
                screenState.Width * 5 / 8,
                screenState.Height * 5 / 12,
                screenState.Width * 3 / 8 - 8,
                screenState.Height * 8 / 15)
        { 
            MenuState = menuState;
        }
        
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
                        MenuState.MateriaScreen.ChangeControl(MenuState.MateriaArrange);
                    }
                    else
                    {
                        MenuState.MateriaScreen.ChangeToDefaultControl();
                    }
                    trashing = false;
                    break;
                case Key.Circle:
                    if (Trashing)
                    {
                        MenuState.MateriaScreen.ChangeControl(MenuState.MateriaPrompt);
                        break;
                    }
                    MateriaOrb neworb = MenuState.Party.Materiatory.Get(option);
                    MateriaOrb oldorb;
                    switch (MenuState.MateriaTop.OptionY)
                    {
                        case 0:
                            oldorb = MenuState.Party.Selected.Weapon.Slots[MenuState.MateriaTop.OptionX];
                            if (oldorb != null)
                                oldorb.Detach(MenuState.Party.Selected);
                            MenuState.Party.Materiatory.Put(oldorb, option);
                            if (neworb != null)
                                neworb.Attach(MenuState.Party.Selected);
                            MenuState.Party.Selected.Weapon.AttachMateria(neworb, MenuState.MateriaTop.OptionX);
                            MenuState.MateriaScreen.ChangeToDefaultControl();
                            break;
                        case 1:
                            oldorb = MenuState.Party.Selected.Armor.Slots[MenuState.MateriaTop.OptionX];
                            if (oldorb != null)
                                oldorb.Detach(MenuState.Party.Selected);
                            MenuState.Party.Materiatory.Put(oldorb, option);
                            if (neworb != null)
                                neworb.Attach(MenuState.Party.Selected);
                            MenuState.Party.Selected.Armor.AttachMateria(neworb, MenuState.MateriaTop.OptionX);
                            MenuState.MateriaScreen.ChangeToDefaultControl();
                            break;
                        default: break;
                    }
                    break;
                default:
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            
            if (IsControl || MenuState.MateriaArrange.IsControl || MenuState.MateriaPrompt.IsControl)
            {
                int j = Math.Min(rows + topRow, Materiatory.MATERIATORY_SIZE);
                
                for (int i = topRow; i < j; i++)
                {
                    MateriaOrb orb = MenuState.Party.Materiatory.Get(i);
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
        }
        
        public override string Info
        {
            get
            {
                MateriaOrb o = MenuState.Party.Materiatory.Get(option);
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
                if (IsControl || MenuState.MateriaPrompt.IsControl) return MenuState.Party.Materiatory.Get(option);
                else return null;
            }
        }
        
        private SevenMenuState MenuState { get; set; }
        
    }

}

