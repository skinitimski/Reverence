using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thread = System.Threading.Thread;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using Cairo;

using Atmosphere.Reverence.Graphics;
using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle.Time;
using Atmosphere.Reverence.Seven.Screen.BattleState;
using Atmosphere.Reverence.Seven.State;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal abstract class Combatant
    {
        public const long SLEEP_DURATION = 100; // 100 v-timer units
        public const long POISON_INTERVAL = 10; // 10 v-timer units
        public const long SLOWNUMB_DURATION = 30000; // 30 seconds
        public const long REGEN_INTERVAL = 1200; // 4 v-timer units
        public const long REGEN_DURATION = 38100; // 127 v-timer units
        public const long BARRIER_DURATION = 38100; // 127 v-timer units
        public const long MBARRIER_DURATION = 38100; // 127 v-timer units
        public const long SHIELD_DURATION = 18200; // 64 v-timer units
        public const long DEATHSENTENCE_DURATION = 60000; // 60 seconds
        public const long PEERLESS_DURATION = 18200; // 64 v-timer units
        public const long PARALYZED_DURATION = 3000; // 20 v-timer units
        public const long SEIZURE_INTERVAL = 1200; // 4 v-timer units
        public const long SEIZURE_DURATION = 38100; // 127 v-timer units

        
        
        protected const int _icon_half_width = (Character.PROFILE_WIDTH_TINY / 2);
        protected const int _icon_half_height = (Character.PROFILE_HEIGHT_TINY / 2);

        protected const int _text_offset_x = _icon_half_width + 10;
        protected const int _text_offset_y = _icon_half_height - 30;
        protected const int _text_spacing_y = 20;


        protected int _x;
        protected int _y;
        
        protected long SleepTime = -1;
        protected long PoisonTime = -1;
        protected long _slownumbTime = -1;
        protected long _regenTimeInt = -1;
        protected long _regenTimeEnd = -1;
        protected long _barrierTime = -1;
        protected long _mbarrierTime = -1;
        protected long _shieldTime = -1;
        protected long _deathsentenceTime = -1;
        protected long _peerlessTime = -1;
        protected long _paralyzedTime = -1;
        protected long _seizureTimeInt = -1;
        protected long _seizureTimeEnd = -1;




        protected Combatant(BattleState battle, int x, int y)
        {            
            _x = x;
            _y = y;

            CurrentBattle = battle;
            C_Timer = new BattleClock(136);
        }
        
        
        
        
        
        
        public void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height)
        {
            DrawIcon(d, g);
            
            Text.ShadowedText(g, NameColor, Name, X + _text_offset_x, Y + _text_offset_y, Text.MONOSPACE_FONT, 12);

#if DEBUG
            string extraInfo = String.Format("{0}/{1} {2}/{3} {4}%", HP, MaxHP, MP, MaxMP, TurnTimer.PercentComplete);
            Text.ShadowedText(g, Colors.WHITE, extraInfo, X + _text_offset_x, Y + _text_offset_y + _text_spacing_y, Text.MONOSPACE_FONT, 12);
            
            string statuses = StatusText();
            
            if (statuses.Length > 0)
            {
                Text.ShadowedText(g, Colors.WHITE, statuses.Substring(0, statuses.Length - 2), X + _text_offset_x, Y + _text_offset_y + _text_spacing_y * 2, Text.MONOSPACE_FONT, 12);
            }
#endif
        }

        protected abstract void DrawIcon(Gdk.Drawable d, Cairo.Context g);

        public abstract void Dispose();
        
        public abstract void AcceptDamage(Combatant source, int delta, AttackType type = AttackType.None);
        public abstract void AcceptMPLoss(Combatant source, int delta);
        public abstract void Recover(Combatant source);
       
        public abstract void Respond(Ability ability);

        public abstract void UseMP(int amount);


        public virtual void Sense() { }



        











        
        public void CheckTimers()
        {           
            if (Sleep && V_Timer.ElapsedUnits - SleepTime >= SLEEP_DURATION)
            {
                CureSleep(this);
            }
            
            
            if (Poison && V_Timer.ElapsedUnits - PoisonTime >= POISON_INTERVAL)
            {
                AcceptDamage(Poisoner, MaxHP / 32);
                PoisonTime = V_Timer.ElapsedUnits;
            }
            
            
//            if (SlowNumb && C_Timer.TotalMilliseconds - _slownumbTime >= SLOWNUMB_DURATION)
//            {
//                InflictPetrify(Petrifier);
//            }
//            
//            
//            if (Regen && V_Timer.TotalMilliseconds - _regenTimeEnd >= REGEN_DURATION)
//            {
//                CureRegen(this);
//            }
//            if (Regen && V_Timer.TotalMilliseconds - _regenTimeInt >= REGEN_INTERVAL)
//            {
//                AcceptDamage(Regenerator, MaxHP / -32);
//                _regenTimeInt = V_Timer.TotalMilliseconds;
//            }
//            
//            
//            if (Barrier && V_Timer.TotalMilliseconds - _barrierTime >= BARRIER_DURATION)
//            {
//                CureBarrier(this);
//            }
//            
//            
//            if (MBarrier && V_Timer.TotalMilliseconds - _barrierTime >= MBARRIER_DURATION)
//            {
//                CureMBarrier(this);
//            }
//            
//            
//            if (Shield && V_Timer.TotalMilliseconds - _shieldTime >= SHIELD_DURATION)
//            {
//                CureShield(this);
//            }
//            
//            
//            if (DeathSentence && C_Timer.TotalMilliseconds - _deathsentenceTime >= DEATHSENTENCE_DURATION)
//            {
//                InflictDeath(Sentencer);
//            }
//            
//            
//            if (Peerless && V_Timer.TotalMilliseconds - _peerlessTime >= PEERLESS_DURATION)
//            {
//                CurePeerless(this);
//            }
//            
//            
//            if (Paralysed && V_Timer.TotalMilliseconds - _paralyzedTime >= PARALYZED_DURATION)
//            {
//                CureParalyzed(this);
//            }
//            
//            
//            if (Seizure && V_Timer.TotalMilliseconds - _seizureTimeEnd >= SEIZURE_DURATION)
//            {
//                CureSeizure(this);
//            }
//            if (Seizure && V_Timer.TotalMilliseconds - _seizureTimeInt >= SEIZURE_INTERVAL)
//            {
//                AcceptDamage(Seizer, MaxHP / 32);
//                _seizureTimeInt = V_Timer.TotalMilliseconds;
//            }
        }

        protected void PauseTimers()
        {
            C_Timer.Pause();
            V_Timer.Pause();
            TurnTimer.Pause();
        }
        protected void UnpauseTimers()
        {
            C_Timer.Unpause();
            V_Timer.Unpause();
            TurnTimer.Unpause();
        }

        private String StatusText()
        {            
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
            if (Paralysed) { statuses.Append("Paralyzed, "); }
            if (Darkness) { statuses.Append("Darkness, "); }
            if (Seizure) { statuses.Append("Seizure, "); }
            if (DeathForce) { statuses.Append("DeathForce, "); }
            if (Resist) { statuses.Append("Resist, "); }
            if (LuckyGirl) { statuses.Append("LuckyGirl, "); }
            if (Imprisoned) { statuses.Append("Imprisoned, "); }

            return statuses.ToString();
        }
        
        
        
        #region Inflict

        public abstract bool InflictDeath(Combatant source);
        public abstract bool InflictFury(Combatant source);
        public abstract bool InflictSadness(Combatant source);
                
        public bool InflictSleep(Combatant source)
        {
            if (Immune(Status.Sleep))
                return false;
            if (Sleep || Petrify || Peerless || Resist) 
                return false;

            SleepTime = V_Timer.ElapsedUnits;
            TurnTimer.Pause();
            return true;
        }

        public bool InflictPoison(Combatant source)
        {
            if (Immune(Status.Poison))
                return false;
            if (Voids(Element.Poison))
                return false;
            if (Poison) 
                return false;

            Poison = true;
            Poisoner = source;
            PoisonTime = V_Timer.ElapsedUnits;

            return true;
        }

        public bool InflictConfusion(Combatant source)
        {
            if (Immune(Status.Confusion))
                return false;
            if (Confusion || Petrify || Peerless || Resist)
                return false;
            Confusion = true;
            return true;
        }
        public bool InflictSilence(Combatant source)
        {
            if (Immune(Status.Silence))
                return false;
            if (Silence || Petrify || Peerless || Resist)
                return false;
            Silence = true;
            return true;
        }
        public bool InflictHaste(Combatant source)
        {
            if (Immune(Status.Haste))
                return false;
            if (Haste || Petrify || Peerless || Resist)
                return false;
            if (Slow)
                CureSlow(source);
            Haste = true;
            C_Timer.DoubleRate();
            V_Timer.DoubleRate();
            return true;
        }
        public bool InflictSlow(Combatant source)
        {
            if (Immune(Status.Slow))
                return false;
            if (Slow || Petrify || Peerless || Resist)
                return false;
            if (Haste)
                CureHaste(source);
            Slow = true;
            C_Timer.HalfRate();
            V_Timer.HalfRate();
            return true;
        }
        public bool InflictStop(Combatant source)
        {
            if (Immune(Status.Stop))
                return false;
            if (Stop || Petrify || Peerless || Resist)
                return false;
            Stop = true;
            PauseTimers();
            return true;
        }
        public bool InflictFrog(Combatant source)
        {
            if (Immune(Status.Frog))
                return false;
            if (Frog || Petrify || Peerless || Resist)
                return false;
            Frog = true;
            return true;
        }
        public bool InflictSmall(Combatant source)
        {
            if (Immune(Status.Small))
                return false;
            if (Small || Petrify || Peerless || Resist)
                return false;
            Small = true;
            return true;
        }
        public bool InflictSlowNumb(Combatant source)
        {
            if (Immune(Status.SlowNumb))
                return false;
            if (SlowNumb || Petrify || Peerless || Resist)
                return false;
            SlowNumb = true;
            Petrifier = source;
            _slownumbTime = C_Timer.ElapsedUnits;
            return true;
        }
        public bool InflictPetrify(Combatant source)
        {
            if (Immune(Status.Petrify))
                return false;
            if (Petrify || Peerless || Resist)
                return false;

            CureSlowNumb(source);

            Petrify = true;
            PauseTimers();

            return true;
        }
        public bool InflictRegen(Combatant source)
        {
            if (Immune(Status.Regen))
                return false;
            if (Regen || Petrify || Peerless || Resist)
                return false;
            Regen = true;
            Regenerator = source;
            _regenTimeEnd = V_Timer.ElapsedUnits;
            _regenTimeInt = V_Timer.ElapsedUnits;
            return true;
        }
        public bool InflictBarrier(Combatant source)
        {
            if (Immune(Status.Barrier))
                return false;
            if (Barrier || Petrify || Peerless || Resist)
                return false;
            Barrier = true;
            _barrierTime = V_Timer.ElapsedUnits;
            return true;
        }
        public bool InflictMBarrier(Combatant source)
        {
            if (Immune(Status.MBarrier))
                return false;
            if (MBarrier || Petrify || Peerless || Resist)
                return false;
            MBarrier = true;
            _mbarrierTime = V_Timer.ElapsedUnits;
            return true;
        }
        public bool InflictReflect(Combatant source)
        {
            if (Immune(Status.Reflect))
                return false;
            if (Reflect || Petrify || Peerless || Resist)
                return false;
            Reflect = true;
            return true;
        }
        public bool InflictShield(Combatant source)
        {
            if (Immune(Status.Shield))
                return false;
            if (Shield || Petrify || Resist)
                return false;
            Shield = true;
            _shieldTime = V_Timer.ElapsedUnits;
            return true;
        }
        public bool InflictDeathSentence(Combatant source)
        {
            if (Immune(Status.DeathSentence))
                return false;
            if (DeathSentence || Petrify || Peerless || Resist || DeathForce)
                return false;
            DeathSentence = true;
            Sentencer = source;
            _deathsentenceTime = C_Timer.ElapsedUnits;
            return true;
        }

        public abstract bool InflictManipulate(Combatant source);

        public bool InflictBerserk(Combatant source)
        {
            if (Immune(Status.Berserk))
                return false;
            if (Berserk || Petrify || Peerless || Resist)
                return false;
            Berserk = true;
            return true;
        }
        public bool InflictPeerless(Combatant source)
        {
            if (Immune(Status.Peerless))
                return false;
            if (Peerless || Petrify || Resist)
                return false;
            Peerless = true;
            _peerlessTime = V_Timer.ElapsedUnits;
            return true;
        }
        public bool InflictParalyzed(Combatant source)
        {
            if (Immune(Status.Paralyzed))
                return false;
            if (Paralysed || Petrify || Peerless || Resist)
                return false;
            Paralysed = true;
            _paralyzedTime = V_Timer.ElapsedUnits;
            TurnTimer.Pause();
            return true;
        }
        public bool InflictDarkness(Combatant source)
        {
            if (Immune(Status.Darkness))
                return false;
            if (Darkness || Petrify || Peerless || Resist)
                return false;
            Darkness = true;
            return true;
        }
        public bool InflictSeizure(Combatant source)
        {
            if (Immune(Status.Seizure))
                return false;
            if (Seizure || Petrify || Peerless || Resist)
                return false;
            Seizure = true;
            Seizer = source;
            _seizureTimeEnd = V_Timer.ElapsedUnits;
            _seizureTimeInt = V_Timer.ElapsedUnits;
            return true;
        }
        public bool InflictDeathForce(Combatant source)
        {
            if (Immune(Status.DeathForce))
                return false;
            if (DeathForce || Petrify || Peerless || Resist)
                return false;
            DeathForce = true;
            return true;
        }
        public bool InflictResist(Combatant source)
        {
            if (Immune(Status.Resist))
                return false;
            if (Resist || Petrify || Peerless)
                return false;
            Resist = true;
            return true;
        }
        public bool InflictLuckyGirl(Combatant source)
        {
            if (Immune(Status.LuckyGirl))
                return false;
            if (LuckyGirl || Petrify || Peerless || Resist)
                return false;
            LuckyGirl = true;
            return true;
        }
        public bool InflictImprisoned(Combatant source)
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



        #region Cure
        public abstract bool CureDeath(Combatant source);
        public abstract bool CureFury(Combatant source);
        public abstract bool CureSadness(Combatant source);

        public bool CureSleep(Combatant source)
        {
            SleepTime = -1;

            if (!(Stop || Petrify || Paralysed || Imprisoned))
            {
                TurnTimer.Unpause();
            }

            return true;
        }

        public bool CurePoison(Combatant source)
        {
            PoisonTime = -1;
            Poisoner = null;
            Poison = false;
            return true;
        }
        public bool CureConfusion(Combatant source)
        {
            Confusion = false;
            return true;
        }
        public bool CureSilence(Combatant source)
        {
            Silence = false;
            return true;
        }
        public bool CureHaste(Combatant source)
        {
            Haste = false;
            C_Timer.NormalRate();
            V_Timer.NormalRate();

            return true;
        }
        public bool CureSlow(Combatant source)
        {
            Slow = false;
            C_Timer.NormalRate();
            V_Timer.NormalRate();

            return true;
        }
        public bool CureStop(Combatant source)
        {
            Stop = false;
            if (!Imprisoned)
            {
                UnpauseTimers();
                if (Sleep || Petrify || Paralysed)
                    TurnTimer.Pause();
            }
            return true;
        }
        public bool CureFrog(Combatant source)
        {
            Frog = false;
            return true;
        }
        public bool CureSmall(Combatant source)
        {
            Small = false;
            return true;
        }
        public bool CureSlowNumb(Combatant source)
        {
            _slownumbTime = -1;
            SlowNumb = false;
            Petrifier = null;
            return true;
        }
        public bool CurePetrify(Combatant source)
        {
            Petrify = false;
            if (!(Sleep || Stop || Paralysed || Imprisoned))
                TurnTimer.Reset();
            return true;
        }
        public bool CureRegen(Combatant source)
        {
            Regen = false;
            Regenerator = null;
            _regenTimeInt = -1;
            _regenTimeEnd = -1;
            return true;
        }
        public bool CureBarrier(Combatant source)
        {
            Barrier = false;
            _barrierTime = -1;
            return true;
        }
        public bool CureMBarrier(Combatant source)
        {
            MBarrier = false;
            _mbarrierTime = -1;
            return true;
        }
        public bool CureReflect(Combatant source)
        {
            Reflect = false;
            return true;
        }
        public bool CureShield(Combatant source)
        {
            Shield = false;
            _shieldTime = -1;
            return true;
        }
        public bool CureDeathSentence(Combatant source)
        {
            DeathSentence = false;
            _deathsentenceTime = -1;
            Sentencer = null;
            return true;
        }
        public bool CureManipulate(Combatant source)
        {
            return true;
        }
        public bool CureBerserk(Combatant source)
        {
            Berserk = false;
            return true;
        }
        public bool CurePeerless(Combatant source)
        {
            Peerless = false;
            _peerlessTime = -1;
            return true;
        }
        public bool CureParalyzed(Combatant source)
        {
            Paralysed = false;
            _paralyzedTime = -1;
            if (!(Sleep || Stop || Petrify || Imprisoned))
                TurnTimer.Unpause();
            return true;
        }
        public bool CureDarkness(Combatant source)
        {
            Darkness = false;
            return true;
        }
        public bool CureSeizure(Combatant source)
        {
            Seizure = false;
            Seizer = null;
            _seizureTimeEnd = -1;
            _seizureTimeInt = -1;
            return true;
        }
        public bool CureDeathForce(Combatant source)
        {
            DeathForce = false;
            return true;
        }
        public bool CureResist(Combatant source)
        {
            Resist = false;
            return true;
        }
        public bool CureLuckyGirl(Combatant source)
        {
            LuckyGirl = false;
            return true;
        }
        public bool CureImprisoned(Combatant source)
        {
            Imprisoned = false;
            if (!Stop)
            {
                UnpauseTimers();
                if (Sleep || Petrify || Paralysed)
                    TurnTimer.Pause();
            }
            return true;
        }
        
        protected void CureAll(Combatant source)
        {
            CureFury(source);
            CureSadness(source);
            CureSleep(source);
            CurePoison(source);
            CureConfusion(source);
            CureSilence(source);
            CureHaste(source);
            CureSlow(source);
            CureStop(source);
            CureFrog(source);
            CureSmall(source);
            CureSlowNumb(source);
            CurePetrify(source);
            CureRegen(source);
            CureBarrier(source);
            CureMBarrier(source);
            CureReflect(source);
            CureShield(source);
            CureDeathSentence(source);
            CureManipulate(source);
            CureBerserk(source);
            CurePeerless(source);
            CureParalyzed(source);
            CureDarkness(source);
            CureSeizure(source);
            CureDeathForce(source);
            CureResist(source);
            CureLuckyGirl(source);
            CureImprisoned(source);
        }

        protected abstract void Kill(Combatant source);


        #endregion Cure


        
        public abstract bool Weak(IEnumerable<Element> elements);
        public abstract bool Halves(IEnumerable<Element> elements);
        public abstract bool Voids(IEnumerable<Element> elements);
        public abstract bool Absorbs(IEnumerable<Element> elements);
        public abstract bool Weak(Element e);
        public abstract bool Halves(Element e);
        public abstract bool Voids(Element e);
        public abstract bool Absorbs(Element e);
        public abstract bool Immune(Status s);

        public abstract IList<Element> Weaknesses { get; }

        
        
        
        public abstract int Atk { get; }
        //int AttackPercent { get; }
        public abstract int Def { get; }
        public abstract int Defp { get; }
        public abstract int Mat { get; }
        public abstract int MDef { get; }
        public abstract int MDefp { get; }
        
        public abstract int Dexterity { get; }
        public abstract int Luck { get; }
        public abstract int Level { get; }
        
        public abstract int HP { get; }
        public abstract int MP { get; }
        public abstract int MaxHP { get; }
        public abstract int MaxMP { get; }
        
        public abstract string Name { get; }

        public abstract bool LongRange { get; }
        
        public abstract bool BackRow { get; }
        public abstract bool Sensed { get; }
        
        
        public abstract bool Death { get; }
        public abstract bool NearDeath { get; }
        public abstract bool Sadness { get; }
        public abstract bool Fury { get; }

        public bool Sleep { get { return SleepTime >= 0; } } 
        public bool Poison { get; protected set; } 
        public bool Confusion  { get; protected set; } 
        public bool Silence  { get; protected set; } 
        public bool Haste  { get; protected set; } 
        public bool Slow  { get; protected set; } 
        public bool Stop { get; protected set; } 
        public bool Frog  { get; protected set; } 
        public bool Small  { get; protected set; } 
        public bool SlowNumb  { get; protected set; } 
        public bool Petrify { get; protected set; } 
        public bool Regen  { get; protected set; } 
        public bool Barrier { get; protected set; } 
        public bool MBarrier  { get; protected set; } 
        public bool Reflect  { get; protected set; } 
        public bool Shield  { get; protected set; } 
        public bool DeathSentence { get; protected set; } 
        public bool Manipulate { get; protected set; } 
        public bool Berserk { get; protected set; } 
        public bool Peerless { get; protected set; } 
        public bool Paralysed { get; protected set; } 
        public bool Darkness { get; protected set; } 
        public bool Seizure { get; protected set; } 
        public bool DeathForce { get; protected set; } 
        public bool Resist { get; protected set; } 
        public bool LuckyGirl { get; protected set; } 
        public bool Imprisoned { get; protected set; } 
        
        public BattleClock C_Timer { get; protected set; } 
        public BattleClock V_Timer { get; protected set; } 
        public TurnTimer TurnTimer { get; protected set; } 
        
        
        public bool IsDead { get { return Petrify || Imprisoned || Death; } }
        
        public bool CannotAct { get { return IsDead || Sleep || Berserk || Confusion || Paralysed; } }

        private bool _waiting;
        public bool WaitingToResolve
        { 
            get
            {
                return _waiting;
            }
            set
            {
                Console.WriteLine("{0} is {1} waiting", Name, value ? "now" : "no longer");

                _waiting = value;
            }
        }
        
        public int X { get { return _x; } }
        public int Y { get { return _y; } }

        public Color NameColor
        {
            get
            { 
                Color nameColor = Colors.WHITE;
                
                if (Death)
                {
                    nameColor = Colors.TEXT_RED;
                }
                else if (NearDeath)
                {
                    nameColor = Colors.YELLOW;
                }

                return nameColor;
            }
        }

        
        public Combatant LastAttacker { get; protected set; }
        
        public Combatant LastAttackerPhysical { get; protected set; }
        
        public Combatant LastAttackerMagical { get; protected set; }

        public Combatant Poisoner { get; private set; }
        
        public Combatant Seizer { get; private set; }
        
        public Combatant Petrifier { get; private set; }

        public Combatant Regenerator { get; private set; }

        public Combatant Sentencer { get; private set; }

        public BattleState CurrentBattle { get; private set; }
    }
}
