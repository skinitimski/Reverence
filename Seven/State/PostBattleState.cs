using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cairo;

using GameState = Atmosphere.Reverence.State;
using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Menu;
using GameMenu = Atmosphere.Reverence.Menu.Menu;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Asset.Materia;
using Screens = Atmosphere.Reverence.Seven.Screen.PostBattleState;
using LevelUp = Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory.LevelUp;
using Mastered = Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory.Mastered;
using MateriaLevelUp = Atmosphere.Reverence.Seven.Screen.PostBattleState.Victory.MateriaLevelUp;

namespace Atmosphere.Reverence.Seven.State
{
    internal class PostBattleState : GameState
    {
        public const int MS_PER_BAR_FILL = 3000;

        private int[] Exp_multiplier;
        private int Gil_multiplier;
        private State _state;
        private MenuScreen _screen;
        private bool _stopGivingExp;
        private readonly object _sync = new Object();

        #region Nested
        private enum State
        {
            BeforeGainExp,
            AfterGainExp,
            HoardScreen
        }

        #endregion



        internal PostBattleState(int exp, int ap, int gil, List<IInventoryItem> items)
        {
            Exp = exp;
            AP = ap;
            Gil = gil;
            Items = new List<Inventory.Record>();

            LevelUp = new LevelUp[Party.PARTY_SIZE];
            MateriaLevelUp = new MateriaLevelUp[Party.PARTY_SIZE];

            Exp_multiplier = new int[] { 100, 100, 100 };
            Gil_multiplier = 100;

            for (int i = 0; i < 3; i++)
            {
                if (Seven.Party[i] != null)
                {
                    foreach (MateriaOrb m in Seven.Party[i].Materia)
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

            _state = State.BeforeGainExp;
        }


        public PostBattleState(BattleState battle)
            : this(battle.Exp, battle.AP, battle.Gil, battle.Items)
        {
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
            ScreenState state = new ScreenState
            {
                Width = Seven.Config.WindowWidth,
                Height = Seven.Config.WindowHeight
            };
            



            Screens.Victory.VictoryLabel victoryLabel = new Screens.Victory.VictoryLabel(state);

            List<GameMenu> victoryMenus = new List<GameMenu>();
            victoryMenus.Add(victoryLabel);
            victoryMenus.Add(new Screens.Victory.VictoryEXP(state));
            victoryMenus.Add(new Screens.Victory.VictoryAP(state));
            victoryMenus.Add(new Screens.Victory.VictoryTop(state));
            victoryMenus.Add(new Screens.Victory.VictoryMiddle(state));
            victoryMenus.Add(new Screens.Victory.Bottom(state));

            Mastered = new Mastered(state);
            Mastered.Visible = false;
            victoryMenus.Add(Mastered);
            
            for (int i = 0; i < LevelUp.Length; i++)
            {
                LevelUp[i] = new LevelUp(state, i);
                LevelUp[i].Visible = false;
                victoryMenus.Add(LevelUp[i]);
            }
            
            for (int i = 0; i < MateriaLevelUp.Length; i++)
            {
                MateriaLevelUp[i] = new MateriaLevelUp(state, i);
                MateriaLevelUp[i].Visible = false;
                victoryMenus.Add(MateriaLevelUp[i]);
            }

            VictoryScreen = new MenuScreen(victoryMenus, victoryLabel);



            Screens.Hoard.Label hoardLabel = new Screens.Hoard.Label(state);
            HoardItemLeft = new Screens.Hoard.ItemLeft(state, Gil);
            
            List<GameMenu> hoardMenus = new List<GameMenu>();
            hoardMenus.Add(hoardLabel);
            hoardMenus.Add(new Screens.Hoard.GilLeft(state));
            hoardMenus.Add(new Screens.Hoard.HoardGilRight(state));
            hoardMenus.Add(HoardItemLeft);
            hoardMenus.Add(new Screens.Hoard.ItemRight(state));

            HoardScreen = new MenuScreen(hoardMenus, hoardLabel);

            _screen = VictoryScreen;
        }

        public override void RunIteration()
        {            
            for (int i = 0; i < LevelUp.Length; i++)
            {
                if (LevelUp[i].Visible && LevelUp[i].IsDone)
                {
                    LevelUp[i].Visible = false;
                }
            }  

            for (int i = 0; i < MateriaLevelUp.Length; i++)
            {
                if (MateriaLevelUp[i].Visible && MateriaLevelUp[i].IsDone)
                {
                    MateriaLevelUp[i].Visible = false;
                }
            }

            if (Mastered.Visible && Mastered.IsDone)
            {
                Mastered.Visible = false;
            }
        }

        public override void Draw(Gdk.Drawable d, int width, int height)
        {
            _screen.Draw(d);
        }

        public override void KeyPressHandle(Key k)
        {
            switch (_state)
            {
                case State.BeforeGainExp:

                    GiveExp();
                    _state = State.AfterGainExp;
                    break;

                case State.AfterGainExp:

                    lock (_sync)
                    {
                        _stopGivingExp = true;
                    }
                    
                    GiveExpAnimation.Join();
                    GiveExpAnimation = null;

                    for (int i = 0; i < LevelUp.Length; i++)
                    {
                        LevelUp[i].Visible = false;
                    }

                    Mastered.Visible = false;

                    _screen = HoardScreen;
                    _screen.ChangeControl(HoardItemLeft);
                    _state = State.HoardScreen;

                    break;

                case State.HoardScreen:

                    _screen.Control.ControlHandle(k);

                    break;
            }
        }

        public override void KeyReleaseHandle(Key k)
        {
        }

        private void GiveExp()
        {
            // Give AP to each materia orb attached to 
            // each character in the party

            List<MateriaOrb>[] leveledUp = new List<MateriaOrb>[Party.PARTY_SIZE];
            List<MateriaOrb> newMasters = new List<MateriaOrb>();

            for (int i = 0; i < Party.PARTY_SIZE; i++)
            {
                leveledUp[i] = new List<MateriaOrb>();

                Character c = Seven.Party[i];

                if (c != null)
                {
                    foreach (MateriaOrb m in c.Materia)
                    {
                        if (m != null && !m.Master)
                        {
                            int level = m.Level;

                            m.AddAP(AP);

                            if (m.Level > level)
                            {
                                leveledUp[i].Add(m);
                            }

                            if (m.Master)
                            {
                                newMasters.Add(m);
                            }
                        }
                    }
                }
            }

            // Each character not in the party gets half the
            // EXP that the others get (and no AP)
            foreach (Character c in Seven.Party.Reserves)
            {
                if (c != null)
                {
                    c.GainExperience(Exp / 2);
                }
            }

            // Experience is given in chunks such that the progress bars fill
            // at a constant rate. This means we dole out less exp per period
            // at lower levels, and more exp per period at higher levels.

            GiveExpAnimation = new Thread(() =>
            {
                int[] exp = new int[Party.PARTY_SIZE];
                int[] expGained = new int[Party.PARTY_SIZE];

                int refreshPeriod = Seven.Config.RefreshPeriod;
                int msPerBarFill = MS_PER_BAR_FILL;
                int periodsPerBarFill = msPerBarFill / refreshPeriod;

                for (int i = 0; i < Party.PARTY_SIZE; i++)
                {
                    Character c = Seven.Party[i];
                    
                    if (c != null && !c.Death)
                    {
                        exp[i] = Exp * Exp_multiplier[i] / 100;
                    }
                }


                while (true)
                {
                    lock (_sync)
                    {
                        if (_stopGivingExp)
                        {
                            for (int i = 0; i < Party.PARTY_SIZE; i++)
                            {    
                                Character c = Seven.Party[i];
                                
                                if (c != null && !c.Death)
                                {                            
                                    int expLeft = (exp[i] - expGained[i]);
                                    c.GainExperience(expLeft); 
                                }                       
                            }

                            break;
                        }
                    }

                    bool allDone = true;
                    
                    for (int i = 0; i < Party.PARTY_SIZE; i++)
                    {
                        if (expGained[i] < exp[i])
                        {
                            Character c = Seven.Party[i];

                            if (c != null && !c.Death)
                            {
                                int expForLevel = c.ExpNextLevel - c.ExpCurrentLevel;

                                int expChunk = expForLevel / periodsPerBarFill;

                                if (expGained[i] + expChunk > exp[i])
                                {
                                    expChunk = exp[i] - expGained[i];
                                }

                                if (c.Exp + expChunk > c.ExpNextLevel)
                                {
                                    LevelUp[i].Show();
                                }

                                expGained[i] += expChunk;
                                Seven.Party[i].GainExperience(expChunk);
                            }
                            
                            allDone = false;
                        }
                    }

                    if (allDone)
                    {
                        break;
                    }

                    System.Threading.Thread.Sleep(refreshPeriod);
                }
            });

            if (newMasters.Count > 0)
            {
                Mastered.Show(newMasters);
            }

            for (int i = 0; i < leveledUp.Length; i++)
            {
                if (leveledUp[i].Count > 0)
                {
                    MateriaLevelUp[i].Show(leveledUp[i]);
                }
            }

            GiveExpAnimation.Start();
        }

        protected override void InternalDispose()
        {    
            lock (_sync)
            {
                _stopGivingExp = true;
            }

            if (GiveExpAnimation != null)
            {                
                GiveExpAnimation.Join();
                GiveExpAnimation = null;
            }

            _screen.Dispose();
        }

        public int Exp { get; private set; }

        public int AP { get; private set; }

        public int Gil { get; set; }

        public List<Inventory.Record> Items { get; private set; }
        
        public MenuScreen VictoryScreen { get; private set; }
        
        public MenuScreen HoardScreen { get; private set; }

        public Screens.Hoard.ItemLeft HoardItemLeft { get; private set; }

        private Thread GiveExpAnimation { get; set; }
        
        private LevelUp[] LevelUp { get; set; }
        private MateriaLevelUp[] MateriaLevelUp { get; set; }

        private Mastered Mastered { get; set; }
    }
}
