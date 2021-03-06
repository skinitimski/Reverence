using System;
using System.Collections.Generic;

using GameMenu = Atmosphere.Reverence.Menu.Menu;

namespace Atmosphere.Reverence.Menu
{
    public sealed class MenuScreen : IDisposable
    {
        private IEnumerable<GameMenu> _menus;
        private IController _defaultControl;
        private IController _control;
        
        
        
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Atmosphere.Reverence.Seven.Screen.MenuState.MenuScreen"/> class.
        /// </summary>
        /// <param name='menus'>
        /// The menus in the screen. The order given dictates the order in which 
        /// they are draw on the screen, from first to last.
        /// </param>
        /// <param name='defaultControl'>
        /// Default control.
        /// </param>
        public MenuScreen(IEnumerable<GameMenu> menus, IController defaultControl)
        {
            _menus = menus;
            _defaultControl = defaultControl;
            _control = defaultControl;
        }
        
        public void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            foreach (GameMenu m in _menus)
            {
                m.Draw(d, g, width, height, screenChanged);
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

        public void UpdateBackground()
        {
            foreach (GameMenu m in _menus)
            {
                m.Visible = m.Visible;
            }
        }

        public void Dispose()
        {
            foreach (Menu menu in _menus)
            {
                menu.Dispose();
            }
        }
        
        
        public IController Control { get { return _control; } }
    }
}

