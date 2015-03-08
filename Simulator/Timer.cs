using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Atmosphere.BattleSimulator
{
    public class Clock
    {
        #region Constants

        // to convert from 100-ns (os time) to ms (parameter time)
        protected const int CONVERSION_FACTOR = 10000;
        public const int TICKS_PER_MS = 10000;

        #endregion Constants


        #region Member Data

        /// <summary>
        /// Clock starting epoch (in ticks).
        /// </summary>
        protected long _startTime;

        /// <summary>
        /// How much time has elapsed (in ticks).
        /// </summary>
        protected long _elapsed_hold;

        /// <summary>
        /// Value indicating whether or not this clock is paused.
        /// </summary>
        protected bool _isPaused;
        private object _sync = new Object();

        #endregion Member Data


        /// <summary>
        /// Creates a running clock.
        /// </summary>
        public Clock()
        {
            _startTime = GetCurrentTime();
            _isPaused = false;

            _elapsed_hold = 0;
        }

        /// <summary>
        /// Creates a running clock with specified <paramref name="elapsed" /> time (in ms)
        /// </summary>
        public Clock(long elapsed)
            : this()
        {
            _elapsed_hold = elapsed * CONVERSION_FACTOR;
        }




        #region Methods

        protected static long GetCurrentTime()
        {
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// Sums up the elapsed time in ticks.
        /// </summary>
        protected virtual long CalculateElapsed()
        {
            if (_isPaused)
            {
                return _elapsed_hold;
            }
            else
            {
                return (_elapsed_hold + (GetCurrentTime() - _startTime));
            }
        }

        /// <summary>
        /// Pauses this timer (if it is not paused). Returns true upon success.
        /// </summary>
        public bool Pause()
        {
            bool ret = true;

            long current = GetCurrentTime();

            lock (_sync)
            {
                if (_isPaused)
                {
                    ret = false;
                }
                else
                {
                    _isPaused = true;
                    _elapsed_hold = _elapsed_hold + (current - _startTime);
                }
            }

            return ret;
        }

        /// <summary>
        /// Unpauses this timer (if it is paused). Returns true upon success.
        /// </summary>
        public bool Unpause()
        {
            bool ret = true;
            
            long current = GetCurrentTime();
            
            lock (_sync)
            {
                if (!_isPaused)
                {
                    ret = false;
                }
                else
                {
                    _startTime = current;
                    _isPaused = false;
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the elapsed time in ms.
        /// </summary>
        public long Elapsed
        {
            get { return CalculateElapsed() / CONVERSION_FACTOR; }
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
        /// Returns the number of seconds (less the minutes and hours) that have elapsed
        /// </summary>
        public long Seconds
        {
            get
            {
                long e = Elapsed;//ms
                e = (e % 3600000);
                e = (e % 60000);
                long s = e / 1000;
                return s;
            }
        }

        /// <summary>
        /// Returns the number of minutes (less the hours) that have elapsed
        /// </summary>
        public long Minutes
        {
            get
            {
                long e = Elapsed;//ms
                e = (e % 3600000);
                long m = e / 60000;
                return m;
            }
        }

        /// <summary>
        /// Returns the number of hours that have elapsed
        /// </summary>
        public long Hours
        {
            get
            {
                long e = Elapsed;//ms
                long h = e / 3600000;
                return h;
            }
        }

        #endregion Properties

    }

    public class ScaledClock : Clock
    {
        #region Member Data

        private int _ticksPerUnit;

        #endregion Member Data


        /// <summary>Constructs a new ScaledClock object</summary>
        /// <param name="ticksPerUnitTime">Number of ticks per unit time. The more ticks/time, the 
        /// lower the clock progresses. A value equal to TICKS_PER_MS will cause the clock
        /// to run in realtime.
        /// </param>
        public ScaledClock(int ticksPerUnitTime)
        {
            _ticksPerUnit = ticksPerUnitTime;
        }

        protected override long CalculateElapsed()
        {
            return ((GetCurrentTime() - _startTime) * TICKS_PER_MS) / _ticksPerUnit;
        }

        public void DoubleSpeed()
        {
            Pause();
            _ticksPerUnit = _ticksPerUnit / 2;
            Unpause();
        }

        public void HalveSpeed()
        {
            Pause();
            _ticksPerUnit = _ticksPerUnit * 2;
            Unpause();
        }

        public int TicksPer
        {
            get { return _ticksPerUnit; }
            set { _ticksPerUnit = value; }
        }
    }

    public class Timer : Clock
    {

        #region Member Data

        protected long _timeout;

        #endregion Member Data



        #region Constructors

        /// <summary>
        /// Returns a running timer with specified timeout (in ms)
        /// </summary>
        public Timer(int timeout)
            : this(timeout, 0, true)
        {
        }

        public Timer(int timeout, bool start)
            : this(timeout, 0, start)
        {
        }
        
        public Timer(int timeout, int elapsed) 
            : this(timeout, elapsed, true)
        {
        }

        /// <summary>
        /// Returns an optionally running timer with specified timeout (in ms) and elapsed time (in ms).
        /// </summary>
        public Timer(int timeout, int elapsed, bool start)
            : base(elapsed)
        {
            _isPaused = true;

            _elapsed_hold = (long)elapsed * CONVERSION_FACTOR;
            _timeout = (long)timeout * CONVERSION_FACTOR;

            if (start)
            {
                Unpause();
            }
        }

        #endregion Constructors



        #region Methods
        
        /// <summary>
        /// Resets this timer and restarts it.
        /// </summary>
        public bool Reset()
        {
            return Reset(true);
        }
        
        /// <summary>
        /// Resets this timer and optionally restarts it.
        /// </summary>
        public bool Reset(bool restart)
        {
            Pause();
            _elapsed_hold = 0;

            return restart ? Unpause() : restart;
        }

        #endregion Methods




        #region Properties

        public int Timeout
        {
            get { return (int)(_timeout / CONVERSION_FACTOR); }
        }
        /// <summary>
        /// Returns true if this Timer is over its timeout limit; returns false otherwise;
        /// </summary>
        public bool IsUp
        {
            get { return CalculateElapsed() > _timeout; }
        }

        #endregion Properties
    }

    public class ScaledTimer : Timer
    {

        #region Member Data

        private int _ticksPerUnit;

        #endregion Member Data





        #region Constructors
        
        /// <summary>
        /// Constructs a new ScaledTimer object
        /// </summary>
        /// <param name="ticksPerUnitTime">
        /// Number of ticks per unit time. The more ticks/time, the lower the clock progresses. A value equal 
        /// to TICKS_PER_MS will cause the timer to run in realtime.
        /// </param>
        public ScaledTimer(int timeout, int ticksPerUnitTime)
            : this(timeout, 0, ticksPerUnitTime)
        {
        }

        public ScaledTimer(int timeout, int ticksPerUnitTime, bool start)
            : this(timeout, 0, ticksPerUnitTime, start)
        {
        }
        
        public ScaledTimer(int timeout, int elapsed, int ticksPerUnitTime)
            : this(timeout, elapsed, ticksPerUnitTime, true)
        {
        }
        
        public ScaledTimer(int timeout, int elapsed, int ticksPerUnitTime, bool start)
            : base(timeout, elapsed, start)
        {
            _ticksPerUnit = ticksPerUnitTime;
        }

        #endregion Constructors

        protected override long CalculateElapsed()
        {
            long tempElapsed = base.CalculateElapsed();

            return (tempElapsed * TICKS_PER_MS) / _ticksPerUnit;
        }

        public void DoubleSpeed()
        {
            Pause();
            _ticksPerUnit = _ticksPerUnit / 2;
            Unpause();
        }

        public void HalveSpeed()
        {
            Pause();
            _ticksPerUnit = _ticksPerUnit * 2;
            Unpause();
        }

        public int TicksPer
        {
            get { return _ticksPerUnit; }
            set { _ticksPerUnit = value; }
        }
    }


}
