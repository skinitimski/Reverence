using System;

using Cairo;

using Atmosphere.Reverence.Time;

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

        protected State(Game game)
        {
            Parent = game;
        }
        
        protected abstract void InternalInit();

        public abstract void KeyPressHandle(Key k);

        public abstract void KeyReleaseHandle(Key k);
        
        public abstract void Draw(Gdk.Drawable d, int width, int height, bool screenChanged);
        
        public abstract void RunIteration();
        
        public void Dispose()
        {
            InternalDispose();
            _initialized = false;
        }
        
        protected abstract void InternalDispose();

        public Game Parent { get; private set; }
    }
}

