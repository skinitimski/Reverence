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


        
        
        
        
        
        
        
        public abstract void Draw(Cairo.Context g);
        public abstract void Dispose();
        
        public abstract void AcceptDamage(int delta, AttackType type = AttackType.None);
        public abstract void AcceptMPLoss(int delta);
       

        public abstract void UseMP(int amount);


        public virtual void Sense() { }



        protected void PhysicalAttack(int power, int atkp, Combatant target, Element[] elements, bool resetTurnTimer = true, string msg = " attacks")
        {
            BattleEvent e = new BattleEvent(this, () => PhysicalAttack(power, atkp, this, target, elements));

            if (Confusion)
            {
                msg += " (confused)";
            }
            else if (Berserk)
            {
                msg += " (berserk)";
            }

            e.Dialogue = c => Name + msg;
            e.ResetSourceTurnTimer = resetTurnTimer;

            Seven.BattleState.EnqueueAction(e);
        }

        public void UseSpell(IEnumerable<Combatant> targets, Spell spell, SpellModifiers modifiers, bool resetTurnTimer = true)
        {
            bool canUse = true;
            
            if (!modifiers.CostsNothing)
            {
                if (MP >= spell.MPCost)
                {
                    UseMP(spell.MPCost);
                }
                else
                {
                    canUse = false;
                }
            }
            
            if (canUse)
            {
                BattleEvent e = new BattleEvent(this, () => spell.Cast(this, targets, modifiers));
                       
                e.Dialogue = c => Name + " casts " + spell.Name;
                e.ResetSourceTurnTimer = resetTurnTimer;
            
                Seven.BattleState.EnqueueAction(e);
            }
            else
            {
                BattleEvent e = new BattleEvent(this, () => { });

                string msg = "Not enough MP for " + Name + "!";

                e.Dialogue = c => msg;
                e.ResetSourceTurnTimer = resetTurnTimer;

                Seven.BattleState.EnqueueAction(e);
            }
        }




        
        private static void PhysicalAttack(int power, int atkp, Combatant source, Combatant target, IEnumerable<Element> elements)
        {
            bool restorative = false;
                        
            if (Formula.PhysicalHit(atkp, source, target, elements))
            {
                int bd = Formula.PhysicalBase(source);
                int dam = Formula.PhysicalDamage(bd, power, target);

                dam = Formula.RunPhysicalModifiers(dam, ref restorative, source, target, elements);

                target.AcceptDamage(dam, AttackType.Physical);
            }
            else
            {
                Seven.BattleState.AddMissIcon(target);
            }
        }











        
        public void CheckTimers()
        {
            
            if (Sleep && V_Timer.TotalMilliseconds - _sleepTime >= SLEEP_DURATION)
                CureSleep();
            
            
            if (Poison && V_Timer.TotalMilliseconds - PoisonTime >= POISON_INTERVAL)
            {
                AcceptDamage(MaxHP / 32);
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
                AcceptDamage(MaxHP / 32);
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
                AcceptDamage(MaxHP / -32);
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
                
        public bool InflictSleep()
        {
            if (Immune(Status.Sleep))
                return false;
            if (Sleep || Petrify || Peerless || Resist) return false;
            Sleep = true;
            _sleepTime = V_Timer.TotalMilliseconds;
            TurnTimer.Pause();
            return true;
        }
        public bool InflictPoison()
        {
            if (Immune(Status.Poison))
                return false;
            if (Poison) return false;
            Poison = true;
            PoisonTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictConfusion()
        {
            if (Immune(Status.Confusion))
                return false;
            if (Confusion || Petrify || Peerless || Resist)
                return false;
            Confusion = true;
            return true;
        }
        public bool InflictSilence()
        {
            if (Immune(Status.Silence))
                return false;
            if (Silence || Petrify || Peerless || Resist)
                return false;
            Silence = true;
            return true;
        }
        public bool InflictHaste()
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
        public bool InflictSlow()
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
        public bool InflictStop()
        {
            if (Immune(Status.Stop))
                return false;
            if (Stop || Petrify || Peerless || Resist)
                return false;
            Stop = true;
            PauseTimers();
            return true;
        }
        public bool InflictFrog()
        {
            if (Immune(Status.Frog))
                return false;
            if (Frog || Petrify || Peerless || Resist)
                return false;
            Frog = true;
            return true;
        }
        public bool InflictSmall()
        {
            if (Immune(Status.Small))
                return false;
            if (Small || Petrify || Peerless || Resist)
                return false;
            Small = true;
            return true;
        }
        public bool InflictSlowNumb()
        {
            if (Immune(Status.SlowNumb))
                return false;
            if (SlowNumb || Petrify || Peerless || Resist)
                return false;
            SlowNumb = true;
            _slownumbTime = C_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictPetrify()
        {
            if (Immune(Status.Petrify))
                return false;
            if (Petrify || Peerless || Resist)
                return false;
            Petrify = true;
            return true;
        }
        public bool InflictRegen()
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
        public bool InflictBarrier()
        {
            if (Immune(Status.Barrier))
                return false;
            if (Barrier || Petrify || Peerless || Resist)
                return false;
            Barrier = true;
            _barrierTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictMBarrier()
        {
            if (Immune(Status.MBarrier))
                return false;
            if (MBarrier || Petrify || Peerless || Resist)
                return false;
            MBarrier = true;
            _mbarrierTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictReflect()
        {
            if (Immune(Status.Reflect))
                return false;
            if (Reflect || Petrify || Peerless || Resist)
                return false;
            Reflect = true;
            return true;
        }
        public bool InflictShield()
        {
            if (Immune(Status.Shield))
                return false;
            if (Shield || Petrify || Resist)
                return false;
            Shield = true;
            _shieldTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictDeathSentence()
        {
            if (Immune(Status.DeathSentence))
                return false;
            if (DeathSentence || Petrify || Peerless || Resist || DeathForce)
                return false;
            DeathSentence = true;
            _deathsentenceTime = C_Timer.TotalMilliseconds;
            return true;
        }

        public abstract bool InflictManipulate();

        public bool InflictBerserk()
        {
            if (Immune(Status.Berserk))
                return false;
            if (Berserk || Petrify || Peerless || Resist)
                return false;
            Berserk = true;
            return true;
        }
        public bool InflictPeerless()
        {
            if (Immune(Status.Peerless))
                return false;
            if (Peerless || Petrify || Resist)
                return false;
            Peerless = true;
            _peerlessTime = V_Timer.TotalMilliseconds;
            return true;
        }
        public bool InflictParalyzed()
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
        public bool InflictDarkness()
        {
            if (Immune(Status.Darkness))
                return false;
            if (Darkness || Petrify || Peerless || Resist)
                return false;
            Darkness = true;
            return true;
        }
        public bool InflictSeizure()
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
        public bool InflictDeathForce()
        {
            if (Immune(Status.DeathForce))
                return false;
            if (DeathForce || Petrify || Peerless || Resist)
                return false;
            DeathForce = true;
            return true;
        }
        public bool InflictResist()
        {
            if (Immune(Status.Resist))
                return false;
            if (Resist || Petrify || Peerless)
                return false;
            Resist = true;
            return true;
        }
        public bool InflictLuckyGirl()
        {
            if (Immune(Status.LuckyGirl))
                return false;
            if (LuckyGirl || Petrify || Peerless || Resist)
                return false;
            LuckyGirl = true;
            return true;
        }
        public bool InflictImprisoned()
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


        
        public abstract bool Weak(IEnumerable<Element> elements);
        public abstract bool Halves(IEnumerable<Element> elements);
        public abstract bool Voids(IEnumerable<Element> elements);
        public abstract bool Absorbs(IEnumerable<Element> elements);
        public abstract bool Weak(Element e);
        public abstract bool Halves(Element e);
        public abstract bool Voids(Element e);
        public abstract bool Absorbs(Element e);
        public abstract bool Immune(Status s);
        
        
        
        
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
    }
}
