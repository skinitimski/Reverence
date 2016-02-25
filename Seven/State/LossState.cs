using System;

using Atmosphere.Reverence.Seven.Screen.LossState;

namespace Atmosphere.Reverence.Seven.State
{
    internal class LossState : State
    {
        public LossState(Seven seven)
            : base(seven)
        {
        }
        
        
        
        
        
        protected override void InternalInit()
        {
            Screen = new Prompt(Seven);
        }
        
        public override void Draw(Gdk.Drawable d, int width, int height, bool screenChanged)
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
        }
        
        public override void RunIteration() { }
        
        
        protected override void InternalDispose() { }
        
        private Prompt Screen { get; set; }
    }
}

