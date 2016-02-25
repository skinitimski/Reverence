using System;
using System.Collections.Generic;

using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class SummonMenuEntry
    {        
        public SummonMenuEntry(string name, Spell s)
        {
            Name = name;
            Spell = s;
            AllCount = 0;
            QMagicCount = 0;
        }
        
        public void AddAbility(MateriaOrb s)
        {
            switch (s.Name)
            {
                case "All":
                    AllCount = s.Level + 1;
                    break;

                case "Quadra Magic":
                    if (s.Name != "Knights of Round")
                    {
                        QMagicCount = s.Level + 1;
                    }
                    break;
            }
        }
        
        public static int Compare(SummonMenuEntry left, SummonMenuEntry right)
        {
            return Spell.Compare(left.Spell, right.Spell);
        }
        
        public Spell Spell { get; private set; }
        
        public string Name { get; private set; }
        
        public int AllCount { get; private set; }
        
        public int QMagicCount { get; private set; }
        
        public int Order { get { return Spell.Order; } }
        
        public string ID { get { return Name == null ? "" : Resource.CreateID(Name); } }
    }
}

