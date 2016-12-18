using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Cairo;

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
    internal class PostBattleState : State
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



        internal PostBattleState(Seven seven, int exp, int ap, int gil, List<IInventoryItem> items)
            : base(seven)
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
                if (Party[i] != null)
                {
                    foreach (MateriaOrb m in Party[i].Materia)
                    {
                        if (m != null)
                        {
                            if (m.Name == "gilplus")
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
                            if (m.Name == "expplus")
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
                Width = Seven.Configuration.WindowWidth,
                Height = Seven.Configuration.WindowHeight
            };
            



            Screens.Victory.VictoryLabel victoryLabel = new Screens.Victory.VictoryLabel(state);

            List<GameMenu> victoryMenus = new List<GameMenu>();
            victoryMenus.Add(victoryLabel);
            victoryMenus.Add(new Screens.Victory.VictoryEXP(this, state));
            victoryMenus.Add(new Screens.Victory.VictoryAP(this, state));
            victoryMenus.Add(new Screens.Victory.Top(this, state));
            victoryMenus.Add(new Screens.Victory.Middle(this, state));
            victoryMenus.Add(new Screens.Victory.Bottom(this, state));

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
            HoardItemLeft = new Screens.Hoard.ItemLeft(this, state, Gil);
            
            List<GameMenu> hoardMenus = new List<GameMenu>();
            hoardMenus.Add(hoardLabel);
            hoardMenus.Add(new Screens.Hoard.GilLeft(this, state));
            hoardMenus.Add(new Screens.Hoard.HoardGilRight(this, state));
            hoardMenus.Add(HoardItemLeft);
            hoardMenus.Add(new Screens.Hoard.ItemRight(this, state));

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

        public override void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height, bool screenChanged)
        {
            _screen.Draw(d, g, width, height, screenChanged);
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

                Character c = Party[i];

                if (c != null && !c.Death)
                {
                    foreach (MateriaOrb m in c.Materia)
                    {
                        if (m != null && !(m is EnemySkillMateria) && !m.Master)
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
            foreach (Character c in Party.Reserves)
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

                int refreshPeriod = Seven.Configuration.RefreshPeriod;
                int msPerBarFill = MS_PER_BAR_FILL;
                int periodsPerBarFill = msPerBarFill / refreshPeriod;

                for (int i = 0; i < Party.PARTY_SIZE; i++)
                {
                    Character c = Party[i];
                    
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
                                Character c = Party[i];
                                
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
                            Character c = Party[i];

                            if (c != null && !c.Death)
                            {
                                int expForLevel = c.ExpNextLevel - c.ExpCurrentLevel;

                                int expChunk = expForLevel / periodsPerBarFill;

                                if (expGained[i] + expChunk > exp[i])
                                {
                                    expChunk = exp[i] - expGained[i];
                                }

                                if (c.Exp + expChunk >= c.ExpNextLevel)
                                {
                                    LevelUp[i].Show();
                                }

                                expGained[i] += expChunk;
                                Party[i].GainExperience(expChunk);
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
