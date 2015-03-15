using System;

namespace Atmosphere.Reverence.Exceptions
{
    public class GameDataException : Exception
    {
        public GameDataException(string msg) : base(msg)
        {
        }

        public GameDataException(string msg, Exception inner) : base(msg, inner)
        {
        }

        public GameDataException(string msg, params object[] parts) : base(String.Format(msg, parts))
        {
        }
    }
}

