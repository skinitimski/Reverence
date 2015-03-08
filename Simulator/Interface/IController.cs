using System;

namespace Atmosphere.BattleSimulator
{
    public interface IController
    {
        void ControlHandle(Key k);
        void Draw(Gdk.Drawable d);

        void SetAsControl();
        void SetNotControl();
        void Reset();

        string Info { get; }

        bool IsControl { get; }
    }
}