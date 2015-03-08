using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class EquipLabel : Menu
    {
        public static EquipLabel Instance;

        static EquipLabel()
        {
            Instance = new EquipLabel();
        }
        private EquipLabel()
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

            Graphics.ShadowedText(g, "Equip", X + 20, Y + 25);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }





    internal sealed class EquipTop : ControlMenu
    {
        public static EquipTop Instance;

        #region Layout

        const int x3 = 140; // name(s)
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = 330; // equipment labels
        const int x8 = x7 + 60; // equipment names
        const int cx = x7 + 25;

        const int y = 50; // main row
        const int ya = 30; // subrow A1
        const int yb = 55; // subrow A2
        const int yc = 80; // subrow A3
        const int yi = 35; // subrow B1
        const int yj = yi + 50; // subrow B2
        const int yk = yj + 50; // subrow B3

        const int xpic = 15;
        const int ypic = 15;

        static int cy = yi;

        #endregion

        private int _option = 0;
        private bool _changed = false;

        static EquipTop()
        {
            Instance = new EquipTop();
        }
        private EquipTop()
            : base(
                2,
                Globals.HEIGHT / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT * 3 / 10 - 6)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (_option > 0) _option--;
                    break;
                case Key.Down:
                    if (_option < 2) _option++;
                    break;
                case Key.X:
                    if (_changed)
                        Game.MainMenu.ChangeScreen(MenuScreen.MateriaScreen);
                    else
                        Game.MainMenu.ChangeScreen(MenuScreen.MainScreen);
                    _changed = false;
                    break;
                case Key.Square:
                    Game.MainMenu.ChangeScreen(MenuScreen.MateriaScreen);
                    break;
                case Key.Circle:
                    MenuScreen.EquipScreen.ChangeControl(EquipList.Instance);
                    break;
                case Key.Triangle:
                    if (_option == 2)
                        if (Selected.Accessory.Name != null)
                        {
                            Inventory.AddToInventory(Selected.Accessory);
                            Selected.Accessory = new Accessory();
                        }
                    break;
                default:
                    break;
            }
            switch (_option)
            {
                case 0: cy = yi; break;
                case 1: cy = yj; break;
                case 2: cy = yk; break;
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
            string weapon, armor, acc;


            #region Character Status

            d.DrawPixbuf(gc, Graphics.GetProfile(Selected.Name), 0, 0,
                X + xpic, Y + ypic,
                Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT,
                Gdk.RgbDither.None, 0, 0);

            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x3, Y + y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + y + yc);
            g.ShowText("MP");
            g.Color = new Color(1, 1, 1);

            Graphics.ShadowedText(g, Selected.Name, X + x3, Y + y);

            lvl = Selected.Level.ToString();
            hp = Selected.HP.ToString() + "/";
            hpm = Selected.MaxHP.ToString();
            mp = Selected.MP.ToString() + "/";
            mpm = Selected.MaxMP.ToString();

            te = g.TextExtents(lvl);
            Graphics.ShadowedText(g, lvl, X + x4 - te.Width, Y + y + ya);

            te = g.TextExtents(hp);
            Graphics.ShadowedText(g, hp, X + x5 - te.Width, Y + y + yb);

            te = g.TextExtents(hpm);
            Graphics.ShadowedText(g, hpm, X + x6 - te.Width, Y + y + yb);

            te = g.TextExtents(mp);
            Graphics.ShadowedText(g, mp, X + x5 - te.Width, Y + y + yc);

            te = g.TextExtents(mpm);
            Graphics.ShadowedText(g, mpm, X + x6 - te.Width, Y + y + yc);

            #endregion Status

            #region Equipment

            g.Color = new Color(.3, .8, .8);
            g.MoveTo(X + x7, Y + yi);
            g.ShowText("Wpn.");
            g.MoveTo(X + x7, Y + yj);
            g.ShowText("Arm.");
            g.MoveTo(X + x7, Y + yk);
            g.ShowText("Acc.");
            g.Color = new Color(1, 1, 1);

            weapon = Selected.Weapon.Name;
            armor = Selected.Armor.Name;
            acc = Selected.Accessory.Name;

            te = g.TextExtents(weapon);
            Graphics.ShadowedText(g, weapon, X + x8, Y + yi);
            te = g.TextExtents(armor);
            Graphics.ShadowedText(g, armor, X + x8, Y + yj);
            te = g.TextExtents(acc);
            Graphics.ShadowedText(g, acc, X + x8, Y + yk);

            #endregion Equipment


            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy - 10);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public int Option { get { return _option; } }
        public bool Changed { get { return _changed; } set { _changed = value; } }
        public override string Info
        {
            get
            {
                switch (_option)
                {
                    case 0:
                        return Selected.Weapon.Description;
                    case 1:
                        return Selected.Armor.Description;
                    case 2:
                        return Selected.Accessory.Description;
                    default: return "";
                }
            }
        }
    }





    internal sealed class EquipStats : Menu
    {
        public static EquipStats Instance;

        #region Layout

        const int x0 = 50; // column 1
        const int x1 = x0 + 225; // column 2
        const int x2 = x1 + 40; // column 3
        const int x3 = x2 + 80; // column 3

        const int yr = 30; // 7th row (left)
        const int line = 28; // row spacing (left)

        #endregion

        static EquipStats()
        {
            Instance = new EquipStats();
        }
        private EquipStats()
            : base(
                2,
                Globals.HEIGHT * 7 / 12,
                Globals.WIDTH * 5 / 8,
                Globals.HEIGHT * 11 / 30)
        { 
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            #region Left

            string str, vit, dex, mag, spi, lck;
            int atk, atkp, def, defp, mat, mdf, mdfp;

            str = Selected.Strength.ToString();
            vit = Selected.Vitality.ToString();
            dex = Selected.Dexterity.ToString();
            mag = Selected.Magic.ToString();
            spi = Selected.Spirit.ToString();
            lck = Selected.Luck.ToString();

            atk = Ally.Attack(Selected);
            atkp = Ally.AttackPercent(Selected);
            def = Ally.Defense(Selected);
            defp = Ally.DefensePercent(Selected);
            mat = Ally.MagicAttack(Selected);
            mdf = Ally.MagicDefense(Selected);
            mdfp = Ally.MagicDefensePercent(Selected);

            Cairo.Color greenish = new Color(.3, .8, .8);

            Graphics.ShadowedText(g, greenish, "Attack", X + x0, Y + yr + (line * 0));
            Graphics.ShadowedText(g, greenish, "Attack %", X + x0, Y + yr + (line * 1));
            Graphics.ShadowedText(g, greenish, "Defense", X + x0, Y + yr + (line * 2));
            Graphics.ShadowedText(g, greenish, "Defense %", X + x0, Y + yr + (line * 3));
            Graphics.ShadowedText(g, greenish, "Magic", X + x0, Y + yr + (line * 4));
            Graphics.ShadowedText(g, greenish, "Magic def", X + x0, Y + yr + (line * 5));
            Graphics.ShadowedText(g, greenish, "Magic def %", X + x0, Y + yr + (line * 6));

            te = g.TextExtents(atk.ToString());
            Graphics.ShadowedText(g, atk.ToString(), X + x1 - te.Width, Y + yr + (line * 0));
            te = g.TextExtents(atkp.ToString());
            Graphics.ShadowedText(g, atkp.ToString(), X + x1 - te.Width, Y + yr + (line * 1));
            te = g.TextExtents(def.ToString());
            Graphics.ShadowedText(g, def.ToString(), X + x1 - te.Width, Y + yr + (line * 2));
            te = g.TextExtents(defp.ToString());
            Graphics.ShadowedText(g, defp.ToString(), X + x1 - te.Width, Y + yr + (line * 3));
            te = g.TextExtents(mat.ToString());
            Graphics.ShadowedText(g, mat.ToString(), X + x1 - te.Width, Y + yr + (line * 4));
            te = g.TextExtents(mdf.ToString());
            Graphics.ShadowedText(g, mdf.ToString(), X + x1 - te.Width, Y + yr + (line * 5));
            te = g.TextExtents(mdfp.ToString());
            Graphics.ShadowedText(g, mdfp.ToString(), X + x1 - te.Width, Y + yr + (line * 6));

            if (EquipList.Instance.IsControl)
            {
                int t_atk, t_atkp, t_def, t_defp, t_mat, t_mdf, t_mdfp;

                IItem temp;

                switch (EquipTop.Instance.Option)
                {
                    case 0:
                        temp = Selected.Weapon;
                        Selected.Weapon = (Weapon)EquipList.Instance.Selection;
                        break;
                    case 1:
                        temp = Selected.Armor;
                        Selected.Armor = (Armor)EquipList.Instance.Selection;
                        break;
                    case 2:
                        temp = Selected.Accessory;
                        Selected.Accessory = (Accessory)EquipList.Instance.Selection;
                        break;
                    default:
                        temp = Selected.Weapon;
                        break;
                }

                t_atk = Ally.Attack(Selected);
                t_atkp = Ally.AttackPercent(Selected);
                t_def = Ally.Defense(Selected);
                t_defp = Ally.DefensePercent(Selected);
                t_mat = Ally.MagicAttack(Selected);
                t_mdf = Ally.MagicDefense(Selected);
                t_mdfp = Ally.MagicDefensePercent(Selected);

                switch (EquipTop.Instance.Option)
                {
                    case 0:
                        Selected.Weapon = (Weapon)temp;
                        break;
                    case 1:
                        Selected.Armor = (Armor)temp;
                        break;
                    case 2:
                        Selected.Accessory = (Accessory)temp;
                        break;
                    default: break;
                }

                Color c;
                Color yellow = new Color(.8, .8, 0);
                Color red = new Color(0.8, 0, 0);
                Color white = new Color(1, 1, 1);

                Graphics.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 0));
                Graphics.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 1));
                Graphics.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 2));
                Graphics.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 3));
                Graphics.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 4));
                Graphics.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 5));
                Graphics.ShadowedText(g, white, ">", X + x2, Y + yr + (line * 6));

                te = g.TextExtents(t_atk.ToString());
                if (t_atk < atk) c = red;
                else if (t_atk > atk) c = yellow;
                else c = white;
                Graphics.ShadowedText(g, c, t_atk.ToString(), X + x3 - te.Width, Y + yr + (line * 0));

                te = g.TextExtents(t_atkp.ToString());
                if (t_atkp < atkp) c = red;
                else if (t_atkp > atkp) c = yellow;
                else c = white;
                Graphics.ShadowedText(g, c, t_atkp.ToString(), X + x3 - te.Width, Y + yr + (line * 1));

                te = g.TextExtents(t_def.ToString());
                if (t_def < def) c = red;
                else if (t_def > def) c = yellow;
                else c = white;
                Graphics.ShadowedText(g, c, t_def.ToString(), X + x3 - te.Width, Y + yr + (line * 2));

                te = g.TextExtents(t_defp.ToString());
                if (t_defp < defp) c = red;
                else if (t_defp > defp) c = yellow;
                else c = white;
                Graphics.ShadowedText(g, c, t_defp.ToString(), X + x3 - te.Width, Y + yr + (line * 3));

                te = g.TextExtents(t_mat.ToString());
                if (t_mat < mat) c = red;
                else if (t_mat > mat) c = yellow;
                else c = white;
                Graphics.ShadowedText(g, c, t_mat.ToString(), X + x3 - te.Width, Y + yr + (line * 4));

                te = g.TextExtents(t_mdf.ToString());
                if (t_mdf < mdf) c = red;
                else if (t_mdf > mdf) c = yellow;
                else c = white;
                Graphics.ShadowedText(g, c, t_mdf.ToString(), X + x3 - te.Width, Y + yr + (line * 5));

                te = g.TextExtents(t_mdfp.ToString());
                if (t_mdfp < mdfp) c = red;
                else if (t_mdfp > mdfp) c = yellow;
                else c = white;
                Graphics.ShadowedText(g, c, t_mdfp.ToString(), X + x3 - te.Width, Y + yr + (line * 6));

            }

            #endregion Left

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

    }





    internal sealed class EquipSelected : Menu
    {
        public static EquipSelected Instance;

        #region Layout

        const int x0 = 30;
        const int x1 = 150;

        const int ya = 40;
        const int yb = ya + 40;

        const int yh = 0; // first row (right)
        const int yi = yh + 40; // successive rows
        const int yj = yi + 37; //    |
        const int yk = yj + 40; //    | 
        const int yl = yk + 37; //    x

        const int xs = yl - yk;
        const int ysa = 7;
        const int ysb = 11;
        const int zs = 5;

        const int x2 = x1 + (8 * xs) / 2;

        #endregion

        static EquipSelected()
        {
            Instance = new EquipSelected();
        }
        private EquipSelected()
            : base(
                2,
                Globals.HEIGHT * 5 / 12,
                Globals.WIDTH * 5 / 8,
                Globals.HEIGHT / 6)
        {
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Gdk.GC gc = new Gdk.GC(d);
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            TextExtents te;

            if (Growth != "")
            {
                g.Color = new Color(.1, .1, .2);
                g.Rectangle(X + x1, Y + ya - (yj - yi) + ysa, 8 * xs, yj - yi);
                g.Fill();

                Cairo.Color gray1 = new Color(.2, .2, .2);
                Cairo.Color gray2 = new Color(.7, .7, .8);

                int links, slots;

                slots = Slots.Length;
                links = Links;


                for (int j = 0; j < links; j++)
                {
                    Graphics.RenderLine(g, gray2, 3,
                        X + x1 + (xs / 2) + (j * 2 * xs), Y + ya - ysb - zs,
                        X + x1 + (xs / 2) + ((j * 2 + 1) * xs), Y + ya - ysb - zs);
                    Graphics.RenderLine(g, gray2, 3,
                        X + x1 + (xs / 2) + (j * 2 * xs), Y + ya - ysb,
                        X + x1 + (xs / 2) + ((j * 2 + 1) * xs), Y + ya - ysb);
                    Graphics.RenderLine(g, gray2, 3,
                        X + x1 + (xs / 2) + (j * 2 * xs), Y + ya - ysb + zs,
                        X + x1 + (xs / 2) + ((j * 2 + 1) * xs), Y + ya - ysb + zs);
                }
                for (int i = 0; i < slots; i++)
                {
                    Graphics.RenderCircle(g, gray2, 14,
                        X + x1 + (i * xs) + (xs / 2), Y + yi - ysb);
                    Graphics.RenderCircle(g, gray1, 10,
                        X + x1 + (i * xs) + (xs / 2), Y + yi - ysb);
                }

                Graphics.ShadowedText(g, new Color(.3, .8, .8), "Slot", X + x0, Y + ya);
                Graphics.ShadowedText(g, new Color(.3, .8, .8), "Growth", X + x0, Y + yb);

                string growth = Growth;

                te = g.TextExtents(growth);
                Graphics.ShadowedText(g, growth, X + x2 - (te.Width / 2), Y + yb);
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private string Growth
        {
            get
            {
                if (EquipTop.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return Selected.Weapon.Growth.ToString();
                        case 1: return Selected.Armor.Growth.ToString();
                        default: return "";
                    }
                }
                else if (EquipList.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return ((Weapon)EquipList.Instance.Selection).Growth.ToString();
                        case 1: return ((Armor)EquipList.Instance.Selection).Growth.ToString();
                        default: return "";
                    }
                }
                else return "";
            }
        }
        private Materia[] Slots
        {
            get
            {
                if (EquipTop.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return Selected.Weapon.Slots;
                        case 1: return Selected.Armor.Slots;
                        default: return null;
                    }
                }
                else if (EquipList.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return ((Weapon)EquipList.Instance.Selection).Slots;
                        case 1: return ((Armor)EquipList.Instance.Selection).Slots;
                        default: return null;
                    }
                }
                else return null;
            }
        }
        private int Links
        {
            get
            {
                if (EquipTop.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return Selected.Weapon.Links;
                        case 1: return Selected.Armor.Links;
                        default: return 0;
                    }
                }
                else if (EquipList.Instance.IsControl)
                {
                    switch (EquipTop.Instance.Option)
                    {
                        case 0: return ((Weapon)EquipList.Instance.Selection).Links;
                        case 1: return ((Armor)EquipList.Instance.Selection).Links;
                        default: return 0;
                    }
                }
                else return 0;
            }
        }


    }





    internal sealed class EquipInfo : Menu
    {
        public static EquipInfo Instance;

        static EquipInfo()
        {
            Instance = new EquipInfo();
        }
        private EquipInfo()
            : base(
                2,
                Globals.HEIGHT * 7 / 20,
                Globals.WIDTH - 10,
                Globals.HEIGHT / 15)
        {
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

                Graphics.ShadowedText(g, MenuScreen.EquipScreen.Control.Info, X + 20, Y + 27);

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

    }





    internal sealed class EquipList : ControlMenu
    {
        public static EquipList Instance;

        #region Layout

        const int x = 35;
        const int y = 30;
        const int cx = 20;
        const int cy = 22;
        const int rows = 10;

        #endregion 

        int _option = 0;
        int _topRow = 0;

        private List<InventoryRecord> _equipment;

        static EquipList()
        {
            Instance = new EquipList();
        }
        private EquipList()
            : base(
                Globals.WIDTH * 5 / 8,
                Globals.HEIGHT * 5 / 12,
                Globals.WIDTH * 3 / 8 - 8,
                Globals.HEIGHT * 8 / 15)
        {
            _equipment = new List<InventoryRecord>();
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (_option > 0) _option--;
                    if (_topRow > _option) _topRow--;
                    break;
                case Key.Down:
                    if (_option < _equipment.Count - 1) _option++;
                    if (_topRow < _option - rows + 1) _topRow++;
                    break;
                case Key.Circle:
                    int slot = _equipment[_option].Slot;
                    EquipTop.Instance.Changed = true;
                    switch (EquipTop.Instance.Option)
                    {
                        case 0:
                            Weapon.SwapMateria(Selected.Weapon, (Weapon)_equipment[_option].Item, Selected);
                            Selected.Weapon = (Weapon)Inventory.SwapOut(Selected.Weapon, slot);
                            break;
                        case 1:
                            Armor.SwapMateria(Selected.Armor, (Armor)_equipment[_option].Item, Selected);
                            Inventory.AddToInventory(Selected.Armor);
                            Inventory.DecreaseCount(slot);
                            Selected.Armor = (Armor)_equipment[_option].Item;
                            break;
                        case 2:
                            if (Selected.Accessory.Name != null)
                                Inventory.AddToInventory(Selected.Accessory);
                            Inventory.DecreaseCount(slot);
                            Selected.Accessory = (Accessory)_equipment[_option].Item;
                            break;
                    }
                    MenuScreen.EquipScreen.ChangeToDefaultControl();
                    break;
                case Key.X:
                    MenuScreen.EquipScreen.ChangeToDefaultControl();
                    break;
                default:
                    break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.Color = new Color(1, 1, 1);
            g.SetFontSize(24);

            int j = Math.Min(rows + _topRow, _equipment.Count);

            for (int i = _topRow; i < j; i++)
                Graphics.ShadowedText(g, _equipment[i].Item.Name, 
                    X + x, Y + (i - _topRow + 1) * y);
            
            if (IsControl)
                Graphics.RenderCursor(g, X + cx, Y + cy + (_option - _topRow) * y);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            switch (EquipTop.Instance.Option)
            {
                case 0:
                    Update(ItemType.Weapon);
                    break;
                case 1:
                    Update(ItemType.Armor);
                    break;
                case 2:
                    Update(ItemType.Accessory);
                    break;
            }
            if (_equipment.Count == 0)
                MenuScreen.EquipScreen.ChangeToDefaultControl();
        }
        public override void SetNotControl()
        {
            base.SetNotControl();
            _equipment.Clear();
            _option = 0;
            _topRow = 0;
        }

        private void Update(ItemType it)
        {
            switch (it)
            {
                case ItemType.Weapon:
                    _equipment = Inventory.GetWeaponsOfType(Selected.Weapon.Wielder);
                    break;
                case ItemType.Armor:
                    _equipment = Inventory.GetArmor(Selected.Sex);
                    break;
                case ItemType.Accessory:
                    _equipment = Inventory.GetAccessories();
                    break;
                default: break;
            }
        }

        public IItem Selection { get { return _equipment[_option].Item; } }

        public override string Info
        { get { return Selection.Description; } }
    }




}