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
        #region Nested

        private class EnemyAbility : Ability
        {
            public EnemyAbility(XmlNode xml, Lua lua)
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

                DamageFormula = GetFormula(xml.SelectSingleNode("formula"), lua);
                HitFormula = GetHitFormula(xml.SelectSingleNode("hitFormula"), lua);
            }

            public override string GetMessage(Combatant source)
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
            public EnemyItem(XmlNode node)
                : this()
            {               
                Id = node.Attributes["id"].Value;
                Type = (InventoryItemType)Enum.Parse(typeof(InventoryItemType), node.Attributes["type"].Value);

                if (node.Name != "morph")
                {
                    Chance = Int32.Parse(node.Attributes["chance"].Value);
                }
            }

            public string Id { get; private set; }

            public InventoryItemType Type { get; private set; }

            public int Chance { get; private set; }
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
        private EnemyItem _morph;

        #endregion Member Data
        

        
        public Enemy(BattleState battle, XmlNode node, int x, int y, int e, string designation = "")
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
                _win.Add(new EnemyItem(win));
            }
            foreach (XmlNode steal in node.SelectNodes("steal/item"))
            {
                _steal.Add(new EnemyItem(steal));
            }
            foreach (XmlNode morph in node.SelectNodes("morph/item"))
            {
                _morph = new EnemyItem(morph);
            }

            foreach (XmlNode attackNode in node.SelectNodes("attacks/attack"))
            {
                EnemyAbility attack = new EnemyAbility(attackNode, battle.Lua);

                Attacks.Add(attack.Name, attack);
            }


            // AI: Setup

            XmlNode setupNode = node.SelectSingleNode("ai/setup");

            if (setupNode != null)
            {
                string setup = String.Format("return function (self) {0} end", setupNode.InnerText);

                try
                {
                    AISetup = (LuaFunction) battle.Lua.DoString(setup).First();
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
                AIMain = (LuaFunction) battle.Lua.DoString(main).First();
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
                    AICounter = (LuaFunction) battle.Lua.DoString(counter).First();
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
                    AICounterPhysical = (LuaFunction) battle.Lua.DoString(counter).First();
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
                    AICounterMagical = (LuaFunction) battle.Lua.DoString(counter).First();
                }
                catch (Exception ex)
                {
                    throw new ImplementationException("Error loading enemy AI counter-magical script; enemy = " + Name, ex);
                }
            }
            
            
            // AI: Counter - Death
            
            XmlNode counterDeathNode = node.SelectSingleNode("ai/counter-death");
            
            if (counterDeathNode != null)
            {
                string counter = String.Format("return function (self) {0} end", counterDeathNode.InnerText);
                
                try
                {
                    AICounterDeath = (LuaFunction) battle.Lua.DoString(counter).First();
                }
                catch (Exception ex)
                {
                    throw new ImplementationException("Error loading enemy AI counter-death script; enemy = " + Name, ex);
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
            
                AIConfu = (LuaFunction) battle.Lua.DoString(String.Format("return function (self) a = chooseRandomEnemy(); self:UseAttack(\"{0}\", a) end", confuAttack)).First();
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
            
                AIBerserk = (LuaFunction) battle.Lua.DoString(String.Format("return function (self) a = chooseRandomAlly(); self:UseAttack(\"{0}\", a) end", berserkAttack)).First();
            }
            else
            {
                if (!Immune(Status.Berserk))
                {
                    throw new GameDataException("No berserk attack specified and not immune to berserk: {0}", Name);
                }
            }


            // Timers
            
            int vStep = CurrentBattle.Party.BattleSpeed;
            
            C_Timer = CurrentBattle.TimeFactory.CreateClock();
            V_Timer = CurrentBattle.TimeFactory.CreateClock(vStep);
            TurnTimer = CurrentBattle.TimeFactory.CreateTimer(TURN_TIMER_TIMEOUT, GetTurnTimerStep(vStep), e, false);
        }


        #region Methods
        
        public override void AcceptDamage(Combatant source, int delta, AttackType type = AttackType.None)
        {
            CurrentBattle.AddDamageIcon(delta, this);

            if (type == AttackType.Physical)
            {
                if (Sleep)
                {
                    CureSleep(source);
                }
                if (Confusion)
                {
                    CureConfusion(source);
                }
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
                Kill(source);
            }
        }

        public override void AcceptMPLoss(Combatant source, int delta)
        {
            CurrentBattle.AddDamageIcon(delta, this, true);

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
        
        public override void Recover(Combatant source)
        {
            CurrentBattle.AddRecoveryIcon(this);
            
            if (Death)
            {
                CureDeath(source);
            }
            
            _hp = MaxHP;
            _mp = MaxMP;
        }

        public override void Respond(Ability ability)
        {
            if (!CannotAct)
            {                                                                                                                                                                                                                                                                                                                                                                                                       
                if (ability.Type == AttackType.Physical)
                {
                    RunAICounterPhysical();
                }
                else if (ability.Type == AttackType.Magical)
                {
                    RunAICounterMagical();
                }
                else
                {
                    RunAICounter();
                }
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
            return Dexterity * vStep / CurrentBattle.Party.NormalSpeed();
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
            if (AICounter != null)
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
        }
        
        private void RunAICounterPhysical()
        {
            if (AICounterPhysical != null)
            {
                try
                {
                    AICounterPhysical.Call(this);
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in Enemy AI Counter - Physical script, enemy = " + Name, e);
                }
            }
        }
        
        private void RunAICounterMagical()
        {
            if (AICounterMagical != null)
            {
                try
                {
                    AICounterMagical.Call(this);
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in Enemy AI Counter - Magical script, enemy = " + Name, e);
                }
            }
        }
        
        private void RunAICounterDeath()
        {
            if (AICounterDeath != null)
            {
                try
                {
                    AICounterDeath.Call(this);
                }
                catch (Exception e)
                {
                    throw new ImplementationException("Error in Enemy AI Counter - Death script, enemy = " + Name, e);
                }
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
            CastMagicSpell(id, new List<Combatant>() { target }, false);
        }
        
        public void CastMagicSpellAndWait(string id, IEnumerable<Combatant> targets)
        {            
            CastMagicSpell(id, targets, false);
        }
        
        private void CastMagicSpell(string id, IEnumerable<Combatant> targets, bool resetTurnTimer)
        {
            Spell spell = CurrentBattle.Seven.Data.GetMagicSpell(id);
            
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
            Spell spell = CurrentBattle.Seven.Data.GetMagicSpell(id);
            
            if (spell == null)
            {
                throw new GameDataException("Enemy '{0}' used undefined magic spell '{1}'", Name, id);
            }
            
            spell.Use(this, targets, new AbilityModifiers { CounterAttack = true, ResetTurnTimer = false });
        }




        
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
        
        public string StealItem(Ally thief)
        {
            string id = null;

            if (HasItems)
            {
                int diff = 40 + Level - thief.Level;
                int stealMod = 512 * diff / 100;
            
                foreach (EnemyItem item in _steal)
                {
                    int chance = item.Chance * stealMod / 256;
                    int r = CurrentBattle.Random.Next(64);

                    if (r <= chance)
                    {
                        IInventoryItem stolen = CurrentBattle.Seven.Data.GetInventoryItem(item.Id, item.Type);

                        id = stolen.ID;

                        CurrentBattle.Party.Inventory.AddToInventory(stolen);
                        _steal.Clear();
                        break;
                    }
                }
            }

            return id;
        }

        public IInventoryItem WinItem()
        {
            IInventoryItem won = null;

            foreach (EnemyItem item in _win)
            {
                int r = CurrentBattle.Random.Next(64);

                if (r <= item.Chance)
                {
                    won = CurrentBattle.Seven.Data.GetInventoryItem(item.Id, item.Type);

                    break;
                }
            }

            return won;
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

            if (!Immune(Status.Death) && !(DeathForce || Peerless || Petrify || Resist))
            {
                if (DeathSentence)
                {
                    CureDeathSentence(source);
                }

                if (!_death)
                {                 
                    CurrentBattle.AddDeathIcon(this);

                    Kill(source);

                    _death = true;
                    inflicted = true;
                }
            }

            return inflicted;
        }

        public override bool InflictManipulate(Combatant source)
        {
            return false;
        }

        #endregion Inflict
        
        
        #region Cure Status
        
        public override bool CureFury(Combatant source)
        {
            bool cured = false;

            if (_fury)
            {
                _fury = false;
                cured = true;
            }

            return cured;
        }

        public override bool CureSadness(Combatant source)
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
        
        public override bool CureDeath(Combatant source)
        {
            bool cured = false;
            
            if (Death)
            {
                _death = false;
                cured = true;

                CureAll(source);
                UnpauseTimers();
            }
            
            return cured;
        }
        
        protected override void Kill(Combatant source)
        {
            _hp = 0;
            _death = true;

            CureAll(source);
            TurnTimer.Reset();
            PauseTimers();

            RunAICounterDeath();
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

        public override bool NearDeath { get { return !Death && HP <= (MaxHP / 4); } }

        public override bool Sadness { get { return _sadness; } }

        public override bool Fury { get { return _fury; } }

        public bool FlaggedForRemoval { get; set; }
        
        private LuaFunction AISetup { get; set; }
        
        private LuaFunction AIMain { get; set; }
        
        private LuaFunction AICounter { get; set; }
        
        private LuaFunction AICounterPhysical { get; set; }
        
        private LuaFunction AICounterMagical { get; set; }
        
        private LuaFunction AICounterDeath { get; set; }
        
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

