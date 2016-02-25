using System;
using System.Linq;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Asset;
using GameItem = Atmosphere.Reverence.Seven.Asset.Item;
using SevenMenuState = Atmosphere.Reverence.Seven.State.MenuState;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Item
{
    internal sealed class List : ControlMenu
    {
        #region Layout
        
        const int x1 = 26; // item name
        const int y = 29; // line spacing
        const int cx = 15;
        const int cy = 22;
        const int rows = 15;
        
        #endregion
        
        private int option;
        private int topRow = 0;
        private int x2;         // count

        public List(SevenMenuState menuState, ScreenState screenState)
            : base(
                screenState.Width / 2,
                screenState.Height * 11 / 60,
                screenState.Width / 2 - 9,
                screenState.Height * 23 / 30)
        {
            x2 = screenState.Width / 2 - 24; // count
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
                    if (topRow > option)
                    {
                        topRow--;
                    }
                    break;
                case Key.Down:
                    if (option < Inventory.INVENTORY_SIZE - 1)
                    {
                        option++;
                    }
                    if (topRow < option - rows + 1)
                    {
                        topRow++;
                    }
                    break;
                case Key.X:
                    MenuState.ItemScreen.ChangeControl(MenuState.ItemTop);
                    break;
                case Key.Circle:

                    if (SelectedItem.CanUseInField)
                    {                  
                        GameItem fieldItem = (GameItem)SelectedItem;

                        switch (fieldItem.FieldTarget)
                        {
                            case FieldTarget.Character:
                            case FieldTarget.Party:
                                MenuState.ItemScreen.ChangeControl(MenuState.ItemStats);
                                break;
                        }
                    }
                    else
                    {
                        MenuState.Seven.ShowMessage(c => "!");
                    }

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
            
            TextExtents te;
            
            
            int j = Math.Min(rows + topRow, Inventory.INVENTORY_SIZE);

            
            for (int i = topRow; i < j; i++)
            {
                IInventoryItem item = MenuState.Party.Inventory.GetItem(i);
                if (item != null)
                {
                    int count = MenuState.Party.Inventory.GetCount(i);
                    te = g.TextExtents(count.ToString());

                    if (item.CanUseInField)
                    {
                        Text.ShadowedText(g, item.Name,
                                              X + x1, Y + (i - topRow + 1) * y);
                        Text.ShadowedText(g, count.ToString(),
                                              X + x2 - te.Width, Y + (i - topRow + 1) * y);
                    }
                    else
                    {
                        Text.ShadowedText(g, Colors.GRAY_4, item.Name,
                                              X + x1, Y + (i - topRow + 1) * y);
                        Text.ShadowedText(g, Colors.GRAY_4, count.ToString(),
                                              X + x2 - te.Width, Y + (i - topRow + 1) * y);
                    }
                }
            }
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy + (option - topRow) * y);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
        }

        public override void Reset()
        {
            option = 0;
            topRow = 0;
        }



        public IInventoryItem SelectedItem { get { return MenuState.Party.Inventory.GetItem(option); } }

        public int InventorySlot { get { return option; } }
        
        public override string Info
        {
            get
            {
                string info = String.Empty;

                IInventoryItem i = MenuState.Party.Inventory.GetItem(option);

                if (i != null)
                {
                    Weapon w = i as Weapon;

                    if (w != null)
                    {
                        info = "A weapon for " + w.Wielder;
                    }
                    else
                    {
                        info = i.Description;
                    }
                }
                return info;
            }
        }
        
        private SevenMenuState MenuState { get; set; }
    }
}

