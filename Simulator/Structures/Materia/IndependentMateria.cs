using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public class IndependentMateria : Materia
    {
        public IndependentMateria(string name, int ap) : base(name, ap) { }

        public override Cairo.Color Color
        {
            get { return new Cairo.Color(.8, .0, .8); }
        }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();
                if (_abilities.Length == 1)
                    abilities.Add(_abilities[0]);
                else
                    abilities.Add(_abilities[Level]);
                return abilities;
            }
        }

        protected override int TypeOrder { get { return 3; } }
        public override MateriaType Type { get { return MateriaType.Independent; } }

    }

}
