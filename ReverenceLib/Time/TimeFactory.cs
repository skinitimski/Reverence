using System;
using System.Collections.Generic;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Time
{
    public class TimeFactory
    {
        private List<Clock> _all = new List<Clock>();
        private bool[] _pauseState = null;
        private object _sync = new object();


        
        public bool PauseAllClocks()
        {
            bool ret = false;
            
            lock (_sync)
            {
                if (_pauseState == null)
                {
                    _pauseState = new bool[_all.Count];
                    
                    for (int i = 0; i < _pauseState.Length; i++)
                    {
                        _pauseState[i] = _all[i].Pause();
                    }
                    
                    ret = true;
                }
            }
            
            return ret;
        }
        
        public bool UnpauseAllClocks()
        {
            bool ret = false;
            
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    for (int i = 0; i < _pauseState.Length; i++)
                    {
                        if (_pauseState[i])
                        {
                            _all[i].Unpause();
                        }
                    }
                    
                    _pauseState = null;
                    
                    ret = true;
                }
            }
            
            return ret;
        }

        
        /// <summary>
        /// Creates a standard running clock.
        /// </summary>
        public Clock CreateClock()
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new clocks when paused.");
                }
                
                Clock c = new Clock();
                
                _all.Add(c);
                
                return c;
            }
        }
        
        public Clock CreateClock(bool start)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new clocks when paused.");
                }

                Clock c = new Clock(start);
            
                _all.Add(c);
                
                return c;
            }
        }
        
        public Clock CreateClock(int ticksPerMs)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new clocks when paused.");
                }

                Clock c = new Clock(ticksPerMs);

                _all.Add(c);

                return c;
            }
        }
        
        public Clock CreateClock(int ticksPerMs, bool start)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new clocks when paused.");
                }

                Clock c = new Clock(ticksPerMs, start);
            
                _all.Add(c);
            
                return c;
            }
        }
        
        public Clock CreateClock(int ticksPerMs, int elapsed, bool start)
        {      
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new clocks when paused.");
                }      
                Clock c = new Clock(ticksPerMs, elapsed, start);
            
                _all.Add(c);
            
                return c;
            }
        }









        public Timer CreateTimer(int timeout)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new timers when paused.");
                }

                Timer t = new Timer(timeout);

                _all.Add(t);

                return t;
            }
        }
        
        public Timer CreateTimer(int timeout, bool start)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new timers when paused.");
                }

                Timer t = new Timer(timeout, start);
            
                _all.Add(t);
            
                return t;
            }
        }
        
        public Timer CreateTimer(int timeout, int ticksPerMs)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new timers when paused.");
                }

                Timer t = new Timer(timeout, ticksPerMs);
            
                _all.Add(t);
            
                return t;
            }
        }
        
        public Timer CreateTimer(int timeout, int ticksPerMs, bool start)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new timers when paused.");
                }

                Timer t = new Timer(timeout, ticksPerMs, start);
            
                _all.Add(t);
            
                return t;
            }
        }
        
        /// <summary>
        /// Returns an optionally running timer with specified timeout (in ms) and elapsed time (in ms).
        /// </summary>
        public Timer CreateTimer(int timeout, int ticksPerMs, int elapsed, bool start)
        {
            lock (_sync)
            {
                if (_pauseState != null)
                {
                    throw new ImplementationException("Cannot create new timers when paused.");
                }

                Timer t = new Timer(timeout, ticksPerMs, elapsed, start);
            
                _all.Add(t);
            
                return t;
            }
        }


    }
}

