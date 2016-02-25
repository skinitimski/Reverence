using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

using Cairo;
using NLua;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class EnemySkillMateria : MateriaOrb
    {
        private List<string> _abilities;

        public const string NAME = "Enemy Skill";
        public const string DESCRIPTION = "Use skills learned from enemies";
       


        public EnemySkillMateria(int ap, Data data) 
            : base() 
        {
            Name = NAME;
            Description = DESCRIPTION;
            Type = MateriaType.Command;
            AP = ap;
            Data = data;
        }

        public static EnemySkillMateria Merge(IEnumerable<EnemySkillMateria> orbs)
        {
            return Merge((EnemySkillMateria[])orbs.ToArray());
        }

        public static EnemySkillMateria Merge(EnemySkillMateria[] orbs)
        {
            if (orbs.Length == 0)
            {
                throw new ImplementationException("Cannot merge zero enemy skill materias.");
            }

            EnemySkillMateria merged = orbs[0];

            for (int i = 1; i < orbs.Length; i++)
            {
                merged.AP |= orbs[i].AP;
            }

            return merged;
        }
        
        public void LearnSkill(int skill)
        {
            AP = AP & (1 << skill);
        }
        
        public override IEnumerable<string> Abilities
        {
            get
            {
                return Data.GetEnemySkills().Select(x => x.Name);
            }
        }
        
        public override IEnumerable<string> AbilityDescriptions
        {
            get
            {
                throw new ImplementationException("Not intended to get ability descriptions of Enemy Skill materia");
            }
        }

        private Data Data { get; set; }
    }
}

