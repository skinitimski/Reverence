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

//using Atmosphere.Reverence.Seven.Screen.MenuState.Equip;
//using Atmosphere.Reverence.Seven.Screen.MenuState.Hoard;
//using Atmosphere.Reverence.Seven.Screen.MenuState.Item;
//using Atmosphere.Reverence.Seven.Screen.MenuState.Main;
//using Atmosphere.Reverence.Seven.Screen.MenuState.Materia;
//using Atmosphere.Reverence.Seven.Screen.MenuState.Phs;
//using Atmosphere.Reverence.Seven.Screen.MenuState.Status;
//using Atmosphere.Reverence.Seven.Screen.MenuState.Victory;

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
        private MenuScreen _equipScreen;
        private MenuScreen _materiaScreen;
        private MenuScreen _statusScreen;
        private MenuScreen _phsScreen;
        private MenuScreen _victoryScreen;
        private MenuScreen _hoardScreen;

        
        public MenuScreen MainScreen { get { return _mainScreen; } }
        public MenuScreen ItemScreen { get { return _itemScreen; } }
        public MenuScreen EquipScreen { get { return _equipScreen; } }
        public MenuScreen MateriaScreen { get { return _materiaScreen; } }
        public MenuScreen StatusScreen { get { return _statusScreen; } }
        public MenuScreen PhsScreen { get { return _phsScreen; } }
        public MenuScreen VictoryScreen { get { return _victoryScreen; } }
        public MenuScreen HoardScreen { get { return _hoardScreen; } }

        public Screens.Main.Status MainStatus { get { return _mainStatus; } }
        public Screens.Main.Options MainOptions { get { return _mainOptions; } }



        public MenuState()
        {
            _mainStatus = new Screens.Main.Status();
            _mainOptions = new Screens.Main.Options();

            GameMenu[] mainMenus = new GameMenu[4];
            mainMenus[0] = _mainStatus;
            mainMenus[1] = _mainOptions;
            mainMenus[2] = new Screens.Main.Time();
            mainMenus[3] = new Screens.Main.Location();
            
            
            _mainScreen = new MenuScreen(mainMenus, 1);
            
            //            ItemScreen = new MenuScreen(5, Item.List.Instance);
            //            ItemScreen._menus[0] = Item.Top.Instance;
            //            ItemScreen._menus[1] = Item.Stats.Instance;
            //            ItemScreen._menus[2] = Item.List.Instance;
            //            ItemScreen._menus[3] = Item.Info.Instance;
            //            ItemScreen._menus[4] = Item.Label.Instance;
            //            
            //            EquipScreen = new MenuScreen(6, Equip.Top.Instance);
            //            EquipScreen._menus[0] = Equip.Top.Instance;
            //            EquipScreen._menus[1] = Equip.List.Instance;
            //            EquipScreen._menus[2] = Equip.Stats.Instance;
            //            EquipScreen._menus[3] = Equip.Selected.Instance;
            //            EquipScreen._menus[4] = Equip.Info.Instance;
            //            EquipScreen._menus[5] = Equip.Label.Instance;
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
            //            StatusScreen = new MenuScreen(4, Status.Label.Instance);
            //            StatusScreen._menus[0] = Status.One.Instance;
            //            StatusScreen._menus[1] = Status.Two.Instance;
            //            StatusScreen._menus[2] = Status.Three.Instance;
            //            StatusScreen._menus[3] = Status.Label.Instance;
            //            
            //            PhsScreen = new MenuScreen(5, Phs.Stats.Instance);
            //            PhsScreen._menus[0] = Phs.Top.Instance;
            //            PhsScreen._menus[1] = Phs.Stats.Instance;
            //            PhsScreen._menus[2] = Phs.List.Instance;
            //            PhsScreen._menus[3] = Phs.Info.Instance;
            //            PhsScreen._menus[4] = Phs.Label.Instance;
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
        }

        

        protected override void InternalInit()
        {
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
//                    if (Equip.Top.Instance.IsControl || 
//                        Materia.Top.Instance.IsControl ||
//                        Status.Label.Instance.IsControl)
//                        Menu.IncrementSelection();
                    break;
                case Key.R1:
                case Key.R2:
//                    if (Equip.Top.Instance.IsControl ||
//                        Materia.Top.Instance.IsControl ||
//                        Status.Label.Instance.IsControl)
//                        Menu.DecrementSelection();
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


        public MenuScreen Layer { get { return _layer; } }
    }
}
