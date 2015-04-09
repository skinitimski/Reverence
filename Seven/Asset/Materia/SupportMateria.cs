using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class SupportMateria : MateriaOrb
    {
        private static readonly Color ORB_COLOR = new Color(.0, .6, .8);

        public SupportMateria(string name, int ap) : base(name, ap) { }

        public override Color Color { get { return ORB_COLOR; } }
        
        protected override int TypeOrder { get { return 1; } }

        public override MateriaType Type { get { return MateriaType.Support; } }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();

                abilities.Add(AllAbilities[0]);

                return abilities;
            }
        }
    }
}
