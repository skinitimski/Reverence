using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.InitialState
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

        const int width = 325;
        const int height = 400;

        const int options = 4;
        
        #endregion Layout
        
        private int _option = 0;
        
        public Prompt()
            : base(
                Config.Instance.WindowWidth / 2 - width / 2,
                Config.Instance.WindowHeight / 2 - height / 2,
                width, 
                height)
        {
            Visible = false;
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
                    if (_option == 0)
                    {
                        Seven.Instance.LoadNewGame();
                    }
                    else
                    {
                        Seven.Instance.LoadSavedGame(_option - 1);
                    }
                    break;
                default: break;
            }
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            int y_cursor = y_option0 + y_spacing * _option;

            Shapes.RenderCursor(g, X + x_cursor, Y + y_cursor);

            Text.ShadowedText(g, "Which save data?", X + x_prompt, Y + y_prompt);

            Text.ShadowedText(g, "New game", X + x_options, Y + y_option0);

            for (int option = 1; option < options; option++)
            {                
                int save = option - 1;

                Text.ShadowedText(g, "Save " + save.ToString(), X + x_options, Y + y_option0 + y_spacing * option);
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            _option = 0;
            Visible = true;
        }
        
        public override void SetNotControl()
        {
            base.SetNotControl();
            Visible = false;
        }
        
        
        public override string Info
        { get { return ""; } }
        
    }
}

