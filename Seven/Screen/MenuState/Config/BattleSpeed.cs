using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Config
{
    internal sealed class BattleSpeed : ControlMenu
    {
        public const int WIDTH = 350;
        public const int HEIGHT = 50;

        private const int OPTION_MAX = 16;

        const int x0 = 10;
        const int y = 30;

        int x_spacing = (WIDTH - x0 * 2) / ( OPTION_MAX + 2);

        int _option_orig;
        int _option;

        public BattleSpeed(SevenMenuState menuState, int x, int y, Main owner)
                : base(x, y, WIDTH, HEIGHT)
        {
            Owner = owner;
            MenuState = menuState;

            _option = MenuState.Seven.Party.BattleSpeed / OPTION_MAX;
        }
            
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Left: 
                    if (_option > 0)
                    {
                        _option--;
                    }
                    break;
                case Key.Right: 
                    if (_option < OPTION_MAX)
                    {
                        _option++;
                    }
                    break;
                case Key.X:
                    _option = _option_orig;
                    SetNotControl();
                    break;
                case Key.Circle:
                    MenuState.Seven.Party.BattleSpeed = _option * OPTION_MAX;
                    SetNotControl();
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

            Color c;

            for (int i = 0; i <= OPTION_MAX; i++)
            {                    
                c = i == _option ? Colors.WHITE : Colors.GRAY_4;
                
                int x = X + x0 + x_spacing * (i + 1);

                char symbol;

                if (i > 9)
                {
                    symbol = '7';
                }
                else
                {
                    symbol = '0';
                }

                symbol += (char)i;
                
                Text.ShadowedText(g, c, symbol.ToString(), x, Y + y);
            }
                
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
            



        public override void SetAsControl()
        {
            Owner.SetNotControl();
            base.SetAsControl();

            _option_orig = _option;
        }

        public override void SetNotControl()
        {
            base.SetNotControl();
            Owner.SetAsControl();
        }
        
        public override string Info { get { return String.Empty; } }

        private Main Owner { get; set; }
        
        private SevenMenuState MenuState { get; set; }
    }
}

