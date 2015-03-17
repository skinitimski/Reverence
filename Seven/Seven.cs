using System;
using System.Xml;

using NLua;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Time;
using GameState = Atmosphere.Reverence.State;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.State;

namespace Atmosphere.Reverence.Seven
{
    public class Seven : Game
    {
        private Party _party;
        private MenuState _menuState;
        private Clock _clock;



        public static readonly Seven Instance;



        static Seven()
        {
            Instance = new Seven();
        }

        private Seven()
            : base()
        {


        }

        protected override void Init()
        {
            Weapon.LoadWeapons();
            Armor.LoadArmor();
            Accessory.LoadAccessories();
            Item.LoadItems();
            MateriaBase.LoadMateria();
            Spell.LoadSpells();
        }

        protected override Atmosphere.Reverence.State GetInitialState()
        {
            return new InitialState();
        }

        public void LoadSavedGame()
        {
            XmlNode saveGame = Resource.GetXmlFromResource("data.savegame.xml", typeof(Seven).Assembly);
            saveGame = saveGame.SelectSingleNode("*");

            _party = new Party(saveGame);

            int time = Int32.Parse(saveGame.SelectSingleNode("./time").InnerText);
            _clock = new Clock(Clock.TICKS_PER_MS, time, true);

            _menuState = new MenuState();
            _menuState.Init();

            SetState(_menuState);
        }

        internal static GameState CurrentState { get { return Instance.State; } }
        
        internal static Lua Lua { get { return Instance.LuaEnvironment; } }

        internal static MenuState MenuState { get { return Instance._menuState; } }

        internal static Party Party { get { return Instance._party; } }

        internal static Clock Clock { get { return Instance._clock; } }
    }
}

