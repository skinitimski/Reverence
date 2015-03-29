using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Main
{    
    internal sealed class Status : ControlMenu
    {
        #region Layout
        
        const int xp0 = 30; // pic
        const int xp1 = 60; // pic back row
        const int yp = 15; // ypic

        const int cx = 18;
        const int xs = xp1 + Character.PROFILE_WIDTH + 15; // status
        const int y0 = yp + 35;  // row 0
        const int y1 = y0 + 150; // row 1 
        const int y2 = y1 + 150; // row 2
        
        private int option = 0;
        private int option_hold = -1;
        private int cy = y0;
        private int cy_h = 0;
        
        #endregion

        public Status()
            : base(
                2,
                Config.Instance.WindowHeight / 9,
                Config.Instance.WindowWidth * 4 / 5,
                Config.Instance.WindowHeight * 7 / 9)
        {
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
                    Seven.Party.DecrementSelection();
                    break;
                case Key.Down: 
                    if (option < 2)
                    {
                        option++;
                    }
                    Seven.Party.IncrementSelection();
                    break;
                case Key.X:
                    Seven.MenuState.MainScreen.ChangeControl(Seven.MenuState.MainOptions);
                    break;
                case Key.Circle:
                    switch (Seven.MenuState.MainOptions.Selection)
                    {
                        case Options.Option.Magic: // Magic
                            if (Seven.Party[option] == null)
                            {
                                break;
                            }
                            //MenuState.MainMenu.ChangeScreen(Screen._magicScreen);
                            break;
                        case Options.Option.Materia: // Materia
                            if (Seven.Party[option] == null)
                            {
                                break;
                            }
                            Seven.MenuState.ChangeScreen(Seven.MenuState.MateriaScreen);
                            break;
                        case Options.Option.Equip: // Equip
                            if (Seven.Party[option] == null)
                            {
                                break;
                            }
                            Seven.MenuState.ChangeScreen(Seven.MenuState.EquipScreen);
                            break;
                        case Options.Option.Status: // Status
                            if (Seven.Party[option] == null)
                            {
                                break;
                            }
                            Seven.MenuState.ChangeScreen(Seven.MenuState.StatusScreen);
                            break;
                        case Options.Option.Order: // Order
                            if (option_hold != -1)
                            {
                                if (option_hold == option)
                                {
                                    Seven.Party[option].BackRow = !Seven.Party[option].BackRow;
                                    option_hold = -1;
                                }
                                else
                                {
                                    Character temp = Seven.Party[option];
                                    Seven.Party[option] = Seven.Party[option_hold];
                                    Seven.Party[option_hold] = temp;
                                    option_hold = -1;
                                }
                            }
                            else
                            {
                                option_hold = option;
                            }
                            break;
                        case Options.Option.Limit: // Limit
                            if (Seven.Party[option] == null)
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
            switch (option)
            {
                case 0:
                    cy = y0;
                    break;
                case 1:
                    cy = y1;
                    break;
                case 2:
                    cy = y2;
                    break;
                default:
                    break;
            }
            switch (option_hold)
            {
                case 0:
                    cy_h = y0;
                    break;
                case 1:
                    cy_h = y1;
                    break;
                case 2:
                    cy_h = y2;
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
                      

            if (Seven.Party[0] != null)
            {
                DrawCharacterStatus(d, gc, g, Seven.Party[0], y0);
            }
            
            if (Seven.Party[1] != null)
            {
                DrawCharacterStatus(d, gc, g, Seven.Party[1], y1);
            }
            
            if (Seven.Party[2] != null)
            {
                DrawCharacterStatus(d, gc, g, Seven.Party[2], y2);
            }

            
            if (IsControl)
            {
                if (option_hold != -1)
                {
                    Shapes.RenderBlinkingCursor(g, Colors.GRAY_8, X + cx, Y + cy_h);
                }

                if (option != option_hold)
                {
                    Shapes.RenderCursor(g, X + cx, Y + cy);
                }
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private void DrawCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Cairo.Context g, Character c, int y)
        {
            Images.RenderProfile(d, gc, X + (c.BackRow ? xp1 : xp0), Y + yp + y - y0, c);
            
            Stats.RenderCharacterStatus(d, gc, g, c, X + xs, Y + y);
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Seven.Party.SetSelection(option);
        }
        
        public override string Info
        { get { return ""; } }
    }

}

