using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Graphics;
using Atmosphere.Reverence.Menu;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Phs
{      
    internal sealed class List : ControlMenu
    {
        private Character[,] _characters;
        
        #region Layout
        
        const int x = 50;
        const int y = 10;
        const int xs = 100;
        const int ys = 110;
        static int cx = x - 10;
        static int cy = y + 50;
        
#endregion
        
        private int optionX;
        private int optionY;

        public List(SevenMenuState menuState, Menu.ScreenState screenState)
            : base(
                screenState.Width / 2,
                screenState.Height * 22 / 60,
                screenState.Width / 2 - 9,
                screenState.Height * 7 / 12)
        { 
            _characters = new Character[3, 3];
            MenuState = menuState;
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (optionY > 0)
                    {
                        optionY--;
                    }
                    break;
                case Key.Down:
                    if (optionY < 2)
                    {
                        optionY++;
                    }
                    break;
                case Key.Left:
                    if (optionX > 0)
                    {
                        optionX--;
                    }
                    break;
                case Key.Right:
                    if (optionX < 2)
                    {
                        optionX++;
                    }
                    break;
                case Key.Circle:
                    Character temp = MenuState.Party.Reserves[optionY, optionX];
                    MenuState.Party.Reserves[optionY, optionX] = MenuState.Party[MenuState.PhsStats.Option];
                    MenuState.Party[MenuState.PhsStats.Option] = temp;
                    Update();
                    MenuState.PhsScreen.ChangeControl(MenuState.PhsStats);
                    break;
                case Key.X:
                    MenuState.PhsScreen.ChangeControl(MenuState.PhsStats);
                    break;
                default:
                    break;
            }
            switch (optionX)
            {
                case 0:
                    cx = x - 10;
                    break;
                case 1:
                    cx = x - 10 + xs;
                    break;
                case 2:
                    cx = x - 10 + xs + xs;
                    break;
            }
            switch (optionY)
            {
                case 0:
                    cy = y + 50;
                    break;
                case 1:
                    cy = y + 50 + ys;
                    break;
                case 2:
                    cy = y + 50 + ys + ys;
                    break;
            }
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            for (int a = 0; a <= 2; a++)
            {
                for (int b = 0; b <= 2; b++)
                {
                    if (MenuState.Party.Reserves[a, b] != null)
                    {
                        Images.RenderProfileSmall(d, gc, X + x + b * xs, Y + y + a * ys, MenuState.Party.Reserves[a, b]);
                    }
                }
            }
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy - 15);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public void Update()
        {
            for (int a = 0; a <= 2; a++)
            {
                for (int b = 0; b <= 2; b++)
                {
                    if (b + (3 * a) < MenuState.Party.Reserves.Length)
                    {
                        _characters[a, b] = MenuState.Party.Reserves[b, a];
                    }
                }
            }

            optionX = 0;
            optionY = 0;
            cx = x - 10;
            cy = y + 50;
        }
        
        public Character Selection
        { get { return MenuState.Party.Reserves[optionY, optionX]; } }
        
        public override string Info
        { get { return (Selection == null) ? "" : Selection.Name; } }

        private SevenMenuState MenuState { get; set; }
    }

}

