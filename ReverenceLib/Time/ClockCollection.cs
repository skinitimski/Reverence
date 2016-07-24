using System;
using System.Collections.Generic;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Time
{
    public class ClockCollection
    {
        private List<IClock> _all = new List<IClock>();
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
        
        public void Add(IClock clock)
        {
            _all.Add(clock);
        }
        
        public bool Remove(IClock clock)
        {
            return _all.Remove(clock);
        }
    }
}

