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
using Atmosphere.Reverence.Seven.State;

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

        private class EnemyAbility : Ability
        {
            public EnemyAbility(XmlNode xml)
                : base()
            {
                Name = xml.SelectSingleNode("@id").Value;
                
                Power = Int32.Parse(xml.SelectSingleNode("power").InnerText);
                Hitp = Int32.Parse(xml.SelectSingleNode("hitp").InnerText);

                XmlNode costNode = xml.SelectSingleNode("cost");            
                MPCost = costNode == null ? 0 : Int32.Parse(costNode.InnerText);

                Target = (BattleTarget)Enum.Parse(typeof(BattleTarget), xml.SelectSingleNode("target").InnerText);
                Type = (AttackType)Enum.Parse(typeof(AttackType), xml.SelectSingleNode("type").InnerText);
                
                XmlNode hiddenNode = xml.SelectSingleNode("hidden");
                InfoVisible = hiddenNode == null ? true : Boolean.Parse(hiddenNode.InnerText);


                Elements = GetElements(xml.SelectNodes("element")).ToArray();
                Statuses = GetStatusChanges(xml.SelectNodes("statusChange")).ToArray();

                
                XmlNode hitsNode = xml.SelectSingleNode("hits");
                
                if (hitsNode != null)
                {
                    Hits = Int32.Parse(hitsNode.InnerText);
                }
                
                XmlNode formulaNode = xml.SelectSingleNode("formula");
                
                if (formulaNode != null)
                {
                    DamageFormula = (DamageFormula)Delegate.CreateDelegate(typeof(DamageFormula), this, formulaNode.InnerText);
                }
                else
                {
                    switch (Type)
                    {
                        case AttackType.Magical:
                            DamageFormula = MagicalAttack;
                            break;
                        case AttackType.Physical:
                            DamageFormula = PhysicalAttack;
                            break;
                        default:
                            throw new GameDataException("Neither a formula nor an attack type -- ability '{0}'", Name);
                    }
                }
            }

            protected override string GetMessage(Combatant source)
            {
                string msg;

                if (InfoVisible)
                {
                    msg = source.Name + " uses " + Name;
                }
                else
                {
                    msg = " attacks";
                }

                return msg;
            }

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
                
                _table.Add(name, node);
            }
        }
        
        private Enemy(BattleState battle, XmlNode node, int x, int y, int e, string designation)
            : base(battle, x, y)
        {         
            _weak = new List<Element>();
            _halve = new List<Element>();
            _void = new List<Element>();
            _absorb = new List<Element>();
            _immune = new List<Status>();

            _win = new List<EnemyItem>();
            _steal = new List<EnemyItem>();

            Attacks = new Dictionary<string, EnemyAbility>();
            Variables = new Dictionary<string, object>();

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
                EnemyAbility attack = new EnemyAbility(attackNode);

                Attacks.Add(attack.Name, attack);
            }


            // AI: Setup

            XmlNode setupNode = node.SelectSingleNode("ai/setup");

            if (setupNode != null)
            {
                string setup = String.Format("return function (self) {0} end", setupNode.InnerText);

                try
                {
                    AISetup = (LuaFunction)Seven.Lua.DoString(setup).First();
                }
                catch (Exception ex)
                {
                    throw new ImplementationException("Error loading enemy AI setup script; enemy = " + Name, ex);
                }
            }


            // AI: Main

            string main = String.Format("return function (self) {0} end", node.SelectSingleNode("ai/main").InnerText);
            
            try
            {
                AIMain = (LuaFunction)Seven.Lua.DoString(main).First();
            }
            catch (Exception ex)
            {
                throw new ImplementationException("Error loading enemy AI main script; enemy = " + Name, ex);
            }
            
            
            // AI: Counter
            
            XmlNode counterNode = node.SelectSingleNode("ai/counter");
            
            if (counterNode != null)
            {
                string counter = String.Format("return function (self) {0} end", counterNode.InnerText);
                
                try
                {
                    AICounter = (LuaFunction)Seven.Lua.DoString(counter).First();
                }
                catch (Exception ex)
                {
                    throw new ImplementationException("Error loading enemy AI counter script; enemy = " + Name, ex);
                }
            }
            
            
            // AI: Counter - Physical
            
            XmlNode counterPhysicalNode = node.SelectSingleNode("ai/counter-physical");
            
            if (counterPhysicalNode != null)
            {
                string counter = String.Format("return function (self) {0} end", counterPhysicalNode.InnerText);
                
                try
                {
                    AICounterPhysical = (LuaFunction)Seven.Lua.DoString(counter).First();
                }
                catch (Exception ex)
                {
                    throw new ImplementationException("Error loading enemy AI counter-physical script; enemy = " + Name, ex);
                }
            }
            
            
            // AI: Counter - Magical
            
            XmlNode counterMagicalNode = node.SelectSingleNode("ai/counter-physical");
            
            if (counterMagicalNode != null)
            {
                string counter = String.Format("return function (self) {0} end", counterMagicalNode.InnerText);
                
                try
                {
                    AICounterMagical = (LuaFunction)Seven.Lua.DoString(counter).First();
                }
                catch (Exception ex)
                {
                    throw new ImplementationException("Error loading enemy AI counter-magical script; enemy = " + Name, ex);
                }
            }


            // Confusion Attack

            XmlNode confuAttackNode = node.SelectSingleNode("ai/confuAttack");

            if (confuAttackNode != null)
            {            
                string confuAttack = confuAttackNode.InnerText;
            
                if (!Attacks.ContainsKey(confuAttack))
                {
                    throw new GameDataException("Specified confu attack '{0}' is not configured; enemy = {1}", confuAttack, Name);
                }
            
                AIConfu = (LuaFunction)Seven.Lua.DoString(String.Format("return function (self) a = chooseRandomEnemy(); self:UseAttack(\"{0}\", a) end", confuAttack)).First();
            }
            else
            {
                if (!Immune(Status.Confusion))
                {
                    throw new GameDataException("No confusion attack specified and not immune to confusion: {0}", Name);
                }
            }
        


            // Berserk Attack

            XmlNode berserkAttackNode = node.SelectSingleNode("ai/berserkAttack");

            if (berserkAttackNode != null)
            {
                string berserkAttack = berserkAttackNode.InnerText;
            
                if (!Attacks.ContainsKey(berserkAttack))
                {
                    throw new GameDataException("Specified berserk attack '{0}' is not configured; enemy = {1}", berserkAttack, Name);
                }
            
                AIBerserk = (LuaFunction)Seven.Lua.DoString(String.Format("return function (self) a = chooseRandomAlly(); self:UseAttack(\"{0}\", a) end", berserkAttack)).First();
            }
            else
            {
                if (!Immune(Status.Berserk))
                {
                    throw new GameDataException("No berserk attack specified and not immune to berserk: {0}", Name);
                }
            }


            // Timers
            
            int vStep = Seven.Party.BattleSpeed;
            
            C_Timer = CurrentBattle.TimeFactory.CreateClock();
            V_Timer = CurrentBattle.TimeFactory.CreateClock(vStep);
            TurnTimer = CurrentBattle.TimeFactory.CreateTimer(TURN_TIMER_TIMEOUT, GetTurnTimerStep(vStep), e, false);
        }

        public static Enemy CreateEnemy(BattleState battle, string name, int x, int y, int e, string designation = "")
        {       
            if (!_table.ContainsKey(name))
            {
                throw new GameDataException("No enemy defined with name = " + name);
            }

            Enemy enemy = null;

            try
            {
                enemy = new Enemy(battle, _table[name], x, y, e, designation);
            }
            catch (Exception ex)
            {
                throw new GameDataException("Error creating enemy with name = " + name, ex);
            }

            return enemy;
        }



        #region Methods
        
        public override void AcceptDamage(Combatant source, int delta, AttackType type = AttackType.None)
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
                        
            int hp = _hp - delta;
            
            if (hp < 0)
            {
                hp = 0;
            }
            else if (hp >= _maxhp)
            {
                hp = _maxhp;
            }

            _hp = hp;

            if (HP == 0)
            {
                _death = true;
            }

            if (source is Ally)
            {
                LastAttacker = source;

                switch (type)
                {
                    case AttackType.Physical:
                        LastAttackerPhysical = source;
                        break;
                    case AttackType.Magical:
                        LastAttackerMagical = source;
                        break;
                }

                if (type == AttackType.Physical && AICounterPhysical != null)
                {
                    RunAICounterPhysical();
                }
                else if (type == AttackType.Magical && AICounterMagical != null)
                {
                    RunAICounterMagical();
                }
                else if (AICounter != null)
                {
                    RunAICounter();
                }
            }
        }

        public override void AcceptMPLoss(Combatant source, int delta)
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
            
            if (source is Ally)
            {
                LastAttacker = source;
            }
        }
        
        public override void Recover()
        {
            Seven.BattleState.AddRecoveryIcon(this);
            
            if (Death)
            {
                CureDeath();
            }
            
            _hp = MaxHP;
            _mp = MaxMP;
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

        
        protected override void DrawIcon(Gdk.Drawable d, Cairo.Context g)
        {
            g.Color = Colors.ENEMY_RED;
            g.Rectangle(X - _icon_half_width, Y - _icon_half_height, Character.PROFILE_WIDTH_TINY, Character.PROFILE_HEIGHT_TINY);
            g.Fill();

#if DEBUG
            string variables = GetVariableText();

            if (variables.Length > 0)
            {
                Text.ShadowedText(g, Colors.WHITE, variables, X + _text_offset_x, Y + _text_offset_y - _text_spacing_y, Text.MONOSPACE_FONT, 12);
            }
#endif
        }

        private string GetVariableText()
        {
            StringBuilder variables = new StringBuilder();
            
            foreach (KeyValuePair<string, object> variable in Variables)
            {
                variables.Append(variable.Key);
                variables.Append("=");
                variables.Append(variable.Value);
                variables.Append(";");
            }

            if (variables.Length > 0)
            {
                variables.Length -= 1;
            }

            return variables.ToString();
        }


        protected override int GetTurnTimerStep(int vStep)
        {
            return Dexterity * vStep / Seven.Party.NormalSpeed();
        }

        public void EnterBattle()
        {
            RunAISetup();
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
        
        private void RunAISetup()
        {
            if (AISetup != null)
            {
                try
                {
                    AISetup.Call(this);
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in Enemy AI Setup script, enemy = " + Name, e);
                }
            }
        }
        
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
        
        private void RunAICounter()
        {
            try
            {
                AICounter.Call(this);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error in Enemy AI Counter script, enemy = " + Name, e);
            }
        }
        
        private void RunAICounterPhysical()
        {
            try
            {
                AICounterPhysical.Call(this);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error in Enemy AI Counter script, enemy = " + Name, e);
            }
        }
        
        private void RunAICounterMagical()
        {
            try
            {
                AICounterMagical.Call(this);
            }
            catch (Exception e)
            {
                throw new ImplementationException("Error in Enemy AI Counter script, enemy = " + Name, e);
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



        
        public void Attack(string id, Combatant target)
        {
            Attack(id, new List<Combatant>() { target });
        }
        
        public void AttackAndWait(string id, Combatant target)
        {
            AttackAndWait(id, new List<Combatant>() { target });
        }
        
        public void Attack(string id, IEnumerable<Combatant> targets)
        {
            Attack(id, targets, true);
        }   
        
        public void AttackAndWait(string id, IEnumerable<Combatant> targets)
        {
            Attack(id, targets, false);
        }    
        
        private void Attack(string id, IEnumerable<Combatant> targets, bool resetTurnTimer)
        {
            if (!Attacks.ContainsKey(id))
            {
                throw new GameDataException("Enemy '{0}' used undefined attack '{1}'", Name, id);
            }
            
            Attacks[id].Use(this, targets, new AbilityModifiers { ResetTurnTimer = resetTurnTimer});
        }       




        
        public void CounterAttack(string id, Combatant target)
        {
            CounterAttack(id, new List<Combatant>() { target });
        }    
        
        public void CounterAttack(string id, IEnumerable<Combatant> targets)
        {
            CounterAttack(id, targets, true);
        }
        
        private void CounterAttack(string id, IEnumerable<Combatant> targets, bool resetTurnTimer)
        {
            if (!Attacks.ContainsKey(id))
            {
                throw new GameDataException("Enemy '{0}' used undefined attack '{1}'", Name, id);
            }
            
            Attacks[id].Use(this, targets, new AbilityModifiers { CounterAttack = true, ResetTurnTimer = false });
        }
        
        
        
        
        
        public void CastMagicSpell(string id, Combatant target)
        {
            CastMagicSpell(id, new List<Combatant>() { target }, true);
        }
        
        public void CastMagicSpell(string id, IEnumerable<Combatant> targets)
        {
            CastMagicSpell(id, targets, true);
        }
        
        public void CastMagicSpellAndWait(string id, Combatant target)
        {            
            CastMagicSpell(id,  new List<Combatant>() { target }, false);
        }
        
        public void CastMagicSpellAndWait(string id, IEnumerable<Combatant> targets)
        {            
            CastMagicSpell(id, targets, false);
        }
        
        private void CastMagicSpell(string id, IEnumerable<Combatant> targets, bool resetTurnTimer)
        {
            Spell spell = MagicSpell.Get(id);
            
            if (spell == null)
            {
                throw new GameDataException("Enemy '{0}' used undefined magic spell '{1}'", Name, id);
            }
            
            spell.Use(this, targets, new AbilityModifiers { ResetTurnTimer = resetTurnTimer });
        }
        
        
        
        
        
        public void CounterWithMagicSpell(string id, Combatant target)
        {
            CounterWithMagicSpell(id, new List<Combatant>() { target }, true);
        }
        
        public void CounterWithMagicSpell(string id, IEnumerable<Combatant> targets)
        {
            CounterWithMagicSpell(id, targets, true);
        }
        
        private void CounterWithMagicSpell(string id, IEnumerable<Combatant> targets, bool resetTurnTimer)
        {
            Spell spell = MagicSpell.Get(id);
            
            if (spell == null)
            {
                throw new GameDataException("Enemy '{0}' used undefined magic spell '{1}'", Name, id);
            }
            
            spell.Use(this, targets, new AbilityModifiers { CounterAttack = true, ResetTurnTimer = false });
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

        public override bool InflictFury(Combatant source)
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

        public override bool InflictSadness(Combatant source)
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

        public override bool InflictDeath(Combatant source)
        {
            bool inflicted = false;

            CureDeathSentence();

            if (!_death)
            {
                _death = true;
                _hp = 0;
                inflicted = true;
            }

            return inflicted;
        }

        public override bool InflictManipulate(Combatant source)
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
        
        private LuaFunction AISetup { get; set; }
        
        private LuaFunction AIMain { get; set; }
        
        private LuaFunction AICounter { get; set; }
        
        private LuaFunction AICounterPhysical { get; set; }
        
        private LuaFunction AICounterMagical { get; set; }
        
        private LuaFunction AIConfu { get; set; }

        private LuaFunction AIBerserk { get; set; }

        private Dictionary<string, EnemyAbility> Attacks { get; set; }

        private Dictionary<string, object> Variables { get; set; }

        public object this[string key]
        {
            get
            {
                object variable;

                if (Variables.ContainsKey(key))
                {
                    variable = Variables[key];
                }
                else
                {
                    throw new GameDataException("Emeny '{0}' attempted to get the value of an undefined variable '{1}'", Name, key);
                }

                return variable;
            }
            set
            {
                Variables[key] = value;
            }
        }
        
        #endregion Properties
        
        
    }
}

