using System;

namespace Atmosphere.Reverence.Menu
{
    public abstract class ControlMenu : Menu, IController
    {
        protected bool _isControl = false;
        
        protected ControlMenu(int x, int y, int w, int h) : this(x, y, w, h, true) { }
        protected ControlMenu(int x, int y, int w, int h, bool opaque)
        : base(x, y, w, h, opaque) { }
        
        public abstract void ControlHandle(Key k);
        
        public bool IsControl { get { return _isControl; } }
        
        public virtual void Reset() { }
        
        public virtual void SetAsControl() 
        {
            _isControl = true;
        }
        public virtual void SetNotControl()
        {
            _isControl = false;
        }
        
        public abstract string Info { get; }
    }
}

