using System;

namespace Atmosphere.Reverence.Time
{
    public class Timer : Clock
    {
        
        #region Member Data

        /// <summary>
        /// Timeout in ticks.
        /// </summary>
        protected long _timeout;

        protected long _timeoutMs;
        
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
            : this(timeout, TICKS_PER_MS, start)
        {
        }
        
        public Timer(int timeout, int ticksPerMs)
            : this(timeout, ticksPerMs, DEFAULT_START)
        {
        }
        
        public Timer(int timeout, int ticksPerMs, bool start) 
            : this(timeout, ticksPerMs, DEFAULT_ELAPSED, start)
        {
        }
        
        /// <summary>
        /// Returns an optionally running timer with specified timeout (in ms) and elapsed time (in ms).
        /// </summary>
        public Timer(int timeout, int ticksPerMs, int elapsed, bool start)
            : base(ticksPerMs, elapsed, start)
        {
            _timeoutMs = timeout;
            _timeout = timeout * TICKS_PER_MS;
        }
        
        #endregion Constructors
        
        
        
        
        #region Properties


        /// <summary>
        /// Returns true if this Timer is over its timeout limit; returns false otherwise.
        /// </summary>
        public bool IsUp
        {
            get { return GetTotalElapsedTicks() > _timeout; }
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
                    pe = (int)((GetTotalElapsedTicks() * 100L) / _timeout);
                }

                return pe;
            }
        }
        
        #endregion Properties
    }
}

