using System;

using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven.Battle.Time
{
    public class BattleClock : IClock
    {
        #region Constants

        protected const bool DEFAULT_START = true;
        public const int TICKS_PER_GAME_TICK = 100;
        
        #endregion Constants
        
        
        #region Member Data
        
        /// <summary>
        /// The last system tick at which this battle clock's value was checked.
        /// </summary>
        private long _lastChecked;
        
        /// <summary>
        /// The accumulated clock value.
        /// </summary>
        private long _value;

        /// <summary>
        /// How much this clock's value increases with each game tick.
        /// </summary>
        private long _increasePerGameTick;
        
        /// <summary>
        /// Value indicating whether or not this clock is paused.
        /// </summary>
        private bool _isPaused;

        /// <summary>
        /// Sync object for locking purposes.
        /// </summary>
        private object _sync = new Object();
        
        #endregion Member Data
        
        
        public BattleClock(long increasePerGameTick)
            : this(increasePerGameTick, DEFAULT_START)
        {
        }
        
        public BattleClock(long increasePerGameTick, bool start)
        {            
            _isPaused = true;
            _increasePerGameTick = increasePerGameTick;
            
            if (start)
            {
                Unpause();
            }
        }
        
        
        
        
        #region Methods
        
        /// <summary>
        /// Gets the elapsed time of the current session
        /// </summary>
        private void UpdateValue()
        {
            lock (_sync)
            {
                long lastCheckedTemp = _lastChecked;
                _lastChecked = Clock.GetCurrentTime();

                long elapsedTicks = Clock.GetCurrentTime() - lastCheckedTemp;
                long elapsedGameTicks = elapsedTicks / TICKS_PER_GAME_TICK;

                _value += elapsedGameTicks * _increasePerGameTick;
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
                    UpdateValue();
                    _isPaused = true;
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
                if (_isPaused)
                {
                    _lastChecked = Clock.GetCurrentTime();
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
            
            _value = 0;
            
            return restart ? Unpause() : restart;
        }
        
        public override string ToString()
        {
            return Value.ToString();
        }
        
        #endregion Methods
        
        
        
        #region Properties
        
        /// <summary>
        /// Returns true if this <see cref="BattleClock" /> is currently paused; returns false otherwise
        /// </summary>
        public bool IsPaused
        {
            get
            { 
                lock (_sync)
                {
                    return _isPaused;
                }
            }
        }
        
        /// <summary>
        /// Gets the (total) elapsed time in ms (scaled).
        /// </summary>
        public long Value
        {
            get
            {
                lock (_sync)
                {
                    UpdateValue();
                    return _value;
                }
            }
        }
        
        #endregion Properties

    }
}

