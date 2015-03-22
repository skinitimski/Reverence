using System;
using System.Collections.Generic;

using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class Summon
    {
        public const int TOTAL_SUMMONS = 16;
        //private List<AddedAbility> _addedAbilities;
        
        public Summon(string name, SummonMateria s)
        {
            Name = name;
            Spell = s.Spell;
            AllCount = 0;
            QMagicCount = 0;
            //_addedAbilities = new List<AddedAbility>();
        }
        
        public void AddAbility(SupportMateria s)
        {
            //_addedAbilities.Add(s.GetAbility());
            if (s.ID == "all")
            {
                AllCount = s.Level + 1;
            }
//            if (s.ID == "quadramagic")
//                if (Name == "Knights of Round")
//                    _addedAbilities.Remove(s.GetAbility());
            else
            {
                QMagicCount = s.Level + 1;
            }
        }
        
        public static int Compare(Summon left, Summon right)
        {
            return Spell.Compare(left.Spell, right.Spell);
        }
        
        public Spell Spell { get; private set; }

        public string Name { get; private set; }

        public int AllCount { get; private set; }

        public int QMagicCount { get; private set; }

        public int Order { get { return Spell.Order; } }

        public string ID { get { return Name == null ? "" : Resource.CreateID(Name); } }

        //public List<AddedAbility> AddedAbility { get { return _addedAbilities; } }
    }
}

