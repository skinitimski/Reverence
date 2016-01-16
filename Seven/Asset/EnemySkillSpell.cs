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
            _enemySkillSpells = Spell.LoadSpells("data.spells.summon.xml");
        }
        
        public static Spell Get(string id)
        {
            return _enemySkillSpells[id];
        }
        
        public static IEnumerable<Spell> GetAll()
        {
            return _enemySkillSpells.Values.ToList();
        }
        
        public static int Count { get { return _enemySkillSpells.Count; } }
    }
}

