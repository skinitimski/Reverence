using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml;

using NLua;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Atmosphere.Reverence.Seven.Battle;
using Atmosphere.Reverence.Seven.Data;
using Atmosphere.Reverence.Seven.State;

namespace Atmosphere.Reverence.Seven
{
    public class Seven : Game
    {
        public const int SAVE_FILES = 6;


        internal Party Party { get; set; }
        internal DataStore Data { get; set; }
        private MenuState _menuState;
        private BattleState _battleState;
        private PostBattleState _postBattleState;







        private Seven(string configPath)
            : base(configPath)
        {
            Data = new DataStore(typeof(FF7).Assembly);
        }

        
        
        public static Lua GetLua()
        {            
            Lua lua = new Lua();
            lua.LoadCLRPackage();
            
            lua.DoString(@" import ('Systen') ");
            lua.DoString(@" import ('Systen.IO') ");
            lua.DoString(@" import ('Systen.Text') ");
            lua.DoString(@" import ('Systen.Text.RegularExpressions') ");

            lua.DoString(" import ('" + typeof(Seven).Assembly.GetName().Name + "') ");
            lua.DoString(Resource.GetTextFromResource("lua.scripts", typeof(Seven).Assembly));
            lua.DoString("Element = luanet.import_type(\"Atmosphere.Reverence.Seven.Asset.Element\")");
            lua.DoString("Status = luanet.import_type(\"Atmosphere.Reverence.Seven.Asset.Status\")");
            lua.DoString("Party = luanet.import_type(\"Atmosphere.Reverence.Seven.Party\")");

            return lua;
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

            Party = null;
        }

        protected override Atmosphere.Reverence.State GetInitialState()
        {
            return new InitialState(this);
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
            Party = new Party(Data);

            _menuState = new MenuState(this);
            _menuState.Init();

            SetState(_menuState);
        }



        public void SaveGame(int save)
        {
            string savefile = String.Format("savegame.{0}.xml", save);
            string path = Path.Combine(Configuration.SavePath, savefile);

            Party.SaveToFile(path);
        }

        public bool CanLoadSavedGame(int save)
        {
            return File.Exists(GetSaveFilePath(save));
        }

        public void LoadSavedGame(int save)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(Configuration.SavePath, GetSaveFilePath(save)));
            XmlNode saveGame = doc.SelectSingleNode("*");


            Party = new Party(Data, saveGame);


            _menuState = new MenuState(this);
            _menuState.Init();

            SetState(_menuState);
            //TestPostBattleState();
        }

        private String GetSaveFilePath(int save)
        {
            string savefile = String.Format("savegame.{0}.xml", save);
            
            savefile = Path.Combine(Configuration.SavePath, savefile);

            return savefile;
        }
        
        public void BeginBattle()
        {
            int i = 1; //_random.Next(2);

            String battleId = "debug" + i;
            //battleId = "gelnika.e";

            _battleState = new BattleState(this, battleId);
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

        public void LoseGame()
        {
            if (State != _battleState)
            {
                throw new ImplementationException("I don't think you can lose the game outside of battle.");
            }
            
            _battleState.Dispose();

            LossState loss = new LossState(this);
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





        
#if DEBUG
        public void TestPostBattleState()
        {
            int exp = 50000;
            int ap = 150000;
            int gil = 100000;

            System.Collections.Generic.List<IInventoryItem> items = new System.Collections.Generic.List<IInventoryItem>();
//            items.Add(Item.GetItem("titanbangle", InventoryItemType.armor));
//            items.Add(Item.GetItem("titanbangle", InventoryItemType.armor));
//            items.Add(Item.GetItem("fairytale", InventoryItemType.weapon));

            _postBattleState = new PostBattleState(this, exp, ap, gil, items);
            _postBattleState.Init();
            SetState(_postBattleState);
        }
#endif



        public static void Main(string[] args)
        {
            Seven seven = new Seven(args[0]);

            Game.RunGame(seven);
        }
    }
}

