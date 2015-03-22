using System;
using System.Xml;

using NLua;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Exceptions;
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
        private BattleState _battleState;
        private PostBattleState _postBattleState;
        private Clock _clock;



        public static readonly Seven Instance;



        static Seven()
        {
            Instance = new Seven();
        }

        private Seven()
            : base()
        {
            LuaEnvironment.DoString(" import ('" + typeof(Seven).Assembly.GetName().Name + "') ");
            LuaEnvironment.DoString(Resource.GetTextFromResource("lua.scripts", typeof(Seven).Assembly));
            LuaEnvironment.DoString("Element = luanet.import_type(\"Atmosphere.Reverence.Seven.Asset.Element\")");
        }

        protected override void Init()
        {
            LuaEnvironment["Seven"] = this;
            Weapon.LoadWeapons();
            Armor.LoadArmor();
            Accessory.LoadAccessories();
            Item.LoadItems();
            MateriaBase.LoadMateria();
            Spell.LoadSpells();
        }

        protected override void Cleanup()
        {
            if (_menuState != null)
            {
                _menuState.Dispose();
            }
            if (_battleState != null)
            {
                _battleState.Dispose();
            }
            if (_postBattleState != null)
            {
                _postBattleState.Dispose();
            }
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
//
//            SetState(_menuState);

            BeginBattle();
        }
        
        public void BeginBattle()
        {
            _battleState = new BattleState();
            _battleState.Init();
            
            SetState(_battleState);
        }
        
        public void EndBattle()
        {
            if (State != _battleState)
            {
                throw new ImplementationException("Cannot go to Post-Battle state except from Battle state");
            }

            _postBattleState = new PostBattleState(_battleState);
            _postBattleState.Init();

            _battleState.Dispose();
            
            SetState(_postBattleState);
        }
        
        public void EndPostBattle()
        {
            _postBattleState.Dispose();

            GoToMenu();
        }
        
        public void GoToMenu()
        {
            SetState(_menuState);
        }

        internal static GameState CurrentState { get { return Instance.State; } }
        
        internal static Lua Lua { get { return Instance.LuaEnvironment; } }
        
        internal static MenuState MenuState { get { return Instance._menuState; } }
        
        internal static BattleState BattleState { get { return Instance._battleState; } }
        
        internal static PostBattleState PostBattleState { get { return Instance._postBattleState; } }

        internal static Party Party { get { return Instance._party; } }

        internal static Clock Clock { get { return Instance._clock; } }
    }
}

