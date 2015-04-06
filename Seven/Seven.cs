using System;
using System.IO;
using System.Xml;

using NLua;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using GameState = Atmosphere.Reverence.State;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.State;

namespace Atmosphere.Reverence.Seven
{
    public class Seven : Game
    {
        public const int SAVE_FILES = 2;


        private Party _party;
        private MenuState _menuState;
        private BattleState _battleState;
        private PostBattleState _postBattleState;



        internal static Seven Instance { get; private set; }





        private Seven(string configPath)
            : base(configPath)
        {
        }

        protected override void PrimeLua()
        {            
            LuaEnvironment.DoString(" import ('" + typeof(Seven).Assembly.GetName().Name + "') ");
            LuaEnvironment.DoString(Resource.GetTextFromResource("lua.scripts", typeof(Seven).Assembly));
            LuaEnvironment.DoString(Resource.GetTextFromResource("lua.scripts.battle", typeof(Seven).Assembly));
            LuaEnvironment.DoString("Element = luanet.import_type(\"Atmosphere.Reverence.Seven.Asset.Element\")");
            LuaEnvironment.DoString("Status = luanet.import_type(\"Atmosphere.Reverence.Seven.Asset.Status\")");
            LuaEnvironment.DoString("Party = luanet.import_type(\"Atmosphere.Reverence.Seven.Party\")");
            LuaEnvironment[typeof(Seven).Name] = this;
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

            _party = null;
        }

        protected override Atmosphere.Reverence.State GetInitialState()
        {
            return new InitialState();
        }

        public new void Reset()
        {
            base.Reset();
        }

        public new void Quit()
        {
            base.Quit();
        }



        public void LoadNewGame()
        {
            _party = new Party();


            _menuState = new MenuState();
            _menuState.Init();

            SetState(_menuState);
        }




        public void LoadSavedGame(int save)
        {

            
            XmlNode saveGame;

//            string savefile = String.Format("data.savegame{0}.xml", save);
//            saveGame = Resource.GetXmlFromResource(savefile, typeof(Seven).Assembly);
//            saveGame = saveGame.SelectSingleNode("*");

            string savefile = String.Format("savegame.{0}.xml", save);
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Configuration.SavePath, savefile));
            saveGame = doc.SelectSingleNode("*");


            _party = new Party(saveGame);


            _menuState = new MenuState();
            _menuState.Init();

            SetState(_menuState);

            //BeginBattle();
        }
        
        public void BeginBattle()
        {
            _battleState = new BattleState("debug0");
            _battleState.Init();

            LuaEnvironment[typeof(BattleState).Name] = _battleState;
            
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

        public void LoseGame()
        {
            if (State != _battleState)
            {
                throw new ImplementationException("I don't think you can lose the game outside of battle.");
            }
            
            _battleState.Dispose();

            LossState loss = new LossState();
            loss.Init();
            
            SetState(loss);
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





        public static void Main(string[] args)
        {
            Instance = new Seven(args[0]);
            Game.RunGame(Instance);
        }


        internal static GameState CurrentState { get { return Instance.State; } }
        
        internal static Lua Lua { get { return Instance.LuaEnvironment; } }
        
        internal static Config Config { get { return Instance.Configuration; } }
        
        internal static MenuState MenuState { get { return Instance._menuState; } }
        
        internal static BattleState BattleState { get { return Instance._battleState; } }
        
        internal static PostBattleState PostBattleState { get { return Instance._postBattleState; } }

        internal static Party Party { get { return Instance._party; } }
    }
}

