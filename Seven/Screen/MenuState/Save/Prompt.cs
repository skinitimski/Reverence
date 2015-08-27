using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Save
{  
    internal sealed class Prompt : ControlMenu
    {
        #region Layout

        const int x_prompt = 50;
        const int x_options = 100;
        const int x_cursor = x_options - 30;

        const int y_prompt = 50;
        const int y_option0 = y_prompt + y_spacing;
        const int y_spacing = 50;

        const int y_displacement_cursor = -10;

        const int width = 325;
        const int height = 400;

        const int options = Seven.SAVE_FILES;
        
        #endregion Layout

        
        public Prompt(Menu.ScreenState screenState)
            : base(
                screenState.Width / 2 - width / 2,
                screenState.Height / 2 - height / 2,
                width, 
                height)
        {
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (Option > 0)
                    {
                        Option--;
                    }
                    else
                    {
                        Option = options - 1;
                    }
                    break;
                case Key.Down:
                    if (Option < options - 1)
                    {
                        Option++;
                    }
                    else
                    {
                        Option = 0;
                    }
                    break;
                case Key.Circle:
                    Seven.MenuState.SaveScreen.ChangeControl(Seven.MenuState.SaveConfirm);
                    break;
                case Key.X:
                    Seven.MenuState.ChangeScreen(Seven.MenuState.MainScreen);
                    break;
                default: break;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            int y_cursor = y_option0 + y_spacing * Option + y_displacement_cursor;

            Shapes.RenderCursor(g, X + x_cursor, Y + y_cursor);

            Text.ShadowedText(g, "Pick save file", X + x_prompt, Y + y_prompt);

            for (int option = 0; option < options; option++)
            {                
                Text.ShadowedText(g, "Save " + option, X + x_options, Y + y_option0 + y_spacing * option);
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Option = 0;
        }
        
        public int Option { get; private set; }
        
        public override string Info
        { get { return ""; } }
        
    }
}

