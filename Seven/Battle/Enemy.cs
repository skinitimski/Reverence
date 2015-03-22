using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class Enemy : Combatant
    {
        //     Name: SOLDIER:3rd
        // Lvl: 13         EXP: 54            Win: [ 8] Loco weed
        //  HP: 250         AP: 6           Steal: [ 8] Hardedge
        //  MP: 40         Gil: 116         Morph:      Bolt Plume
        // ---
        //Weak:   Fire, 'Confusion' Status Abilities
        // ---
        // Att: 27         Def: 38          Df%: 12          Dex: 60
        // MAt: 8          MDf: 72                           Lck: 8
        
        #region Nested
        
        private struct EnemyItem
        {
            private IInventoryItem _item;
            private int _chance;
            
            public EnemyItem(string xmlstring)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)));
                
                XmlNode node = xml.DocumentElement;
                
                string id = node.Attributes["id"].Value;
                string type = node.Attributes["type"].Value;
                _chance = Int32.Parse(node.Attributes["chance"].Value);
                
                _item = Asset.Item.GetItem(id, type);
            }
            
            public IInventoryItem Item { get { return _item; } }
            public int Chance { get { return _chance; } }
        }
        
        #endregion Nested
        
        
        #region Member Data
        
        private int _level;
        
        private int _hp;
        private int _mp;
        private int _maxhp;
        private int _maxmp;
        
        private int _exp;
        private int _ap;
        private int _gil;
        
        private int _attack;
        private int _defense;
        private int _defensePercent;
        private int _dexterity;
        private int _magicAttack;
        private int _magicDefense;
        private int _luck;
        
        private string _name;
        private string _xml;
        
        private bool _backRow = false;
        private bool _sensed = false;
        
        private bool _fury = false;
        private bool _sadness = false;
        private bool _death = false;
        private bool _manipulate = false;
        
        private List<Element> _weak;
        private List<Element> _halve;
        private List<Element> _void;
        private List<Element> _absorb;
        private List<Status> _immune;
        
        private List<EnemyItem> _win;
        private List<EnemyItem> _steal;
        private IInventoryItem _morph;
        
        
        private Thread _ai;
        
        private static Dictionary<string, XmlNode> _table;

        #endregion Member Data
        
        
        static Enemy()
        {
            _table = new Dictionary<string, XmlNode>();
            
            XmlDocument gamedata = Resource.GetXmlFromResource("data.enemies.xml", typeof(Seven).Assembly);
            
            foreach (XmlNode node in gamedata.SelectNodes("//enemies/enemy"))
            {
                if (node.NodeType == XmlNodeType.Comment)
                {
                    continue;
                }
                                
                string name = node.SelectSingleNode("name").InnerText;
                
                _table.Add(Resource.CreateID(name), node);
            }
        }

        private Enemy()
            : base()
        {
        }
        
        private Enemy(XmlNode node, int x, int y)
            : this()
        {
            _weak = new List<Element>();
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();
            
            _win = new List<EnemyItem>();
            _steal = new List<EnemyItem>();
            
            //_abilityState = new AbilityState();
            
            _name = node.SelectSingleNode("name").InnerText;
            _attack = Int32.Parse(node.SelectSingleNode("atk").InnerText);
            _defense = Int32.Parse(node.SelectSingleNode("def").InnerText);
            _defensePercent = Int32.Parse(node.SelectSingleNode("defp").InnerText);
            _dexterity = Int32.Parse(node.SelectSingleNode("dex").InnerText);
            _magicAttack = Int32.Parse(node.SelectSingleNode("mat").InnerText);
            _magicDefense = Int32.Parse(node.SelectSingleNode("mdf").InnerText);
            _luck = Int32.Parse(node.SelectSingleNode("lck").InnerText);
            
            _level = Int32.Parse(node.SelectSingleNode("lvl").InnerText);
            _maxhp = _hp = Int32.Parse(node.SelectSingleNode("hp").InnerText);
            _maxmp = _mp = Int32.Parse(node.SelectSingleNode("mp").InnerText);
            
            _exp = Int32.Parse(node.SelectSingleNode("exp").InnerText);
            _ap = Int32.Parse(node.SelectSingleNode("ap").InnerText);
            _gil = Int32.Parse(node.SelectSingleNode("gil").InnerText);
            
            
            foreach (XmlNode weak in node.SelectSingleNode("weaks").ChildNodes)
                _weak.Add((Element)Enum.Parse(typeof(Element), weak.InnerText));
            foreach (XmlNode halve in node.SelectSingleNode("halves").ChildNodes)
                _halve.Add((Element)Enum.Parse(typeof(Element), halve.InnerText));
            foreach (XmlNode v in node.SelectSingleNode("voids").ChildNodes)
                _void.Add((Element)Enum.Parse(typeof(Element), v.InnerText));
            foreach (XmlNode absorb in node.SelectSingleNode("absorbs").ChildNodes)
                _absorb.Add((Element)Enum.Parse(typeof(Element), absorb.InnerText));
            foreach (XmlNode immunity in node.SelectSingleNode("immunities").ChildNodes)
                _immune.Add((Status)Enum.Parse(typeof(Status), immunity.InnerText));
            
            foreach (XmlNode win in node.SelectSingleNode("win").ChildNodes)
                _win.Add(new EnemyItem(win.OuterXml));
            foreach (XmlNode steal in node.SelectSingleNode("steal").ChildNodes)
                _steal.Add(new EnemyItem(steal.OuterXml));
            foreach (XmlNode morph in node.SelectSingleNode("morph").ChildNodes)
                if (morph.Attributes["id"] != null)
            {
                string id = morph.Attributes["id"].Value;
                string type = morph.Attributes["type"].Value;
                
                _morph = Item.GetItem(id, type);
            }
            
            
            int vStep = Seven.Party.BattleSpeed;
            
            C_Timer = new Clock(vStep);
            V_Timer = new Clock(vStep);
            TurnTimer = new Time.Timer(6000, _dexterity * vStep / Seven.Party.NormalSpeed());
            
            _ai = new Thread(new ThreadStart(AI));
            
            _x = x;
            _y = y;
        }

        public static Enemy CreateEnemy(string id, int x, int y)
        {       
            if (_table.Count <= 0)
            {
                throw new GameDataException("No enemies are defined!");
            }

            return new Enemy(_table[id], Seven.BattleState.Random.Next(40, 300), Seven.BattleState.Random.Next(50, 200));
        }


        public static Enemy CreateRandomEnemy(int x, int y)
        {
            if (_table.Count <= 0)
            {
                throw new GameDataException("No enemies are defined!");
            }
            
            int c = Seven.BattleState.Random.Next(_table.Count);
            int i = 0;

            foreach (string s in _table.Keys)
            {
                if (i == c) return new Enemy(_table[s], x, y);
                i++;
            }

            return null;
        }
        
        
        #region Methods
        
        public override void AcceptDamage(Combatant attacker, AttackType type, int delta)
        {
            if (type == AttackType.Physical)
            {
                if (Sleep)
                {
                    CureSleep();
                }
                if (Confusion)
                {
                    CureConfusion();
                }
            }

            _hp -= delta;
            if (_hp < 0)
                _hp = 0;
            if (_hp == 0)
                InflictDeath();
        }
        
        public override string ToString()
        {
            if (Sensed)
                return String.Format("{0} - HP:{1}/{2} MP:{3}/{4} - Time: {5}/{6}",
                                     Name, HP, MaxHP, MP, MaxMP,
                                     (TurnTimer.TotalMilliseconds > TurnTimer.Timeout) ? TurnTimer.Timeout : TurnTimer.TotalMilliseconds,
                                     TurnTimer.Timeout);
            else
                return Name;
        }
        
//        public Enemy Clone()
//        {
//            return new Enemy(Resource.CreateID(Name));
//        }

        public void ResetAI()
        {
            _ai.Abort();
            _ai = new Thread(new ThreadStart(AI));
        }
        
        public override void Draw(Cairo.Context g)
        {
            g.Color = new Cairo.Color(1, 1, 1);
            g.Rectangle(X, Y, 20, 20);
            g.Fill();
        }
        
        
        #region AI
        
        private void AI()
        {
            while (true)
            {
                if (TurnTimer.IsUp && !WaitingToResolve)
                {
                    Ally attackee;

                    int i = Seven.BattleState.Random.Next(3);
                    
                    while (Seven.BattleState.Allies[i] == null || Seven.BattleState.Allies[i].IsDead)
                    {
                        i = (i + 1) % 3;
                    }

                    attackee = Seven.BattleState.Allies[i];
                    
                    int bd = Atk + ((Atk + Level) / 32) * (Atk * Level / 32);
                    int dam = (1 * (512 - attackee.Def) * bd) / (16 * 512);

                    BattleEvent e = new BattleEvent(this, () => attackee.AcceptDamage(this, AttackType.Physical, dam));

                    e.Dialogue = c => Name + " attacks";
                    
//                    _abilityState.LongRange = false;
//                    _abilityState.QuadraMagic = false;
//                    _abilityState.Type = AttackType.Physical;
//                    _abilityState.Performer = this;
//                    _abilityState.Target = new Combatant[1];
//                    _abilityState.Target[0] = attackee;
//                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };
//                    
                    Seven.BattleState.EnqueueAction(e);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
        
        protected override void ConfuAI()
        {
            while (true)
            {
                if (TurnTimer.IsUp && !WaitingToResolve)
                {
                    Enemy attackee;
                    int i = Seven.BattleState.Random.Next(Seven.BattleState.EnemyList.Count);
                    attackee = Seven.BattleState.EnemyList[i];
                    
                    int bd = Formula.PhysicalBase(this);
                    int dam = Formula.PhysicalDamage(bd, 16, attackee);
                    
//                    _abilityState.LongRange = false;
//                    _abilityState.QuadraMagic = false;
//                    _abilityState.Type = AttackType.Physical;
//                    _abilityState.Performer = this;
//                    _abilityState.Target = new Combatant[1];
//                    _abilityState.Target[0] = attackee;
//                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };
//                    
//                    Seven.BattleState.EnqueueAction(_abilityState);
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }
        
        protected override void BerserkAI()
        {
            while (true)
            {
                if (TurnTimer.IsUp && !WaitingToResolve)
                {
                    Ally attackee;
                    int i = Seven.BattleState.Random.Next(3);
                    
                    while (Seven.BattleState.Allies[i] == null)
                        i = (i + 1) % 3;
                    attackee = Seven.BattleState.Allies[i];
                    
                    
                    int bd = Formula.PhysicalBase(this);
                    int dam = Formula.PhysicalDamage(bd, 16, attackee);
                    
//                    _abilityState.LongRange = false;
//                    _abilityState.QuadraMagic = false;
//                    _abilityState.Type = AttackType.Physical;
//                    _abilityState.Performer = this;
//                    _abilityState.Target = new Combatant[1];
//                    _abilityState.Target[0] = attackee;
//                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };
//                    
//                    Seven.BattleState.EnqueueAction(_abilityState);
                }
                else 
                {
                    Thread.Sleep(100);
                }
            }
        }
        
#endregion
        
        public override void Sense()
        {
            _sensed = true;
        }
        
        
        public override bool Weak(params Element[] e)
        {
            foreach (Element d in e)
            {
                if (_weak.Contains(d))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool Halves(params Element[] e)
        {
            foreach (Element d in e)
            {
                if (_halve.Contains(d))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool Voids(params Element[] e)
        {
            foreach (Element d in e)
            {
                if (_void.Contains(d))
                {
                    return true;
                }
            }
            return false;
        }
        public override bool Absorbs(params Element[] e)
        {
            foreach (Element d in e)
            {
                if (_absorb.Contains(d))
                {
                    return true;
                }
            }

            return false;
        }
        public bool Immune(Status s)
        {
            return _immune.Contains(s);
        }
        
        
        public void StealItem(Ally thief)
        {
            int diff = 40 + Level - thief.Level;
            int stealMod = 512 * diff / 100;
            
            foreach (EnemyItem item in _steal)
            {
                int chance = item.Chance * stealMod / 256;
                int r = Seven.BattleState.Random.Next(64);
                if (r <= chance)
                {
                    Seven.Party.Inventory.AddToInventory(item.Item);
                    _steal.Clear();
                    return;
                }
            }
            //if (_steal.Count == 0)
            //    ; // Nothing to steal
            //else ; // Couldn't steal anything
        }

        public IInventoryItem WinItem()
        {
            foreach (EnemyItem item in _win)
            {
                int r = Seven.BattleState.Random.Next(64);
                if (r <= item.Chance)
                {
                    return item.Item;
                }
            }
            return null;
        }
        
        
        
        
        #region Inflict Status
        public override bool InflictFury()
        {
            if (_fury)
                return false;
            _sadness = false;
            _fury = true;
            return true;
        }
        public override bool InflictSadness()
        {
            if (_sadness)
                return false;
            _fury = false;
            _sadness = true;
            return true;
        }
        public override bool InflictDeath()
        {
            if (_death)
            {
                return false;
            }

            _death = true;
            _hp = 0;
            return true;
        }
        public override bool InflictSleep()
        {
            if (Immune(Status.Sleep))
                return false;
            if (Sleep || Petrify || Peerless || Resist) return false;
            Sleep = true;
            _sleepTime = V_Timer.TotalMilliseconds;
            TurnTimer.Pause();
            return true;
        }
        public override bool InflictPoison()
        {
            if (Immune(Status.Poison))
                return false;
            if (Poison) return false;
            Poison = true;
            PoisonTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictConfusion()
        {
            if (Immune(Status.Confusion))
                return false;
            if (Confusion || Petrify || Peerless || Resist)
                return false;
            Confusion = true;
            _confuThread = new Thread(new ThreadStart(ConfuAI));
            _confuThread.Start();
            return true;
        }
        public override bool InflictSilence()
        {
            if (Immune(Status.Silence))
                return false;
            if (Silence || Petrify || Peerless || Resist)
                return false;
            Silence = true;
            return true;
        }
        public override bool InflictHaste()
        {
            if (Immune(Status.Haste))
                return false;
            if (Haste || Petrify || Peerless || Resist)
                return false;
            if (Slow)
                CureSlow();
            Haste = true;
            DoubleTimers();
            return true;
        }
        public override bool InflictSlow()
        {
            if (Immune(Status.Slow))
                return false;
            if (Slow || Petrify || Peerless || Resist)
                return false;
            if (Haste)
                CureHaste();
            Slow = true;
            HalveTimers();
            return true;
        }
        public override bool InflictStop()
        {
            if (Immune(Status.Stop))
                return false;
            if (Stop || Petrify || Peerless || Resist)
                return false;
            Stop = true;
            PauseTimers();
            return true;
        }
        public override bool InflictFrog()
        {
            if (Immune(Status.Frog))
                return false;
            if (Frog || Petrify || Peerless || Resist)
                return false;
            Frog = true;
            return true;
        }
        public override bool InflictSmall()
        {
            if (Immune(Status.Small))
                return false;
            if (Small || Petrify || Peerless || Resist)
                return false;
            Small = true;
            return true;
        }
        public override bool InflictSlowNumb()
        {
            if (Immune(Status.SlowNumb))
                return false;
            if (SlowNumb || Petrify || Peerless || Resist)
                return false;
            SlowNumb = true;
            _slownumbTime = C_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictPetrify()
        {
            if (Immune(Status.Petrify))
                return false;
            if (Petrify || Peerless || Resist)
                return false;
            Petrify = true;
            return true;
        }
        public override bool InflictRegen()
        {
            if (Immune(Status.Regen))
                return false;
            if (Regen || Petrify || Peerless || Resist)
                return false;
            Regen = true;
            _regenTimeEnd = V_Timer.TotalMilliseconds;
            _regenTimeInt = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictBarrier()
        {
            if (Immune(Status.Barrier))
                return false;
            if (Barrier || Petrify || Peerless || Resist)
                return false;
            MBarrier = true;
            _barrierTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictMBarrier()
        {
            if (Immune(Status.MBarrier))
                return false;
            if (MBarrier || Petrify || Peerless || Resist)
                return false;
            MBarrier = true;
            _mbarrierTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictReflect()
        {
            if (Immune(Status.Reflect))
                return false;
            if (Reflect || Petrify || Peerless || Resist)
                return false;
            Reflect = true;
            return true;
        }
        public override bool InflictShield()
        {
            if (Immune(Status.Shield))
                return false;
            if (Shield || Petrify || Resist)
                return false;
            Shield = true;
            _shieldTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictDeathSentence()
        {
            if (Immune(Status.DeathSentence))
                return false;
            if (DeathSentence || Petrify || Peerless || Resist || DeathForce)
                return false;
            DeathSentence = true;
            _deathsentenceTime = C_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictManipulate()
        {
            return false;
        }
        public override bool InflictBerserk()
        {
            if (Immune(Status.Berserk))
                return false;
            if (Berserk || Petrify || Peerless || Resist)
                return false;
            Berserk = true;
            _berserkThread = new Thread(new ThreadStart(BerserkAI));
            _berserkThread.Start();
            return true;
        }
        public override bool InflictPeerless()
        {
            if (Immune(Status.Peerless))
                return false;
            if (Peerless || Petrify || Resist)
                return false;
            Peerless = true;
            _peerlessTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictParalyzed()
        {
            if (Immune(Status.Paralyzed))
                return false;
            if (Paralysed || Petrify || Peerless || Resist)
                return false;
            Paralysed = true;
            _paralyzedTime = V_Timer.TotalMilliseconds;
            TurnTimer.Pause();
            return true;
        }
        public override bool InflictDarkness()
        {
            if (Immune(Status.Darkness))
                return false;
            if (Darkness || Petrify || Peerless || Resist)
                return false;
            Darkness = true;
            return true;
        }
        public override bool InflictSeizure()
        {
            if (Immune(Status.Seizure))
                return false;
            if (Seizure || Petrify || Peerless || Resist)
                return false;
            Seizure = true;
            _seizureTimeEnd = V_Timer.TotalMilliseconds;
            _seizureTimeInt = V_Timer.TotalMilliseconds;
            return true;
        }
        public override bool InflictDeathForce()
        {
            if (Immune(Status.DeathForce))
                return false;
            if (DeathForce || Petrify || Peerless || Resist)
                return false;
            DeathForce = true;
            return true;
        }
        public override bool InflictResist()
        {
            if (Immune(Status.Resist))
                return false;
            if (Resist || Petrify || Peerless)
                return false;
            Resist = true;
            return true;
        }
        public override bool InflictLuckyGirl()
        {
            if (Immune(Status.LuckyGirl))
                return false;
            if (LuckyGirl || Petrify || Peerless || Resist)
                return false;
            LuckyGirl = true;
            return true;
        }
        public override bool InflictImprisoned()
        {
            if (Immune(Status.Imprisoned))
                return false;
            if (Imprisoned || Petrify)
                return false;
            Imprisoned = true;
            PauseTimers();
            return true;
        }
        #endregion Inflict
        
        
        #region Cure Status
        
        public override bool CureFury()
        {
            if (!_fury)
                return false;
            _fury = false;
            return true;
        }
        public override bool CureSadness()
        {
            if (!_sadness)
                return false;
            _sadness = false;
            return true;
        }
//        public override bool CureDeath()
//        {
//            if (!_death)
//                return false;
//            _death = false;
//            return true;
//        }
        
#endregion
        
        
        public override void Dispose()
        {
            if (_ai != null && _ai.IsAlive)
                _ai.Abort();
        }
        
        #endregion Methods
        
        
        
        #region Properties
        
        public override int Level { get { return _level; } }
        
        public int Exp { get { return _exp; } }
        public int AP { get { return _ap; } }
        public int Gil { get { return _gil; } }
        
        public override int Atk { get { return _attack; } }
        public override int Def { get { return _defense; } }
        public override int Defp { get { return _defensePercent; } }
        public override int Dexterity { get { return _dexterity; } }
        public override int Mat { get { return _magicAttack; } }
        public override int MDef { get { return _magicDefense; } }
        public override int MDefp { get { return 0; } }
        public override int Luck { get { return _luck; } }
        
        public override int HP { get { return _hp; } }
        public override int MP { get { return _mp; } set { _mp = value; } }
        public override int MaxHP { get { return _maxhp; } }
        public override int MaxMP { get { return _maxmp; } }
        
        public override string Name { get { return _name; } }

        public override bool LongRange { get { return false; } }
        
        public override bool BackRow { get { return _backRow; } }
        public override bool Sensed { get { return _sensed; } }
        public override bool Death { get { return _death; } }
        public override bool NearDeath { get { return HP <= (MaxHP / 4); } }
        public override bool Sadness { get { return _sadness; } }
        public override bool Fury { get { return _fury; } }
        
        public Thread AIThread { get { return _ai; } }
        
        #endregion Properties
        
        
    }
}

