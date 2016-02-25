using System;

using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal class MagicMenuEntry
    {
        private Spell _spell;
        private int _allCount;
        private int _qmagicCount;
        private int _mpTurboFactor;
        //private List<AddedAbility> _addedAbilities;
        
        public MagicMenuEntry(Spell s)
        {
            _spell = s;
            _allCount = 0;
            _qmagicCount = 0;
            _mpTurboFactor = 0;
            //_addedAbilities = new List<AddedAbility>();
        }
        
        public void AddAbility(MateriaOrb s)
        {
            if (s.Type != MateriaType.Support)
            {
                throw new ImplementationException("Used non-support materia '{0}' in a support context.", s.Name);
            }

            //_addedAbilities.Add(s.GetAbility());
            if (s.Name == "All")
            {
                _allCount = Math.Min(s.Level + 1, 5);
            }
            if (s.Name == "Quadra Magic")
            {
                _qmagicCount = Math.Min(s.Level + 1, 5);
            }
            if (s.Name == "MP Turbo")
            {
                _mpTurboFactor = Math.Min(s.Level + 1, 5);
            }
        }
        
        public static int Compare(MagicMenuEntry left, MagicMenuEntry right)
        {
            return Spell.Compare(left.Spell, right.Spell);
        }
        
        public Spell Spell { get { return _spell; } }
        public int AllCount { get { return _allCount; } }
        public int QMagicCount { get { return _qmagicCount; } }
        public int MPTurboFactor { get { return _mpTurboFactor; } }
        public string Name { get { return _spell.Name; } }
    }
}

