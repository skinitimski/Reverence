using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Status
{      
    internal sealed class Label : ControlMenu
    {
        public Label(SevenMenuState menuState, Menu.ScreenState screenState)
            : base(
                screenState.Width * 3 / 4,
                screenState.Height / 20,
                screenState.Width / 4 - 10,
                screenState.Height / 15)
        {
            MenuState = menuState;
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:
                    if (MenuState.StatusOne.Visible)
                    {
                        MenuState.StatusOne.Visible = false;
                        MenuState.StatusTwo.Visible = true;
                    }
                    else if (MenuState.StatusTwo.Visible)
                    {
                        MenuState.StatusTwo.Visible = false;
                        MenuState.StatusThree.Visible = true;
                    }
                    else
                    {
                        MenuState.StatusThree.Visible = false;
                        MenuState.StatusOne.Visible = true;
                    }
                    break;
                case Key.X:
                    MenuState.ChangeScreen(MenuState.MainScreen);
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
            
            Text.ShadowedText(g, "Status", X + 20, Y + 25);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            
            MenuState.StatusOne.Visible = true;
            MenuState.StatusTwo.Visible = false;
            MenuState.StatusThree.Visible = false;
        }
        
        public override string Info { get { return ""; } }

        private SevenMenuState MenuState { get; set; }
    }
}

