using System;
using System.Text;

using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class BattleEvent : ICloneable
    {
        public delegate string TimedDialogue(Clock c);

        private BattleEvent()
        {
            ActionTimer = new Timer(2000, 0, false);
            ResetSourceTurnTimer = true;
            Dialogue = c => String.Empty;
        }
        
        public BattleEvent(Combatant source, Action action)
            : this()
        {
            Source = source;
            Action = action;
        }
        
        /// <summary>Creates a deep clone of this AbilityState.</summary>
        /// <returns>A new deep clone</returns>
        public object Clone()
        {
            // Shallow objects
            BattleEvent state = (BattleEvent)this.MemberwiseClone();

            // Deep objects
            state.Source = Source;
            state.Action = Action;
            state.ActionTimer = new Timer(2000, 0, false);
            
            return state;
        }
        
        public void DoAction()
        {
            Action();
            ActionTimer.Unpause();
            
            // used to simulate realtime animation! haha
            while (!ActionTimer.IsUp)
            {
                System.Threading.Thread.Sleep(100);
            }

            if (ResetSourceTurnTimer)
            {
                Source.TurnTimer.Reset();

                Source.WaitingToResolve = false;
            }
        }

        public string GetStatus()
        {
            return Dialogue(ActionTimer);
        }
        
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(" {0} : {1}{2}", typeof(BattleEvent).Name, GetStatus(), Environment.NewLine);
            sb.AppendFormat("\tsource {0}", Source.Name);
            sb.AppendLine();
            return sb.ToString();
        }




        public TimedDialogue Dialogue { get; set; }

        public Combatant Source { get; private set; }
        public Action Action { get; private set; }
        private Timer ActionTimer { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Atmosphere.Reverence.Seven.Battle.BattleEvent"/>
        /// should reset the turn timer of its source when it completes its action. Defaults to true.
        /// </summary>
        public bool ResetSourceTurnTimer { get; set; }


        
        //        public int HitP { get { return _hitp; } set { _hitp = value; } }
        //        public int MPTurboFactor { get { return _mpTurbo; } set { _mpTurbo = value; } }
        //        public bool NoSplit { get { return _noSplit; } set { _noSplit = value; } }
        //        public bool LongRange { get { return _longRange; } set { _longRange = value; } }
        //        public bool QuadraMagic { get { return _quadraMagic; } set { _quadraMagic = value; } }
        //        public AttackType Type { get { return _type; } set { _type = value; } }
        //        public Combatant[] Target { get { return _target; } set { _target = value; } }
        //        public Element[] Elements { get { return _elements; } set { _elements = value; } }
    }
}

