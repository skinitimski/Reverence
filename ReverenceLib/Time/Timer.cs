using System;

namespace Atmosphere.Reverence.Time
{
    public class Timer : Clock
    {
        
        #region Member Data

        protected long _timeoutMs;
        protected long _timeoutTicks;
        
        #endregion Member Data
        
        
        
        #region Constructors
        
        /// <summary>
        /// Returns a running timer with specified timeout (in ms)
        /// </summary>
        public Timer(int timeout)
            : this(timeout, DEFAULT_START)
        {
        }
        
        public Timer(int timeout, bool start)
            : this(timeout, DEFAULT_ELAPSED, start)
        {
        }
        
        public Timer(int timeout, int elapsed)
            : this(timeout, elapsed, DEFAULT_START)
        {
        }
        
        /// <summary>
        /// Returns an optionally running timer with specified timeout (in ms) and elapsed time (in ms).
        /// </summary>
        public Timer(int timeout, int elapsed, bool start)
            : base(elapsed, start)
        {
            _timeoutMs = timeout;
            _timeoutTicks = _timeoutMs * TICKS_PER_MS;
        }


        
        #endregion Constructors
        
        
        
        
        #region Properties


        /// <summary>
        /// Returns true if this Timer is over its timeout limit; returns false otherwise.
        /// </summary>
        public bool IsUp
        {
            get { return GetTotalElapsedTicks() > _timeoutTicks; }
        }

        /// <summary>
        /// Gets the timeout in ms.
        /// </summary>
        public long Timeout { get { return _timeoutMs; } }

        public int PercentElapsed
        {
            get
            {
                int pe = 0;

                if (IsUp)
                {
                    pe = 100;
                }
                else
                {
                    pe = (int)((GetTotalElapsedTicks() * 100L) / _timeoutTicks);
                }

                return pe;
            }
        }
        
        #endregion Properties
    }
}

