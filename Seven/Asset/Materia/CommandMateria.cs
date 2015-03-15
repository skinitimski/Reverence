using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    public class CommandMateria : MateriaBase
    {
        public CommandMateria(string name, int ap) : base(Resource.CreateID(name), ap) { }

        public override Cairo.Color Color
        {
            get { return new Cairo.Color(.8, .8, .2); }
        }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();
                if (Level >= _abilities.Length)
                    abilities.Add(_abilities[Level - 1]);
                else
                    abilities.Add(_abilities[Level]);
                return abilities;
            }
        }

        protected override int TypeOrder { get { return 2; } }
        public override MateriaType Type { get { return MateriaType.Command; } }
    }


}
