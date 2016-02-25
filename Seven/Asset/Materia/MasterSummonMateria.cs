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
    internal class MasterSummonMateria : MasterMateria
    {
        public const string NAME = "Master Summon";
        public const string DESCRIPTION = "Equips all summon spells";

        private MasterSummonMateria(List<Spell> spells)
            : base(NAME, DESCRIPTION, MateriaType.Magic)
        {
        }
        
        public override List<string> Abilities
        {
            get
            {
                return base.Abilities;
            }
        }
    }
}

