using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;

namespace Atmosphere.Reverence.Seven.Screen.InitialState
{  
    internal sealed class Prompt : ControlMenu
    {
        #region Layout
        
        const int x0 = 55; // yes
        const int x1 = 26; // question
        const int x2 = 180; // no
        const int y0 = 50;
        const int y1 = 100;
        const int cx = 15;
        const int cy = 15;
        
        
        #endregion Layout
        
        private int option = 0;
        
        public Prompt()
            : base(265, 200, 270, 150)
        {
            Visible = false;
        }

        public override void ControlHandle(Key k)
        {
            switch (k)
            {
                case Key.Left:
                    if (option > 0) option--;
                    break;
                case Key.Right:
                    if (option < 1) option++;
                    break;
                case Key.Circle:
                    switch (option)
                    {
                        case 0:
                            Seven.Instance.LoadSavedGame();
                            break;
                        case 1:
                            // No
                            break;
                        default: break;
                    }
                    break;
                default: break;
            }
        }
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            switch (option)
            {
                case 0:
                    Shapes.RenderCursor(g, X + x0 - cx, Y + y1 - cy); 
                    break;
                case 1:
                    Shapes.RenderCursor(g, X + x2 - cx, Y + y1 - cy); 
                    break;
                default: break;
            }

            Text.ShadowedText(g, "Start new game?", X + x1, Y + y0);
            Text.ShadowedText(g, "Yes", X + x0, Y + y1);
            Text.ShadowedText(g, "No", X + x2, Y + y1);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public override void SetAsControl()
        {
            base.SetAsControl();
            option = 1;
            Visible = true;
        }
        
        public override void SetNotControl()
        {
            base.SetNotControl();
            Visible = false;
        }
        
        
        public override string Info
        { get { return ""; } }
        
    }
}

