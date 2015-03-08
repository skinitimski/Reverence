using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public class MenuState : State
    {
        // Singleton

        private MenuScreen _layer;



        public MenuState()
        {
        }

        

        protected override void InternalInit()
        {
            _layer = MenuScreen.MainScreen;
            _layer.Control.SetAsControl();
        }



        public override void Draw(Gdk.Drawable d)
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
                    if (EquipTop.Instance.IsControl || 
                        MateriaTop.Instance.IsControl ||
                        StatusLabel.Instance.IsControl)
                        Menu.IncrementSelection();
                    break;
                case Key.R1:
                case Key.R2:
                    if (EquipTop.Instance.IsControl ||
                        MateriaTop.Instance.IsControl ||
                        StatusLabel.Instance.IsControl)
                        Menu.DecrementSelection();
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
