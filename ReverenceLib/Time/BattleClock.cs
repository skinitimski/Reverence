using System;

namespace Atmosphere.Reverence.Time
{
    public class BattleClock : IClock
    {
        public const int TICKS_PER_GAME_TICK = 100000;
        public const int VALUE_PER_UNIT = 8192;


                
        /// <summary>
        /// The last system time (in ticks) that this clock was unpaused.
        /// </summary>
        private long _lastUnpause;
        
        /// <summary>
        /// The accumulated clock value prior to the last pause.
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


        
        private readonly long _increasePerGameTick_double;
        private readonly long _increasePerGameTick_normal;
        private readonly long _increasePerGameTick_half;


                
        public BattleClock(long increasePerGameTick)
            : this(increasePerGameTick, 0, true)
        {
        }

        public BattleClock(long increasePerGameTick, long elapsed, bool start)
        {            
            _isPaused = true;
            _increasePerGameTick = increasePerGameTick;
            _value = elapsed;

            // Note that we do this in case the original increase is an odd number -- if 
            //   we were to halve it we'd lose information if we tried to double it again
            _increasePerGameTick_double = _increasePerGameTick * 2;
            _increasePerGameTick_normal = _increasePerGameTick;
            _increasePerGameTick_half   = _increasePerGameTick / 2;

            if (start)
            {
                Unpause();
            }
        }
        

        
        
        #region Methods
        
        /// <summary>
        /// Gets the elapsed time of the current session
        /// </summary>
        private long GetAccumulatedValueSinceLastUnpause()
        {
            long elapsedTicks = Clock.GetCurrentTime() - _lastUnpause;
            long elapsedGameTicks = elapsedTicks / TICKS_PER_GAME_TICK;

            return elapsedGameTicks * IncreasePerGameTick;
        }
        
        /// <summary>
        /// Pauses this timer (if it is not paused). Returns true upon success.
        /// </summary>
        public bool Pause()
        {
            bool ret = false;

            if (!_isPaused)
            {
                _isPaused = true;
                _value += GetAccumulatedValueSinceLastUnpause();
                ret = true;
            }
                        
            return ret;
        }
        
        /// <summary>
        /// Unpauses this timer (if it is paused). Returns true upon success.
        /// </summary>
        public bool Unpause()
        {
            bool ret = false;
             
            if (_isPaused)
            {
                _lastUnpause = Clock.GetCurrentTime();
                _isPaused = false;
                ret = true;
            }
            
            return ret;
        }
        
        public void NormalRate()
        {
            Pause();
            _increasePerGameTick = _increasePerGameTick_normal;
            Unpause();
        }
        
        public void DoubleRate()
        {
            Pause();
            _increasePerGameTick = _increasePerGameTick_double;
            Unpause();
        }
        
        public void HalfRate()
        {
            Pause();
            _increasePerGameTick = _increasePerGameTick_half;
            Unpause();
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
            return CurrentValue.ToString();
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
                return _isPaused;
            }
        }
        
        public long CurrentValue
        {
            get
            {
                long currentValue = _value;
                
                if (!_isPaused)
                {
                    currentValue += GetAccumulatedValueSinceLastUnpause();
                }
                
                return currentValue;
            }
        }
        
        public long ElapsedUnits
        {
            get
            {
                return CurrentValue / VALUE_PER_UNIT;
            }
        }

        public virtual long IncreasePerGameTick { get { return _increasePerGameTick; } }
        
        #endregion Properties

    }
}

