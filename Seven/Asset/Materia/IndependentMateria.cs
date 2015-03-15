using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    public class IndependentMateria : MateriaBase
    {
        public IndependentMateria(string name, int ap) : base(name, ap) { }

        public override Color Color
        {
            get { return new Color(.8, .0, .8); }
        }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();

                if (_abilities.Length == 1)
                {
                    abilities.Add(_abilities[0]);
                }
                else
                {
                    abilities.Add(_abilities[Level]);
                }

                return abilities;
            }
        }

        protected override int TypeOrder { get { return 3; } }
        public override MateriaType Type { get { return MateriaType.Independent; } }

    }

}
