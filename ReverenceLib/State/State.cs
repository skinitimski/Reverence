using System;

namespace Atmosphere.Reverence.State
{
    public abstract class State : IDisposable
    {
        private bool _initialized = false;
        
        public void Init()
        {
            if (_initialized)
                return;
            _initialized = true;
            InternalInit();
        }
        
        protected abstract void InternalInit();
        
        [GLib.ConnectBefore()]
        public abstract void KeyPressHandle(Key k);
        [GLib.ConnectBefore()]
        public abstract void KeyReleaseHandle(Key k);
        
        public abstract void Draw(Gdk.Drawable d);
        
        public abstract void RunIteration();
        
        
        
        
        public void Dispose()
        {
            InternalDispose();
            _initialized = false;
        }
        
        protected abstract void InternalDispose();
    }
}

