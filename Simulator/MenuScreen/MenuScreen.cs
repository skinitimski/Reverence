using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public class MenuScreen
    {
        private Menu[] _menus;
        private IController _defaultControl;
        private IController _control;


        public static MenuScreen MainScreen;
        public static MenuScreen ItemScreen;
        public static MenuScreen EquipScreen;
        public static MenuScreen MateriaScreen;
        public static MenuScreen StatusScreen;
        public static MenuScreen PhsScreen;
        public static MenuScreen VictoryScreen;
        public static MenuScreen HoardScreen;



        public MenuScreen(int menus, IController control)
        {
            _menus = new Menu[menus];
            _defaultControl = control;
            _control = control;
            _control.SetAsControl();
        }

        public static void Init()
        {  
            MainScreen = new MenuScreen(4, MainOptions.Instance);
            MainScreen._menus[0] = MainStatus.Instance;
            MainScreen._menus[1] = MainOptions.Instance;
            MainScreen._menus[2] = MainTime.Instance;
            MainScreen._menus[3] = MainLocation.Instance;

            ItemScreen = new MenuScreen(5, ItemList.Instance);
            ItemScreen._menus[0] = ItemTop.Instance;
            ItemScreen._menus[1] = ItemStats.Instance;
            ItemScreen._menus[2] = ItemList.Instance;
            ItemScreen._menus[3] = ItemInfo.Instance;
            ItemScreen._menus[4] = ItemLabel.Instance;

            EquipScreen = new MenuScreen(6, EquipTop.Instance);
            EquipScreen._menus[0] = EquipTop.Instance;
            EquipScreen._menus[1] = EquipList.Instance;
            EquipScreen._menus[2] = EquipStats.Instance;
            EquipScreen._menus[3] = EquipSelected.Instance;
            EquipScreen._menus[4] = EquipInfo.Instance;
            EquipScreen._menus[5] = EquipLabel.Instance;

            MateriaScreen = new MenuScreen(7, MateriaTop.Instance);
            MateriaScreen._menus[0] = MateriaTop.Instance;
            MateriaScreen._menus[1] = MateriaStats.Instance;
            MateriaScreen._menus[2] = MateriaList.Instance;
            MateriaScreen._menus[3] = MateriaInfo.Instance;
            MateriaScreen._menus[4] = MateriaLabel.Instance;
            MateriaScreen._menus[5] = MateriaArrange.Instance;
            MateriaScreen._menus[6] = MateriaPrompt.Instance;

            StatusScreen = new MenuScreen(4, StatusLabel.Instance);
            StatusScreen._menus[0] = StatusOne.Instance;
            StatusScreen._menus[1] = StatusTwo.Instance;
            StatusScreen._menus[2] = StatusThree.Instance;
            StatusScreen._menus[3] = StatusLabel.Instance;

            PhsScreen = new MenuScreen(5, PHSStats.Instance);
            PhsScreen._menus[0] = PHSTop.Instance;
            PhsScreen._menus[1] = PHSStats.Instance;
            PhsScreen._menus[2] = PHSList.Instance;
            PhsScreen._menus[3] = PHSInfo.Instance;
            PhsScreen._menus[4] = PHSLabel.Instance;

            VictoryScreen = new MenuScreen(6, VictoryLabel.Instance);
            VictoryScreen._menus[0] = VictoryLabel.Instance;
            VictoryScreen._menus[1] = VictoryEXP.Instance;
            VictoryScreen._menus[2] = VictoryAP.Instance;
            VictoryScreen._menus[3] = VictoryTop.Instance;
            VictoryScreen._menus[4] = VictoryMiddle.Instance;
            VictoryScreen._menus[5] = VictoryBottom.Instance;

            HoardScreen = new MenuScreen(5, HoardLabel.Instance);
            HoardScreen._menus[0] = HoardLabel.Instance;
            HoardScreen._menus[1] = HoardGilLeft.Instance;
            HoardScreen._menus[2] = HoardGilRight.Instance;
            HoardScreen._menus[3] = HoardItemLeft.Instance;
            HoardScreen._menus[4] = HoardItemRight.Instance;

            Item.Target = Globals.Party[0];

            Menu.SetSelection();
        }

        public void Draw(Gdk.Drawable d)
        {
            foreach (Menu m in _menus) m.Draw(d);
        }

        public void ChangeControl(IController to)
        {
            Menu toMenu = to as Menu;
            if (toMenu != null && !_menus.Contains<Menu>(toMenu))
                throw new GameImplementationException("Tried to switch control to a Menu not in this screen.");
            else
            {
                _control.SetNotControl();
                _control = to;
                _control.SetAsControl();
            }
        }
        public void ChangeToDefaultControl()
        {
            ChangeControl(_defaultControl);
        }


        public IController Control { get { return _control; } }
        public Menu[] Menus { get { return _menus; } }

    }




}
