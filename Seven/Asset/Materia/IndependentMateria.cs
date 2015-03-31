using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class IndependentMateria : MateriaBase
    {
        private static readonly Color ORB_COLOR = new Color(.8, .0, .8);

        public IndependentMateria(string name, int ap) : base(name, ap) { }

        public override Color Color { get { return ORB_COLOR; } }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();

                if (AllAbilities.Length == 1)
                {
                    abilities.Add(AllAbilities[0]);
                }
                else
                {
                    abilities.Add(AllAbilities[Level]);
                }

                return abilities;
            }
        }

        protected override int TypeOrder { get { return 3; } }
        public override MateriaType Type { get { return MateriaType.Independent; } }

    }

}
