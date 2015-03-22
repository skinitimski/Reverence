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
            Phs = 8,
            Save = 9
        }

        #region Layout
        
        const int x = 35;
        const int y = 30;
        const int cx = 20;
        const int cy = 28;
        
        #endregion Layout
        
        private Option option = 0;

        public Options()
            : base(
                Config.Instance.WindowWidth * 3 / 4 - 10,
                Config.Instance.WindowHeight / 20,
                Config.Instance.WindowWidth / 4,
                Config.Instance.WindowHeight * 11 / 20)
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
                            //MenuState.MainMenu.ChangeLayer(Screen._configScreen);
                            break;
                        case Option.Phs: // PHS
                            Seven.MenuState.ChangeScreen(Seven.MenuState.PhsScreen);
                            break;
                        case Option.Save: // Save
                            //MenuState.MainMenu.ChangeLayer(Screen._saveScreen);
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
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + ((int)option * y));
            }
            
            #region Menu Options
            // Option 0
            Text.ShadowedText(g, "Item",
                                  X + x,
                                  Y + (1 * y) + 5);
            // Option 1
            Text.ShadowedText(g, "Magic",
                                  X + x,
                                  Y + (2 * y) + 5);
            // Option 2
            Text.ShadowedText(g, "Materia",
                                  X + x,
                                  Y + (3 * y) + 5);
            // Option 3
            Text.ShadowedText(g, "Equip",
                                  X + x,
                                  Y + (4 * y) + 5);
            // Option 4
            Text.ShadowedText(g, "Status",
                                  X + x,
                                  Y + (5 * y) + 5);
            // Option 5
            Text.ShadowedText(g, "Order",
                                  X + x,
                                  Y + (6 * y) + 5);
            // Option 6
            Text.ShadowedText(g, "Limit",
                                  X + x,
                                  Y + (7 * y) + 5);
            // Option 7
            Text.ShadowedText(g, "Config",
                                  X + x,
                                  Y + (8 * y) + 5);
            // Option 8
            Text.ShadowedText(g, "PHS",
                                  X + x,
                                  Y + (9 * y) + 5);
            // Option 9
            Text.ShadowedText(g, "Save",
                                  X + x,
                                  Y + (10 * y) + 5);
#endregion
            
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public Option Selection { get { return option; } }
        public override string Info
        { get { return ""; } }
    }

}

