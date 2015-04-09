using System;

using Atmosphere.Reverence.Seven.Asset.Materia;

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
        
        public void AddAbility(SupportMateria s)
        {
            //_addedAbilities.Add(s.GetAbility());
            if (s.ID == "all")
            {
                _allCount = Math.Min(s.Level + 1, 5);
            }
            if (s.ID == "quadramagic")
            {
                _qmagicCount = Math.Min(s.Level + 1, 5);
            }
            if (s.ID == "mpturbo")
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
        public string ID { get { return _spell.ID; } }
    }
}

