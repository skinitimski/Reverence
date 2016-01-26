using System;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class EnemySkillMateria : CommandMateria
    {
        public const int TOTAL_ENEMY_SKILLS = 24;
        
        public EnemySkillMateria(int ap) : base("enemyskill", ap) 
        {
        }


        
        public void LearnSkill(int skill)
        {
            AP = AP & (1 << skill);
        }
        
        public Spell[] EnemySkills
        {
            get
            {
                Spell[] skills = new Spell[TOTAL_ENEMY_SKILLS];

                for (int i = 0; i < TOTAL_ENEMY_SKILLS; i++)
                {
                    skills[i] = EnemySkillSpell.Get(AllAbilities[i]);
                }

                return skills;
            }
        }
    }
}

