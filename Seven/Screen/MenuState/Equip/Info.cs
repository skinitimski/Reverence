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
                Config.Instance.WindowHeight * 7 / 20,
                Config.Instance.WindowWidth - 10,
                Config.Instance.WindowHeight / 15)
        {
        }
        
        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace("Lucida Console", FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, Seven.MenuState.EquipScreen.Control.Info, X + 20, Y + 27);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
    }
}

