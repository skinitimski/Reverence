using System;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

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

        public Options(Menu.ScreenState screenState)
            : base(
                screenState.Width * 3 / 4 - 10,
                screenState.Height / 20,
                screenState.Width / 4,
                screenState.Height * 11 / 20)
        { }
        
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
                            Seven.MenuState.ChangeScreen(Seven.MenuState.ItemScreen);
                            break;
                            //case Option.Magic: // Magic
                        case Option.Materia: // Materia
                        case Option.Equip: // Equip
                        case Option.Status: // Status
                        case Option.Order: // Order
                            //case Option.Limit: // Limit
                            Seven.MenuState.MainScreen.ChangeControl(Seven.MenuState.MainStatus);
                            break;
                        case Option.Config: // Config
                            Seven.MenuState.ChangeScreen(Seven.MenuState.ConfigScreen);
                            break;
                        case Option.PHS: // PHS
                            Seven.MenuState.ChangeScreen(Seven.MenuState.PhsScreen);
                            break;
                        case Option.Save: // Save
                            Seven.MenuState.ChangeScreen(Seven.MenuState.SaveScreen);
                            break;
                        default:
                            break;
                    }
                    break;
                case Key.Square:
                    Seven.Instance.BeginBattle();
                    break;
                default:
                    break;
            }
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
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
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private void DrawOption(Cairo.Context g, Option o)
        {
            Text.ShadowedText(g, o.ToString(),
                              X + x,
                              Y + (((int)o + 1) * y) + 5);
        }
        
        public Option Selection { get { return option; } }
        public override string Info
        { get { return ""; } }
    }

}

