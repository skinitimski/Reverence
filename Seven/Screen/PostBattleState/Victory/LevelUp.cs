using System;
using Cairo;

using Atmosphere.Reverence.Graphics;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using ScreenState = Atmosphere.Reverence.Menu.ScreenState;
using State = Atmosphere.Reverence.Seven.State;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{
    internal class LevelUp : GameMenu
    {
        const int xs = 20;        
        const int ys = 28;

        const int width = 200;
        const int height = 48;




        public LevelUp(ScreenState screenState, int partyIndex)
            : base(60, 
                   (partyIndex * (screenState.Height / 4 - 6)) + 190, 
                   width, 
                   height)
        {
        }

        protected override void DrawContents(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);
            
            Text.ShadowedText(g, "Level Up!", X + xs, Y + ys);
        }

        public void Show()
        {
            AnimationTimer = new Timer(State.PostBattleState.MS_PER_BAR_FILL / 3);

            Visible = true;
        }
        
        private Timer AnimationTimer { get; set; }
        
        public bool IsDone { get { return AnimationTimer.IsUp; } }
    }
}

