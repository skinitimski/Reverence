using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Graphics;

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
                Shapes.RenderCursor(g, X + cx, Y + cy);
            }
            else
            {
                Shapes.RenderBlinkingCursor(g, X + cx, Y + cy);
            }

            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        private void DrawCharacterStatus(Gdk.Drawable d, Gdk.GC gc, Cairo.Context g, Character c, int y)
        {            
            TextExtents te;
            
            string lvl, hp, hpm, mp, mpm;

            Images.RenderProfile(d, gc, X + x1, Y + yp + y - y0, c);
                
            g.Color = COLOR_TEXT_TEAL;
            g.MoveTo(X + x3, Y + y + ya);
            g.ShowText("LV");
            g.MoveTo(X + x3, Y + y + yb);
            g.ShowText("HP");
            g.MoveTo(X + x3, Y + y + yc);
            g.ShowText("MP");
            g.Color = Colors.WHITE;
                
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

