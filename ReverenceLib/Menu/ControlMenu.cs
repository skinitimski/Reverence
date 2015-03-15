using System;

namespace Atmosphere.Reverence.Menu
{
    public abstract class ControlMenu : Menu, IController
    {
        protected ControlMenu(int x, int y, int w, int h) : this(x, y, w, h, true) { }
        protected ControlMenu(int x, int y, int w, int h, bool opaque)
        : base(x, y, w, h, opaque) { }
        
        public abstract void ControlHandle(Key k);
        
        public bool IsControl { get; private set; }
        
        public virtual void Reset() { }
        
        public virtual void SetAsControl() 
        {
            IsControl = true;
        }
        public virtual void SetNotControl()
        {
            IsControl = false;
        }
        
        public abstract string Info { get; }
    }
}

