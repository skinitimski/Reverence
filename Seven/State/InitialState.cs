using System;

using GameState = Atmosphere.Reverence.State;

namespace Atmosphere.Reverence.Seven.State
{
    internal class InitialState : GameState
    {
        public InitialState()
        {
        }





        protected override void InternalInit()
        {
        }
        
        public override void Draw(Gdk.Drawable d, int width, int height)
        {
        }
        
        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
        {
            switch (k)
            {
                case Key.Circle:
                    Seven.Instance.LoadSavedGame();
                    break;
                default:
                    break;
            }
        }
        [GLib.ConnectBefore()]
        public override void KeyReleaseHandle(Key k)
        {
            switch (k)
            {
                default:
                    break;
            }
        }
        
        public override void RunIteration() { }
        
        
        protected override void InternalDispose() { }
    }
}

