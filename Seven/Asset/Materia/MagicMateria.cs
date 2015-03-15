using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    public class MagicMateria : MateriaBase
    {
        public MagicMateria(string name, int ap) : base(Resource.CreateID(name), ap) { }

        public override Cairo.Color Color
        {
            get { return new Cairo.Color(0, .7, .05); }
        }

        public List<Spell> GetSpells
        {
            get
            {
                List<Spell> sp = new List<Spell>();
                foreach (string s in Abilities)
                    if (s != "")
                        sp.Add(Spell.SpellTable[Resource.CreateID(s)]);
                return sp;
            }
        }
        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();
                for (int i = 0; i < _abilities.Length; i++)
                    if (Level >= i) abilities.Add(_abilities[i]);
                return abilities;
            }
        }

        protected override int TypeOrder { get { return 0; } }
        public override MateriaType Type { get { return MateriaType.Magic; } }
    }




}
