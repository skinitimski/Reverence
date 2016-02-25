using System;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven
{
    internal class Materiatory
    {
        public const int MATERIATORY_SIZE = 200;
        private MateriaOrb[] _materiatory;

        public Materiatory() 
        {
            _materiatory = new MateriaOrb[MATERIATORY_SIZE];
        }
        
        public Materiatory(Data data, XmlNode savegame)
            : this()
        {            
            foreach (XmlNode node in savegame.SelectNodes("./materiatory/orb"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                                
                string id = node.Attributes ["id"].Value;
                int ap = Int32.Parse(node.Attributes ["ap"].Value);
                int slot = Int32.Parse(node.Attributes ["slot"].Value);
                
                _materiatory[slot] = data.GetMateria(id, ap);
            }
        }
        
        public MateriaOrb Get(int slot)
        {
            return _materiatory [slot];
        }
        
        public void Put(MateriaOrb orb, int slot)
        {
            _materiatory [slot] = orb;
        }

        public void Put(MateriaOrb orb)
        {
            int i = 0;
            
            while (_materiatory[i] != null)
            {
                i++;
            }
            
            _materiatory [i] = orb;
        }

        
        public void Swap(Weapon from, Weapon to, Character c)
        {
            for (int i = 0; i < @from.Slots.Length; i++)
            {
                MateriaOrb m = to.Slots[i];
                
                if (m != null)
                {
                    if (i > to.Slots.Length)
                    {
                        m.Detach(c);
                        Put(m);
                    }
                    else
                    {
                        to.Slots[i] = m;
                    }
                }
                
                @from.Slots[i] = null;
            }
        }
        
        public void Swap(Armor before, Armor after, Character c)
        {
            for (int i = 0; i < before.Slots.Length; i++)
            {
                MateriaOrb m = before.Slots[i];
                
                if (m != null)
                {
                    if (i > after.Slots.Length)
                    {
                        m.Detach(c);
                        Put(m);
                    }
                    else
                    {
                        after.Slots[i] = m;
                    }
                }
                
                before.Slots[i] = null;
            }
        }

        public void Sort()
        {
            Array.Sort<MateriaOrb>(_materiatory, MateriaOrb.CompareByType);

            // HACK. We want nulls at the end, but for some reason they're not
            // getting passed to the comparison.
            int q = 0;
            while (_materiatory[q] == null)
            {
                q++;
            }

            MateriaOrb[] tempNull = new MateriaOrb[q];
            MateriaOrb[] tempNotNull = new MateriaOrb[MATERIATORY_SIZE - q];
            Array.Copy(_materiatory, 0, tempNull, 0, q);
            Array.Copy(_materiatory, q, tempNotNull, 0, MATERIATORY_SIZE - q);
            Array.Copy(tempNull, 0, _materiatory, MATERIATORY_SIZE - q, q);
            Array.Copy(tempNotNull, 0, _materiatory, 0, MATERIATORY_SIZE - q);
            // END HACK


            
            // sort magic
            int a = 0;
            while (_materiatory[a].Type == MateriaType.Magic)
            {
                a++;
            }

            MateriaOrb[] tempMagic = new MateriaOrb[a];
            Array.Copy(_materiatory, 0, tempMagic, 0, a);
            Array.Sort<MateriaOrb>(tempMagic, MateriaOrb.CompareByOrder);
            Array.Copy(tempMagic, 0, _materiatory, 0, a);
            
            
            // sort support
            int b = a;
            while (_materiatory[b].Type == MateriaType.Support)
            {
                b++;                
            }

            MateriaOrb[] tempSupport = new MateriaOrb[b - a];
            Array.Copy(_materiatory, a, tempSupport, 0, b - a);
            Array.Sort<MateriaOrb>(tempSupport, MateriaOrb.CompareByOrder);
            Array.Copy(tempSupport, 0, _materiatory, a, b - a);


            // sort command
            int c = b;
            while (_materiatory[c].Type == MateriaType.Command)
            {
                c++;
            }

            MateriaOrb[] tempCommand = new MateriaOrb[c - b];
            Array.Copy(_materiatory, b, tempCommand, 0, c - b);
            Array.Sort<MateriaOrb>(tempCommand, MateriaOrb.CompareByOrder);
            Array.Copy(tempCommand, 0, _materiatory, b, c - b);


            // sort independent
            int d = c;
            while (_materiatory[d].Type == MateriaType.Independent)
            {
                d++;
            }

            MateriaOrb[] tempIndependent = new MateriaOrb[d - c];
            Array.Copy(_materiatory, c, tempIndependent, 0, d - c);
            Array.Sort<MateriaOrb>(tempIndependent, MateriaOrb.CompareByOrder);
            Array.Copy(tempIndependent, 0, _materiatory, c, d - c);


            // sort summon
            int e = d;
            while (e < MATERIATORY_SIZE && _materiatory[e] != null)
            {
                e++;
            }

            MateriaOrb[] tempSummon = new MateriaOrb[e - d];
            Array.Copy(_materiatory, d, tempSummon, 0, e - d);
            Array.Sort<MateriaOrb>(tempSummon, MateriaOrb.CompareByOrder);
            Array.Copy(tempSummon, 0, _materiatory, d, e - d);
            

        }
        
        public void WriteToXml(XmlWriter writer)
        {
            writer.WriteStartElement(typeof(Materiatory).Name.ToLower());

            for (int i = 0; i < MATERIATORY_SIZE; i++)
            {
                MateriaOrb materia = _materiatory[i];
                if (materia != null)
                {
                    writer.WriteStartElement("orb");
                    
                    writer.WriteAttributeString("name", materia.Name);
                    writer.WriteAttributeString("type", materia.Type.ToString());
                    writer.WriteAttributeString("ap", materia.AP.ToString());
                    writer.WriteAttributeString("slot", i.ToString());
                    
                    writer.WriteEndElement();
                }
            }

            writer.WriteEndElement(); // materiatory;
        }
    }
}
