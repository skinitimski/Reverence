using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Phs
{      
    internal sealed class Stats : ControlMenu
    {
        #region Layout
        
        const int x1 = 30; // pic
        const int x3 = 170; // name
        const int x4 = x3 + 65; // lvl
        const int x5 = x4 + 42; // hp
        const int x6 = x5 + 65; // hpm
        const int x7 = x3 + 110; // fury/sadness
        const int cx = 15;
        const int yp = 15;
        
        const int y0 = 60;
        const int y1 = 210;
        const int y2 = 360;
        const int ya = 30;
        const int yb = 55;
        const int yc = 80;
        
        #endregion Layout
        
        private int option;
        private int cy = y0;

        public Stats()
            : base(
                2,
                Config.Instance.WindowHeight * 7 / 60,
                Config.Instance.WindowWidth / 2 - 6,
                Config.Instance.WindowHeight * 5 / 6)
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
                    Seven.MenuState.ChangeScreen(Seven.MenuState.MainScreen);
                    break;
                case Key.Circle:
                    Seven.MenuState.PhsScreen.ChangeControl(Seven.MenuState.PhsList);
                    break;
            }
            switch (option)
            {
                case 0: cy = y0; break;
                case 1: cy = y1; break;
                case 2: cy = y2; break;
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
            
            if (Seven.Party [0] != null)
            {
                d.DrawPixbuf(gc, Seven.Party [0].Profile, 0, 0,
                             X + x1, Y + yp,
                             Character.PROFILE_WIDTH, Character.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);
                
                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y0 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y0 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y0 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);
                
                Color namec = new Color(1, 1, 1);
                if (Seven.Party [0].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Seven.Party [0].NearDeath)
                    namec = new Color(.8, .8, 0);
                
                Text.ShadowedText(g, namec, Seven.Party [0].Name, X + x3, Y + y0);

                lvl = Seven.Party [0].Level.ToString();
                hp = Seven.Party [0].HP.ToString() + "/";
                hpm = Seven.Party [0].MaxHP.ToString();
                mp = Seven.Party [0].MP.ToString() + "/";
                mpm = Seven.Party [0].MaxMP.ToString();
                
                te = g.TextExtents(lvl);
                Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + y0 + ya);
                te = g.TextExtents(hp);
                Text.ShadowedText(g, hp, X + x5 - te.Width, Y + y0 + yb);
                te = g.TextExtents(hpm);
                Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + y0 + yb);
                te = g.TextExtents(mp);
                Text.ShadowedText(g, mp, X + x5 - te.Width, Y + y0 + yc);
                te = g.TextExtents(mpm);
                Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + y0 + yc);
            }

            #endregion Character 1
            
            
            #region Character 2
            
            if (Seven.Party [1] != null)
            {
                d.DrawPixbuf(gc, Seven.Party [1].Profile, 0, 0,
                             X + x1, Y + yp + (y1 - y0),
                             Character.PROFILE_WIDTH, Character.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);
                
                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y1 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y1 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y1 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);
                
                Color namec = new Color(1, 1, 1);
                if (Seven.Party [1].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Seven.Party [1].NearDeath)
                    namec = new Color(.8, .8, 0);
                
                Text.ShadowedText(g, namec, Seven.Party [1].Name, X + x3, Y + y1);
                
                lvl = Seven.Party [1].Level.ToString();
                hp = Seven.Party [1].HP.ToString() + "/";
                hpm = Seven.Party [1].MaxHP.ToString();
                mp = Seven.Party [1].MP.ToString() + "/";
                mpm = Seven.Party [1].MaxMP.ToString();

                te = g.TextExtents(lvl);
                Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + y1 + ya);
                te = g.TextExtents(hp);
                Text.ShadowedText(g, hp, X + x5 - te.Width, Y + y1 + yb);
                te = g.TextExtents(hpm);
                Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + y1 + yb);
                te = g.TextExtents(mp);
                Text.ShadowedText(g, mp, X + x5 - te.Width, Y + y1 + yc);
                te = g.TextExtents(mpm);
                Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + y1 + yc);
            }
            
            #endregion Character 2
            
            
            #region Character 3
            
            if (Seven.Party [2] != null)
            {
                d.DrawPixbuf(gc, Seven.Party [2].Profile, 0, 0,
                             X + x1, Y + yp + (y2 - y0),
                             Character.PROFILE_WIDTH, Character.PROFILE_HEIGHT, Gdk.RgbDither.None, 0, 0);
                
                g.Color = new Color(.3, .8, .8);
                g.MoveTo(X + x3, Y + y2 + ya);
                g.ShowText("LV");
                g.MoveTo(X + x3, Y + y2 + yb);
                g.ShowText("HP");
                g.MoveTo(X + x3, Y + y2 + yc);
                g.ShowText("MP");
                g.Color = new Color(1, 1, 1);
                
                Color namec = new Color(1, 1, 1);
                if (Seven.Party [2].Death)
                    namec = new Color(0.8, 0, 0);
                else if (Seven.Party [2].NearDeath)
                    namec = new Color(.8, .8, 0);
                
                Text.ShadowedText(g, namec, Seven.Party [2].Name, X + x3, Y + y2);
                
                lvl = Seven.Party [2].Level.ToString();
                hp = Seven.Party [2].HP.ToString() + "/";
                hpm = Seven.Party [2].MaxHP.ToString();
                mp = Seven.Party [2].MP.ToString() + "/";
                mpm = Seven.Party [2].MaxMP.ToString();
                
                te = g.TextExtents(lvl);
                Text.ShadowedText(g, lvl, X + x4 - te.Width, Y + y2 + ya);
                te = g.TextExtents(hp);
                Text.ShadowedText(g, hp, X + x5 - te.Width, Y + y2 + yb);
                te = g.TextExtents(hpm);
                Text.ShadowedText(g, hpm, X + x6 - te.Width, Y + y2 + yb);
                te = g.TextExtents(mp);
                Text.ShadowedText(g, mp, X + x5 - te.Width, Y + y2 + yc);
                te = g.TextExtents(mpm);
                Text.ShadowedText(g, mpm, X + x6 - te.Width, Y + y2 + yc);
            }
            
            #endregion Character 3
            
            
            if (IsControl)
            {
                Shapes.RenderCursor(g, X + cx, Y + cy);
            }
            else
            {
                Shapes.RenderBlinkingCursor(g, X + cx, Y + cy);
            }
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            Seven.MenuState.PhsList.Update();
        }
        
        public int Option { get { return option; } }
        
        public override string Info
        {  get { return ""; } }
    }

}

