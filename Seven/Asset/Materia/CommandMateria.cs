using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class CommandMateria : MateriaBase
    {
        private static readonly Color ORB_COLOR = new Color(.8, .8, .2);

        public CommandMateria(string name, int ap) : base(Resource.CreateID(name), ap) { }
        
        public override Color Color { get { return ORB_COLOR; } }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();

                if (Level >= _abilities.Length)
                {
                    abilities.Add(_abilities[Level - 1]);
                }
                else
                {
                    abilities.Add(_abilities[Level]);
                }

                return abilities;
            }
        }

        protected override int TypeOrder { get { return 2; } }
        public override MateriaType Type { get { return MateriaType.Command; } }
    }


}
