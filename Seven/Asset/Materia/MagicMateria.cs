using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class MagicMateria : MateriaOrb
    {
        private static readonly Color ORB_COLOR = new Color(0, .7, .05);

        public MagicMateria(string name, int ap) : base(Resource.CreateID(name), ap) { }

        public override Color Color { get { return ORB_COLOR; } }
        
        protected override int TypeOrder { get { return 0; } }
        
        public override MateriaType Type { get { return MateriaType.Magic; } }

        public override List<string> Abilities
        {
            get
            {
                return AllAbilities.TakeWhile((s, i) => i <= Level).ToList();
            }
        }
        
        public virtual List<Spell> GetSpells
        {
            get
            {
                return Abilities.Select(x => Spell.GetMagicSpell(Resource.CreateID(x))).ToList();
            }
        }
    }




}
