using System;
using System.Xml;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.State;

namespace Atmosphere.Reverence.Seven
{
    public class Seven : Game
    {
        private Party _party;
        private MenuState _menuState;
        private Clock _clock;

        private XmlDocument _saveGame;
        
        public static readonly Seven Instance;





        static Seven()
        {
            Instance = new Seven();
        }

        private Seven()
            : base()
        {


        }




        protected override Atmosphere.Reverence.State GetInitialState()
        {
            return new InitialState();
        }

        public void LoadSavedGame()
        {
            _saveGame = Resource.GetXmlFromResource("data.savegame.xml", typeof(Seven).Assembly);

            _party = new Party(_saveGame);

            int time = Int32.Parse(_saveGame.SelectSingleNode("//time").InnerText);
            _clock = new Clock(time); // depends on Globals ctor

            _menuState = new MenuState();

            SetState(_menuState);
        }



        internal static MenuState MenuState { get { return Instance._menuState; } }

        internal static Party Party { get { return Instance._party; } }

        internal static Clock Clock { get { return Instance._clock; } }
        
        internal static XmlDocument SaveGame { get { return Instance._saveGame; } }
    }
}

