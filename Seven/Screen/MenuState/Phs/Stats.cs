using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Phs
{      
    internal sealed class Stats : ControlMenu
    {
        #region Layout
        
        const int x_pic = 30;

        const int x_status = x_pic + Character.PROFILE_WIDTH + 15; 
        const int x_cursor = x_pic - 15;
        
        const int y_firstRow = 60;

        const int row_height = 150;  

        const int y_displacement_cursor = 30;
        const int y_displacement_pic = -35;

        
        #endregion Layout
        
        private int option = 0;

        public Stats(SevenMenuState menuState, Menu.ScreenState screenState)
            : base(
                2,
                screenState.Height * 7 / 60,
                screenState.Width / 2 - 6,
                screenState.Height * 5 / 6)
        {
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
                    if (option < 2) option++;
                    break;
                case Key.X:

                    if (MenuState.Party[0] == null && MenuState.Party[1] == null && MenuState.Party[2] == null)
                    {
                        MenuState.Parent.ShowMessage(c => "No party!");
                    }
                    else
                    {
                        MenuState.ChangeScreen(MenuState.MainScreen);
                    }
                    break;
                case Key.Circle:
                    MenuState.PhsScreen.ChangeControl(MenuState.PhsList);
                    break;
            }
        }
        
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                if (MenuState.Party[i] != null)
                {
                    DrawCharacterStatus(d, g, MenuState.Party[i], y_firstRow + i * row_height);
                }
            }        

            if (IsControl)
            {
                Shapes.RenderCursor(g, X + x_cursor, Y + y_firstRow + y_displacement_cursor + option * row_height);
            }
            else
            {
                Shapes.RenderBlinkingCursor(g, X + x_cursor, Y + y_firstRow + y_displacement_cursor + option * row_height);
            }
        }

        private void DrawCharacterStatus(Gdk.Drawable d, Cairo.Context g, Character c, int y)
        {            
            Images.RenderProfile(d, X + x_pic, Y + y + y_displacement_pic, c);

            Graphics.Stats.RenderCharacterStatus(d, g, c, X + x_status, Y + y, false);
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            MenuState.PhsList.Update();
        }
        
        public int Option { get { return option; } }
        
        public override string Info
        {  get { return ""; } }

        private SevenMenuState MenuState { get; set; }
    }

}

