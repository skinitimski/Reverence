using System;

namespace Atmosphere.Reverence.Exceptions
{
    public class ImplementationException : Exception
    {
        public ImplementationException(string msg) : base(msg)
        {
        }

        public ImplementationException(string msg, Exception inner) : base(msg, inner)
        {
        }

        public ImplementationException(string msg, params object[] parts) : base(String.Format(msg, parts))
        {
        }
    }
}

