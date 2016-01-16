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
    internal static class SummonSpell
    {
        private static Dictionary<string, Spell> _summonSpells;

        static SummonSpell()
        {
            _summonSpells = Spell.LoadSpells("data.spells.summon.xml");
        }

        public static Spell Get(string id)
        {
            return _summonSpells[id];
        }
        
        public static IEnumerable<Spell> GetAll()
        {
            return _summonSpells.Values.ToList();
        }
                
        public static int Count { get { return _summonSpells.Count; } }
    }
}

