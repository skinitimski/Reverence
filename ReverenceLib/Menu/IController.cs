using System;

using Gdk;

namespace Atmosphere.Reverence.Menu
{
    public interface IController
    {
        void ControlHandle(Key k);
        void Draw(Drawable d);
        
        void SetAsControl();
        void SetNotControl();
        void Reset();
        
        string Info { get; }
        
        bool IsControl { get; }
    }
}

