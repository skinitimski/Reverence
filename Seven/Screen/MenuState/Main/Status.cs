using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Main
{    
    internal sealed class Status : ControlMenu
    {
        #region Layout

        const int x_pic = 30; // pic
        const int x_pic_backrow = x_pic + 30; // pic back row
                
        const int x_status = x_pic_backrow + Character.PROFILE_WIDTH + 15; 
        const int x_cursor = x_pic - 15;
        
        const int y_firstRow = 50;
        
        const int row_height = 150;  
        
        const int y_displacement_cursor = 30;
        const int y_displacement_pic = -35;
        
        #endregion




        
        private int option = 0;
        private int option_hold = -1;


        public Status(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height / 9,
                screenState.Width * 4 / 5,
                screenState.Height * 7 / 9)
        {
            MenuState = menuState;
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up: 
                    if (option > 0)
                    {
                        option--;
                    } 
                    MenuState.Party.DecrementSelection();
                    break;
                case Key.Down: 
                    if (option < 2)
                    {
                        option++;
                    }
                    MenuState.Party.IncrementSelection();
                    break;
                case Key.X:
                    MenuState.MainScreen.ChangeControl(MenuState.MainOptions);
                    break;
                case Key.Circle:
                    switch (MenuState.MainOptions.Selection)
                    {
                        case Options.Option.Magic: // Magic
                            if (MenuState.Party[option] == null)
                            {
                                break;
                            }
                            //MenuState.MainMenu.ChangeScreen(Screen._magicScreen);
                            break;
                        case Options.Option.Materia: // Materia
                            if (MenuState.Party[option] == null)
                            {
                                break;
                            }
                            MenuState.ChangeScreen(MenuState.MateriaScreen);
                            break;
                        case Options.Option.Equip: // Equip
                            if (MenuState.Party[option] == null)
                            {
                                break;
                            }
                            MenuState.ChangeScreen(MenuState.EquipScreen);
                            break;
                        case Options.Option.Status: // Status
                            if (MenuState.Party[option] == null)
                            {
                                break;
                            }
                            MenuState.ChangeScreen(MenuState.StatusScreen);
                            break;
                        case Options.Option.Order: // Order
                            if (option_hold != -1)
                            {
                                if (option_hold == option)
                                {
                                    MenuState.Party[option].BackRow = !MenuState.Party[option].BackRow;
                                    option_hold = -1;
                                }
                                else
                                {
                                    Character temp = MenuState.Party[option];
                                    MenuState.Party[option] = MenuState.Party[option_hold];
                                    MenuState.Party[option_hold] = temp;
                                    option_hold = -1;
                                }
                            }
                            else
                            {
                                option_hold = option;
                            }
                            break;
                        case Options.Option.Limit: // Limit
                            if (MenuState.Party[option] == null)
                            {
                                break;
                            }
                            //MenuState.MainMenu.ChangeLayer(Layer._limitLayer);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
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
                if (MenuState.Party[i] != null)
                {
                    DrawCharacterStatus(d, gc, g, MenuState.Party[i], y_firstRow + i * row_height);
                }
            } 

            
            if (IsControl)
            {
                if (option_hold != -1)
                {
                    Shapes.RenderBlinkingCursor(g, X + x_cursor, Y + y_firstRow + y_displacement_cursor + option * row_height);
                }

                if (option != option_hold)
                {
                    Shapes.RenderCursor(g, X + x_cursor, Y + y_firstRow + y_displacement_cursor + option * row_height);
                }
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private void DrawCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Cairo.Context g, Character c, int y)
        {
            Images.RenderProfile(d, gc, X + (c.BackRow ? x_pic_backrow : x_pic), Y + y + y_displacement_pic, c);
            
            Stats.RenderCharacterStatus(d, gc, g, c, X + x_status, Y + y);
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            MenuState.Party.SetSelection(option);
        }
        
        public override string Info
        { get { return ""; } }
        
        private SevenMenuState MenuState { get; set; }
    }

}

