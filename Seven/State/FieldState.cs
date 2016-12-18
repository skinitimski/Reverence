using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thread = System.Threading.Thread;
using ThreadStart = System.Threading.ThreadStart;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Gtk;
using Gdk;
using Cairo;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Field;
using Atmosphere.Reverence.Seven.Screen.BattleState;
using NLua;

namespace Atmosphere.Reverence.Seven.State
{    
    internal class FieldState : State
    {
        private bool _movingLeft;
        private bool _movingRight;
        private bool _movingUp;
        private bool _movingDown;

        private int _character_x;
        private int _character_y;



        private FieldState(Seven seven)
            : base(seven)
        {
        }
        
        public FieldState(Seven seven, string formationId)
            : this(seven)
        {
            Random = new Random();

            Lua = Seven.GetLua();
            Lua[typeof(FieldState).Name] = this;
        }
        
        protected override void InternalInit()
        {
            ScreenState state = new ScreenState
            {
                Width = Seven.Configuration.WindowWidth,
                Height = Seven.Configuration.WindowHeight
            };
        }





        
        public override void RunIteration()
        {
        }

        
        #region Member Methods
        
        public override string ToString()
        {
            return String.Empty;
        }
        
        #endregion Member
        
        
        
        
        
        #region Override Methods
        
        public override void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            Room.Draw(d, g, width, height, screenChanged);


        }


        
        [GLib.ConnectBefore()]
        public override void KeyPressHandle(Key k)
        {
            switch (k)
            {
                case Key.Left:
                    _movingLeft = true;
                    break;
                case Key.Right:
                    _movingRight = true;
                    break;
                case Key.Up:
                    _movingUp = true;
                    break;
                case Key.Down:
                    _movingDown = true;
                    break;
            }
        }

        [GLib.ConnectBefore()]
        public override void KeyReleaseHandle(Key k)
        {
            switch (k)
            {
                case Key.Left:
                    _movingLeft = false;
                    break;
                case Key.Right:
                    _movingRight = false;
                    break;
                case Key.Up:
                    _movingUp = false;
                    break;
                case Key.Down:
                    _movingDown = false;
                    break;
            }
        }
        
        #endregion Override
        
        
        

        

        protected override void InternalDispose()
        {          
        }


        
        
        
        #region Properties

        public Lua Lua { get; private set; }

        public Random Random { get; private set; }

        public Room Room { get; private set; }

        #endregion Properties
    }
}


