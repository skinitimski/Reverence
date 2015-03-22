using System;

using GameState = Atmosphere.Reverence.State;
using Atmosphere.Reverence.Seven.Screen.InitialState;

namespace Atmosphere.Reverence.Seven.State
{
    internal class InitialState : GameState
    {
        public InitialState()
        {
        }





        protected override void InternalInit()
        {
            Screen = new Prompt();
        }
        
        public override void Draw(Gdk.Drawable d, int width, int height)
        {
            Screen.Draw(d);
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
            Screen.ControlHandle(k);
        }
        
        public override void RunIteration() { }
        
        
        protected override void InternalDispose() { }

        private Prompt Screen { get; set; }
    }
}

