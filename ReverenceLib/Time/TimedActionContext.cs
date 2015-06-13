using System;

namespace Atmosphere.Reverence.Time
{
    public class TimedActionContext
    {
        private TimedActionContext()
        {
            Dialogue = c => String.Empty;
        }

        public TimedActionContext(TimedAction action, int duration)
            : this()
        {
            Action = action;

            ActionTimer = new Timer(duration, false);
        }

        public TimedActionContext(TimedAction action, int duration, TimedDialogue dialogue)
            : this(action, duration)
        {
            Dialogue = dialogue;
        }



        public string GetStatus()
        {
            return Dialogue(ActionTimer);
        }

        public void DoAction()
        {
            ActionTimer.Reset(true);

            Action(ActionTimer);
        }

        private TimedDialogue Dialogue { get; set; }

        private TimedAction Action { get; set; }

        private Timer ActionTimer { get; set; }
    }
}

