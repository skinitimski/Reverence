using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace Atmosphere.BattleSimulator
{
    public static class Materiatory
    {
        public const int MATERIATORY_SIZE = 200;

        private static Materia[] _materiatory;

        public static void Init()
        {
            _materiatory = new Materia[MATERIATORY_SIZE];

            XmlDocument savegame = Globals.SaveGame;

            foreach (XmlNode node in savegame.SelectSingleNode("//materiatory").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)));

                XmlNode orbNode = xml.DocumentElement;

                string id = orbNode.Attributes["id"].Value;
                MateriaType type = (MateriaType)Enum.Parse(typeof(MateriaType), orbNode.Attributes["type"].Value);
                int ap = Int32.Parse(orbNode.Attributes["ap"].Value);
                int slot = Int32.Parse(orbNode.Attributes["slot"].Value);

                _materiatory[slot] = Materia.Create(id, ap, type);
            }
        }

        public static Materia Get(int slot)
        {
            return _materiatory[slot];
        }

        public static void Put(Materia orb, int slot)
        {
            _materiatory[slot] = orb;
        }
        public static void Put(Materia orb)
        {
            int i = 0;

            while (_materiatory[i] != null) i++;

            _materiatory[i] = orb;
        }
        public static void Sort()
        {
            Array.Sort<Materia>(_materiatory, Materia.CompareByType);
			
			// sort magic
			int a = 0;
			while (_materiatory[a].Type == MateriaType.Magic) a++;
			Materia[] tempMagic = new Materia[a];
			Array.Copy(_materiatory, 0, tempMagic, 0, a);
			Array.Sort<Materia>(tempMagic, Materia.CompareByOrder);
			Array.Copy(tempMagic, 0, _materiatory, 0, a);
			

			// sort support
			int b = a;
            while (_materiatory[b].Type == MateriaType.Support) b++;                
            Materia[] tempSupport = new Materia[b - a];
			Array.Copy(_materiatory, a, tempSupport, 0, b - a);
			Array.Sort<Materia>(tempSupport, Materia.CompareByOrder);
			Array.Copy(tempSupport, 0, _materiatory, a, b - a);

			// sort command
			int c = b;
			while (_materiatory[c].Type == MateriaType.Command) c++;
			Materia[] tempCommand = new Materia[c - b];
			Array.Copy(_materiatory, b, tempCommand, 0, c - b);
			Array.Sort<Materia>(tempCommand, Materia.CompareByOrder);
            Array.Copy(tempCommand, 0, _materiatory, b, c - b);

            // sort independent
            int d = c;
            while (_materiatory[d].Type == MateriaType.Independent) d++;
            Materia[] tempIndependent = new Materia[d - c];
            Array.Copy(_materiatory, c, tempIndependent, 0, d - c);
            Array.Sort<Materia>(tempIndependent, Materia.CompareByOrder);
            Array.Copy(tempIndependent, 0, _materiatory, c, d - c);

            // sort summon
            int e = d;
            while (e < MATERIATORY_SIZE && _materiatory[e] != null) e++;
            Materia[] tempSummon = new Materia[e - d];
            Array.Copy(_materiatory, d, tempSummon, 0, e - d);
            Array.Sort<Materia>(tempSummon, Materia.CompareByOrder);
            Array.Copy(tempSummon, 0, _materiatory, d, e - d);
			
		}
    }
}
