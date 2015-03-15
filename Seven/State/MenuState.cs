using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Menu;
using GameState = Atmosphere.Reverence.State;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using Atmosphere.Reverence.Seven.Screen.MenuState;
using Screens = Atmosphere.Reverence.Seven.Screen.MenuState;

using Atmosphere.Reverence.Seven.Screen.MenuState.Main;

namespace Atmosphere.Reverence.Seven.State
{
    internal class MenuState : GameState
    {
        // Singleton

        private MenuScreen _layer;

        
        private MenuScreen _mainScreen;
        private Screens.Main.Status _mainStatus;
        private Screens.Main.Options _mainOptions;

        private MenuScreen _itemScreen;
        
        private MenuScreen _magicScreen;
        private MenuScreen _materiaScreen;

        private MenuScreen _equipScreen;
        private Screens.Equip.Top _equipTop;
        private Screens.Equip.List _equipList;


        private MenuScreen _statusScreen;
        private Screens.Status.One _statusOne;
        private Screens.Status.Two _statusTwo;
        private Screens.Status.Three _statusThree;
        private Screens.Status.Label _statusLabel;

        private MenuScreen _phsScreen;
        private Screens.Phs.List _phsList;
        private Screens.Phs.Stats _phsStats;


        private MenuScreen _victoryScreen;
        private MenuScreen _hoardScreen;



        public MenuState()
        {
        }

        

        protected override void InternalInit()
        {
            //
            // MAIN MENU
            // 
            
            _mainStatus = new Screens.Main.Status();
            _mainOptions = new Screens.Main.Options();
            
            List<GameMenu> mainMenus = new List<GameMenu>();
            mainMenus.Add(_mainStatus);
            mainMenus.Add(_mainOptions);
            mainMenus.Add(new Screens.Main.Time());
            mainMenus.Add(new Screens.Main.Location());
            
            _mainScreen = new MenuScreen(mainMenus, _mainOptions);


            //
            // EQUIP
            //

            _equipTop = new Screens.Equip.Top();
            _equipList = new Screens.Equip.List();

            List<GameMenu> equipMenus =  new List<GameMenu>();
            equipMenus.Add(_equipTop);
            equipMenus.Add(_equipList);
            equipMenus.Add(new Screens.Equip.Stats());
            equipMenus.Add(new Screens.Equip.Selected());
            equipMenus.Add(new Screens.Equip.Info());
            equipMenus.Add(new Screens.Equip.Label());

            _equipScreen = new MenuScreen(equipMenus, _equipTop);
            
            //            EquipScreen = new MenuScreen(6, Equip.Top.Instance);
            //            EquipScreen._menus[0] = Equip.Top.Instance;
            //            EquipScreen._menus[1] = Equip.List.Instance;
            //            EquipScreen._menus[2] = Equip.Stats.Instance;
            //            EquipScreen._menus[3] = Equip.Selected.Instance;
            //            EquipScreen._menus[4] = Equip.Info.Instance;
            //            EquipScreen._menus[5] = Equip.Label.Instance;


            //
            // STATUS
            //

            _statusOne = new Screens.Status.One();
            _statusTwo = new Screens.Status.Two();
            _statusThree = new Screens.Status.Three();
            _statusLabel = new Screens.Status.Label();

            List<GameMenu>  statusMenus =  new List<GameMenu>();
            statusMenus.Add(_statusOne);
            statusMenus.Add(_statusTwo);
            statusMenus.Add(_statusThree);
            statusMenus.Add(_statusLabel);

            _statusScreen = new MenuScreen(statusMenus, _statusLabel);

            
            //
            // PHS
            //
            
            _phsList = new Screens.Phs.List();
            _phsStats = new Screens.Phs.Stats();
            
            List<GameMenu> phsMenus = new List<GameMenu>();
            phsMenus.Add(new Screens.Phs.Top());
            phsMenus.Add(_phsStats);
            phsMenus.Add(_phsList);
            phsMenus.Add(new Screens.Phs.Info());
            phsMenus.Add(new Screens.Phs.Label());
            
            _phsScreen = new MenuScreen(phsMenus, _phsStats);
            
            
            
            
            //            ItemScreen = new MenuScreen(5, Item.List.Instance);
            //            ItemScreen._menus[0] = Item.Top.Instance;
            //            ItemScreen._menus[1] = Item.Stats.Instance;
            //            ItemScreen._menus[2] = Item.List.Instance;
            //            ItemScreen._menus[3] = Item.Info.Instance;
            //            ItemScreen._menus[4] = Item.Label.Instance;
            //            
            //            
            //            MateriaScreen = new MenuScreen(7, Materia.Top.Instance);
            //            MateriaScreen._menus[0] = Materia.Top.Instance;
            //            MateriaScreen._menus[1] = Materia.Stats.Instance;
            //            MateriaScreen._menus[2] = Materia.List.Instance;
            //            MateriaScreen._menus[3] = Materia.Info.Instance;
            //            MateriaScreen._menus[4] = Materia.Label.Instance;
            //            MateriaScreen._menus[5] = Materia.Arrange.Instance;
            //            MateriaScreen._menus[6] = Materia.Prompt.Instance;
            //            
            //            
            //
            //            VictoryScreen = new MenuScreen(6, Victory.Label.Instance);
            //            VictoryScreen._menus[0] = Victory.Label.Instance;
            //            VictoryScreen._menus[1] = Victory.Exp.Instance;
            //            VictoryScreen._menus[2] = Victory.AP.Instance;
            //            VictoryScreen._menus[3] = Victory.Top.Instance;
            //            VictoryScreen._menus[4] = Victory.Middle.Instance;
            //            VictoryScreen._menus[5] = Victory.Bottom.Instance;
            //
            //            HoardScreen = new MenuScreen(5, Hoard.Label.Instance);
            //            HoardScreen._menus[0] = Hoard.Label.Instance;
            //            HoardScreen._menus[1] = Hoard.GilLeft.Instance;
            //            HoardScreen._menus[2] = Hoard.GilRight.Instance;
            //            HoardScreen._menus[3] = Hoard.ItemLeft.Instance;
            //            HoardScreen._menus[4] = Hoard.ItemRight.Instance;
            
            //Menu.SetSelection();
            
            _layer = MainScreen;
            _layer.Control.SetAsControl();
        }



        public override void Draw(Gdk.Drawable d, int width, int height)
        {
            _layer.Draw(d);
        }

        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
        {
            switch (k)
            {
                case Key.L1:
                case Key.L2:
                    if (//_equipTop.IsControl || 
                        //Materia.Top.Instance.IsControl ||
                        _statusLabel.IsControl)
                    {
                        Seven.Party.IncrementSelection();
                    }
                    break;
                case Key.R1:
                case Key.R2:
                    if (//_equipTop.IsControl || 
                        //Materia.Top.Instance.IsControl ||
                        _statusLabel.IsControl)
                    {
                        Seven.Party.DecrementSelection();
                    }
                    break;
                default:
                    break;
            }

            _layer.Control.ControlHandle(k);
        }
        [GLib.ConnectBefore()]
        public override void KeyReleaseHandle(Key k)
        {
            switch (k)
            {
                default:
                    break;
            }
        }

        public override void RunIteration() { }
        


        public void ChangeScreen(MenuScreen el)
        {
            _layer.Control.SetNotControl();
            _layer = el;
            el.ChangeToDefaultControl();
        }


        

        protected override void InternalDispose() { }





        
        
        
        public MenuScreen MainScreen { get { return _mainScreen; } }
        public MenuScreen ItemScreen { get { return _itemScreen; } }
        public MenuScreen MagicScreen { get { return _magicScreen; } }
        public MenuScreen MateriaScreen { get { return _materiaScreen; } }
        public MenuScreen EquipScreen { get { return _equipScreen; } }
        public MenuScreen StatusScreen { get { return _statusScreen; } }
        public MenuScreen PhsScreen { get { return _phsScreen; } }
        public MenuScreen VictoryScreen { get { return _victoryScreen; } }
        public MenuScreen HoardScreen { get { return _hoardScreen; } }
        
        
        
        public Screens.Main.Status MainStatus { get { return _mainStatus; } }
        public Screens.Main.Options MainOptions { get { return _mainOptions; } }

        public Screens.Equip.Top EquipTop { get { return _equipTop; } }
        public Screens.Equip.List EquipList { get { return _equipList; } }
        
        public Screens.Status.One StatusOne { get { return _statusOne; } }
        public Screens.Status.Two StatusTwo { get { return _statusTwo; } }
        public Screens.Status.Three StatusThree { get { return _statusThree; } }
        
        public Screens.Phs.List PhsList { get { return _phsList; } }
        public Screens.Phs.Stats PhsStats { get { return _phsStats; } }
    }
}
