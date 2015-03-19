using System;
using System.Collections.Generic;

using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Summon
    {
        public const int TOTAL_SUMMONS = 16;
        private string _name;
        private Spell[] _spell;
        private int _allCount;
        private int _qmagicCount;
        //private List<AddedAbility> _addedAbilities;
        
        public Summon(string name, SummonMateria s)
        {
            _name = name;
            _spell = s.GetSpells;
            _allCount = 0;
            _qmagicCount = 0;
            //_addedAbilities = new List<AddedAbility>();
        }
        
        public void AddAbility(SupportMateria s)
        {
            //_addedAbilities.Add(s.GetAbility());
            if (s.ID == "all")
            {
                _allCount = s.Level + 1;
            }
//            if (s.ID == "quadramagic")
//                if (_name == "Knights of Round")
//                    _addedAbilities.Remove(s.GetAbility());
            else
            {
                _qmagicCount = s.Level + 1;
            }
        }
        
        public static int Compare(Summon left, Summon right)
        {
            return Spell.Compare(left.Spell, right.Spell);
        }
        
        public Spell Spell { get { return _spell[0]; } }

        public string Name { get { return _name; } }

        public int AllCount { get { return _allCount; } }

        public int QMagicCount { get { return _qmagicCount; } }

        public int Order { get { return _spell[0].Order; } }

        public string ID { get { return _name == null ? "" : Resource.CreateID(Name); } }
        //public List<AddedAbility> AddedAbility { get { return _addedAbilities; } }
    }
}

