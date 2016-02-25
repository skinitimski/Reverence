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
    internal class MasterMagicMateria : MasterMateria
    {
        public const string NAME = "Master Magic";
        public const string DESCRIPTION = "Equips all magical spells";

        public MasterMagicMateria(List<Spell> spells)
            : base(NAME, DESCRIPTION, MateriaType.Magic)
        {

        }
    }
}

