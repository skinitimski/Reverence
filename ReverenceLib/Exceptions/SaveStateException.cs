using System;

namespace Atmosphere.Reverence.Exceptions
{
    public class SaveStateException : Exception
    {
        public SaveStateException(string msg) : base(msg)
        {
        }

        public SaveStateException(string msg, Exception inner) : base(msg, inner)
        {
        }

        public SaveStateException(string msg, params object[] parts) : base(String.Format(msg, parts))
        {
        }
    }
}

