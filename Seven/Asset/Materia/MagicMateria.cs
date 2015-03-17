using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class MagicMateria : MateriaBase
    {
        private static readonly Color ORB_COLOR = new Color(0, .7, .05);

        public MagicMateria(string name, int ap) : base(Resource.CreateID(name), ap) { }

        public override Color Color { get { return ORB_COLOR; } }

        public List<Spell> GetSpells
        {
            get
            {
                List<Spell> sp = new List<Spell>();

                foreach (string s in Abilities)
                {
                    if (s != String.Empty)
                    {
                        sp.Add(Spell.SpellTable[Resource.CreateID(s)]);
                    }
                }

                return sp;
            }
        }
        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();

                for (int i = 0; i < _abilities.Length; i++)
                {
                    if (Level >= i) abilities.Add(_abilities[i]);
                }

                return abilities;
            }
        }

        protected override int TypeOrder { get { return 0; } }
        public override MateriaType Type { get { return MateriaType.Magic; } }
    }




}
