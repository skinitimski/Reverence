using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    public class PostBattleState : State
    {
        private int _exp;
        private int _ap;
        private int _gil;
        private int[] _exp_multiplier;
        private int _gil_multiplier;

        private List<InventoryRecord> _items;
        private State _state;

        private bool _done = false;

        private MenuScreen _screen;


        #region Nested
        private enum State
        {
            BeforeGain,
            AfterGain,
            BeforeGive,
            AfterGive
        }
        #endregion

#if DEBUG
        internal PostBattleState(int exp, int ap, int gil, List<IItem> items)
        {
            _exp = exp;
            _ap = ap;
            _gil = gil;
            _items = new List<InventoryRecord>();

            _exp_multiplier = new int[] { 100, 100, 100 };
            _gil_multiplier = 100;

            for (int i = 0; i < 3; i++)
                if (Globals.Party[i] != null)
                    foreach (Materia m in Globals.Party[i].Materia)
                        if (m != null)
                        {
                            if (m.ID == "gilplus")
                                switch (m.Level)
                                {
                                    case 0:
                                        _gil_multiplier += 50;
                                        break;
                                    case 1:
                                        _gil_multiplier += 100;
                                        break;
                                    case 2:
                                        _gil_multiplier += 200;
                                        break;
                                }
                            if (m.ID == "expplus")
                                switch (m.Level)
                                {
                                    case 0:
                                        _exp_multiplier[i] += 50;
                                        break;
                                    case 1:
                                        _exp_multiplier[i] += 75;
                                        break;
                                    case 2:
                                        _exp_multiplier[i] += 100;
                                        break;
                                }
                        }

            CollectItems(items);

            _state = State.BeforeGain;
        }
#endif


        public PostBattleState(BattleState battle)
        {
            _exp = battle.Exp;
            _ap = battle.AP;
            _gil = battle.Gil;
            _items = new List<InventoryRecord>();

            _exp_multiplier = new int[] { 100, 100, 100 };

            for (int i = 0; i < 3; i++)
                if (battle.Allies[i] != null)
                    foreach (Materia m in battle.Allies[i].Materia)
                        if (m != null)
                        {
                            if (m.ID == "gilplus")
                                switch (m.Level)
                                {
                                    case 0:
                                        _gil_multiplier += 50;
                                        break;
                                    case 1:
                                        _gil_multiplier += 100;
                                        break;
                                    case 2:
                                        _gil_multiplier += 200;
                                        break;
                                }
                            if (m.ID == "expplus")
                                switch (m.Level)
                                {
                                    case 0:
                                        _exp_multiplier[i] += 50;
                                        break;
                                    case 1:
                                        _exp_multiplier[i] += 75;
                                        break;
                                    case 2:
                                        _exp_multiplier[i] += 100;
                                        break;
                                }
                        }

            CollectItems(battle.Items);

            _state = State.BeforeGain;
        }

        private void CollectItems(List<IItem> items)
        {
            foreach (IItem item in items)
            {
                bool added = false;
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].ID.Equals(item.ID))
                    {
                        InventoryRecord ir = _items[i];
                        ir.Count++;
                        _items[i] = ir;
                        added = true;
                        break;
                    }
                }
                if (!added)
                {
                    InventoryRecord ir = new InventoryRecord();
                    ir.Item = item;
                    ir.Count = 1;
                    _items.Add(ir);
                }
            }
        }

        protected override void InternalInit()
        {
            _screen = MenuScreen.VictoryScreen;
        }

        public override void RunIteration() { }

        public override void Draw(Gdk.Drawable d)
        {
            _screen.Draw(d);
        }
        public override void KeyPressHandle(Key k)
        {
            switch (_state)
            {
                case State.BeforeGain:
                    GiveExperience();
                    _state = State.AfterGain;
                    break;
                case State.AfterGain:
                    _screen = MenuScreen.HoardScreen;
                    MenuScreen.HoardScreen.ChangeControl(HoardItemLeft.Instance);
                    _state = State.BeforeGive;
                    break;
                case State.BeforeGive:
                    GiveGil();
                    _state = State.AfterGive;
                    break;
                case State.AfterGive:
                    _screen.Control.ControlHandle(k);
                    break;
            }
        }

        public override void KeyReleaseHandle(Key k) { }


        private void GiveExperience()
        {
            for (int i = 0; i < 3; i++)
                if (Globals.Party[i] != null)
                {
                    Character c = Globals.Party[i];
                    c.GainExperience(_exp * _exp_multiplier[i] / 100);
                    foreach (Materia m in c.Materia)
                        if (m != null)
                            m.AddAP(_ap);
                }
            foreach (Character c in Globals.Reserves)
                if (c != null)
                    c.GainExperience(_exp / 2);
        }
        private void GiveGil()
        {
            Globals.Gil += _gil * _gil_multiplier / 100;
        }

        protected override void InternalDispose()
        {
            
        }

        public int Exp { get { return _exp; } }
        public int AP { get { return _ap; } }
        public int Gil { get { return _gil; } }
        public List<InventoryRecord> Items { get { return _items; } }


    }
}
