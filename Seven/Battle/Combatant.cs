using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Atmosphere.Reverence.Time;
using Atmosphere.Reverence.Seven.Asset;

namespace Atmosphere.Reverence.Seven.Battle
{
    public abstract class Combatant
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

        
        protected System.Threading.Thread _confuThread;
        protected System.Threading.Thread _berserkThread;

        
        
        
        
        
        
        
        public abstract void Draw(Cairo.Context g);
        public abstract void Dispose();
        
        public abstract void AcceptDamage(Combatant attacker, AttackType type, int delta);
        public virtual void Sense() { }
        
        protected abstract void ConfuAI();
        protected abstract void BerserkAI();
        
        public void CheckTimers()
        {
            
            if (Sleep && V_Timer.TotalMilliseconds - _sleepTime >= SLEEP_DURATION)
                CureSleep();
            
            
            if (Poison && V_Timer.TotalMilliseconds - PoisonTime >= POISON_INTERVAL)
            {
                AcceptDamage(this, AttackType.None, MaxHP / 32);
                PoisonTime = V_Timer.TotalMilliseconds;
            }
            
            
            if (SlowNumb && C_Timer.TotalMilliseconds - _slownumbTime >= SLOWNUMB_DURATION)
            {
                CureSlowNumb();
                InflictPetrify();
            }
            
            
            if (Regen && V_Timer.TotalMilliseconds - _regenTimeEnd >= REGEN_DURATION)
                CureRegen();
            if (Regen && V_Timer.TotalMilliseconds - _regenTimeInt >= REGEN_INTERVAL)
            {
                AcceptDamage(this, AttackType.None, MaxHP / 32);
                _regenTimeInt = V_Timer.TotalMilliseconds;
            }
            
            
            if (Barrier && V_Timer.TotalMilliseconds - _barrierTime >= BARRIER_DURATION)
                CureBarrier();
            
            
            if (MBarrier && V_Timer.TotalMilliseconds - _barrierTime >= MBARRIER_DURATION)
                CureMBarrier();
            
            
            if (Shield && V_Timer.TotalMilliseconds - _shieldTime >= SHIELD_DURATION)
                CureShield();
            
            
            if (DeathSentence && C_Timer.TotalMilliseconds - _deathsentenceTime >= DEATHSENTENCE_DURATION)
            {
                CureDeathSentence();
                //InflictDeath();
            }
            
            
            if (Peerless && V_Timer.TotalMilliseconds - _peerlessTime >= PEERLESS_DURATION)
                CurePeerless();
            
            
            if (Paralysed && V_Timer.TotalMilliseconds - _paralyzedTime >= PARALYZED_DURATION)
                CureParalyzed();
            
            
            if (Seizure && V_Timer.TotalMilliseconds - _seizureTimeEnd >= SEIZURE_DURATION)
                CureSeizure();
            if (Seizure && V_Timer.TotalMilliseconds - _seizureTimeInt >= SEIZURE_INTERVAL)
            {
                AcceptDamage(this, AttackType.None, MaxHP / -32);
                _seizureTimeInt = V_Timer.TotalMilliseconds;
            }
        }
        
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
            //TurnTimer.TicksPer = Seven.Party.TurnTimerSpeed(this, V_Timer.TicksPer);
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
        
        
        
        #region Inflict
        public abstract bool InflictDeath();
        public abstract bool InflictFury();
        public abstract bool InflictSadness();
        public abstract bool InflictSleep();
        public abstract bool InflictPoison();
        public abstract bool InflictConfusion();
        public abstract bool InflictSilence();
        public abstract bool InflictHaste();
        public abstract bool InflictSlow();
        public abstract bool InflictStop();
        public abstract bool InflictFrog();
        public abstract bool InflictSmall();
        public abstract bool InflictSlowNumb();
        public abstract bool InflictPetrify();
        public abstract bool InflictRegen();
        public abstract bool InflictBarrier();
        public abstract bool InflictMBarrier();
        public abstract bool InflictReflect();
        public abstract bool InflictShield();
        public abstract bool InflictDeathSentence();
        public abstract bool InflictManipulate();
        public abstract bool InflictBerserk();
        public abstract bool InflictPeerless();
        public abstract bool InflictParalyzed();
        public abstract bool InflictDarkness();
        public abstract bool InflictSeizure();
        public abstract bool InflictDeathForce();
        public abstract bool InflictResist();
        public abstract bool InflictLuckyGirl();
        public abstract bool InflictImprisoned();
        #endregion Inflict
        
        #region Cure
        //public abstract bool CureDeath();
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
            Poison = false;
            return true;
        }
        public bool CureConfusion()
        {
            Confusion = false;
            if (_confuThread != null && _confuThread.IsAlive)
                _confuThread.Abort();
            if (_confuThread != null)
                _confuThread = null;
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
            return true;
        }
        public bool CureManipulate()
        {
            return true;
        }
        public bool CureBerserk()
        {
            Berserk = false;
            if (_berserkThread != null && _berserkThread.IsAlive)
                _berserkThread.Abort();
            if (_berserkThread != null)
                _berserkThread = null;
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
        
        
        public abstract bool Weak(params Element[] e);
        public abstract bool Halves(params Element[] e);
        public abstract bool Voids(params Element[] e);
        public abstract bool Absorbs(params Element[] e);
        
        
        
        
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
        public abstract int MP { get; set; }
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
        
        public bool WaitingToResolve { get; set; }
        
        public int X { get { return _x; } }
        public int Y { get { return _y; } }
    }
}