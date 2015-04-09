using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cairo;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class SummonMateria : MateriaOrb
    {
        private static readonly Color ORB_COLOR = new Color(.9, 0, 0);

        public SummonMateria(string name, int ap) : base(Resource.CreateID(name), ap) { }

        public override Cairo.Color Color { get { return ORB_COLOR; } }

        protected override int TypeOrder { get { return 4; } }

        public override MateriaType Type { get { return MateriaType.Summon; } }

        public override List<string> Abilities
        {
            get
            {
                return new List<string> { AllAbilities.First() };
            }
        }
        
        public virtual List<Spell> GetSpells
        {
            get
            {
                return Abilities.Select(x => Spell.GetSummonSpell(Resource.CreateID(x))).ToList();
            }
        }
        
        public int Shots { get { return Level + 1; } }
    }




}
