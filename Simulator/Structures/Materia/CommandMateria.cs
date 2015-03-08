using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public class CommandMateria : Materia
    {
        public CommandMateria(string name, int ap) : base(Globals.CreateID(name), ap) { }

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

    public class EnemySkillMateria : CommandMateria
    {
        public const int TOTAL_ENEMY_SKILLS = 24;

        public EnemySkillMateria(int ap) : base("enemyskill", ap) 
        {
            _ap = ap;
        }
        public EnemySkillMateria() : base("enemyskill", 0) { }

        public override void AddAP(int delta)
        {
        }

        public void LearnSkill(int skill)
        {
            _ap = _ap & (1 << skill);
        }

        public Spell[] EnemySkills
        {
            get
            {
                Spell[] skills = new Spell[TOTAL_ENEMY_SKILLS];
                for (int i = 0; i < TOTAL_ENEMY_SKILLS; i++)
                    skills[i] = Spell.SpellTable[Globals.CreateID(AllAbilities[i])];
                return skills;
            }
        }
    }

}
