using System;
using System.Text;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.State;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal abstract class BattleEvent
    {
        private BattleEvent()
        {
        }
                
        protected BattleEvent(TimeFactory factory, int duration)
            : this()
        {
            ActionTimer = factory.CreateTimer(duration, false);
        }


        protected abstract void RunIteration(long elapsed, bool lastIteration);

        protected abstract string GetStatus(long elapsed);

        protected virtual void CompleteHook() 
        {
        }


        public void Begin()
        {
            ActionTimer.Unpause();
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
            return GetStatus(ActionTimer.TotalMilliseconds);
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" {0} : {1}{2}", GetType().Name, GetStatus(), Environment.NewLine);
            sb.AppendLine();
            return sb.ToString();
        }




        protected long ElapsedMilliseconds { get { return ActionTimer.TotalMilliseconds; } }
        
        private Timer ActionTimer { get; set; }
    }
}

