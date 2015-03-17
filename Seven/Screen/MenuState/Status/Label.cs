using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Status
{      
    internal sealed class Label : ControlMenu
    {
        public static Label Instance;
        
        static Label()
        {
            Instance = new Label();
        }
        public Label()
            : base(
                Config.Instance.WindowWidth * 3 / 4,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth / 4 - 10,
                Config.Instance.WindowHeight / 15)
        { }
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:
                    if (Seven.MenuState.StatusOne.Visible)
                    {
                        Seven.MenuState.StatusOne.Visible = false;
                        Seven.MenuState.StatusTwo.Visible = true;
                    }
                    else if (Seven.MenuState.StatusTwo.Visible)
                    {
                        Seven.MenuState.StatusTwo.Visible = false;
                        Seven.MenuState.StatusThree.Visible = true;
                    }
                    else
                    {
                        Seven.MenuState.StatusThree.Visible = false;
                        Seven.MenuState.StatusOne.Visible = true;
                    }
                    break;
                case Key.X:
                    Seven.MenuState.ChangeScreen(Seven.MenuState.MainScreen);
                    break;
                default:
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, "Status", X + 20, Y + 25);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        public override void SetAsControl()
        {
            base.SetAsControl();
            
            Seven.MenuState.StatusOne.Visible = true;
            Seven.MenuState.StatusTwo.Visible = false;
            Seven.MenuState.StatusThree.Visible = false;
        }
        
        public override string Info
        { get { return ""; } }
    }
}
