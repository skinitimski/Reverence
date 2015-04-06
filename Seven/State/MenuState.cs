﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Atmosphere.Reverence;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Exceptions;
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
        public MenuState()
        {
        }

        

        protected override void InternalInit()
        {
            //
            // MAIN MENU
            // 
            
            MainStatus = new Screens.Main.Status();
            MainOptions = new Screens.Main.Options();
            
            List<GameMenu> mainMenus = new List<GameMenu>();
            mainMenus.Add(MainStatus);
            mainMenus.Add(MainOptions);
            mainMenus.Add(new Screens.Main.Time());
            mainMenus.Add(new Screens.Main.Location());
            
            MainScreen = new MenuScreen(mainMenus, MainOptions);



            // 
            // ITEM
            //
                        
            ItemTop = new Screens.Item.Top();
            ItemStats = new Screens.Item.Stats();
            ItemList = new Screens.Item.List();

            List<GameMenu> itemMenus = new List<GameMenu>();
            itemMenus.Add(ItemTop);
            itemMenus.Add(ItemStats);
            itemMenus.Add(ItemList);
            itemMenus.Add(new Screens.Item.Info());
            itemMenus.Add(new Screens.Item.Label());

            ItemScreen = new MenuScreen(itemMenus, ItemTop);


            //
            // MATERIA
            //

            MateriaTop = new Screens.Materia.Top();
            MateriaList = new Screens.Materia.List();
            MateriaArrange = new Screens.Materia.Arrange();
            MateriaPrompt = new Screens.Materia.Prompt();

            List<GameMenu> materiaMenus  = new List<GameMenu>();
            materiaMenus.Add(MateriaTop);
            materiaMenus.Add(MateriaList);
            materiaMenus.Add(new Screens.Materia.Stats());
            materiaMenus.Add(new Screens.Materia.Info());
            materiaMenus.Add(new Screens.Materia.Label());
            materiaMenus.Add(MateriaArrange);
            materiaMenus.Add(MateriaPrompt);

            MateriaScreen = new MenuScreen(materiaMenus, MateriaTop);


            //
            // EQUIP
            //

            EquipTop = new Screens.Equip.Top();
            EquipList = new Screens.Equip.List();

            List<GameMenu> equipMenus =  new List<GameMenu>();
            equipMenus.Add(EquipTop);
            equipMenus.Add(EquipList);
            equipMenus.Add(new Screens.Equip.Stats());
            equipMenus.Add(new Screens.Equip.Selected());
            equipMenus.Add(new Screens.Equip.Info());
            equipMenus.Add(new Screens.Equip.Label());

            EquipScreen = new MenuScreen(equipMenus, EquipTop);


            //
            // STATUS
            //

            StatusOne = new Screens.Status.One();
            StatusTwo = new Screens.Status.Two();
            StatusThree = new Screens.Status.Three();
            StatusLabel = new Screens.Status.Label();

            List<GameMenu>  statusMenus =  new List<GameMenu>();
            statusMenus.Add(StatusOne);
            statusMenus.Add(StatusTwo);
            statusMenus.Add(StatusThree);
            statusMenus.Add(StatusLabel);

            StatusScreen = new MenuScreen(statusMenus, StatusLabel);
            
            
            //
            // CONFIG
            //
            
            ConfigMain = new Screens.Config.Main();
            
            List<GameMenu> configMenus = new List<GameMenu>();
            configMenus.Add(new Screens.Config.Info());
            configMenus.Add(new Screens.Config.Label());
            configMenus.Add(ConfigMain);

            ConfigScreen = new MenuScreen(configMenus, ConfigMain);
            
            
            //
            // PHS
            //
            
            PhsList = new Screens.Phs.List();
            PhsStats = new Screens.Phs.Stats();
            
            List<GameMenu> phsMenus = new List<GameMenu>();
            phsMenus.Add(new Screens.Phs.Top());
            phsMenus.Add(PhsStats);
            phsMenus.Add(PhsList);
            phsMenus.Add(new Screens.Phs.Info());
            phsMenus.Add(new Screens.Phs.Label());
            
            PhsScreen = new MenuScreen(phsMenus, PhsStats);
            
            
            //
            // SAVE
            //

            SavePrompt = new Screens.Save.Prompt();
            SaveConfirm = new Screens.Save.Confirm();

            List<GameMenu> saveMenus = new List<GameMenu>();
            saveMenus.Add(new Screens.Save.Label());
            saveMenus.Add(SavePrompt);
            saveMenus.Add(SaveConfirm);

            SaveScreen = new MenuScreen(saveMenus, SavePrompt);

            
            ActiveLayer = MainScreen;
            ActiveLayer.Control.SetAsControl();
        }



        public override void Draw(Gdk.Drawable d, int width, int height)
        {
            try
            {
                ActiveLayer.Draw(d);

                if (MessageBox != null)
                {
                    MessageBox.Draw(d);
                }
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error drawing active menu layer.", e);
            }
        }

        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
        {
                if (MessageBox != null)
                {
                    return;
                }

            switch (k)
            {
                case Key.L1:
                case Key.L2:
                    if (EquipTop.IsControl || 
                        MateriaTop.IsControl ||
                        StatusLabel.IsControl)
                    {
                        Seven.Party.IncrementSelection();
                    }
                    break;
                case Key.R1:
                case Key.R2:
                    if (EquipTop.IsControl || 
                        MateriaTop.IsControl ||
                        StatusLabel.IsControl)
                    {
                        Seven.Party.DecrementSelection();
                    }
                    break;
                default:
                    break;
            }

            ActiveLayer.Control.ControlHandle(k);
        }

        [GLib.ConnectBefore()]
        public override void KeyReleaseHandle(Key k)
        {
        }


        public override void RunIteration()
        {
            if (MessageBox != null && MessageBox.Timer.IsUp)
            {
                MessageBox = null;
            }
        }
        


        public void ChangeScreen(MenuScreen el)
        {
            ActiveLayer.Control.SetNotControl();
            ActiveLayer = el;
            el.ChangeToDefaultControl();
        }


        public void ShowMessage(TimedDialogue message)
        {
            MessageBox = new MessageBox(message);                
        }

        public void UpdateAllBackgrounds()
        {
            MainScreen.UpdateBackground();
            ItemScreen.UpdateBackground();
            //MagicScreen.UpdateBackground();
            MateriaScreen.UpdateBackground();
            EquipScreen.UpdateBackground();
            StatusScreen.UpdateBackground();
            ConfigScreen.UpdateBackground();
            PhsScreen.UpdateBackground();
            SaveScreen.UpdateBackground();
        }

        

        protected override void InternalDispose() { }




        private MessageBox MessageBox { get; set; }




        public MenuScreen ActiveLayer { get; private set; }


        
        
        
        public MenuScreen MainScreen { get; private set; }
        public MenuScreen ItemScreen { get; private set; }
        //public MenuScreen MagicScreen { get; private set; }
        public MenuScreen MateriaScreen { get; private set; }
        public MenuScreen EquipScreen { get; private set; }
        public MenuScreen StatusScreen { get; private set; }
        //public MenuScreen LimitScreen { get; private set; }
        public MenuScreen ConfigScreen { get; private set; }
        public MenuScreen PhsScreen { get; private set; }
        public MenuScreen SaveScreen { get; private set; }
        
        
        
        public Screens.Main.Options MainOptions { get; private set; }
        public Screens.Main.Status MainStatus { get; private set; }
        
        public Screens.Item.Top ItemTop { get; private set; }
        public Screens.Item.List ItemList { get; private set; }
        public Screens.Item.Stats ItemStats { get; private set; }
        
        public Screens.Materia.Top MateriaTop { get; private set; }
        public Screens.Materia.List MateriaList { get; private set; }
        public Screens.Materia.Arrange MateriaArrange { get; private set; }
        public Screens.Materia.Prompt MateriaPrompt { get; private set; }
        
        public Screens.Equip.Top EquipTop { get; private set; }
        public Screens.Equip.List EquipList { get; private set; }
        
        public Screens.Status.One StatusOne { get; private set; }
        public Screens.Status.Two StatusTwo { get; private set; }
        public Screens.Status.Three StatusThree { get; private set; }
        public Screens.Status.Label StatusLabel { get; private set; }

        public Screens.Config.Main ConfigMain { get; private set; }
        
        public Screens.Phs.List PhsList { get; private set; }
        public Screens.Phs.Stats PhsStats { get; private set; }

        public Screens.Save.Prompt SavePrompt { get; private set; }
        public Screens.Save.Confirm SaveConfirm { get; private set; }
    }
}
