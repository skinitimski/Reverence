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
    internal static class MagicSpell
    {
        private static Dictionary<string, Spell> _magicSpells;

        static MagicSpell()
        {
            _magicSpells = Spell.LoadSpells("data.spells.magic.xml");
        }
                
        public static Spell Get(string id)
        {
            return _magicSpells[id];
        }
        
        public static IEnumerable<Spell> GetAll()
        {
            return _magicSpells.Values.ToList();
        }
        
        public static int Count { get { return _magicSpells.Count; } }
    }
}

