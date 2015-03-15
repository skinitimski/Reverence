using System;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Seven.Asset.Materia;

namespace Atmosphere.Reverence.Seven
{
    internal class Materiatory
    {
        public const int MATERIATORY_SIZE = 200;


        private MateriaBase[] _materiatory;

        public Materiatory()
        {
            _materiatory = new MateriaBase[MATERIATORY_SIZE];
        }
        
        public Materiatory(XmlDocument savegame)
            : this()
        {            
            foreach (XmlNode node in savegame.SelectSingleNode("//materiatory").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                                
                string id = node.Attributes ["id"].Value;
                MateriaType type = (MateriaType)Enum.Parse(typeof(MateriaType), node.Attributes ["type"].Value);
                int ap = Int32.Parse(node.Attributes ["ap"].Value);
                int slot = Int32.Parse(node.Attributes ["slot"].Value);
                
                _materiatory[slot] = MateriaBase.Create(id, ap, type);
            }
        }
        
        public MateriaBase Get(int slot)
        {
            return _materiatory [slot];
        }
        
        public void Put(MateriaBase orb, int slot)
        {
            _materiatory [slot] = orb;
        }

        public void Put(MateriaBase orb)
        {
            int i = 0;
            
            while (_materiatory[i] != null)
            {
                i++;
            }
            
            _materiatory [i] = orb;
        }

        public void Sort()
        {
            Array.Sort<MateriaBase>(_materiatory, MateriaBase.CompareByType);
            
            // sort magic
            int a = 0;
            while (_materiatory[a].Type == MateriaType.Magic)
                a++;
            MateriaBase[] tempMagic = new MateriaBase[a];
            Array.Copy(_materiatory, 0, tempMagic, 0, a);
            Array.Sort<MateriaBase>(tempMagic, MateriaBase.CompareByOrder);
            Array.Copy(tempMagic, 0, _materiatory, 0, a);
            
            
            // sort support
            int b = a;
            while (_materiatory[b].Type == MateriaType.Support)
                b++;                
            MateriaBase[] tempSupport = new MateriaBase[b - a];
            Array.Copy(_materiatory, a, tempSupport, 0, b - a);
            Array.Sort<MateriaBase>(tempSupport, MateriaBase.CompareByOrder);
            Array.Copy(tempSupport, 0, _materiatory, a, b - a);
            
            // sort command
            int c = b;
            while (_materiatory[c].Type == MateriaType.Command)
                c++;
            MateriaBase[] tempCommand = new MateriaBase[c - b];
            Array.Copy(_materiatory, b, tempCommand, 0, c - b);
            Array.Sort<MateriaBase>(tempCommand, MateriaBase.CompareByOrder);
            Array.Copy(tempCommand, 0, _materiatory, b, c - b);
            
            // sort independent
            int d = c;
            while (_materiatory[d].Type == MateriaType.Independent)
                d++;
            MateriaBase[] tempIndependent = new MateriaBase[d - c];
            Array.Copy(_materiatory, c, tempIndependent, 0, d - c);
            Array.Sort<MateriaBase>(tempIndependent, MateriaBase.CompareByOrder);
            Array.Copy(tempIndependent, 0, _materiatory, c, d - c);
            
            // sort summon
            int e = d;
            while (e < MATERIATORY_SIZE && _materiatory[e] != null)
                e++;
            MateriaBase[] tempSummon = new MateriaBase[e - d];
            Array.Copy(_materiatory, d, tempSummon, 0, e - d);
            Array.Sort<MateriaBase>(tempSummon, MateriaBase.CompareByOrder);
            Array.Copy(tempSummon, 0, _materiatory, d, e - d);
            

        }
    }
}
