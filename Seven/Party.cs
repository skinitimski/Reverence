using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven
{
    internal class Party
    {
        public const int PARTY_SIZE = 3;
        public const int DEFAULT_BATTLE_SPEED = 128;

        private Character[] _party = new Character[PARTY_SIZE];
        private Dictionary<string, Character> _characters;
        private int _selectionIndex;


        private Party()
        {
        }
        
        public Party(DataStore data)
            : this()
        {
            Clock = new Clock();

            InitCharacters(data);

            AddCharacters();

            _party[0] = Cloud;

            Materiatory = new Materiatory();
            Inventory = new Inventory();
            Reserves = new Character[3, 3];

            Gil = 100;
            
            BattleSpeed = DEFAULT_BATTLE_SPEED;
        }

        public Party(DataStore data, XmlNode savegame)
            : this()
        {
            InitCharacters(savegame, data);

            AddCharacters();

            for (int k = 0; k <= 2; k++)
            {
                XmlNode node = savegame.SelectSingleNode("./slot" + k.ToString());

                if (node != null)
                {
                    if (!String.IsNullOrEmpty(node.InnerText))
                    {
                        _party[k] = _characters[node.InnerText];
                    }
                }
            }

            Materiatory = new Materiatory(data, savegame);
            Inventory = new Inventory(data, savegame);
            Reserves = new Character[3, 3];
            
            int i = 0;
            int j = 0;
            
            foreach (XmlNode node in savegame.SelectNodes("./reserve"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                
                Reserves[j, i] = _characters[node.InnerXml];
                i++;
                if (i % 3 == 0)
                {
                    j++;
                    i = 0;
                }
            }

            Gil = Int32.Parse(savegame.SelectSingleNode("./gil").InnerXml);
            
            int time = Int32.Parse(savegame.SelectSingleNode("./time").InnerText);
            Clock = new Clock(time, true);
            
            BattleSpeed = Int32.Parse(savegame.SelectSingleNode("./battleSpeed").InnerText);

            

            for (int a = 0; a < 4; a++)
            {
                XmlNode cornerNode = savegame.SelectSingleNode("./config/menu/corner" + a);
                
                int r = Int32.Parse(cornerNode.Attributes["r"].Value);
                int g = Int32.Parse(cornerNode.Attributes["g"].Value);
                int b = Int32.Parse(cornerNode.Attributes["b"].Value);
                
                Menu.Menu.SetCornerColor(a, r, g, b);
            }
        }

        private void InitCharacters(XmlNode savegame, DataStore data)
        {
            XmlDocument characterData = Resource.GetXmlFromResource("data.characters.xml", data.Assembly);

            Cloud = new Character(savegame.SelectSingleNode("./characters/Cloud"), 
                                  characterData.SelectSingleNode("./characters/Cloud"), data);
            Tifa = new Character(savegame.SelectSingleNode("./characters/Tifa"), 
                                 characterData.SelectSingleNode("./characters/Tifa"), data);
            Aeris = new Character(savegame.SelectSingleNode("./characters/Aeris"),
                                  characterData.SelectSingleNode("./characters/Aeris"), data);
            Barret = new Character(savegame.SelectSingleNode("./characters/Barret"),
                                   characterData.SelectSingleNode("./characters/Barret"), data);
            RedXIII = new Character(savegame.SelectSingleNode("./characters/RedXIII"), 
                                    characterData.SelectSingleNode("./characters/RedXIII"), data);
            Yuffie = new Character(savegame.SelectSingleNode("./characters/Yuffie"), 
                                   characterData.SelectSingleNode("./characters/Yuffie"), data);
            CaitSith = new Character(savegame.SelectSingleNode("./characters/CaitSith"), 
                                     characterData.SelectSingleNode("./characters/CaitSith"), data);
            Vincent = new Character(savegame.SelectSingleNode("./characters/Vincent"), 
                                    characterData.SelectSingleNode("./characters/Vincent"), data);
            Cid = new Character(savegame.SelectSingleNode("./characters/Cid"), 
                                characterData.SelectSingleNode("./characters/Cid"), data);
            Sephiroth = new Character(savegame.SelectSingleNode("./characters/Sephiroth"), 
                                      characterData.SelectSingleNode("./characters/Sephiroth"), data);
        }
        
        private void InitCharacters(DataStore data)
        {
            XmlDocument characterData = Resource.GetXmlFromResource("data.characters.xml", data.Assembly);

            Cloud = new Character(characterData.SelectSingleNode("./characters/Cloud"), data);
            Tifa = new Character(characterData.SelectSingleNode("./characters/Tifa"), data);
            Aeris = new Character(characterData.SelectSingleNode("./characters/Aeris"), data);
            Barret = new Character(characterData.SelectSingleNode("./characters/Barret"), data);
            RedXIII = new Character(characterData.SelectSingleNode("./characters/RedXIII"), data);
            Yuffie = new Character(characterData.SelectSingleNode("./characters/Yuffie"), data);
            CaitSith = new Character(characterData.SelectSingleNode("./characters/CaitSith"), data);
            Vincent = new Character(characterData.SelectSingleNode("./characters/Vincent"), data);
            Cid = new Character(characterData.SelectSingleNode("./characters/Cid"), data);
            Sephiroth = new Character(characterData.SelectSingleNode("./characters/Sephiroth"), data);
        }

        private void AddCharacters()
        {
            _characters = new Dictionary<string, Character>();
            _characters.Add(Cloud.Name, Cloud);
            _characters.Add(Tifa.Name, Tifa);
            _characters.Add(Aeris.Name, Aeris);
            _characters.Add(Barret.Name, Barret);
            _characters.Add(RedXIII.Name, RedXIII);
            _characters.Add(Yuffie.Name, Yuffie);
            _characters.Add(CaitSith.Name, CaitSith);
            _characters.Add(Vincent.Name, Vincent);
            _characters.Add(Cid.Name, Cid);
            _characters.Add(Sephiroth.Name, Sephiroth);
        }

        public void SetSelection(int index)
        {
            _selectionIndex = index;
        }

        public void SetSelection()
        {
            if (_party[0] != null)
            {
                _selectionIndex = 0;
            }
            else if (_party[1] != null)
            {
                _selectionIndex = 1;
            }
            else if (_party[2] != null)
            {
                _selectionIndex = 2;
            }
            else
            {
                throw new ImplementationException("Party has no members.");
            }
        }

        public void IncrementSelection()
        {
            do
            {
                _selectionIndex = (_selectionIndex + 1) % PARTY_SIZE;
            } while (_party[_selectionIndex] == null);
        }

        public void DecrementSelection()
        {
            do
            {
                _selectionIndex = (_selectionIndex + 2) % PARTY_SIZE;
            } while (_party[_selectionIndex] == null);
        }






















        public void SaveToFile(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            using (XmlWriter writer = XmlWriter.Create(sw, Resource.XmlWriterSettings))
            {
                writer.WriteStartElement("party");

                writer.WriteStartElement("characters");
                Cloud.WriteToXml(writer);
                Tifa.WriteToXml(writer);
                Barret.WriteToXml(writer);
                Aeris.WriteToXml(writer);
                RedXIII.WriteToXml(writer);
                CaitSith.WriteToXml(writer);
                Yuffie.WriteToXml(writer);
                Vincent.WriteToXml(writer);
                Cid.WriteToXml(writer);
                Sephiroth.WriteToXml(writer);
                writer.WriteEndElement(); // characters

                for (int i = 0; i < PARTY_SIZE; i++)
                {
                    writer.WriteElementString("slot" + i, this[i].Name);
                }

                foreach (Character c in Reserves)
                {
                    if (c != null)
                    {
                        writer.WriteElementString("reserve", c.Name);
                    }
                }

                Inventory.WriteToXml(writer);
                Materiatory.WriteToXml(writer);

                writer.WriteElementString("gil", Gil.ToString());
                writer.WriteElementString("time", Clock.TotalMilliseconds.ToString());

                writer.WriteStartElement("config");
                writer.WriteStartElement("menu");

                for (int i = 0; i < 4; i++)
                {
                    int r, g, b;

                    Menu.Menu.GetCornerColor(i, out r, out g, out b);

                    writer.WriteStartElement("corner" + i.ToString());
                    writer.WriteAttributeString("r", r.ToString());
                    writer.WriteAttributeString("g", g.ToString());
                    writer.WriteAttributeString("b", b.ToString());
                    writer.WriteEndElement(); // corner
                }

                writer.WriteEndElement(); // menu
                writer.WriteEndElement(); // config
                        
                writer.WriteEndElement(); // party
            }
        }




        
        public int NormalSpeed()
        {
            int sum = 0;
            int count = 0;

            for (int i = 0; i < PARTY_SIZE; i++)
            {
                if (this[i] != null)
                {
                    sum += this[i].Dexterity;
                    count++;
                }
            }

            // round up
            if (sum % 2 == 1) sum++;
            
            return (sum / count) + 50;
        }





        public Character this[int index]
        {
            get { return _party[index]; }
            set { _party[index] = value; }
        }

        public Character[,] Reserves { get; private set; }

        public Character Selected
        {
            get { return this[_selectionIndex]; } 
        }



        public Character Cloud { get; private set; }

        public Character Aeris { get; private set; }

        public Character Tifa { get; private set; }

        public Character Barret { get; private set; }

        public Character RedXIII { get; private set; }

        public Character Yuffie { get; private set; }

        public Character CaitSith { get; private set; }

        public Character Vincent { get; private set; }

        public Character Cid { get; private set; }

        public Character Sephiroth { get; private set; }

        public Inventory Inventory { get; private set; }

        public Materiatory Materiatory { get; private set; }

        public int Gil { get; set; }

        public int Size { get { return PARTY_SIZE; } }

        public Clock Clock { get; private set; }

        
        /// <summary>
        /// Multiple of 16.
        /// </summary>            
        public int BattleSpeed { get; set; }
    }
}

