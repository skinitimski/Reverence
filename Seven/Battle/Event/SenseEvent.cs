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
    internal class SenseEvent : CombatantActionEvent
    {
        private const int PAUSE = 1000;
        private const int MESSAGE_DURATION = 1500; // TODO: config value

        private int _duration;

        public SenseEvent(Ally source, IEnumerable<Combatant> targets)
            : base(source, true)
        {
            Target = targets.First();

            List<String> messages = new List<String>();

            messages.Add(Target.Name + " Lvl: " + Target.Level);
            messages.Add(String.Format("HP: {0}/{1} MP: {2}/{3}", Target.HP, + Target.MaxHP, + Target.MP, + Target.MaxMP));

            foreach (Element element in Target.Weaknesses)
            {
                string description = element.ToString();

                switch (element)
                {
                    case Element.Restorative:
                        description = "holy power";
                        break;
                }

                messages.Add("Weak against " + description + ".");
            }


            int dialogue_duration = messages.Count * MESSAGE_DURATION;
            _duration = PAUSE + dialogue_duration;
                

            Messages = messages;
        }

        protected override void RunIteration(long elapsed, bool isLast)
        {
            if (!HasSensed && elapsed > PAUSE)
            {   
                Target.Sense();

                HasSensed = true;
            }
        }
        
        protected override string GetStatus(long elapsed)
        {
            string info = null;
            
            if (elapsed < PAUSE)
            {
                info = Source.Name + " senses " + Target.Name;
            }
            else
            {
                elapsed -= PAUSE;
                
                int index = (int)elapsed / MESSAGE_DURATION;
                
                if (index >= Messages.Count)
                {
                    index = Messages.Count - 1;
                }
                
                info = Messages[index]; 
            }
            
            return info;
        }
        
        public override string ToString()
        {
            return String.Format(" {0} senses {1}", Source.Name, Target.Name);
        }

        protected override int Duration { get { return _duration; } }

        private Combatant Target { get; set; }
        
        private bool HasSensed { get; set; }

        private List<String> Messages { get; set; }
    }
}

