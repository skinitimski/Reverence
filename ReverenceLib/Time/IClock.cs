using System;

namespace Atmosphere.Reverence.Time
{
    public interface IClock
    {
        bool Pause();
        bool Unpause();
    }
}

