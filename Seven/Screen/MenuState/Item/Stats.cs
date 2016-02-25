using System;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Graphics;
using GameItem = Atmosphere.Reverence.Seven.Asset.Item;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Item
{
    internal sealed class Stats : ControlMenu
    {   
        #region Layout
        
        const int x_pic = 15; // pic
        
        const int x_status = x_pic + Character.PROFILE_WIDTH + 15; 
        const int x_cursor = x_pic + Character.PROFILE_WIDTH + 115;
        
        const int y_firstRow = 48;        
        const int row_height = 150;  
        
        const int y_displacement_cursor = 0;
        const int y_displacement_pic = -35;

        #endregion
        
        private int option;

        private GameItem _selectedItem;
        
        public Stats(SevenMenuState menuState, ScreenState screenState)
            : base(
                2,
                screenState.Height * 11 / 60,
                screenState.Width / 2,
                screenState.Height * 23 / 30)
        {
            MenuState = menuState;
        }

        public override void SetAsControl()
        {
            base.SetAsControl();

            if (!MenuState.ItemList.SelectedItem.CanUseInField)
            {
                throw new ImplementationException("Tried to use an item in the field that can't be used in the field.");
            }
            
            _selectedItem = (GameItem)MenuState.ItemList.SelectedItem;
        }
        
        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0)
                    {
                        MenuState.Party.DecrementSelection();
                        option--;
                    }
                    break;
                case Key.Down:
                    if (option < 2)
                    {
                        MenuState.Party.IncrementSelection();
                        option++;
                    }
                    break;
                case Key.X:
                    MenuState.ItemScreen.ChangeControl(MenuState.ItemList);
                    break;
                case Key.Circle:

                    if (_selectedItem.FieldTarget == FieldTarget.Character && MenuState.Party.Selected == null)
                    {
                        MenuState.Seven.ShowMessage(c => "!");
                        break;
                    }

                    int slot = MenuState.ItemList.InventorySlot;



                    IInventoryItem item = MenuState.Party.Inventory.GetItem(slot);
                    
                    if (!item.CanUseInField)
                    {
                        throw new ImplementationException("Tried to use an item in the field that can't be used in the field.");
                    }
                    
                    bool used = ((GameItem)item).UseInField(MenuState.Party);
                    
                    if (used)
                    {
                        MenuState.Party.Inventory.DecreaseCount(slot);

                        if (MenuState.Party.Inventory.GetCount(slot) == 0)
                        {
                            MenuState.ItemScreen.ChangeControl(MenuState.ItemList);
                        }
                    }
                    else
                    {
                        MenuState.Seven.ShowMessage(c => "!");
                    }

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
                switch (_selectedItem.FieldTarget)
                {
                    case FieldTarget.Character:
                        Shapes.RenderCursor(g, X + x_cursor, Y + y_firstRow + y_displacement_cursor + option * row_height);
                        break;
                    case FieldTarget.Party:
                        for (int i = 0; i < Party.PARTY_SIZE; i++)
                        {                            
                            Shapes.RenderBlinkingCursor(g, X + x_cursor, Y + y_firstRow + y_displacement_cursor + i * row_height);
                        }
                        break;
                }
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        private void DrawCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Cairo.Context g, Character c, int y)
        {       
            Images.RenderProfile(d, gc, X + x_pic, Y + y + y_displacement_pic, c);

            Graphics.Stats.RenderCharacterStatus(d, gc, g, c, X + x_status, Y + y);
        }


        public override string Info
        { get { return MenuState.ItemList.Info; } }
        
        private SevenMenuState MenuState { get; set; }
    }
}

