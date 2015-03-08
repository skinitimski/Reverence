using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class ItemLabel : Menu
    {
        public static ItemLabel Instance;

        static ItemLabel()
        {
            Instance = new ItemLabel();
        }
        private ItemLabel()
            : base(
                Globals.WIDTH * 3 / 4,
                Globals.HEIGHT / 20,
                Globals.WIDTH / 4 - 10,
                Globals.HEIGHT / 15)
        { }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, "Item", X + 20, Y + 25);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }




    internal sealed class ItemTop : ControlMenu
    {
        public static ItemTop Instance;

        #region Layout

        const int x1 = 50; // Use
        const int x2 = 150; // Arrange
        const int x3 = 300; // Key Items
        const int y = 25;

        const int xs = 15; // spacing for cursor
        static int cx = x1 - xs; // see? told ya
        const int cy = y - 10;

        #endregion

        private int option = 0;


        static ItemTop()
        {
            Instance = new ItemTop();
        }
        private ItemTop()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT / 15)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Left:
                    if (option > 0) option--;
                    break;
                case Key.Right:
                    if (option < 2) option++;
                    break;
                case Key.X:
                    Game.MainMenu.ChangeScreen(MenuScreen.MainScreen);
                    ItemList.Instance.Reset();
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case 0:
                            MenuScreen.ItemScreen.ChangeControl(ItemList.Instance);
                            break;
                        case 1:
                            Inventory.Sort();
                            break;
                        case 2:
                            break;
                    }
                    break;
                default:
                    break;
            }
            switch (option)
            {
                case 0: cx = x1 - xs; break;
                case 1: cx = x2 - xs; break;
                case 2: cx = x3 - xs; break;
                default: break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy);

            Graphics.ShadowedText(g, "Use", X + x1, Y + y);
            Graphics.ShadowedText(g, "Arrange", X + x2, Y + y);
            Graphics.ShadowedText(g, "Key Items", X + x3, Y + y);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override string Info
        {
            get
            {
                switch (option)
                {
                    case 0:
                        return "Use an item";
                    case 1:
                        return "Arrange by type/name";
                    case 2:
                        return "You have no key items, trust me";
                    default:
                        return "";
                }
            }
        }
    }




    internal sealed class ItemInfo : Menu
    {
        public static ItemInfo Instance;

        static ItemInfo()
        {
            Instance = new ItemInfo();
        }
        private ItemInfo()
            : base(
                2,
                Globals.HEIGHT / 20 + Globals.HEIGHT / 15,
                Globals.WIDTH - 10,
                Globals.HEIGHT / 15)
        { }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            Graphics.ShadowedText(g, MenuScreen.ItemScreen.Control.Info, X + 20, Y + 28);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
    



    internal sealed class ItemStats : ControlMenu
    {
        public static ItemStats Instance;

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

        #endregion

        private int option;
        private int cy = y0 - 8;


        static ItemStats()
        {
            Instance = new ItemStats();
        }
        private ItemStats()
            : base(
                2,
                Globals.HEIGHT * 11 / 60,
                Globals.WIDTH / 2,
                Globals.HEIGHT * 23 / 30)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    break;
                case Key.Down:
                    if (option < 2) option++;
                    break;
                case Key.X:
                    MenuScreen.ItemScreen.ChangeControl(ItemList.Instance);
                    break;
                case Key.Circle:
                    if (Globals.Party[option] != null)
                        if (Inventory.UseItem(ItemList.Instance.Option))
                            if (Inventory.GetCount(ItemList.Instance.Option) == 0)
                                MenuScreen.ItemScreen.ChangeControl(ItemList.Instance);
                    break;
            }
            switch (option)
            {
                case 0: cy = y0 - 8; Item.Target = Globals.Party[0]; break;
                case 1: cy = y0 - 8 + (y1 - y0); Item.Target = Globals.Party[1]; break;
                case 2: cy = y0 - 8 + (y2 - y0); Item.Target = Globals.Party[2]; break;
                default: break;
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


            #region Character 1

            if (Globals.Party[0] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[0].Name), 0, 0,
                    X + x1, Y + yp,
                    Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);

                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y0 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y0 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y0 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);

                if (Globals.Party[0].Fury)
                    Graphics.ShadowedText(g, new Color(.7, 0, .7), "[Fury]", X + x7, Y + y0);
                else if (Globals.Party[0].Sadness)
                    Graphics.ShadowedText(g, new Color(.7, 0, .7), "[Sadness]", X + x7, Y + y0);

                Color namec = new Color(1, 1, 1);
                if (Globals.Party[0].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Globals.Party[0].NearDeath)
                    namec = new Color(.8, .8, 0);

                Graphics.ShadowedText(g, namec, Globals.Party[0].Name,
                    X + x3,
                    Y + y0);

                lvl = Globals.Party[0].Level.ToString();
                hp = Globals.Party[0].HP.ToString() + "/";
                hpm = Globals.Party[0].MaxHP.ToString();
                mp = Globals.Party[0].MP.ToString() + "/";
                mpm = Globals.Party[0].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y0 + ya);

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y0 + yb);

                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y0 + yb);

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y0 + yc);

                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y0 + yc);
            }
            #endregion Character 1


            #region Character 2

            if (Globals.Party[1] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[1].Name), 0, 0,
                    X + x1, Y + yp + (y1 - y0),
                    Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);

                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y1 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y1 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y1 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);

                if (Globals.Party[1].Fury)
                    Graphics.ShadowedText(g, new Color(.7, 0, .7), "[Fury]", X + x7, Y + y1);
                else if (Globals.Party[1].Sadness)
                    Graphics.ShadowedText(g, new Color(.7, 0, .7), "[Sadness]", X + x7, Y + y1);

                Color namec = new Color(1, 1, 1);
                if (Globals.Party[1].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Globals.Party[1].NearDeath)
                    namec = new Color(.8, .8, 0);

                Graphics.ShadowedText(g, namec, Globals.Party[1].Name,
                    X + x3,
                    Y + y1);

                lvl = Globals.Party[1].Level.ToString();
                hp = Globals.Party[1].HP.ToString() + "/";
                hpm = Globals.Party[1].MaxHP.ToString();
                mp = Globals.Party[1].MP.ToString() + "/";
                mpm = Globals.Party[1].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y1 + ya);

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y1 + yb);

                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y1 + yb);

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y1 + yc);

                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y1 + yc);
            }

            #endregion Character 2


            #region Character 3

            if (Globals.Party[2] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[2].Name), 0, 0,
                    X + x1, Y + yp + (y2 - y0),
                    Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);

                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y2 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y2 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y2 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);

                if (Globals.Party[2].Fury)
                    Graphics.ShadowedText(g, new Color(.7, 0, .7), "[Fury]", X + x7, Y + y2);
                else if (Globals.Party[2].Sadness)
                    Graphics.ShadowedText(g, new Color(.7, 0, .7), "[Sadness]", X + x7, Y + y2);

                Color namec = new Color(1, 1, 1);
                if (Globals.Party[2].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Globals.Party[2].NearDeath)
                    namec = new Color(.8, .8, 0);

                Graphics.ShadowedText(g, namec, Globals.Party[2].Name, X + x3, Y + y2);

                lvl = Globals.Party[2].Level.ToString();
                hp = Globals.Party[2].HP.ToString() + "/";
                hpm = Globals.Party[2].MaxHP.ToString();
                mp = Globals.Party[2].MP.ToString() + "/";
                mpm = Globals.Party[2].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y2 + ya);

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y2 + yb);

                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y2 + yb);

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y2 + yc);

                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y2 + yc);
            }

            #endregion Character 3


            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        public override string Info
        { get { return ItemList.Instance.Info; } }
    }




    internal sealed class ItemList : ControlMenu
    {
        public static ItemList Instance;

        #region Layout

        const int x1 = 26; // item name
        const int x2 = Globals.WIDTH / 2 - 24; // count
        const int y = 29; // line spacing
        const int cx = 15;
        const int cy = 22;
        const int rows = 15;

        #endregion

        private int option;
        private int topRow = 0;

        static ItemList()
        {
            Instance = new ItemList();
        }
        private ItemList()
            : base(
                Globals.WIDTH / 2,
                Globals.HEIGHT * 11 / 60,
                Globals.WIDTH / 2 - 9,
                Globals.HEIGHT * 23 / 30)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    if (topRow > option) topRow--;
                    break;
                case Key.Down:
                    if (option < Inventory.INVENTORY_SIZE - 1) option++;
                    if (topRow < option - rows + 1) topRow++;
                    break;
                case Key.X:
                    MenuScreen.ItemScreen.ChangeControl(ItemTop.Instance);
                    break;
                case Key.Circle:
                    if (Inventory.GetItem(option) != null)
                        if (Inventory.GetItem(option).Type == ItemType.Field ||
                            Inventory.GetItem(option).Type == ItemType.Hybrid)
                            MenuScreen.ItemScreen.ChangeControl(ItemStats.Instance);
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

            TextExtents te;


            int j = Math.Min(rows + topRow, Inventory.INVENTORY_SIZE);
            
            Color gray = new Color(.4, .4, .4);

            for (int i = topRow; i < j; i++)
            {
                IItem item = Inventory.GetItem(i);
                if (item != null)
                {
                    int count = Inventory.GetCount(i);
                    te = g.TextExtents(count.ToString());
                    if (item.Type == ItemType.Field || item.Type == ItemType.Hybrid)
                    {
                        Graphics.ShadowedText(g, item.Name,
                            X + x1, Y + (i - topRow + 1) * y);
                        Graphics.ShadowedText(g, count.ToString(),
                            X + x2 - te.Width, Y + (i - topRow + 1) * y);
                    }
                    else
                    {
                        Graphics.ShadowedText(g, gray, item.Name,
                            X + x1, Y + (i - topRow + 1) * y);
                        Graphics.ShadowedText(g, gray, count.ToString(),
                            X + x2 - te.Width, Y + (i - topRow + 1) * y);
                    }
                }
            }

            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy + (option - topRow) * y);
            

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
        public int Option { get { return option; } }

        public override string Info
        {
            get
            {
                IItem i = Inventory.GetItem(option);
                return (i == null) ? "" : i.Description;
            }
        }
    }
}