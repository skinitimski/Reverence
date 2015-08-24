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
                Target = (BattleTarget)Enum.Parse(typeof(BattleTarget), node.SelectSingleNode("target").InnerText);
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

            public BattleTarget Target { get; private set; }

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
                InventoryItemType type = (InventoryItemType)Enum.Parse(typeof(InventoryItemType), node.Attributes["type"].Value);
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
        
        private Enemy(XmlNode node, int x, int y, int e, string designation)
            : base(x, y)
        {         
            _weak = new List<Element>();
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();
            
            _win = new List<EnemyItem>();
            _steal = new List<EnemyItem>();
            
            Attacks = new Dictionary<string, Attack>();

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

            if (!String.IsNullOrEmpty(designation))
            {
                _name += " " + designation;
            }
            
            foreach (XmlNode weak in node.SelectNodes("weaks/weak"))
            {
                _weak.Add((Element)Enum.Parse(typeof(Element), weak.InnerText));
            }
            foreach (XmlNode halve in node.SelectNodes("halves/halve"))
            {
                _halve.Add((Element)Enum.Parse(typeof(Element), halve.InnerText));
            }
                                                                                                                                                                                                                        
            foreach (XmlNode v in node.SelectNodes("voids/void"))
            {
                _void.Add((Element)Enum.Parse(typeof(Element), v.InnerText));
            }
            foreach (XmlNode absorb in node.SelectNodes("absorbs/absorb"))
            {
                _absorb.Add((Element)Enum.Parse(typeof(Element), absorb.InnerText));
            }
            foreach (XmlNode immunity in node.SelectNodes("immunities/immunity"))
            {
                _immune.Add((Status)Enum.Parse(typeof(Status), immunity.InnerText));
            }
            
            foreach (XmlNode win in node.SelectNodes("win/item"))
            {
                _win.Add(new EnemyItem(win.OuterXml));
            }
            foreach (XmlNode steal in node.SelectNodes("steal/item"))
            {
                _steal.Add(new EnemyItem(steal.OuterXml));
            }
            foreach (XmlNode morph in node.SelectNodes("morph/item"))
            {
                if (morph.Attributes["id"] != null)
                {
                    string id = morph.Attributes["id"].Value;
                    
                    InventoryItemType type = (InventoryItemType)Enum.Parse(typeof(InventoryItemType), morph.Attributes["type"].Value);
                
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
            catch (Exception ex)
            {
                throw new ImplementationException("Error loading enemy main AI script; enemy = " + Name, ex);
            }
            
            
            string confuAttack = node.SelectSingleNode("ai/confuAttack").InnerText;
            
            if (!Attacks.ContainsKey(confuAttack))
            {
                throw new GameDataException("Specified confu attack '{0}' is not configured; enemy = {1}", confuAttack, Name);
            }
            
            AIConfu = (LuaFunction)Seven.Lua.DoString(String.Format("return function (self) a = chooseRandomEnemy(); self:UseAttack(\"{0}\", a) end", confuAttack)).First();
            
            
            string berserkAttack = node.SelectSingleNode("ai/berserkAttack").InnerText;
            
            if (!Attacks.ContainsKey(berserkAttack))
            {
                throw new GameDataException("Specified berserk attack '{0}' is not configured; enemy = {1}", berserkAttack, Name);
            }
            
            AIBerserk = (LuaFunction)Seven.Lua.DoString(String.Format("return function (self) a = chooseRandomAlly(); self:UseAttack(\"{0}\", a) end", berserkAttack)).First();

            
            int vStep = Seven.Party.BattleSpeed;
            
            C_Timer = new Clock();
            V_Timer = new Clock(vStep);
            TurnTimer = new Time.Timer(TURN_TIMER_TIMEOUT, GetTurnTimerStep(vStep), e, false);
        }

        public static Enemy CreateEnemy(string id, int x, int y, int e, string designation = "")
        {       
            if (!_table.ContainsKey(id))
            {
                throw new GameDataException("No enemy defined with id = " + id);
            }

            Enemy enemy = null;

            try
            {
                enemy = new Enemy(_table[id], x, y, e, designation);
            }
            catch (Exception ex)
            {
                throw new GameDataException("Error creating enemy with id = " + id, ex);
            }

            return enemy;
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
            g.Rectangle(X - iconSize / 2, Y - iconSize / 2, iconSize, iconSize);
            g.Fill();
            g.Fill();

            Text.ShadowedText(g, NameColor, Name, X + iconSize, Y, Text.MONOSPACE_FONT, 18);
#if DEBUG
            string extraInfo = String.Format("{0}/{1} {2}/{3} {4}%", HP, MaxHP, MP, MaxMP, TurnTimer.PercentElapsed);
            Text.ShadowedText(g, Colors.WHITE, extraInfo, X + iconSize, Y + 20, Text.MONOSPACE_FONT, 18);

            StringBuilder statuses = new StringBuilder();

            if (Death) { statuses.Append("Death, "); }
            if (NearDeath) { statuses.Append("NearDeath, "); }
            if (Sadness) { statuses.Append("Sadness, "); }
            if (Fury) { statuses.Append("Fury, "); }
            if (Sleep) { statuses.Append("Sleep, "); }
            if (Poison) { statuses.Append("Poison, "); }
            if (Confusion) { statuses.Append("Confusion, "); }
            if (Silence) { statuses.Append("Silence, "); }
            if (Haste) { statuses.Append("Haste, "); }
            if (Slow) { statuses.Append("Slow, "); }
            if (Stop) { statuses.Append("Stop, "); }
            if (Frog) { statuses.Append("Frog, "); }
            if (Small) { statuses.Append("Small, "); }
            if (SlowNumb) { statuses.Append("SlowNumb, "); }
            if (Petrify) { statuses.Append("Petrify, "); }
            if (Regen) { statuses.Append("Regen, "); }
            if (Barrier) { statuses.Append("Barrier, "); }
            if (MBarrier) { statuses.Append("MBarrier, "); }
            if (Reflect) { statuses.Append("Reflect, "); }
            if (Shield) { statuses.Append("Shield, "); }
            if (DeathSentence) { statuses.Append("DeathSentence, "); }
            if (Manipulate) { statuses.Append("Manipulate, "); }
            if (Berserk) { statuses.Append("Berserk, "); }
            if (Peerless) { statuses.Append("Peerless, "); }
            if (Paralysed) { statuses.Append("Paralysed, "); }
            if (Darkness) { statuses.Append("Darkness, "); }
            if (Seizure) { statuses.Append("Seizure, "); }
            if (DeathForce) { statuses.Append("DeathForce, "); }
            if (Resist) { statuses.Append("Resist, "); }
            if (LuckyGirl) { statuses.Append("LuckyGirl, "); }
            if (Imprisoned) { statuses.Append("Imprisoned, "); }

            if (statuses.Length > 0)
            {
                statuses.Length -= 2;
                Text.ShadowedText(g, Colors.WHITE, statuses.ToString(), X + iconSize, Y + 40, Text.MONOSPACE_FONT, 18);
            }
#endif
        }

        protected override int GetTurnTimerStep(int vStep)
        {
            return Dexterity * vStep / Seven.Party.NormalSpeed();
        }


        public void TakeTurn()
        {
            if (Confusion)
            {
                RunAIConfu();
            }
            else if (Berserk)
            {
                RunAIBerserk();
            }
            else
            {
                RunAIMain();
            }
        }
        
        #region AI
        
        private void RunAIMain()
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
        
        private void RunAIConfu()
        {
            try
            {
                AIConfu.Call(this);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error in Enemy AI Confu script, enemy = " + Name, e);
            }
        }
        
        private void RunAIBerserk()
        {
            try
            {
                AIBerserk.Call(this);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error in Enemy AI Berserk script, enemy = " + Name, e);
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

            PhysicalAttack(attack.Power, attack.Atkp, target, new Element[] { attack.Element }, true, description);
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

        public override bool Weak(Element e)
        {
            return _weak.Contains(e);
        }
        
        public override bool Weak(IEnumerable<Element> elements)
        {
            return elements.Any(e => Weak(e));
        }

        public override bool Halves(Element e)
        {
            return _halve.Contains(e);
        }

        public override bool Halves(IEnumerable<Element> elements)
        {
            return elements.Any(e => Halves(e));
        }

        public override bool Voids(Element e)
        {
            return _void.Contains(e);
        }

        public override bool Voids(IEnumerable<Element> elements)
        {
            return elements.Any(e => Voids(e));
        }
        
        public override bool Absorbs(Element e)
        {
            return _absorb.Contains(e);
        }
        
        public override bool Absorbs(IEnumerable<Element> elements)
        {
            return elements.Any(e => Absorbs(e));
        }

        public override bool Immune(Status s)
        {
            return _immune.Contains(s);
        }
        
        public IInventoryItem StealItem(Ally thief)
        {
            IInventoryItem stolen = null;

            if (HasItems)
            {
                int diff = 40 + Level - thief.Level;
                int stealMod = 512 * diff / 100;
            
                foreach (EnemyItem item in _steal)
                {
                    int chance = item.Chance * stealMod / 256;
                    int r = Seven.BattleState.Random.Next(64);

                    if (r <= chance)
                    {
                        stolen = item.Item;
                        Seven.Party.Inventory.AddToInventory(item.Item);
                        _steal.Clear();
                        break;
                    }
                }
            }

            return stolen;
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

        public override bool InflictManipulate()
        {
            return false;
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

        public bool HasItems { get { return _steal.Count > 0; } }

        public override IList<Element> Weaknesses
        {
            get
            {
                return _weak.ToList();
            }
        }
        
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
        
        public override int HP  { get { return _hp; } }

        public override int MP { get { return _mp; } }

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

