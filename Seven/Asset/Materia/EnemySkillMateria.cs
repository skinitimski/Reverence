using System;

namespace Atmosphere.Reverence.Seven.Asset.Materia
{
    internal class EnemySkillMateria : CommandMateria
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
                {
                    skills[i] = Spell.Get(Resource.CreateID(AllAbilities[i]));
                }
                return skills;
            }
        }
    }
}

