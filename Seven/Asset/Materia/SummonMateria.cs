﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    public class SummonMateria : MateriaBase
    {
        public SummonMateria(string name, int ap) : base(Resource.CreateID(name), ap) { }

        public override Cairo.Color Color
        {
            get { return new Cairo.Color(.9, 0, 0); }
        }

        public Spell[] GetSpells
        {
            get
            {
                Spell[] spells = new Spell[_abilities.Length];
                for (int i = 0; i < _abilities.Length; i++)
                    spells[i] = Spell.SpellTable[Resource.CreateID(_abilities[i])];
                return spells;
            }
        }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();
                foreach (string s in _abilities)
                    abilities.Add(s);
                return abilities;
            }
        }

        protected override int TypeOrder { get { return 4; } }
        public override MateriaType Type { get { return MateriaType.Summon; } }
        public int Shots { get { return Level + 1; } }
    }




}
