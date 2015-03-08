using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Text;
using System.Threading;

namespace Atmosphere.BattleSimulator
{

    public interface ICombatant : IDisposable
    {
        void AcceptDamage(ICombatant attacker, int delta);
        void Draw(Cairo.Context g);
        void CheckTimers();
        void Sense();

        int Atk { get; }
        //int AttackPercent { get; }
        int Def { get; }
        int Defp { get; }
        int Mat { get; }
        int MDef { get; }
        int MDefp { get; }

        int Dexterity { get; }
        int Luck { get; }
        int Level { get; }

        int HP { get; }
        int MP { get; set; }
        int MaxHP { get; }
        int MaxMP { get; }

        bool BackRow { get; }
        bool Sensed { get; }

        string Name { get; }

        #region Status
        bool Sadness { get; }
        bool Fury { get; }
        bool NearDeath { get; }
        bool Death { get; }
        bool Sleep { get; }
        bool Poison { get; }
        bool Confusion { get; }
        bool Silence { get; }
        bool Haste { get; }
        bool Slow { get; }
        bool Stop { get; }
        bool Frog { get; }
        bool Small { get; }
        bool SlowNumb { get; }
        bool Petrify { get; }
        bool Regen { get; }
        bool Barrier { get; }
        bool MBarrier { get; }
        bool Reflect { get; }
        bool Shield { get; }
        bool DeathSentence { get; }
        bool Manipulate { get; }
        bool Berserk { get; }
        bool Peerless { get; }
        bool Paralysed { get; }
        bool Darkness { get; }
        bool Seizure { get; }
        bool DeathForce { get; }
        bool Resist { get; }
        bool LuckyGirl { get; }
        bool Imprisoned { get; }
        #endregion Status

        #region Inflict
        bool InflictDeath();
        bool InflictFury();
        bool InflictSadness();
        bool InflictSleep();
        bool InflictPoison();
        bool InflictConfusion();
        bool InflictSilence();
        bool InflictHaste();
        bool InflictSlow();
        bool InflictStop();
        bool InflictFrog();
        bool InflictSmall();
        bool InflictSlowNumb();
        bool InflictPetrify();
        bool InflictRegen();
        bool InflictBarrier();
        bool InflictMBarrier();
        bool InflictReflect();
        bool InflictShield();
        bool InflictDeathSentence();
        bool InflictManipulate();
        bool InflictBerserk();
        bool InflictPeerless();
        bool InflictParalyzed();
        bool InflictDarkness();
        bool InflictSeizure();
        bool InflictDeathForce();
        bool InflictResist();
        bool InflictLuckyGirl();
        bool InflictImprisoned();
        #endregion Inflict

        #region Cure
        bool CureDeath();
        bool CureFury();
        bool CureSadness();
        bool CureSleep();
        bool CurePoison();
        bool CureConfusion();
        bool CureSilence();
        bool CureHaste();
        bool CureSlow();
        bool CureStop();
        bool CureFrog();
        bool CureSmall();
        bool CureSlowNumb();
        bool CurePetrify();
        bool CureRegen();
        bool CureBarrier();
        bool CureMBarrier();
        bool CureReflect();
        bool CureShield();
        bool CureDeathSentence();
        bool CureManipulate();
        bool CureBerserk();
        bool CurePeerless();
        bool CureParalyzed();
        bool CureDarkness();
        bool CureSeizure();
        bool CureDeathForce();
        bool CureResist();
        bool CureLuckyGirl();
        bool CureImprisoned();
        #endregion Cure

        bool Weak(params Element[] e);
        bool Halves(params Element[] e);
        bool Voids(params Element[] e);
        bool Absorbs(params Element[] e);
        

        ScaledClock C_Timer { get; }
        ScaledClock V_Timer { get; }
        ScaledTimer TurnTimer { get; }
        
        int X { get; }
        int Y { get; }

        AbilityState Ability { get; set; }
    }

    public abstract class Combatant : ICombatant
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

        protected bool _sleep = false;
        protected bool _poison = false;
        protected bool _confusion = false;
        protected bool _silence = false;
        protected bool _haste = false;
        protected bool _slow = false;
        protected bool _stop = false;
        protected bool _frog = false;
        protected bool _small = false;
        protected bool _slowNumb = false;
        protected bool _petrify = false;
        protected bool _regen = false;
        protected bool _barrier = false;
        protected bool _mbarrier = false;
        protected bool _reflect = false;
        protected bool _shield = false;
        protected bool _deathSentence = false;
        protected bool _berserk = false;
        protected bool _peerless = false;
        protected bool _paralyzed = false;
        protected bool _darkness = false;
        protected bool _seizure = false;
        protected bool _deathForce = false;
        protected bool _resist = false;
        protected bool _luckyGirl = false;
        protected bool _imprisoned = false;

        protected long _sleepTime = -1;
        protected long _poisonTime = -1;
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

        protected ScaledClock _c_timer;
        protected ScaledClock _v_timer;
        protected ScaledTimer _turnTimer;

        protected Thread _confuThread;
        protected Thread _berserkThread;

        protected AbilityState _abilityState;







        public abstract void Draw(Cairo.Context g);
        public abstract void Dispose();

        public abstract void AcceptDamage(ICombatant attacker, int delta);
        public virtual void Sense() { }

        protected abstract void ConfuAI();
        protected abstract void BerserkAI();

        public void CheckTimers()
        {

            if (Sleep && _v_timer.Elapsed - _sleepTime >= SLEEP_DURATION)
                CureSleep();


            if (Poison && _v_timer.Elapsed - _poisonTime >= POISON_INTERVAL)
            {
                AcceptDamage(this, MaxHP / 32);
                _poisonTime = _v_timer.Elapsed;
            }


            if (SlowNumb && _c_timer.Elapsed - _slownumbTime >= SLOWNUMB_DURATION)
            {
                CureSlowNumb();
                InflictPetrify();
            }


            if (Regen && _v_timer.Elapsed - _regenTimeEnd >= REGEN_DURATION)
                CureRegen();
            if (Regen && _v_timer.Elapsed - _regenTimeInt >= REGEN_INTERVAL)
            {
                AcceptDamage(this, MaxHP / 32);
                _regenTimeInt = _v_timer.Elapsed;
            }


            if (Barrier && _v_timer.Elapsed - _barrierTime >= BARRIER_DURATION)
                CureBarrier();


            if (MBarrier && _v_timer.Elapsed - _barrierTime >= MBARRIER_DURATION)
                CureMBarrier();


            if (Shield && _v_timer.Elapsed - _shieldTime >= SHIELD_DURATION)
                CureShield();


            if (DeathSentence && _c_timer.Elapsed - _deathsentenceTime >= DEATHSENTENCE_DURATION)
            {
                CureDeathSentence();
                InflictDeath();
            }


            if (Peerless && _v_timer.Elapsed - _peerlessTime >= PEERLESS_DURATION)
                CurePeerless();


            if (Paralysed && _v_timer.Elapsed - _paralyzedTime >= PARALYZED_DURATION)
                CureParalyzed();


            if (Seizure && _v_timer.Elapsed - _seizureTimeEnd >= SEIZURE_DURATION)
                CureSeizure();
            if (Seizure && _v_timer.Elapsed - _seizureTimeInt >= SEIZURE_INTERVAL)
            {
                AcceptDamage(this, MaxHP / -32);
                _seizureTimeInt = _v_timer.Elapsed;
            }
        }

        protected void DoubleTimers()
        {
            _c_timer.DoubleSpeed();
            _v_timer.DoubleSpeed();
            ReLinkTimers();
        }
        protected void HalveTimers()
        {
            _c_timer.HalveSpeed();
            _v_timer.HalveSpeed();
            ReLinkTimers();
        }
        protected void ReLinkTimers()
        {
            _turnTimer.TicksPer = Globals.TurnTimerSpeed(this, _v_timer.TicksPer);
        }
        protected void PauseTimers()
        {
            _c_timer.Pause();
            _v_timer.Pause();
            _turnTimer.Pause();
        }
        protected void UnpauseTimers()
        {
            _c_timer.Unpause();
            _v_timer.Unpause();
            _turnTimer.Unpause();
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
        public abstract bool CureDeath();
        public abstract bool CureFury();
        public abstract bool CureSadness();
        public bool CureSleep()
        {
            _sleepTime = -1;
            _sleep = false;
            if (!(Stop || Petrify || Paralysed || Imprisoned))
                _turnTimer.Unpause();
            return true;
        }
        public bool CurePoison()
        {
            _poisonTime = -1;
            _poison = false;
            return true;
        }
        public bool CureConfusion()
        {
            _confusion = false;
            if (_confuThread != null && _confuThread.IsAlive)
                _confuThread.Abort();
            if (_confuThread != null)
            _confuThread = null;
            return true;
        }
        public bool CureSilence()
        {
            _silence = false;
            return true;
        }
        public bool CureHaste()
        {
            _haste = false;
            HalveTimers();
            return true;
        }
        public bool CureSlow()
        {
            _slow = false;
            DoubleTimers();
            return true;
        }
        public bool CureStop()
        {
            _stop = false;
            if (!Imprisoned)
            {
                UnpauseTimers();
                if (Sleep || Petrify || Paralysed)
                    _turnTimer.Pause();
            }
            return true;
        }
        public bool CureFrog()
        {
            _frog = false;
            return true;
        }
        public bool CureSmall()
        {
            _small = false;
            return true;
        }
        public bool CureSlowNumb()
        {
            _slownumbTime = -1;
            _slowNumb = false;
            return true;
        }
        public bool CurePetrify()
        {
            _petrify = false;
            if (!(Sleep || Stop || Paralysed || Imprisoned))
                _turnTimer.Reset();
            return true;
        }
        public bool CureRegen()
        {
            _regen = false;
            _regenTimeInt = -1;
            _regenTimeEnd = -1;
            return true;
        }
        public bool CureBarrier()
        {
            _barrier = false;
            _barrierTime = -1;
            return true;
        }
        public bool CureMBarrier()
        {
            _mbarrier = false;
            _mbarrierTime = -1;
            return true;
        }
        public bool CureReflect()
        {
            _reflect = false;
            return true;
        }
        public bool CureShield()
        {
            _shield = false;
            _shieldTime = -1;
            return true;
        }
        public bool CureDeathSentence()
        {
            _deathSentence = false;
            _deathsentenceTime = -1;
            return true;
        }
        public bool CureManipulate()
        {
            return true;
        }
        public bool CureBerserk()
        {
            _berserk = false;
            if (_berserkThread != null && _berserkThread.IsAlive)
                _berserkThread.Abort();
            if (_berserkThread != null)
                _berserkThread = null;
            return true;
        }
        public bool CurePeerless()
        {
            _peerless = false;
            _peerlessTime = -1;
            return true;
        }
        public bool CureParalyzed()
        {
            _paralyzed = false;
            _paralyzedTime = -1;
            if (!(Sleep || Stop || Petrify || Imprisoned))
                _turnTimer.Unpause();
            return true;
        }
        public bool CureDarkness()
        {
            _darkness = false;
            return true;
        }
        public bool CureSeizure()
        {
            _seizure = false;
            _seizureTimeEnd = -1;
            _seizureTimeInt = -1;
            return true;
        }
        public bool CureDeathForce()
        {
            _deathForce = false;
            return true;
        }
        public bool CureResist()
        {
            _resist = false;
            return true;
        }
        public bool CureLuckyGirl()
        {
            _luckyGirl = false;
            return true;
        }
        public bool CureImprisoned()
        {
            _imprisoned = false;
            if (!Stop)
            {
                UnpauseTimers();
                if (Sleep || Petrify || Paralysed)
                    _turnTimer.Pause();
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

        public abstract bool BackRow { get; }
        public abstract bool Sensed { get; }


        public abstract bool Death { get; }
        public abstract bool NearDeath { get; }
        public abstract bool Sadness { get; }
        public abstract bool Fury { get; }
        public bool Sleep { get { return _sleep; } }
        public bool Poison { get { return _poison; } }
        public bool Confusion { get { return _confusion; } }
        public bool Silence { get { return _silence; } }
        public bool Haste { get { return _haste; } }
        public bool Slow { get { return _slow; } }
        public bool Stop { get { return _stop; } }
        public bool Frog { get { return _frog; } }
        public bool Small { get { return _small; } }
        public bool SlowNumb { get { return _slowNumb; } }
        public bool Petrify { get { return _petrify; } }
        public bool Regen { get { return _regen; } }
        public bool Barrier { get { return _barrier; } }
        public bool MBarrier { get { return _mbarrier; } }
        public bool Reflect { get { return _reflect; } }
        public bool Shield { get { return _shield; } }
        public bool DeathSentence { get { return _deathSentence; } }
        public bool Manipulate { get { return false; } }
        public bool Berserk { get { return _berserk; } }
        public bool Peerless { get { return _peerless; } }
        public bool Paralysed { get { return _paralyzed; } }
        public bool Darkness { get { return _darkness; } }
        public bool Seizure { get { return _seizure; } }
        public bool DeathForce { get { return _deathForce; } }
        public bool Resist { get { return _resist; } }
        public bool LuckyGirl { get { return _luckyGirl; } }
        public bool Imprisoned { get { return _imprisoned; } }

        public ScaledClock C_Timer { get { return _c_timer; } }
        public ScaledClock V_Timer { get { return _v_timer; } }
        public ScaledTimer TurnTimer { get { return _turnTimer; } }

        public int X { get { return _x; } }
        public int Y { get { return _y; } }

        public AbilityState Ability { get { return _abilityState; } set { _abilityState = value; } }
    }


}
