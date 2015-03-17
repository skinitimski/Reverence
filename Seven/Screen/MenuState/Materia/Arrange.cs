using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset.Materia;

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

        public Arrange()
            : base(
                Config.Instance.WindowWidth * 3 / 8,
                150,
                170,
                140)
        {
            Visible = false;
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
                            Seven.Party.Materiatory.Sort();
                            break;
                        case 1:
                            break;
                        case 2:
                            for (int i = 0; i < Seven.Party.Selected.Weapon.Slots.Length; i++)
                            {
                                MateriaBase orb = Seven.Party.Selected.Weapon.Slots[i];
                                if (orb != null)
                                {
                                    Seven.Party.Selected.Weapon.Slots[i] = null;
                                    //orb.Detach(Selected);
                                    Seven.Party.Materiatory.Put(orb);
                                }
                            }
                            for (int j = 0; j < Seven.Party.Selected.Armor.Slots.Length; j++)
                            {
                                MateriaBase orb = Seven.Party.Selected.Armor.Slots[j];
                                if (orb != null)
                                {
                                    Seven.Party.Selected.Armor.Slots[j] = null;
                                    //orb.Detach(Selected);
                                    Seven.Party.Materiatory.Put(orb);
                                }
                            }
                            break;
                        case 3:
                            Seven.MenuState.MateriaScreen.ChangeControl(Seven.MenuState.MateriaList);
                            Seven.MenuState.MateriaList.Trashing = true;
                            break;
                        default: break;
                    }
                    break;
                case Key.X:
                    Visible = false;
                    Seven.MenuState.MateriaScreen.ChangeToDefaultControl();
                    break;
                default: break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + (option * y));
            }
            
            Text.ShadowedText(g, "Arrange", X + x, Y + (1 * y));
            Text.ShadowedText(g, "Exchange", X + x, Y + (2 * y));
            Text.ShadowedText(g, "Clear", X + x, Y + (3 * y));
            Text.ShadowedText(g, "Trash", X + x, Y + (4 * y));
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Visible = true;
        }
        
        
        public override string Info
        { get { return ""; } }
        
    }

}
