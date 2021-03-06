using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using SevenInitialState = Atmosphere.Reverence.Seven.State.InitialState;

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

        const int y_displacement_cursor = -10;

        const int width = 325;
        const int height = 450;

        const int options = Seven.SAVE_FILES + 1;
        
        #endregion Layout

        
        public Prompt(SevenInitialState initialState, ScreenState screenState)
            : base(
                screenState.Width / 2 - width / 2,
                screenState.Height / 2 - height / 2,
                width, 
                height)
        {
            Visible = false;

            InitialState = initialState;
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
                    if (Option == 0)
                    {
                        InitialState.Seven.LoadNewGame();
                    }
                    else
                    {
                        if (InitialState.Seven.CanLoadSavedGame(Option - 1))
                        {
                            InitialState.Seven.LoadSavedGame(Option - 1);
                        }
                        else
                        {

                        }
                    }
                    break;
                default: break;
            }
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            int y_cursor = y_option0 + y_spacing * Option + y_displacement_cursor;

            Shapes.RenderCursor(g, X + x_cursor, Y + y_cursor);

            Text.ShadowedText(g, "Which save data?", X + x_prompt, Y + y_prompt);

            Text.ShadowedText(g, "New game", X + x_options, Y + y_option0);

            for (int option = 0; option < options - 1; option++)
            {                
                Text.ShadowedText(g, "Save " + option.ToString(), X + x_options, Y + y_option0 + y_spacing * (option + 1));
            }
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Option = 0;
            Visible = true;
        }
        
        public override void SetNotControl()
        {
            base.SetNotControl();
            Visible = false;
        }
                
        public int Option { get; private set; }        
        
        public override string Info
        { get { return ""; } }

        private SevenInitialState InitialState { get; set; }
    }
}

