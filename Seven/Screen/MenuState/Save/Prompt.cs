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
        
        private int _option = 0;
        
        public Prompt()
            : base(
                Seven.Config.WindowWidth / 2 - width / 2,
                Seven.Config.WindowHeight / 2 - height / 2,
                width, 
                height)
        {
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (_option > 0) _option--;
                    break;
                case Key.Down:
                    if (_option < options - 1) _option++;
                    break;
                case Key.Circle:
                    Seven.MenuState.SaveScreen.ChangeControl(Seven.MenuState.SaveConfirm);
                    break;
                default: break;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            int y_cursor = y_option0 + y_spacing * _option + y_displacement_cursor;

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
            _option = 0;
        }
        
        
        public override string Info
        { get { return ""; } }
        
    }
}
