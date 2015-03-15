using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    public class SupportMateria : MateriaBase
    {
        public SupportMateria(string name, int ap) : base(name, ap) { }

        public override Cairo.Color Color
        {
            get { return new Cairo.Color(.2, .6, 1); }
        }

        public override List<string> Abilities
        {
            get
            {
                List<string> abilities = new List<string>();
                abilities.Add(_abilities[0]);
                return abilities;
            }
        }

//        public virtual AddedAbility GetAbility()
//        {
//            return (AddedAbility)Enum.Parse(typeof(AddedAbility), Globals.CleanString(Name));
//        }

        protected override int TypeOrder { get { return 1; } }
        public override MateriaType Type { get { return MateriaType.Support; } }
        

        public override int StrengthMod { get { return 0; } }
        public override int VitalityMod { get { return 0; } }
        public override int DexterityMod { get { return 0; } }
        public override int MagicMod { get { return 0; } }
        public override int SpiritMod { get { return 0; } }
        public override int LuckMod { get { return 0; } }
        public override int HPMod { get { return 0; } }
        public override int MPMod { get { return 0; } }
    }

}
