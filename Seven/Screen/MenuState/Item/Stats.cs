using System;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Graphics;
using GameItem = Atmosphere.Reverence.Seven.Asset.Item;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Item
{
    internal sealed class Stats : ControlMenu
    {   
        #region Layout
        
        const int x1 = 10; // xpic
        const int xs = x1 + Character.PROFILE_WIDTH + 10; // name
        const int cx = 245;
        const int y0 = 60;  // row 0
        const int y1 = 210; // row 1
        const int y2 = 360; // row 2
        const int yp = 12;  // ypic
        const int c0 = y0 - 8;             // cursor index 0
        const int c1 = y0 - 8 + (y1 - y0); // cursor index 1
        const int c2 = y0 - 8 + (y2 - y0); // cursor index 2 
        
        #endregion
        
        private int option;
        private int cy = y0 - 8;

        private GameItem _selectedItem;
        
        public Stats()
            : base(
                2,
                Config.Instance.WindowHeight * 11 / 60,
                Config.Instance.WindowWidth / 2,
                Config.Instance.WindowHeight * 23 / 30)
        {
        }

        public override void SetAsControl()
        {
            base.SetAsControl();

            if (!Seven.MenuState.ItemList.SelectedItem.CanUseInField)
            {
                throw new ImplementationException("Tried to use an item in the field that can't be used in the field.");
            }
            
            _selectedItem = (GameItem)Seven.MenuState.ItemList.SelectedItem;
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0)
                    {
                        Seven.Party.DecrementSelection();
                        option--;
                    }
                    break;
                case Key.Down:
                    if (option < 2)
                    {
                        Seven.Party.IncrementSelection();
                        option++;
                    }
                    break;
                case Key.X:
                    Seven.MenuState.ItemScreen.ChangeControl(Seven.MenuState.ItemList);
                    break;
                case Key.Circle:

                    if (_selectedItem.FieldTarget == FieldTarget.Character && Seven.Party.Selected == null)
                    {
                        // TODO: beep
                        break;
                    }

                    if (Seven.Party.Inventory.UseItemInField(Seven.MenuState.ItemList.InventorySlot))
                    {
                        if (Seven.Party.Inventory.GetCount(Seven.MenuState.ItemList.InventorySlot) == 0)
                        {
                            Seven.MenuState.ItemScreen.ChangeControl(Seven.MenuState.ItemList);
                        }
                    }
                    else
                    {
                        // beep
                    }

                    break;
            }

            switch (option)
            {
                case 0:
                    cy = c0;
                    break;
                case 1: 
                    cy = c1; 
                    break;
                case 2: 
                    cy = c2;  
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
                switch (_selectedItem.FieldTarget)
                {
                    case FieldTarget.Character:
                        Shapes.RenderCursor(g, X + cx, Y + cy);
                        break;
                    case FieldTarget.Party:
                        Shapes.RenderBlinkingCursor(g, X + cx, Y + c0);
                        Shapes.RenderBlinkingCursor(g, X + cx, Y + c1);
                        Shapes.RenderBlinkingCursor(g, X + cx, Y + c2);
                        break;
                }
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        private void DrawCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Cairo.Context g, Character c, int y)
        {       
            Images.RenderProfile(d, gc, X + x1, Y + yp + y - y0, c);

            Graphics.Stats.RenderCharacterStatus(d, gc, g, c, X + xs, Y + y);
        }


        public override string Info
        { get { return Seven.MenuState.ItemList.Info; } }
    }
}

