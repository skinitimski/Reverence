using System;
using System.Linq;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState
{
    public class MenuScreen
    {
        private GameMenu[] _menus;
        private IController _defaultControl;
        private IController _control;
        

        
        

        public MenuScreen(GameMenu[] menus, int controllerIndex)
        {
            if (!(menus [controllerIndex] is IController))
            {
                throw new ImplementationException("Given controlling menu is not an {0}: {1}", typeof(IController).Name, menus[controllerIndex]);
            }

            _menus = menus;
            _defaultControl = (IController)menus[controllerIndex];
            _control = (IController)menus[controllerIndex];
            _control.SetAsControl();
        }
                
        public void Draw(Gdk.Drawable d)
        {
            foreach (GameMenu m in _menus) m.Draw(d);
        }
        
        public void ChangeControl(IController to)
        {
            GameMenu toMenu = to as GameMenu;

            if (toMenu != null && !_menus.Contains<GameMenu>(toMenu))
            {
                throw new ImplementationException("Tried to switch control to a Menu not in this screen.");
            }
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
        public GameMenu[] Menus { get { return _menus; } }
    }
}

