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
        const int x3 = 140; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = x3 + 110; // fury/sadness
        const int cx = x5 - 8;
        const int y0 = 60; // row 0
        const int y1 = 210; // row 1
        const int y2 = 360; // row 2
        const int ya = 30; // subrow 1
        const int yb = 55; // subrow 2
        const int yc = 80; // subrow 3
        const int yp = 12; // ypic
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
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;
            
            
            
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
            
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;

            g.Color = COLOR_TEXT_TEAL;
            g.MoveTo(X + x3, Y + y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + y + yc);
            g.ShowText("MP");
            g.Color = Colors.WHITE;

            if (c.Fury)
            {
                Text.ShadowedText(g, COLOR_TEXT_MAGENTA, "[Fury]", X + x7, Y + y);
            }
            else if (c.Sadness)
            {
                Text.ShadowedText(g, COLOR_TEXT_MAGENTA, "[Sadness]", X + x7, Y + y);
            }
            
            Color namec = Colors.WHITE;
            if (c.Death)
            {
                namec = COLOR_TEXT_RED;
            }
            else if (c.NearDeath)
            {
                namec = COLOR_TEXT_YELLOW;
            }
            
            Text.ShadowedText(g, namec, c.Name, X + x3, Y + y);
            
            lvl = c.Level.ToString();
            hp = c.HP.ToString() + "/";
            hpm = c.MaxHP.ToString();
            mp = c.MP.ToString() + "/";
            mpm = c.MaxMP.ToString();
            
            te = g.TextExtents(lvl);
            Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + y + ya);
            
            te = g.TextExtents(hp);
            Text.ShadowedText(g, hp, X + x5 - te.Width, Y + y + yb);
            
            te = g.TextExtents(hpm);
            Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + y + yb);
            
            te = g.TextExtents(mp);
            Text.ShadowedText(g, mp, X + x5 - te.Width, Y + y + yc);
            
            te = g.TextExtents(mpm);
            Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + y + yc);
        }


        public override string Info
        { get { return Seven.MenuState.ItemList.Info; } }
    }
}

