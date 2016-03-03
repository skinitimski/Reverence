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
    internal class StealEvent : CombatantActionEvent
    {
        private const int PAUSE = 1000;
        private const int DURATION = 2000;

        private StealEvent(Combatant source)
            : base(source, true, PAUSE + DURATION)
        {
        }


        public static StealEvent Create(Ally source, IEnumerable<Combatant> targets)
        {            
            Enemy enemy = (Enemy)targets.First();
                        
            bool canSteal = enemy.HasItems;

                        
            StealEvent @event = new StealEvent(source)
            {
                Target = enemy
            };

            return @event;
        }

        protected override void RunIteration(long elapsed, bool isLast)
        {
            if (!HasStolen && elapsed > PAUSE)
            {
                Stolen = Target.StealItem((Ally)Source);

                HasStolen = true;
            }
        }
        
        protected override string GetStatus(long elapsed)
        {
            string info = null;
            
            if (elapsed < PAUSE)
            {
                info = Source.Name + " steals from " + Target.Name;
            }
            else
            {
                if (Target.HasItems)
                {
                    if (Stolen != null)
                    {
                        // TODO: this doesn't work
                        info = "Stole " + Stolen + "!";
                    }
                    else
                    {
                        info = "Couldn't steal anything...";
                    }
                }
                else
                {
                    info = "Nothing to steal.";
                }
            }
            
            return info;
        }

        private Enemy Target { get; set; }

        private bool HasStolen { get; set; }
        
        private string Stolen { get; set; }

    }
}

