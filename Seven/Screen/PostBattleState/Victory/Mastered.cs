using System;
using System.Collections.Generic;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Seven.Asset.Materia;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using ScreenState = Atmosphere.Reverence.Menu.ScreenState;
using State = Atmosphere.Reverence.Seven.State;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory
{
    internal class Mastered : GameMenu
    {
        const int xs = 20;        
        const int ys_0 = 28;      
        const int ys_1 = 52;

        const int width = 250;
        const int height = 70;

        const int MS_PER_MASTER = 2000;


        public Mastered(ScreenState state)
            : base((state.Width / 2) - (width / 2), 
                   10, 
                   width, 
                   height)
        {
        }

        protected override void DrawContents(Gdk.Drawable d)
        {
            Cairo.Context g = Gdk.CairoHelper.Create(d);
            
            g.SelectFontFace(Text.MONOSPACE_FONT, FontSlant.Normal, FontWeight.Bold);
            g.SetFontSize(24);

            if (ActiveMateria != null)
            {
                Text.ShadowedText(g, ActiveMateria.Name, X + xs, Y + ys_0);
            }

            Text.ShadowedText(g, "  was born", X + xs, Y + ys_1);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }

        public void Show(List<MateriaOrb> newMasters)
        {          
            NewMasters = newMasters;

            AnimationTimer = new Timer(MS_PER_MASTER * newMasters.Count);

            Visible = true;
        }
        
        public bool IsDone { get { return AnimationTimer.IsUp; } }
        
        private Timer AnimationTimer { get; set; }

        private List<MateriaOrb> NewMasters { get; set; }

        private MateriaOrb ActiveMateria
        {
            get
            {
                MateriaOrb active = null;

                if (NewMasters != null && NewMasters.Count > 0)
                {
                    int index = (int)AnimationTimer.TotalMilliseconds / MS_PER_MASTER;

                    if (index > NewMasters.Count)
                    {
                        index = NewMasters.Count - 1;
                    }

                    active = NewMasters[index];
                }

                return active;
            }
        }
    }
}

