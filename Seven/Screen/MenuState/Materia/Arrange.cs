using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset.Materia;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Materia
{  
    internal sealed class Arrange : ControlMenu
    {
        #region Layout
        
        const int x = 35;
        const int y = 30;
        const int cx = 20;
        const int cy = 24;
        
        #endregion Layout
        
        private int option = 0;

        public Arrange(SevenMenuState menuState, ScreenState screenState)
            : base(
                screenState.Width * 3 / 8,
                150,
                170,
                140)
        {
            Visible = false;

            MenuState = menuState;
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    break;
                case Key.Down:
                    if (option < 3) option++;
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case 0:
                            MenuState.Party.Materiatory.Sort();
                            break;
                        case 1:
                            break;
                        case 2:
                            for (int i = 0; i < MenuState.Party.Selected.Weapon.Slots.Length; i++)
                            {
                                MateriaOrb orb = MenuState.Party.Selected.Weapon.Slots[i];
                                if (orb != null)
                                {
                                    MenuState.Party.Selected.Weapon.Slots[i] = null;
                                    orb.Detach(MenuState.Party.Selected);
                                    MenuState.Party.Materiatory.Put(orb);
                                }
                            }
                            for (int j = 0; j < MenuState.Party.Selected.Armor.Slots.Length; j++)
                            {
                                MateriaOrb orb = MenuState.Party.Selected.Armor.Slots[j];
                                if (orb != null)
                                {
                                    MenuState.Party.Selected.Armor.Slots[j] = null;
                                    orb.Detach(MenuState.Party.Selected);
                                    MenuState.Party.Materiatory.Put(orb);
                                }
                            }
                            break;
                        case 3:
                            MenuState.MateriaScreen.ChangeControl(MenuState.MateriaList);
                            MenuState.MateriaList.Trashing = true;
                            break;
                        default: break;
                    }
                    break;
                case Key.X:
                    Visible = false;
                    MenuState.MateriaScreen.ChangeToDefaultControl();
                    break;
                default: break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + (option * y));
            }
            
            Text.ShadowedText(g, "Arrange", X + x, Y + (1 * y));
            Text.ShadowedText(g, "Exchange", X + x, Y + (2 * y));
            Text.ShadowedText(g, "Clear", X + x, Y + (3 * y));
            Text.ShadowedText(g, "Trash", X + x, Y + (4 * y));
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
        }
        
        
        public override string Info
        { get { return ""; } }
        
        private SevenMenuState MenuState { get; set; }
    }

}

