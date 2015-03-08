using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Cairo;

namespace Atmosphere.BattleSimulator
{
    internal sealed class MainStatus : ControlMenu
    {
        public static MainStatus Instance;

        #region Layout

        const int x1 = 30; // pic
        const int x2 = 60; // pic back row
        const int x3 = 200; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = x3 + 110; // fury/sadness
        const int cx = 18; 

        const int y0 = 50; // row 0
        const int y1 = y0 + 150; // row 1 
        const int y2 = y1 + 150; // row 2
        const int ya = 30; // subrow 1
        const int yb = 55; // subrow 2
        const int yc = 80; // subrow 3
        const int yp = 15; // ypic

        private int option = 0;
        private int option_hold = -1;
        private int cy = y0;
        private int cy_h = 0;

        #endregion

        static MainStatus()
        {
            Instance = new MainStatus();
        }
        private MainStatus()
            : base(
                2,
                Globals.HEIGHT / 9,
                Globals.WIDTH * 4 / 5,
                Globals.HEIGHT * 7 / 9)
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
                    MenuScreen.MainScreen.ChangeControl(MainOptions.Instance);
                    break;
                case Key.Circle:
                    switch (MainOptions.Instance.Option)
                    {
                        case 1: // Magic
                            if (Globals.Party[option] == null)
                                break;
                            //MenuState.MainMenu.ChangeScreen(Screen._magicScreen);
                            break;
                        case 2: // Materia
                            if (Globals.Party[option] == null)
                                break;
                            Game.MainMenu.ChangeScreen(MenuScreen.MateriaScreen);
                            break;
                        case 3: // Equip
                            if (Globals.Party[option] == null)
                                break;
                            Game.MainMenu.ChangeScreen(MenuScreen.EquipScreen);
                            break;
                        case 4: // Status
                            if (Globals.Party[option] == null)
                                break;
                            Game.MainMenu.ChangeScreen(MenuScreen.StatusScreen);
                            break;
                        case 5: // Order
                            if (option_hold != -1)
                            {
                                if (option_hold == option)
                                {
                                    Globals.Party[option].BackRow = !Globals.Party[option].BackRow;
                                    option_hold = -1;
                                }
                                else
                                {
                                    Character temp = Globals.Party[option];
                                    Globals.Party[option] = Globals.Party[option_hold];
                                    Globals.Party[option_hold] = temp;
                                    option_hold = -1;
                                }
                            }
                            else
                                option_hold = option;
                            break;
                        case 6: // Limit
                            if (Globals.Party[option] == null)
                                break;
                            //MenuState.MainMenu.ChangeLayer(Layer._limitLayer);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            switch (option)
            {
                case 0: cy = y0; break;
                case 1: cy = y1; break;
                case 2: cy = y2; break;
                default: break;
            }
            switch (option_hold)
            {
                case 0: cy_h = y0; break;
                case 1: cy_h = y1; break;
                case 2: cy_h = y2; break;
                default: break;
            } 
            
            Menu.SetSelection(Globals.Party[option]); 
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
                    X + (Globals.Party[0].BackRow ? x2 : x1), Y + yp, 
                    Graphics.PROFILE_WIDTH, Graphics.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);


                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y0 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y0 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y0 + yc);
                g.ShowText("MP");

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
                Graphics.ShadowedText(g, lvl,
                    X + x4 - te.Width,
                    Y + y0 + ya);

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp,
                    X + x5 - te.Width,
                    Y + y0 + yb);

                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm,
                    X + x6 - te.Width,
                    Y + y0 + yb);

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp,
                    X + x5 - te.Width,
                    Y + y0 + yc);

                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm,
                    X + x6 - te.Width,
                    Y + y0 + yc);
            }
            #endregion Character 1


            #region Character 2

            if (Globals.Party[1] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[1].Name), 0, 0,
                    X + (Globals.Party[1].BackRow ? x2 : x1), Y + yp + (y1 - y0),
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
                Graphics.ShadowedText(g, lvl,
                    X + x4 - te.Width,
                    Y + y1 + ya);

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp,
                    X + x5 - te.Width,
                    Y + y1 + yb);

                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm,
                    X + x6 - te.Width,
                    Y + y1 + yb);

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp,
                    X + x5 - te.Width,
                    Y + y1 + yc);

                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm,
                    X + x6 - te.Width,
                    Y + y1 + yc);
            }

            #endregion Character 2


            #region Character 3

            if (Globals.Party[2] != null)
            {
                d.DrawPixbuf(gc, Graphics.GetProfile(Globals.Party[2].Name), 0, 0,
                    X + (Globals.Party[2].BackRow ? x2 : x1), Y + yp + (y2 - y0),
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

                Graphics.ShadowedText(g, namec, Globals.Party[2].Name,
                    X + x3,
                    Y + y2);

                lvl = Globals.Party[2].Level.ToString();
                hp = Globals.Party[2].HP.ToString() + "/";
                hpm = Globals.Party[2].MaxHP.ToString();
                mp = Globals.Party[2].MP.ToString() + "/";
                mpm = Globals.Party[2].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Graphics.ShadowedText(g, lvl,
                    X + x4 - te.Width,
                    Y + y2 + ya);

                te = g.TextExtents(hp);
                Graphics.ShadowedText(g, hp,
                    X + x5 - te.Width,
                    Y + y2 + yb);

                te = g.TextExtents(hpm);
                Graphics.ShadowedText(g, hpm,
                    X + x6 - te.Width,
                    Y + y2 + yb);

                te = g.TextExtents(mp);
                Graphics.ShadowedText(g, mp,
                    X + x5 - te.Width,
                    Y + y2 + yc);

                te = g.TextExtents(mpm);
                Graphics.ShadowedText(g, mpm,
                    X + x6 - te.Width,
                    Y + y2 + yc);
            }

            #endregion Character 3


            if (IsControl)
            {
                if (option_hold != -1)
                    Graphics.RenderCursor(g, new Color(.8, .8, .8), X + cx, Y + cy_h);
                Graphics.RenderCursor(g, X + cx, Y + cy);
            }

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public override void SetAsControl()
        {
            base.SetAsControl();
            Menu.SetSelection(Globals.Party[option]);
        }

        public override string Info
        { get { return ""; } }
    }




    internal sealed class MainOptions : ControlMenu
    {
        public static MainOptions Instance;

        #region Layout

        const int x = 35;
        const int y = 30;
        const int cx = 20;
        const int cy = 28;

        #endregion Layout

        private int option = 0;

        static MainOptions()
        {
            Instance = new MainOptions();
        }
        private MainOptions()
            : base(
                Globals.WIDTH * 3 / 4 - 10,
                Globals.HEIGHT / 20,
                Globals.WIDTH / 4,
                Globals.HEIGHT * 11 / 20)
        { }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Up:
                    if (option > 0) option--;
                    else option = 9;
                    break;
                case Key.Down:
                    if (option < 9) option++;
                    else option = 0;
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case 0: // Item
                            Game.MainMenu.ChangeScreen(MenuScreen.ItemScreen);
                            break;
                        //case 1: // Magic
                        case 2: // Materia
                        case 3: // Equip
                        case 4: // Status
                        case 5: // Order
                        //case 6: // Limit
                            MenuScreen.MainScreen.ChangeControl(MainStatus.Instance);
                            break;
                        case 7: // Config
                            //MenuState.MainMenu.ChangeLayer(Screen._configScreen);
                            break;
                        case 8: // PHS
                            Game.MainMenu.ChangeScreen(MenuScreen.PhsScreen);
                            break;
                        case 9: // Save
                            //MenuState.MainMenu.ChangeLayer(Screen._saveScreen);
                            break;
                        default:
                            break;
                    }
                    break;
                case Key.Square:
                    Game.GoToBattleState();
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
                Graphics.RenderCursor(g, X + cx, Y + cy + (option * y));
            

            #region Menu Options
            // Option 0
            Graphics.ShadowedText(g, "Item",
                X + x,
                Y + (1 * y) + 5);
            // Option 1
            Graphics.ShadowedText(g, "Magic",
                X + x,
                Y + (2 * y) + 5);
            // Option 2
            Graphics.ShadowedText(g, "Materia",
                X + x,
                Y + (3 * y) + 5);
            // Option 3
            Graphics.ShadowedText(g, "Equip",
                X + x,
                Y + (4 * y) + 5);
            // Option 4
            Graphics.ShadowedText(g, "Status",
                X + x,
                Y + (5 * y) + 5);
            // Option 5
            Graphics.ShadowedText(g, "Order",
                X + x,
                Y + (6 * y) + 5);
            // Option 6
            Graphics.ShadowedText(g, "Limit",
                X + x,
                Y + (7 * y) + 5);
            // Option 7
            Graphics.ShadowedText(g, "Config",
                X + x,
                Y + (8 * y) + 5);
            // Option 8
            Graphics.ShadowedText(g, "PHS",
                X + x,
                Y + (9 * y) + 5);
            // Option 9
            Graphics.ShadowedText(g, "Save",
                X + x,
                Y + (10 * y) + 5);
            #endregion


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public int Option { get { return option; } }
        public override string Info
        { get { return ""; } }
    }




    internal sealed class MainTime : Menu
    {
        public static MainTime Instance;

        #region Layout

        int x1 = 10;
        int x2 = Globals.WIDTH / 4 - 10;
        int y1 = 30;
        int y2 = 65;

        #endregion

        static MainTime()
        {
            Instance = new MainTime();
        }
        private MainTime()
            : base(
                Globals.WIDTH * 3 / 4 - 10,
                Globals.HEIGHT * 7 / 10,
                Globals.WIDTH / 4,
                Globals.HEIGHT * 3 / 20)
        { }


        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);


            TextExtents te;

            Graphics.ShadowedText(g, "Time", X + x1, Y + y1);
            Graphics.ShadowedText(g, "Gil", X + x1, Y + y2);

            g.SelectFontFace("Courier New", FontSlant.Normal, FontWeight.Bold);
            
            long s, m, h;
            s = Game.Clock.Seconds;
            m = Game.Clock.Minutes;
            h = Game.Clock.Hours;

            string time = String.Format("{0}{1}:{2}{3}:{4}{5}",
                h < 10 ? "0" : "", h,
                m < 10 ? "0" : "", m,
                s < 10 ? "0" : "", s);

            te = g.TextExtents(time);
            Graphics.ShadowedText(g, time, X + x2 - te.Width, Y + y1);

            string gil = Globals.Gil.ToString();
            te = g.TextExtents(gil);
            Graphics.ShadowedText(g, gil, X + x2 - te.Width, Y + y2);
            

            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }




    internal sealed class MainLocation : Menu
    {
        public static MainLocation Instance;

        #region Layout

        int x = 35;
        int y = 35;

        #endregion

        static MainLocation()
        {
            Instance = new MainLocation();
        }
        private MainLocation()
            : base(
                Globals.WIDTH / 2,
                Globals.HEIGHT * 17 / 20 + 6,
                Globals.WIDTH / 2 - 10,
                Globals.HEIGHT / 10)
        { }


        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);

            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            

            Graphics.ShadowedText(g, "FF7 Battle Arena", X + x, Y + y);


            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
    }
}