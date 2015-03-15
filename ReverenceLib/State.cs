using System;

using Cairo;

namespace Atmosphere.Reverence
{
    public abstract class State : IDisposable
    {
        private bool _initialized = false;

        
        public void Init()
        {
            if (!_initialized)
            {
                _initialized = true;
                InternalInit();
            }
        }
        
        protected abstract void InternalInit();

        public abstract void KeyPressHandle(Key k);

        public abstract void KeyReleaseHandle(Key k);
        
        public abstract void Draw(Gdk.Drawable d, int width, int height);
        
        public abstract void RunIteration();
        
        
        
        
        public void Dispose()
        {
            InternalDispose();
            _initialized = false;
        }
        
        protected abstract void InternalDispose();
    }
}

