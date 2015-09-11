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
    internal abstract class MateriaInfo : GameMenu
    {
        const int xs = 20;        
        const int ys_0 = 28;      
        const int ys_1 = 52;
        
        protected const int width = 250;
        protected const int height = 70;
        

        
        protected MateriaInfo(int x, int y)
            : base(x, y, width, height)
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
            
            Text.ShadowedText(g, Message, X + xs, Y + ys_1);
            
            ((IDisposable)g.Target).Dispose();
            ((IDisposable)g).Dispose();
        }
        
        public void Show(List<MateriaOrb> materiaList)
        {          
            MateriaList = materiaList;
            
            AnimationTimer = new Timer(MillisecondsPerMateria * materiaList.Count);
            
            Visible = true;
        }
        
        public bool IsDone { get { return AnimationTimer.IsUp; } }

        protected abstract int MillisecondsPerMateria { get; }

        protected abstract String Message { get; }
        
        private Timer AnimationTimer { get; set; }
        
        private List<MateriaOrb> MateriaList { get; set; }
        
        private MateriaOrb ActiveMateria
        {
            get
            {
                MateriaOrb active = null;
                                
                if (MateriaList != null && MateriaList.Count > 0)
                {
                    int index = (int)AnimationTimer.TotalMilliseconds / MillisecondsPerMateria;
                    
                    if (index >= MateriaList.Count)
                    {
                        index = MateriaList.Count - 1;
                    }
                    else if (index < 0)
                    {
                        index = 0;
                    }
                    
                    active = MateriaList[index];
                }
                
                return active;
            }
        }
    }
}

