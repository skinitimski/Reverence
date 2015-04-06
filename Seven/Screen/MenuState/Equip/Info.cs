using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence;

namespace Atmosphere.Reverence.Seven.Screen.MenuState.Equip
{
    internal sealed class Info : Menu.Menu
    {
        public Info()
            : base(
                2,
                Seven.Config.WindowHeight * 7 / 20,
                Seven.Config.WindowWidth - 10,
                Seven.Config.WindowHeight / 15)
        {
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, Seven.MenuState.EquipScreen.Control.Info, X + 20, Y + 27);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
    }
}

