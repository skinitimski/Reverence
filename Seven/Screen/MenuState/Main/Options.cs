using System;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Main
{        
    internal sealed class Options : ControlMenu
    {
        public enum Option
        {
            Item = 0,
            Magic = 1,
            Materia = 2,
            Equip = 3,
            Status = 4,
            Order = 5,
            Limit = 6,
            Config = 7,
            PHS = 8,
            Save = 9
        }

        #region Layout
        
        const int x = 35;
        const int y = 30;
        const int cx = 20;
        const int cy = 28;
        
        #endregion Layout
        
        private Option option = 0;

        public Options(SevenMenuState menuState, ScreenState screenState)
            : base(
                screenState.Width * 3 / 4 - 10,
                screenState.Height / 20,
                screenState.Width / 4,
                screenState.Height * 11 / 20)
        { 
            MenuState = menuState;
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if ((int)option > 0) option--;
                    else option = Option.Save;
                    break;
                case Key.Down:
                    if ((int)option < 9) option++;
                    else option = 0;
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case Option.Item: // Item
                            MenuState.ChangeScreen(MenuState.ItemScreen);
                            break;
                            //case Option.Magic: // Magic
                        case Option.Materia: // Materia
                        case Option.Equip: // Equip
                        case Option.Status: // Status
                        case Option.Order: // Order
                            //case Option.Limit: // Limit
                            MenuState.MainScreen.ChangeControl(MenuState.MainStatus);
                            break;
                        case Option.Config: // Config
                            MenuState.ChangeScreen(MenuState.ConfigScreen);
                            break;
                        case Option.PHS: // PHS
                            MenuState.ChangeScreen(MenuState.PhsScreen);
                            break;
                        case Option.Save: // Save
                            MenuState.ChangeScreen(MenuState.SaveScreen);
                            break;
                        default:
                            break;
                    }
                    break;
                case Key.Square:
                    MenuState.Seven.BeginBattle();
                    break;
                default:
                    break;
            }
        }
        
        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
                        
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + ((int)option * y));
            }

            foreach (Option option in Enum.GetValues(typeof(Option)))
            {
                DrawOption(g, option);
            }
        }

        private void DrawOption(Cairo.Context g, Option o)
        {
            Text.ShadowedText(g, o.ToString(),
                              X + x,
                              Y + (((int)o + 1) * y) + 5);
        }
        
        public Option Selection { get { return option; } }

        public override string Info { get { return String.Empty; } }

        private SevenMenuState MenuState { get; set; }
    }

}

