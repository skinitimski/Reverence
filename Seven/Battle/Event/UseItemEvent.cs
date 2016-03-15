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
    internal class UseItemEvent : CombatantActionEvent
    {
        private const int DURATION = 2000;

        public UseItemEvent(Item item, int slot, Ally source, IEnumerable<Combatant> targets, bool releaseAlly)
            : base(source, releaseAlly)
        {
            Item = item;
            Slot = slot;
            Targets = targets;
            Status = source.Name + " uses item " + item.Name;      
        }

        protected override void RunIteration(long elapsed, bool isLast)
        {
            if (!HasUsed)
            {
                Item.UseInBattle((Ally)Source, Targets);
                HasUsed = true;
            }

            if (isLast)
            {
                Source.CurrentBattle.Party.Inventory.DecreaseCount(Slot);
            }
        }
        
        protected override string GetStatus(long elapsed)
        {
            return Status;
        }
        
        public override string ToString()
        {
            return String.Format(" {0} uses {1}", Source.Name, Source.Name);
        }

        protected override int Duration { get { return DURATION; } }
        private Item Item { get; set; }
        private int Slot { get; set; }
        private IEnumerable<Combatant> Targets { get; set; }
        private string Status { get; set; }
                        
        private bool HasUsed { get; set; }

    }
}

