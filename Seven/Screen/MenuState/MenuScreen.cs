using System;
using System.Collections.Generic;
using System.Linq;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Seven.Screen.MenuState
{
    public sealed class MenuScreen
    {
        private IEnumerable<GameMenu> _menus;
        private IController _defaultControl;
        private IController _control;
        

        
        

        public MenuScreen(IEnumerable<GameMenu> menus, IController defaultControl)
        {
            _menus = menus;
            _defaultControl = defaultControl;
            _control = defaultControl;
            _control.SetAsControl();
        }
                
        public void Draw(Gdk.Drawable d)
        {
            foreach (GameMenu m in _menus)
            {
                m.Draw(d);
            }
        }
        
        public void ChangeControl(IController to)
        {
            //GameMenu toMenu = to as GameMenu;

//            if (toMenu != null && !_menus.Any(x => x.Equals(toMenu)))
//            {
//                throw new ImplementationException("Tried to switch control to a Menu not in this screen.");
//            }
//            else
//            {
                _control.SetNotControl();
                _control = to;
                _control.SetAsControl();
//            }
        }
        public void ChangeToDefaultControl()
        {
            ChangeControl(_defaultControl);
        }
        
        
        public IController Control { get { return _control; } }
    }
}

