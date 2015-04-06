using System;
using System.Collections.Generic;
using System.Xml;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;

namespace Atmosphere.Reverence.Seven
{
    internal class Party
    {
        public const int PARTY_SIZE = 3;
        private Character[] _party = new Character[PARTY_SIZE];
        private static Dictionary<string, Character> _characters;
        private int _selectionIndex;

        private static readonly XmlDocument CHARACTER_DATA = Resource.GetXmlFromResource("data.characters.xml", typeof(Seven).Assembly);

        
        public Party()
        {
            Clock = new Clock();

            InitCharacters(CHARACTER_DATA);

            AddCharacters();

            _party[0] = Cloud;

            Materiatory = new Materiatory();
            Inventory = new Inventory();
            Reserves = new Character[3, 3];

            Gil = 100;
            
            BattleSpeed = CalculateBattleSpeed();
        }

        public Party(XmlNode savegame)
        {
            InitCharacters(savegame, CHARACTER_DATA);

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

            Materiatory = new Materiatory(savegame);
            Inventory = new Inventory(savegame);
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
            Clock = new Clock(Clock.TICKS_PER_MS, time, true);
            
            BattleSpeed = CalculateBattleSpeed();

            

            for (int a = 0; a < 4; a++)
            {
                XmlNode cornerNode = savegame.SelectSingleNode("./config/menu/corner" + a);
                
                int r = Int32.Parse(cornerNode.Attributes["r"].Value);
                int g = Int32.Parse(cornerNode.Attributes["g"].Value);
                int b = Int32.Parse(cornerNode.Attributes["b"].Value);
                
                Menu.Menu.SetCornerColor(a, r, g, b);
            }
        }

        private void InitCharacters(XmlNode savegame, XmlNode gamedata)
        {
            Cloud = new Character(savegame.SelectSingleNode("./characters/Cloud"), 
                                  gamedata.SelectSingleNode("./characters/Cloud"));
            Tifa = new Character(savegame.SelectSingleNode("./characters/Tifa"), 
                                 gamedata.SelectSingleNode("./characters/Tifa"));
            Aeris = new Character(savegame.SelectSingleNode("./characters/Aeris"),
                                  gamedata.SelectSingleNode("./characters/Aeris"));
            Barret = new Character(savegame.SelectSingleNode("./characters/Barret"),
                                   gamedata.SelectSingleNode("./characters/Barret"));
            RedXIII = new Character(savegame.SelectSingleNode("./characters/RedXIII"), 
                                    gamedata.SelectSingleNode("./characters/RedXIII"));
            Yuffie = new Character(savegame.SelectSingleNode("./characters/Yuffie"), 
                                   gamedata.SelectSingleNode("./characters/Yuffie"));
            CaitSith = new Character(savegame.SelectSingleNode("./characters/CaitSith"), 
                                     gamedata.SelectSingleNode("./characters/CaitSith"));
            Vincent = new Character(savegame.SelectSingleNode("./characters/Vincent"), 
                                    gamedata.SelectSingleNode("./characters/Vincent"));
            Cid = new Character(savegame.SelectSingleNode("./characters/Cid"), 
                                gamedata.SelectSingleNode("./characters/Cid"));
            Sephiroth = new Character(savegame.SelectSingleNode("./characters/Sephiroth"), 
                                      gamedata.SelectSingleNode("./characters/Sephiroth"));
        }
        
        private void InitCharacters(XmlNode gamedata)
        {
            Cloud = new Character(gamedata.SelectSingleNode("./characters/Cloud"));
            Tifa = new Character(gamedata.SelectSingleNode("./characters/Tifa"));
            Aeris = new Character(gamedata.SelectSingleNode("./characters/Aeris"));
            Barret = new Character(gamedata.SelectSingleNode("./characters/Barret"));
            RedXIII = new Character(gamedata.SelectSingleNode("./characters/RedXIII"));
            Yuffie = new Character(gamedata.SelectSingleNode("./characters/Yuffie"));
            CaitSith = new Character(gamedata.SelectSingleNode("./characters/CaitSith"));
            Vincent = new Character(gamedata.SelectSingleNode("./characters/Vincent"));
            Cid = new Character(gamedata.SelectSingleNode("./characters/Cid"));
            Sephiroth = new Character(gamedata.SelectSingleNode("./characters/Sephiroth"));
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





        private static int CalculateBattleSpeed()
        {
            return Clock.TICKS_PER_MS;
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

        
        /// <summary>If set to Clock.TICKS_PER_MS, realtime; if less, faster; if greater, slower</summary>
        public int BattleSpeed { get; set; }
    }
}

