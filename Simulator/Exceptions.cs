using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public class SavegameException : Exception
    {
        public SavegameException(string msg) : base(msg) { }
        public SavegameException(string msg, Exception inner) : base(msg, inner) { }
    }
    public class GamedataException : Exception
    {
        public GamedataException(string msg) : base(msg) { }
        public GamedataException(string msg, Exception inner) : base(msg, inner) { }
    }
    public class GameImplementationException : Exception
    {
        public GameImplementationException(string msg) : base(msg) { }
        public GameImplementationException(string msg, Exception inner) : base(msg, inner) { }
    }
    public class WTFException : GameImplementationException
    {
        public WTFException(string msg) : base(msg) { }
        public WTFException(string msg, Exception inner) : base(msg, inner) { }
    }
}