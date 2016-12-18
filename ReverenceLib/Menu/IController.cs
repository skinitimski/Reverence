using System;

using Gdk;

namespace Atmosphere.Reverence.Menu
{
    public interface IController
    {
        void ControlHandle(Key k);
        void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged);
        
        void SetAsControl();
        void SetNotControl();
        void Reset();
        
        string Info { get; }
        
        bool IsControl { get; }
    }
}

