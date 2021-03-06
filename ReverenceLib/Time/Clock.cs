using System;

namespace Atmosphere.Reverence.Time
{
    public class Clock : IClock
    {
        #region Constants

        protected const bool DEFAULT_START = true;
        protected const int DEFAULT_ELAPSED = 0;

        public const int TICKS_PER_MS = 10000;
        
        #endregion Constants
        
        
        #region Member Data
        
        /// <summary>
        /// Clock starting epoch (in ticks).
        /// </summary>
        private long _start;
        
        /// <summary>
        /// How much time has elapsed (in ticks).
        /// </summary>
        private long _elapsed_hold;
        
        /// <summary>
        /// Value indicating whether or not this clock is paused.
        /// </summary>
        private bool _isPaused;

        private object _sync = new Object();
        
        #endregion Member Data
        
        
        /// <summary>
        /// Creates a standard running clock.
        /// </summary>
        public Clock()
            : this(DEFAULT_START)
        {
        }

        public Clock(bool start)
            : this(DEFAULT_ELAPSED, start)
        {
        }

        public Clock(int elapsed)
            : this(elapsed, DEFAULT_START)
        {
        }

        public Clock(int elapsed, bool start)
        {            
            _isPaused = true;
            _elapsed_hold = (long)elapsed * TICKS_PER_MS;
            
            if (start)
            {
                Unpause();
            }
        }
        
        
        
        
        #region Methods
        
        public static long GetCurrentTime()
        {
            return DateTime.Now.Ticks;
        }
        
        /// <summary>
        /// Gets the elapsed time of the current session
        /// </summary>
        protected long GetElapsedTicks()
        {
            lock (_sync)
            {
                return GetCurrentTime() - _start;
            }
        }
        
        /// <summary>
        /// Sums up the total elapsed time in ticks.
        /// </summary>
        protected long GetTotalElapsedTicks()
        {
            lock (_sync)
            {
                long ticks = _elapsed_hold;
            
                if (!_isPaused)
                {
                    ticks += GetElapsedTicks();
                }
            
                return ticks;
            }
        }
        
        /// <summary>
        /// Pauses this timer (if it is not paused). Returns true upon success.
        /// </summary>
        public bool Pause()
        {
            bool ret = false;
            
            lock (_sync)
            {
                if (!_isPaused)
                {
                    _isPaused = true;
                    _elapsed_hold += GetCurrentTime() - _start;
                    ret = true;
                }
            }
            
            return ret;
        }
        
        /// <summary>
        /// Unpauses this timer (if it is paused). Returns true upon success.
        /// </summary>
        public bool Unpause()
        {
            bool ret = false;

            lock (_sync)
            {            
                long current = GetCurrentTime();

                if (_isPaused)
                {
                    _start = current;
                    _isPaused = false;
                    ret = true;
                }
            }
            
            return ret;
        }


        
        /// <summary>
        /// Resets this clock and restarts it.
        /// </summary>
        public bool Reset()
        {
            return Reset(true);
        }
        
        /// <summary>
        /// Resets this clock and optionally restarts it.
        /// </summary>
        public bool Reset(bool restart)
        {
            Pause();

            _elapsed_hold = 0;
            
            return restart ? Unpause() : restart;
        }

        public override string ToString()
        {
            return String.Format("{0:D2}:{1:D2}:{2:D2}.{3}", Hours, Minutes, Seconds, Milliseconds);
        }
                
        #endregion Methods
        
        
        
        #region Properties
        
        /// <summary>
        /// Returns true if this <see cref="Clock" /> is currently paused; returns false otherwise
        /// </summary>
        public bool IsPaused
        {
            get { return _isPaused; }
        }
        
        /// <summary>
        /// Gets the (total) elapsed time in ms (scaled).
        /// </summary>
        public long TotalMilliseconds
        {
            get { return GetTotalElapsedTicks() / TICKS_PER_MS; }
        }
        
        /// <summary>
        /// Returns the number of seconds (less the seconds, minutes, and hours) that have elapsed
        /// </summary>
        public long Milliseconds
        {
            get
            {
                long e = (TotalMilliseconds % 3600000) % 60000 % 1000;
                
                return e;
            }
        }
        
        /// <summary>
        /// Returns the number of seconds (less the minutes and hours) that have elapsed
        /// </summary>
        public long Seconds
        {
            get
            {
                long e = (TotalMilliseconds % 3600000) % 60000;
                
                return e / 1000;
            }
        }
        
        /// <summary>
        /// Returns the number of minutes (less the hours) that have elapsed
        /// </summary>
        public long Minutes
        {
            get
            {
                long e = TotalMilliseconds % 3600000;
                
                return e / 60000;
            }
        }
        
        /// <summary>
        /// Returns the number of hours that have elapsed
        /// </summary>
        public long Hours
        {
            get
            {
                return TotalMilliseconds / 3600000;
            }
        }
        
        #endregion Properties
        
    }
}

