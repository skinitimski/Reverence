using System;
using System.Collections.Generic;
using System.Xml;

using Atmosphere.Reverence.Exceptions;

namespace Atmosphere.Reverence.Seven
{
    internal class Party
    {
        private Character[] _party = new Character[3];
        private static Dictionary<string, Character> _characters;
        private int _selectionIndex;
        
        public Party()
        {
            XmlDocument gamedata = Resource.GetXmlFromResource("data.characters.xml", typeof(Seven).Assembly);

            InitCharacters(gamedata);

            AddCharacters();

            _party [0] = Cloud;
            
            Materiatory = new Materiatory();
            Inventory = new Inventory();
            Reserves = new Character[3, 3];

            Gil = 100;
        }

        public Party(XmlNode savegame)
        {
            XmlDocument gamedata = Resource.GetXmlFromResource("data.characters.xml", typeof(Seven).Assembly);

            InitCharacters(savegame, gamedata);

            AddCharacters();



            for (int k = 1; k <= 3; k++)
            {
                string name = savegame.SelectSingleNode("./party/slot" + k.ToString()).InnerXml;
                
                if (!String.IsNullOrEmpty(name))
                {
                    _party [k - 1] = _characters [name];
                }
            }
            
            Materiatory = new Materiatory(savegame);
            Inventory = new Inventory(savegame);
            Reserves = new Character[3, 3];
            
            int i = 0;
            int j = 0;
            
            foreach (XmlNode node in savegame.SelectNodes("./party/reserve"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;
                
                Reserves [j, i] = _characters [node.InnerXml];
                i++;
                if (i % 3 == 0)
                {
                    j++;
                    i = 0;
                }
            }
            
            Gil = Int32.Parse(savegame.SelectSingleNode("./gil").InnerXml);
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
            Cloud = new Character(gamedata.SelectSingleNode("./Cloud"));
            Tifa = new Character(gamedata.SelectSingleNode("./Tifa"));
            Aeris = new Character(gamedata.SelectSingleNode("./Aeris"));
            Barret = new Character(gamedata.SelectSingleNode("./Barret"));
            RedXIII = new Character(gamedata.SelectSingleNode("./RedXIII"));
            Yuffie = new Character(gamedata.SelectSingleNode("./Yuffie"));
            CaitSith = new Character(gamedata.SelectSingleNode("./CaitSith"));
            Vincent = new Character(gamedata.SelectSingleNode("./Vincent"));
            Cid = new Character(gamedata.SelectSingleNode("./Cid"));
            Sephiroth = new Character(gamedata.SelectSingleNode("./Sephiroth"));
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
            if (_party [0] != null)
            {
                _selectionIndex = 0;
            }
            else if (_party [1] != null)
            {
                _selectionIndex = 1;
            }
            else if (_party [2] != null)
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
                _selectionIndex = (_selectionIndex + 1) % 3;
            }
            while (_party[_selectionIndex] == null);
        }

        public void DecrementSelection()
        {
            do
            {
                _selectionIndex = (_selectionIndex + 2) % 3;
            }
            while (_party[_selectionIndex] == null);
        }

        public Character this [int index]
        {
            get { return _party [index]; }
            set { _party [index] = value; }
        }

        public Character[,] Reserves { get; private set; }

        public Character Selected
        {
            get { return this [_selectionIndex]; } 
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
    }
}

