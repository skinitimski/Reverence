using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using System.Text;
using System.Threading;

namespace Atmosphere.BattleSimulator
{
    public class Enemy : Combatant
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
            private IItem _item;
            private int _chance;

            public EnemyItem(string xmlstring)
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)));

                XmlNode node = xml.DocumentElement;

                string id = node.Attributes["id"].Value;
                string type = node.Attributes["type"].Value;
                _chance = Int32.Parse(node.Attributes["chance"].Value);

                _item = Atmosphere.BattleSimulator.Item.GetItem(id, type);
            }

            public IItem Item { get { return _item; } }
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
        private IItem _morph;


        private Thread _ai;

        private static Dictionary<string, string> _table;

        #endregion Member Data

        
        public static void Init()
        {
            _table = new Dictionary<string, string>();

            XmlDocument gamedata = Util.GetXmlFromResource("data.enemies.xml");

            foreach (XmlNode node in gamedata.SelectSingleNode("//enemies").ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Comment)
                    continue;

                XmlDocument xml = new XmlDocument();
                xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(node.OuterXml)));

                string name = xml.SelectSingleNode("//name").InnerText;

                _table.Add(Globals.CreateID(name), node.OuterXml);
            }
        }

        public Enemy(string xmlstring, int x, int y)
        {
            _xml = xmlstring;

            _weak = new List<Element>();
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();

            _win = new List<EnemyItem>();
            _steal = new List<EnemyItem>();

            _abilityState = new AbilityState();

            XmlDocument xml = new XmlDocument();
            xml.Load(new MemoryStream(Encoding.UTF8.GetBytes(xmlstring)));

            _name = xml.SelectSingleNode("//name").InnerText;
            _attack = Int32.Parse(xml.SelectSingleNode("//atk").InnerText);
            _defense = Int32.Parse(xml.SelectSingleNode("//def").InnerText);
            _defensePercent = Int32.Parse(xml.SelectSingleNode("//defp").InnerText);
            _dexterity = Int32.Parse(xml.SelectSingleNode("//dex").InnerText);
            _magicAttack = Int32.Parse(xml.SelectSingleNode("//mat").InnerText);
            _magicDefense = Int32.Parse(xml.SelectSingleNode("//mdf").InnerText);
            _luck = Int32.Parse(xml.SelectSingleNode("//lck").InnerText);

            _level = Int32.Parse(xml.SelectSingleNode("//lvl").InnerText);
            _maxhp = _hp = Int32.Parse(xml.SelectSingleNode("//hp").InnerText);
            _maxmp = _mp = Int32.Parse(xml.SelectSingleNode("//mp").InnerText);

            _exp = Int32.Parse(xml.SelectSingleNode("//exp").InnerText);
            _ap = Int32.Parse(xml.SelectSingleNode("//ap").InnerText);
            _gil = Int32.Parse(xml.SelectSingleNode("//gil").InnerText);
            

            foreach (XmlNode weak in xml.SelectSingleNode("//weaks").ChildNodes)
                _weak.Add((Element)Enum.Parse(typeof(Element), weak.InnerText));
            foreach (XmlNode halve in xml.SelectSingleNode("//halves").ChildNodes)
                _halve.Add((Element)Enum.Parse(typeof(Element), halve.InnerText));
            foreach (XmlNode v in xml.SelectSingleNode("//voids").ChildNodes)
                _void.Add((Element)Enum.Parse(typeof(Element), v.InnerText));
            foreach (XmlNode absorb in xml.SelectSingleNode("//absorbs").ChildNodes)
                _absorb.Add((Element)Enum.Parse(typeof(Element), absorb.InnerText));
            foreach (XmlNode immunity in xml.SelectSingleNode("//immunities").ChildNodes)
                _immune.Add((Status)Enum.Parse(typeof(Status), immunity.InnerText));

            foreach (XmlNode win in xml.SelectSingleNode("//win").ChildNodes)
                _win.Add(new EnemyItem(win.OuterXml));
            foreach (XmlNode steal in xml.SelectSingleNode("//steal").ChildNodes)
                _steal.Add(new EnemyItem(steal.OuterXml));
            foreach (XmlNode morph in xml.SelectSingleNode("//morph").ChildNodes)
                if (morph.Attributes["id"] != null)
                {
                    string id = morph.Attributes["id"].Value;
                    string type = morph.Attributes["type"].Value;

                    _morph = Atmosphere.BattleSimulator.Item.GetItem(id, type);
                }


            int vStep = Globals.BattleSpeed;

            _c_timer = new ScaledClock(vStep);
            _v_timer = new ScaledClock(vStep);
            _turnTimer = new ScaledTimer(6000, _dexterity * vStep / Globals.NormalSpeed());

            _ai = new Thread(new ThreadStart(AI));

            _x = x;
            _y = y;
        }

        public Enemy(string id) : this(_table[id], Game.Random.Next(40, 300), Game.Random.Next(50, 200)) { }

        public static Enemy GetRandomEnemy(int x, int y)
        {
            if (_table.Count <= 0)
                throw new GamedataException("No enemies are defined!");

            int c = Game.Random.Next(_table.Count);
            int i = 0;
            foreach (string s in _table.Keys)
            {
                if (i == c) return new Enemy(_table[s], x, y);
                i++;
            }
            return null;
        }


        #region Methods

        public override void AcceptDamage(ICombatant attacker, int delta)
        {
            _hp -= delta;
            if (_hp < 0)
                _hp = 0;
            if (_hp == 0)
                _death = true;
        }

        public override string ToString()
        {
            if (Sensed)
                return String.Format("{0} - HP:{1}/{2} MP:{3}/{4} - Time: {5}/{6}",
                    Name, HP, MaxHP, MP, MaxMP,
                    (_turnTimer.Elapsed > _turnTimer.Timeout) ? _turnTimer.Timeout : _turnTimer.Elapsed,
                    _turnTimer.Timeout);
            else
                return Name;
        }

        public Enemy Clone()
        {
            return new Enemy(_xml, X, Y);
        }
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
                if (TurnTimer.IsUp &&
                    _abilityState != Game.Battle.ActiveAbility &&
                    !Game.Battle.PendingAbilities.Contains(_abilityState))
                {
                    Ally attackee;
                    int i = Game.Random.Next(3);

                    while (Game.Battle.Allies[i] == null || Game.Battle.Allies[i].IsDead)
                        i = (i + 1) % 3;
                    attackee = Game.Battle.Allies[i];

                    int bd = Atk + ((Atk + Level) / 32) * (Atk * Level / 32);
                    int dam = (1 * (512 - attackee.Def) * bd) / (16 * 512);

                    _abilityState.LongRange = false;
                    _abilityState.QuadraMagic = false;
                    _abilityState.Type = AttackType.Physical;
                    _abilityState.Performer = this;
                    _abilityState.Target = new ICombatant[1];
                    _abilityState.Target[0] = attackee;
                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };

                    Game.Battle.EnqueueAction(_abilityState);
                }
                else Thread.Sleep(100);
            }
        }

        protected override void ConfuAI()
        {
            while (true)
            {
                if (TurnTimer.IsUp)
                {
                    Enemy attackee;
                    int i = Game.Random.Next(Game.Battle.EnemyList.Count);
                    attackee = Game.Battle.EnemyList[i];

                    int bd = Formula.PhysicalBase(this);
                    int dam = Formula.PhysicalDamage(bd, 16, attackee);

                    _abilityState.LongRange = false;
                    _abilityState.QuadraMagic = false;
                    _abilityState.Type = AttackType.Physical;
                    _abilityState.Performer = this;
                    _abilityState.Target = new ICombatant[1];
                    _abilityState.Target[0] = attackee;
                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };

                    Game.Battle.EnqueueAction(_abilityState);
                }
                else Thread.Sleep(100);
            }
        }

        protected override void BerserkAI()
        {
            while (true)
            {
                if (TurnTimer.IsUp)
                {
                    Ally attackee;
                    int i = Game.Random.Next(3);

                    while (Game.Battle.Allies[i] == null)
                        i = (i + 1) % 3;
                    attackee = Game.Battle.Allies[i];


                    int bd = Formula.PhysicalBase(this);
                    int dam = Formula.PhysicalDamage(bd, 16, attackee);

                    _abilityState.LongRange = false;
                    _abilityState.QuadraMagic = false;
                    _abilityState.Type = AttackType.Physical;
                    _abilityState.Performer = this;
                    _abilityState.Target = new ICombatant[1];
                    _abilityState.Target[0] = attackee;
                    _abilityState.Action += delegate() { attackee.AcceptDamage(this, dam); };

                    Game.Battle.EnqueueAction(_abilityState);
                }
                else Thread.Sleep(100);
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
                if (_weak.Contains(d))
                    return true;
            return false;
        }
        public override bool Halves(params Element[] e)
        {
            foreach (Element d in e)
                if (_halve.Contains(d))
                    return true;
            return false;
        }
        public override bool Voids(params Element[] e)
        {
            foreach (Element d in e)
                if (_void.Contains(d))
                    return true;
            return false;
        }
        public override bool Absorbs(params Element[] e)
        {
            foreach (Element d in e)
                if (_absorb.Contains(d))
                    return true;
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
                int r = Game.Random.Next(64);
                if (r <= chance)
                {
                    Inventory.AddToInventory(item.Item);
                    _steal.Clear();
                    return;
                }
            }
            //if (_steal.Count == 0)
            //    ; // Nothing to steal
            //else ; // Couldn't steal anything
        }
        public IItem WinItem()
        {
            foreach (EnemyItem item in _win)
            {
                int r = Game.Random.Next(64);
                if (r <= item.Chance)
                    return item.Item;
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
                return false;
            _death = true;
            _hp = 0;
            return true;
        }
        public override bool InflictSleep()
        {
            if (Immune(Status.Sleep))
                return false;
            if (Sleep || Petrify || Peerless || Resist) return false;
            _sleep = true;
            _sleepTime = _v_timer.Elapsed;
            _turnTimer.Pause();
            return true;
        }
        public override bool InflictPoison()
        {
            if (Immune(Status.Poison))
                return false;
            if (Poison) return false;
            _poison = true;
            _poisonTime = _v_timer.Elapsed;
            return true;
        }
        public override bool InflictConfusion()
        {
            if (Immune(Status.Confusion))
                return false;
            if (Confusion || Petrify || Peerless || Resist)
                return false;
            _confusion = true;
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
            _silence = true;
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
            _haste = true;
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
            _slow = true;
            HalveTimers();
            return true;
        }
        public override bool InflictStop()
        {
            if (Immune(Status.Stop))
                return false;
            if (Stop || Petrify || Peerless || Resist)
                return false;
            _stop = true;
            PauseTimers();
            return true;
        }
        public override bool InflictFrog()
        {
            if (Immune(Status.Frog))
                return false;
            if (Frog || Petrify || Peerless || Resist)
                return false;
            _frog = true;
            return true;
        }
        public override bool InflictSmall()
        {
            if (Immune(Status.Small))
                return false;
            if (Small || Petrify || Peerless || Resist)
                return false;
            _small = true;
            return true;
        }
        public override bool InflictSlowNumb()
        {
            if (Immune(Status.SlowNumb))
                return false;
            if (SlowNumb || Petrify || Peerless || Resist)
                return false;
            _slowNumb = true;
            _slownumbTime = _c_timer.Elapsed;
            return true;
        }
        public override bool InflictPetrify()
        {
            if (Immune(Status.Petrify))
                return false;
            if (Petrify || Peerless || Resist)
                return false;
            _petrify = true;
            return true;
        }
        public override bool InflictRegen()
        {
            if (Immune(Status.Regen))
                return false;
            if (Regen || Petrify || Peerless || Resist)
                return false;
            _regen = true;
            _regenTimeEnd = _v_timer.Elapsed;
            _regenTimeInt = _v_timer.Elapsed;
            return true;
        }
        public override bool InflictBarrier()
        {
            if (Immune(Status.Barrier))
                return false;
            if (Barrier || Petrify || Peerless || Resist)
                return false;
            _barrier = true;
            _barrierTime = _v_timer.Elapsed;
            return true;
        }
        public override bool InflictMBarrier()
        {
            if (Immune(Status.MBarrier))
                return false;
            if (MBarrier || Petrify || Peerless || Resist)
                return false;
            _mbarrier = true;
            _mbarrierTime = _v_timer.Elapsed;
            return true;
        }
        public override bool InflictReflect()
        {
            if (Immune(Status.Reflect))
                return false;
            if (Reflect || Petrify || Peerless || Resist)
                return false;
            _reflect = true;
            return true;
        }
        public override bool InflictShield()
        {
            if (Immune(Status.Shield))
                return false;
            if (Shield || Petrify || Resist)
                return false;
            _shield = true;
            _shieldTime = _v_timer.Elapsed;
            return true;
        }
        public override bool InflictDeathSentence()
        {
            if (Immune(Status.DeathSentence))
                return false;
            if (DeathSentence || Petrify || Peerless || Resist || DeathForce)
                return false;
            _deathSentence = true;
            _deathsentenceTime = _c_timer.Elapsed;
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
            _berserk = true;
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
            _peerless = true;
            _peerlessTime = _v_timer.Elapsed;
            return true;
        }
        public override bool InflictParalyzed()
        {
            if (Immune(Status.Paralyzed))
                return false;
            if (Paralysed || Petrify || Peerless || Resist)
                return false;
            _paralyzed = true;
            _paralyzedTime = _v_timer.Elapsed;
            _turnTimer.Pause();
            return true;
        }
        public override bool InflictDarkness()
        {
            if (Immune(Status.Darkness))
                return false;
            if (Darkness || Petrify || Peerless || Resist)
                return false;
            _darkness = true;
            return true;
        }
        public override bool InflictSeizure()
        {
            if (Immune(Status.Seizure))
                return false;
            if (Seizure || Petrify || Peerless || Resist)
                return false;
            _seizure = true;
            _seizureTimeEnd = _v_timer.Elapsed;
            _seizureTimeInt = _v_timer.Elapsed;
            return true;
        }
        public override bool InflictDeathForce()
        {
            if (Immune(Status.DeathForce))
                return false;
            if (DeathForce || Petrify || Peerless || Resist)
                return false;
            _deathForce = true;
            return true;
        }
        public override bool InflictResist()
        {
            if (Immune(Status.Resist))
                return false;
            if (Resist || Petrify || Peerless)
                return false;
            _resist = true;
            return true;
        }
        public override bool InflictLuckyGirl()
        {
            if (Immune(Status.LuckyGirl))
                return false;
            if (LuckyGirl || Petrify || Peerless || Resist)
                return false;
            _luckyGirl = true;
            return true;
        }
        public override bool InflictImprisoned()
        {
            if (Immune(Status.Imprisoned))
                return false;
            if (Imprisoned || Petrify)
                return false;
            _imprisoned = true;
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
        public override bool CureDeath()
        {
            if (!_death)
                return false;
            _death = false;
            return true;
        }

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
