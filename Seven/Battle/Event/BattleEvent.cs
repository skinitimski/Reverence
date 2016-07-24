using System;
using System.Text;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.State;
using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Battle.Event
{
    internal abstract class BattleEvent
    {
        protected BattleEvent()
        {
        }


        protected abstract void RunIteration(long elapsed, bool lastIteration);

        protected abstract string GetStatus(long elapsed);
        
        protected virtual void BeginHook() 
        {
        }
        
        protected virtual void CompleteHook() 
        {
        }


        public void Begin()
        {
            ActionTimer = new Timer(Duration);

            BeginHook();
        }

        public bool RunIteration()
        {
            bool isDone = ActionTimer.IsUp;
            long elapsed = ActionTimer.TotalMilliseconds;

            RunIteration(elapsed, isDone);

            if (isDone)
            {
                CompleteHook();
            }

            return isDone;
        }

        public string GetStatus()
        {
            if (ActionTimer == null)
            {
                throw new ImplementationException("Tried to call GetStatus() before starting the event.");
            }

            return GetStatus(ActionTimer.TotalMilliseconds);
        }
        
        public override abstract string ToString();


        protected abstract int Duration { get; }

        protected long ElapsedMilliseconds { get { return ActionTimer.TotalMilliseconds; } }
        
        public Timer ActionTimer { get; private set; }
    }
}

