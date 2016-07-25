using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.State;

namespace Atmosphere.Reverence.Seven.Battle.Event
{
    internal class ChangeRowEvent : CombatantActionEvent
    {
        private const int PAUSE = 250;
        private const int DURATION = 500;

#if DEBUG
        private const string STATUS = "(change row)";
#else
        private const string STATUS = String.Empty;
#endif

        public ChangeRowEvent(Ally source)
            : base(source, true)
        {
            this.Source = source;
        }

        protected override void RunIteration(long elapsed, bool isLast)
        {
            if (!HasChanged && elapsed > PAUSE)
            {   
                Source.ChangeRow();

                HasChanged = true;
            }
        }



        protected override string GetStatus(long elapsed)
        {
            return STATUS;
        }
        
        public override string ToString()
        {
            return String.Format(" {0} changes rows", Source.Name);
        }

        protected override int Duration { get { return DURATION; } }

        private bool HasChanged { get; set; }

        private new Ally Source { get; set; }
    }
}

