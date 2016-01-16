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
using Atmosphere.Reverence.Seven.Screen.BattleState;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal abstract class Combatant
    {
        public const long SLEEP_DURATION = 30000; // 100 v-timer units
        public const long POISON_INTERVAL = 3000; // 10 v-timer units
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

        public const int TURN_TIMER_TIMEOUT = 6000;
        
        
        protected const int _icon_half_width = (Character.PROFILE_WIDTH_TINY / 2);
        protected const int _icon_half_height = (Character.PROFILE_HEIGHT_TINY / 2);

        protected const int _text_offset_x = _icon_half_width + 10;
        protected const int _text_offset_y = _icon_half_height - 30;
        protected const int _text_spacing_y = 20;


        protected int _x;
        protected int _y;
        
        protected long _sleepTime = -1;
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




        protected Combatant(int x, int y)
        {            
            _x = x;
            _y = y;
        }
        
        
        
        
        
        
        public void Draw(Gdk.Drawable d, Cairo.Context g, int width, int height)
        {
            DrawIcon(d, g);
            
            Text.ShadowedText(g, NameColor, Name, X + _text_offset_x, Y + _text_offset_y, Text.MONOSPACE_FONT, 12);

#if DEBUG
            string extraInfo = String.Format("{0}/{1} {2}/{3} {4}%", HP, MaxHP, MP, MaxMP, TurnTimer.PercentElapsed);
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
        public abstract void Recover();
       

        public abstract void UseMP(int amount);


        public virtual void Sense() { }



        











        
        public void CheckTimers()
        {
            
            if (Sleep && V_Timer.TotalMilliseconds - _sleepTime >= SLEEP_DURATION)
            {
                CureSleep();
            }
            
            
            if (Poison && V_Timer.TotalMilliseconds - PoisonTime >= POISON_INTERVAL)
            {
                AcceptDamage(Poisoner, MaxHP / 32);
                PoisonTime = V_Timer.TotalMilliseconds;
            }
            
            
            if (SlowNumb && C_Timer.TotalMilliseconds - _slownumbTime >= SLOWNUMB_DURATION)
            {
                InflictPetrify(Petrifier);
            }
            
            
            if (Regen && V_Timer.TotalMilliseconds - _regenTimeEnd >= REGEN_DURATION)
            {
                CureRegen();
            }
            if (Regen && V_Timer.TotalMilliseconds - _regenTimeInt >= REGEN_INTERVAL)
            {
                AcceptDamage(Regenerator, MaxHP / -32);
                _regenTimeInt = V_Timer.TotalMilliseconds;
            }
            
            
            if (Barrier && V_Timer.TotalMilliseconds - _barrierTime >= BARRIER_DURATION)
            {
                CureBarrier();
            }
            
            
            if (MBarrier && V_Timer.TotalMilliseconds - _barrierTime >= MBARRIER_DURATION)
            {
                CureMBarrier();
            }
            
            
            if (Shield && V_Timer.TotalMilliseconds - _shieldTime >= SHIELD_DURATION)
            {
                CureShield();
            }
            
            
            if (DeathSentence && C_Timer.TotalMilliseconds - _deathsentenceTime >= DEATHSENTENCE_DURATION)
            {
                InflictDeath(Sentencer);
            }
            
            
            if (Peerless && V_Timer.TotalMilliseconds - _peerlessTime >= PEERLESS_DURATION)
            {
                CurePeerless();
            }
            
            
            if (Paralysed && V_Timer.TotalMilliseconds - _paralyzedTime >= PARALYZED_DURATION)
            {
                CureParalyzed();
            }
            
            
            if (Seizure && V_Timer.TotalMilliseconds - _seizureTimeEnd >= SEIZURE_DURATION)
            {
                CureSeizure();
            }
            if (Seizure && V_Timer.TotalMilliseconds - _seizureTimeInt >= SEIZURE_INTERVAL)
            {
                AcceptDamage(Seizer, MaxHP / 32);
                _seizureTimeInt = V_Timer.TotalMilliseconds;
            }
        }

        protected abstract int GetTurnTimerStep(int vStep);
        
        protected void DoubleTimers()
        {
            C_Timer.DoubleSpeed();
            V_Timer.DoubleSpeed();
            ReLinkTimers();
        }
        protected void HalveTimers()
        {
            C_Timer.HalveSpeed();
            V_Timer.HalveSpeed();
            ReLinkTimers();
        }

        protected void ReLinkTimers()
        {
            TurnTimer.TicksPer = GetTurnTimerStep(V_Timer.TicksPer);
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
            if (Paralysed) { statuses.Append("Paralysed, "); }
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
            if (Sleep || Petrify || Peerless || Resist) return false;
            Sleep = true;
            _sleepTime = V_Timer.TotalMilliseconds;
            TurnTimer.Pause();
            return true;
        }
        public bool InflictPoison(Combatant source)
        {
            if (Immune(Status.Poison))
                return false;
            if (Poison) return false;
            Poison = true;
            Poisoner = source;
            PoisonTime = V_Timer.TotalMilliseconds;
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
                CureSlow();
            Haste = true;
            DoubleTimers();
            return true;
        }
        public bool InflictSlow(Combatant source)
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
            _slownumbTime = C_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictPetrify(Combatant source)
        {
            if (Immune(Status.Petrify))
                return false;
            if (Petrify || Peerless || Resist)
                return false;

            CureSlowNumb();

            Petrify = true;

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
            _regenTimeEnd = V_Timer.TotalMilliseconds;
            _regenTimeInt = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictBarrier(Combatant source)
        {
            if (Immune(Status.Barrier))
                return false;
            if (Barrier || Petrify || Peerless || Resist)
                return false;
            Barrier = true;
            _barrierTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictMBarrier(Combatant source)
        {
            if (Immune(Status.MBarrier))
                return false;
            if (MBarrier || Petrify || Peerless || Resist)
                return false;
            MBarrier = true;
            _mbarrierTime = V_Timer.TotalMilliseconds;
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
            _shieldTime = V_Timer.TotalMilliseconds;
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
            _deathsentenceTime = C_Timer.TotalMilliseconds;
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
            _peerlessTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictParalyzed(Combatant source)
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
            _seizureTimeEnd = V_Timer.TotalMilliseconds;
            _seizureTimeInt = V_Timer.TotalMilliseconds;
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
        public abstract bool CureDeath();
        public abstract bool CureFury();
        public abstract bool CureSadness();
        public bool CureSleep()
        {
            _sleepTime = -1;
            Sleep = false;
            if (!(Stop || Petrify || Paralysed || Imprisoned))
                TurnTimer.Unpause();
            return true;
        }
        public bool CurePoison()
        {
            PoisonTime = -1;
            Poisoner = null;
            Poison = false;
            return true;
        }
        public bool CureConfusion()
        {
            Confusion = false;
            return true;
        }
        public bool CureSilence()
        {
            Silence = false;
            return true;
        }
        public bool CureHaste()
        {
            Haste = false;
            HalveTimers();
            return true;
        }
        public bool CureSlow()
        {
            Slow = false;
            DoubleTimers();
            return true;
        }
        public bool CureStop()
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
        public bool CureFrog()
        {
            Frog = false;
            return true;
        }
        public bool CureSmall()
        {
            Small = false;
            return true;
        }
        public bool CureSlowNumb()
        {
            _slownumbTime = -1;
            SlowNumb = false;
            Petrifier = null;
            return true;
        }
        public bool CurePetrify()
        {
            Petrify = false;
            if (!(Sleep || Stop || Paralysed || Imprisoned))
                TurnTimer.Reset();
            return true;
        }
        public bool CureRegen()
        {
            Regen = false;
            Regenerator = null;
            _regenTimeInt = -1;
            _regenTimeEnd = -1;
            return true;
        }
        public bool CureBarrier()
        {
            Barrier = false;
            _barrierTime = -1;
            return true;
        }
        public bool CureMBarrier()
        {
            MBarrier = false;
            _mbarrierTime = -1;
            return true;
        }
        public bool CureReflect()
        {
            Reflect = false;
            return true;
        }
        public bool CureShield()
        {
            Shield = false;
            _shieldTime = -1;
            return true;
        }
        public bool CureDeathSentence()
        {
            DeathSentence = false;
            _deathsentenceTime = -1;
            Sentencer = null;
            return true;
        }
        public bool CureManipulate()
        {
            return true;
        }
        public bool CureBerserk()
        {
            Berserk = false;
            return true;
        }
        public bool CurePeerless()
        {
            Peerless = false;
            _peerlessTime = -1;
            return true;
        }
        public bool CureParalyzed()
        {
            Paralysed = false;
            _paralyzedTime = -1;
            if (!(Sleep || Stop || Petrify || Imprisoned))
                TurnTimer.Unpause();
            return true;
        }
        public bool CureDarkness()
        {
            Darkness = false;
            return true;
        }
        public bool CureSeizure()
        {
            Seizure = false;
            Seizer = null;
            _seizureTimeEnd = -1;
            _seizureTimeInt = -1;
            return true;
        }
        public bool CureDeathForce()
        {
            DeathForce = false;
            return true;
        }
        public bool CureResist()
        {
            Resist = false;
            return true;
        }
        public bool CureLuckyGirl()
        {
            LuckyGirl = false;
            return true;
        }
        public bool CureImprisoned()
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

        public bool Sleep { get; protected set; } 
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
        
        public Clock C_Timer { get; protected set; } 
        public Clock V_Timer { get; protected set; } 
        public Timer TurnTimer { get; protected set; } 

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

        public Combatant Poisoner { get; private set; }
        
        public Combatant Seizer { get; private set; }
        
        public Combatant Petrifier { get; private set; }

        public Combatant Regenerator { get; private set; }

        public Combatant Sentencer { get; private set; }
    }
}
