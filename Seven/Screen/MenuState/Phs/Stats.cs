using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;

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

        public Stats(Menu.ScreenState screenState)
            : base(
                2,
                screenState.Height * 7 / 60,
                screenState.Width / 2 - 6,
                screenState.Height * 5 / 6)
        { }
        
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

                    if (Seven.Party[0] == null && Seven.Party[1] == null && Seven.Party[2] == null)
                    {
                        Seven.Instance.ShowMessage(c => "No party!");
                    }
                    else
                    {
                        Seven.MenuState.ChangeScreen(Seven.MenuState.MainScreen);
                    }
                    break;
                case Key.Circle:
                    Seven.MenuState.PhsScreen.ChangeControl(Seven.MenuState.PhsList);
                    break;
            }
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                if (Seven.Party[i] != null)
                {
                    DrawCharacterStatus(d, gc, g, Seven.Party[i], y_firstRow + i * row_height);
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

            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private void DrawCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Cairo.Context g, Character c, int y)
        {            
            Images.RenderProfile(d, gc, X + x_pic, Y + y + y_displacement_pic, c);

            Graphics.Stats.RenderCharacterStatus(d, gc, g, c, X + x_status, Y + y, false);
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Seven.MenuState.PhsList.Update();
        }
        
        public int Option { get { return option; } }
        
        public override string Info
        {  get { return ""; } }
    }

}

