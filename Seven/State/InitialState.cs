using System;

using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Seven.Screen.InitialState;

namespace Atmosphere.Reverence.Seven.State
{
    internal class InitialState : State
    {
        public InitialState(Seven seven)
            : base(seven)
        {
        }





        protected override void InternalInit()
        {
            ScreenState state = new ScreenState
            {
                Width = Parent.Configuration.WindowWidth,
                Height = Parent.Configuration.WindowHeight
            };

            Screen = new Prompt(this, state);
        }
        
        public override void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            Screen.Draw(d, g, width, height, screenChanged);
            Screen.Visible = true;
        }
        
        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
        {
            Screen.ControlHandle(k);
        }
        [GLib.ConnectBefore()]
        public override void KeyReleaseHandle(Key k)
        {
        }
        
        public override void RunIteration() { }
        
        
        protected override void InternalDispose() { }

        private Prompt Screen { get; set; }
    }
}

