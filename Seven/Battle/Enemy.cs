using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using NLua;

using Atmosphere.Reverence.Exceptions;
using Atmosphere.Reverence.Graphics;
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

        private class Attack
        {
            public Attack(XmlNode node)
            {
                ID = node.SelectSingleNode("@id").Value;
                Power = Int32.Parse(node.SelectSingleNode("power").InnerText);
                Atkp = Int32.Parse(node.SelectSingleNode("atkp").InnerText);
                Target = (TargetType)Enum.Parse(typeof(TargetType), node.SelectSingleNode("target").InnerText);
                Element = (Element)Enum.Parse(typeof(Element), node.SelectSingleNode("element").InnerText);
                InfoVisible = true;

                XmlNode hiddenNode = node.SelectSingleNode("hidden");

                if (hiddenNode != null)
                {
                    InfoVisible = Boolean.Parse(hiddenNode.InnerText);
                }
            }

            public string ID { get; private set; }
            public int Power { get; private set; }
            public int Atkp { get; private set; }
            public TargetType Target { get; private set; }
            public Element Element { get; private set; }
            public bool InfoVisible { get; private set; }
        }
        
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
        
        private int _attack;
        private int _defense;
        private int _defensePercent;
        private int _dexterity;
        private int _magicAttack;
        private int _magicDefense;
        private int _luck;
        
        private string _name;
        
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
            _weak = new List<Element>();
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();
            
            _win = new List<EnemyItem>();
            _steal = new List<EnemyItem>();

            Attacks = new Dictionary<string, Attack>();
        }
        
        private Enemy(XmlNode node, int x, int y)
            : this()
        {            
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
            
            Exp = Int32.Parse(node.SelectSingleNode("exp").InnerText);
            AP = Int32.Parse(node.SelectSingleNode("ap").InnerText);
            Gil = Int32.Parse(node.SelectSingleNode("gil").InnerText);
            
            
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
            {
                if (morph.Attributes["id"] != null)
                {
                    string id = morph.Attributes["id"].Value;
                    string type = morph.Attributes["type"].Value;
                
                    _morph = Item.GetItem(id, type);
                }
            }

            foreach (XmlNode attackNode in node.SelectNodes("attacks/attack"))
            {
                Attack attack = new Attack(attackNode);

                Attacks.Add(attack.ID, attack);
            }

            
            string main = String.Format("return function (self) {0} end", node.SelectSingleNode("ai/main").InnerText);
            
            try
            {
                AIMain = (LuaFunction)Seven.Lua.DoString(main).First();
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error loading enemy main AI script; enemy = " + Name, e);
            }




            
            int vStep = Seven.Party.BattleSpeed;
            
            C_Timer = new Clock(vStep);
            V_Timer = new Clock(vStep);
            TurnTimer = new Time.Timer(6000, _dexterity * vStep / Seven.Party.NormalSpeed());

            _x = x;
            _y = y;
        }

        public static Enemy CreateEnemy(string id, int x, int y)
        {       
            if (!_table.ContainsKey(id))
            {
                throw new GameDataException("No enemy defined with id = " + id);
            }

            return new Enemy(_table[id], x, x);
        }


        public static Enemy CreateRandomEnemy(int x, int y)
        {
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
        
        public override void AcceptDamage(int delta, AttackType type = AttackType.None)
        {
            Seven.BattleState.AddDamageIcon(delta, this);

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
            {
                _hp = 0;
            }
            else if (_hp > _maxhp)
            {
                _hp = _maxhp;
            }

            if (HP == 0)
            {
                _death = true;
            }
        }

        public override void AcceptMPLoss(int delta)
        {
            Seven.BattleState.AddDamageIcon(delta, this, true);

            _mp -= delta;

            if (_mp > _maxmp)
            {
                _mp = _maxmp;
            }
            else if (_mp < 0)
            {
                _mp = 0;
            }
        }

        public override void UseMP(int amount)
        {
            if (_mp - amount < 0)
            {
                throw new ImplementationException("Used more MP than I had -- " + Name);
            }
            _mp -= amount;
        }
        
        public override string ToString()
        {
            string description = Name;

            if (Sensed)
            {
                description = String.Format("{0} - HP:{1}/{2} MP:{3}/{4} - Time: {5}/{6}",
                                     Name, HP, MaxHP, MP, MaxMP,
                                     (TurnTimer.TotalMilliseconds > TurnTimer.Timeout) ? TurnTimer.Timeout : TurnTimer.TotalMilliseconds,
                                     TurnTimer.Timeout);
            }

            return description;
        }
        
//        public Enemy Clone()
//        {
//            return new Enemy(Resource.CreateID(Name));
//        }

        
        public override void Draw(Cairo.Context g)
        {
            Cairo.Color iconColor = Colors.GRAY_8;
            
            int iconSize = 20;

            g.Color = iconColor;
            g.Rectangle(X - iconSize / 2, Y- iconSize / 2, iconSize, iconSize);
            g.Fill();
            g.Fill();
        }
        
        
        #region AI

        public void RunAIMain()
        {
            try
            {
                AIMain.Call(this);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error in Enemy AI Main script, enemy = " + Name, e);
            }
        }




        public void UseAttack(string id, Combatant target)
        {
            if (!Attacks.ContainsKey(id))
            {
                throw new GameDataException("Used attack that doesn't exist, enemy = " + Name);
            }

            Attack attack = Attacks[id];

            string description = " attacks";

            if (attack.InfoVisible)
            {
                description = " uses " + attack.ID;
            }

            PhysicalAttack(attack.Power, attack.Atkp, target, new Element[] { attack.Element }, description);
        }





        
//        private void AI()
//        {
//            while (true)
//            {
//                if (TurnTimer.IsUp && !WaitingToResolve)
//                {
//                    Ally target;
//
//                    int i = Seven.BattleState.Random.Next(3);
//                    
//                    while (Seven.BattleState.Allies[i] == null || Seven.BattleState.Allies[i].IsDead)
//                    {
//                        i = (i + 1) % Party.PARTY_SIZE;
//                    }
//
//                    target = Seven.BattleState.Allies[i];
//
//                    BattleEvent e = new BattleEvent(this, () => Formula.PhysicalAttack(16, this, target));
//
//                    e.Dialogue = c => Name + " attacks";
//
//                    Seven.BattleState.EnqueueAction(e);
//                }
//                else
//                {
//                    Thread.Sleep(100);
//                }
//            }
//        }
//        
//        protected override void ConfuAI()
//        {
//            while (true)
//            {
//                if (TurnTimer.IsUp && !WaitingToResolve)
//                {
//                    Enemy attackee;
//                    int i = Seven.BattleState.Random.Next(Seven.BattleState.EnemyList.Count);
//                    attackee = Seven.BattleState.EnemyList[i];
//                    
//                    int bd = Formula.PhysicalBase(this);
//                    int dam = Formula.PhysicalDamage(bd, 16, attackee);
//                    
//                    BattleEvent e = new BattleEvent(this, () => attackee.AcceptDamage(dam, AttackType.Physical));
//
//                    e.Dialogue = c => Name + " attacks (confused)";
//                    
//                    Seven.BattleState.EnqueueAction(e);
//                }
//                else
//                {
//                    Thread.Sleep(100);
//                }
//            }
//        }
//        
//        protected override void BerserkAI()
//        {
//            while (true)
//            {
//                if (TurnTimer.IsUp && !WaitingToResolve)
//                {
//                    Ally attackee;
//                    int i = Seven.BattleState.Random.Next(3);
//                    
//                    while (Seven.BattleState.Allies[i] == null)
//                        i = (i + 1) % 3;
//                    attackee = Seven.BattleState.Allies[i];
//                    
//                    
//                    int bd = Formula.PhysicalBase(this);
//                    int dam = Formula.PhysicalDamage(bd, 16, attackee);
//                    
//                    BattleEvent e = new BattleEvent(this, () => attackee.AcceptDamage(dam, AttackType.Physical));
//                    
//                    e.Dialogue = c => Name + " attacks (berserk)";
//                    
//                    Seven.BattleState.EnqueueAction(e);
//                }
//                else 
//                {
//                    Thread.Sleep(100);
//                }
//            }
//        }
        
        #endregion
        
        public override void Sense()
        {
            _sensed = true;
        }
        
        
        public override bool Weak(params Element[] e)
        {
            return e.Any(x => _weak.Contains(x));
        }

        public override bool Halves(params Element[] e)
        {
            return e.Any(x => _halve.Contains(x));
        }
        public override bool Voids(params Element[] e)
        {
            return e.Any(x => _void.Contains(x));
        }
        public override bool Absorbs(params Element[] e)
        {
            return e.Any(x => _absorb.Contains(x));
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
            bool inflicted = false;

            if (!_fury)
            {
                _sadness = false;
                _fury = true;
                inflicted = true;
            }

            return inflicted;
        }
        public override bool InflictSadness()
        {
            bool inflicted = false;
            
            if (!_sadness)
            {
                _fury = false;
                _sadness = true;
                inflicted = true;
            }
            
            return inflicted;
        }
        public override bool InflictDeath()
        {
            bool inflicted = false;

            if (!_death)
            {
                _death = true;
                _hp = 0;
                inflicted = true;
            }

            return inflicted;
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
            bool cured = false;

            if (_fury)
            {
                _fury = false;
                cured = true;
            }

            return cured;
        }
        public override bool CureSadness()
        {
            bool cured = false;
            
            if (_sadness)
            {
                _sadness = false;
                cured = true;
            }

            _sadness = false;
            
            return cured;
        }
        
        public override bool CureDeath()
        {
            bool cured = false;
            
            if (Death)
            {
                _death = false;
                cured = true;
            }
            
            return cured;
        }
        
        #endregion
        
        
        public override void Dispose()
        {
        }
        
        #endregion Methods
        
        
        
        #region Properties
        
        public override int Level { get { return _level; } }
        
        public int Exp  { get; private set; }
        public int AP { get; private set; }
        public int Gil  { get; private set; }
        
        public override int Atk { get { return _attack; } }
        public override int Def { get { return _defense; } }
        public override int Defp { get { return _defensePercent; } }
        public override int Dexterity { get { return _dexterity; } }
        public override int Mat { get { return _magicAttack; } }
        public override int MDef { get { return _magicDefense; } }
        public override int MDefp { get { return 0; } }
        public override int Luck { get { return _luck; } }
        
        public override int HP  { get { return _hp; }  }
        public override int MP { get { return _mp; }  }
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

        private LuaFunction AIMain { get; set; }
        private LuaFunction AIConfu { get; set; }
        private LuaFunction AIBerserk { get; set; }

        private Dictionary<string, Attack> Attacks { get; set; }
        
        #endregion Properties
        
        
    }
}

