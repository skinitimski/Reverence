using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GameState = Atmosphere.Reverence.State;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Screens = Atmosphere.Reverence.Seven.Screen.PostBattleState;

namespace Atmosphere.Reverence.Seven.State
{
    internal class PostBattleState : GameState
    {
        private int[] Exp_multiplier;
        private int Gil_multiplier;
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
        public PostBattleState(int exp, int ap, int gil, List<IInventoryItem> items)
        {
            Exp = exp;
            AP = ap;
            Gil = gil;
            Items = new List<Inventory.Record>();

            Exp_multiplier = new int[] { 100, 100, 100 };
            Gil_multiplier = 100;

            for (int i = 0; i < 3; i++)
            {
                if (Seven.Party[i] != null)
                {
                    foreach (MateriaBase m in Seven.Party[i].Materia)
                    {
                        if (m != null)
                        {
                            if (m.ID == "gilplus")
                            {
                                switch (m.Level)
                                {
                                    case 0:
                                        Gil_multiplier += 50;
                                        break;
                                    case 1:
                                        Gil_multiplier += 100;
                                        break;
                                    case 2:
                                        Gil_multiplier += 200;
                                        break;
                                }
                            }
                            if (m.ID == "expplus")
                            {
                                switch (m.Level)
                                {
                                    case 0:
                                        Exp_multiplier[i] += 50;
                                        break;
                                    case 1:
                                        Exp_multiplier[i] += 75;
                                        break;
                                    case 2:
                                        Exp_multiplier[i] += 100;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            CollectItems(items);

            _state = State.BeforeGain;
        }
#endif


        public PostBattleState(BattleState battle)
        {
            Exp = battle.Exp;
            AP = battle.AP;
            Gil = battle.Gil;
            Items = new List<Inventory.Record>();

            Exp_multiplier = new int[] { 100, 100, 100 };

            for (int i = 0; i < 3; i++)
            {
                if (battle.Allies[i] != null)
                {
                    foreach (MateriaBase m in battle.Allies[i].Materia)
                    {
                        if (m != null)
                        {
                            if (m.ID == "gilplus")
                            {
                                switch (m.Level)
                                {
                                    case 0:
                                        Gil_multiplier += 50;
                                        break;
                                    case 1:
                                        Gil_multiplier += 100;
                                        break;
                                    case 2:
                                        Gil_multiplier += 200;
                                        break;
                                }
                            }
                            if (m.ID == "expplus")
                            {
                                switch (m.Level)
                                {
                                    case 0:
                                        Exp_multiplier[i] += 50;
                                        break;
                                    case 1:
                                        Exp_multiplier[i] += 75;
                                        break;
                                    case 2:
                                        Exp_multiplier[i] += 100;
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            CollectItems(battle.Items);

            _state = State.BeforeGain;
        }

        private void CollectItems(List<IInventoryItem> items)
        {
            foreach (IInventoryItem item in items)
            {
                bool added = false;

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].ID.Equals(item.ID))
                    {
                        Inventory.Record ir = Items[i];
                        ir.Count++;
                        Items[i] = ir;
                        added = true;
                        break;
                    }
                }

                if (!added)
                {
                    Inventory.Record ir = new Inventory.Record(0);
                    ir.Item = item;
                    ir.Count = 1;
                    Items.Add(ir);
                }
            }
        }

        protected override void InternalInit()
        {
            Screens.Victory.VictoryLabel victoryLabel = new Screens.Victory.VictoryLabel();

            List<GameMenu> victoryMenus = new List<GameMenu>();
            victoryMenus.Add(victoryLabel);
            victoryMenus.Add(new Screens.Victory.VictoryEXP());
            victoryMenus.Add(new Screens.Victory.VictoryAP());
            victoryMenus.Add(new Screens.Victory.VictoryTop());
            victoryMenus.Add(new Screens.Victory.VictoryMiddle());
            victoryMenus.Add(new Screens.Victory.Bottom());

            VictoryScreen = new MenuScreen(victoryMenus, victoryLabel);


            Screens.Hoard.Label hoardLabel = new Screens.Hoard.Label();
            HoardItemLeft = new Screens.Hoard.ItemLeft();
            
            List<GameMenu> hoardMenus = new List<GameMenu>();
            hoardMenus.Add(hoardLabel);
            hoardMenus.Add(new Screens.Hoard.GilLeft());
            hoardMenus.Add(new Screens.Hoard.HoardGilRight());
            hoardMenus.Add(HoardItemLeft);
            hoardMenus.Add(new Screens.Hoard.ItemRight());

            HoardScreen = new MenuScreen(hoardMenus, hoardLabel);

            _screen = VictoryScreen;
        }

        public override void RunIteration()
        {
        }

        public override void Draw(Gdk.Drawable d, int width, int height)
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
                    _screen = HoardScreen;
                    HoardScreen.ChangeControl(HoardItemLeft);
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

        public override void KeyReleaseHandle(Key k)
        {
        }

        private void GiveExperience()
        {
            for (int i = 0; i < 3; i++)
            {
                if (Seven.Party[i] != null)
                {
                    Character c = Seven.Party[i];
                    c.GainExperience(Exp * Exp_multiplier[i] / 100);
                    foreach (MateriaBase m in c.Materia)
                    {
                        if (m != null)
                        {
                            m.AddAP(AP);
                        }
                    }
                }
            }
            foreach (Character c in Seven.Party.Reserves)
            {
                if (c != null)
                {
                    c.GainExperience(Exp / 2);
                }
            }
        }

        private void GiveGil()
        {
            Seven.Party.Gil += Gil * Gil_multiplier / 100;
        }

        protected override void InternalDispose()
        {
            
        }

        public int Exp { get; private set; }

        public int AP { get; private set; }

        public int Gil { get; private set; }

        public List<Inventory.Record> Items { get; private set; }
        
        public MenuScreen VictoryScreen { get; private set; }
        
        public MenuScreen HoardScreen { get; private set; }

        public Screens.Hoard.ItemLeft HoardItemLeft { get; private set; }
    }
}
