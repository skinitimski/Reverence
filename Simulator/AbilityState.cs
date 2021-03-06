﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public delegate void Action();

    public sealed class AbilityState : ICloneable
    {
        /// <summary>The ability has the ALL materia attached</summary>
        private int _hitp;
        private bool _noSplit;
        private int _mpTurbo;
        private bool _hpAbsorb;
        private bool _mpAbsorb;
        private bool _quadraMagic;
        private int _addedCut;
        private bool _addedSteal;
        private bool _longRange;
        private AttackType _type;
        private ICombatant _performer;
        private ICombatant[] _target;
        private Element[] _elements;

        private Action _action;
        private Timer _actionTimer;

        public AbilityState()
        {
            Reset();
        }

        public void Reset()
        {
            _hitp = 0;
            _noSplit = false;
            _mpTurbo = 0;
            _hpAbsorb = false;
            _mpAbsorb = false;
            _quadraMagic = false;
            _addedCut = 0;
            _addedSteal = false;
            _longRange = false;
            _type = AttackType.None;
            _performer = null;
            _target = null;

            _actionTimer = new Timer(2000, 0, false);
        }

        /// <summary>Creates a deep clone of this AbilityState.</summary>
        /// <returns>A new deep clone</returns>
        public object Clone()
        {
            // Shallow objects
            AbilityState state = (AbilityState)this.MemberwiseClone();

            // Deep objects
            state._actionTimer = new Timer(2000, 0, false);

            return state;
        }

        public void DoAction()
        {
            _action();
            _actionTimer.Unpause();

            // used to simulate realtime animation! haha
            while (!_actionTimer.IsUp) ;
            _performer.TurnTimer.Reset();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(" AbilityState : ");
            sb.AppendFormat("\tfrom {0} to {1}", Performer.Name, Target[0].Name);
            sb.AppendLine();
            sb.AppendFormat("\ttype: {0}", _type.ToString());
            return sb.ToString();
        }

        public int HitP { get { return _hitp; } set { _hitp = value; } }
        public int MPTurboFactor { get { return _mpTurbo; } set { _mpTurbo = value; } }
        public bool NoSplit { get { return _noSplit; } set { _noSplit = value; } }
        public bool LongRange { get { return _longRange; } set { _longRange = value; } }
        public bool QuadraMagic { get { return _quadraMagic; } set { _quadraMagic = value; } }
        public AttackType Type { get { return _type; } set { _type = value; } }
        public Action Action { get { return _action; } set { _action = value; } }
        public ICombatant Performer { get { return _performer; } set { _performer = value; } }
        public ICombatant[] Target { get { return _target; } set { _target = value; } }
        public Element[] Elements { get { return _elements; } set { _elements = value; } }
    }
}
