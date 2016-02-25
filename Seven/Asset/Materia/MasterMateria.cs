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
    internal class MasterMateria : MateriaOrb
    {
        private List<string> _abilities;

        public MasterMateria(MateriaType type, Data data)
            : base()
        {
            Name = "Master" + type.ToString();
            Type = type;
           
            switch (type)
            {
                case MateriaType.Magic:
                    Description = "Equips all magical spells";
                    _abilities = data.GetMagicSpells().Select(x => x.Name).ToList();
                    break;
                case MateriaType.Command:
                    Description = "Equips all commands";
                    // TODO : define command abilities
                    _abilities = new List<string> { "Sense", "Steal" };
                    break;
                case MateriaType.Summon:
                    Description = "Equips all summon spells";
                    _abilities = data.GetSummonSpells().Select(x => x.Name).ToList();
                    break;
                    
                case MateriaType.Support:
                case MateriaType.Independent:
                    throw new ImplementationException("No master materia definition for type " + type);
            }

            Tiers = new int[1];
        }

        public override IEnumerable<string> Abilities
        {
            get
            {
                return _abilities;
            }
        }

        public override IEnumerable<string> AbilityDescriptions
        {
            get
            {
                string desc = null;

                switch (Type)
                {
                    case MateriaType.Magic:
                        desc = "All Magic";
                        break;
                    case MateriaType.Command:
                        desc = "All Commands";
                        break;
                    case MateriaType.Summon:
                        desc = "All Summons";
                        break;
                    case MateriaType.Support:
                    case MateriaType.Independent:
                        throw new ImplementationException("No master materia definition for type " + Type);
                }

                yield return desc;
            }
        }
    }
}

