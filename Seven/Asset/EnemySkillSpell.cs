using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Thread = System.Threading.Thread;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Asset
{
    internal static class EnemySkillSpell
    {
        private static Dictionary<string, Spell> _enemySkillSpells;

        static EnemySkillSpell()
        {
            _enemySkillSpells = Spell.LoadSpells("data.spells.enemyskill.xml");
        }
        
        public static Spell Get(string name)
        {
            if (!_enemySkillSpells.ContainsKey(name))
            {
                throw new GameDataException("Could not find enemy skill spell with name " + name);
            }

            return _enemySkillSpells[name];
        }
        
        public static IEnumerable<Spell> GetAll()
        {
            return _enemySkillSpells.Values.ToList();
        }
        
        public static int Count { get { return _enemySkillSpells.Count; } }
    }
}

