using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Atmosphere.BattleSimulator
{
    static class Formula
    {


        public static void PhysicalAttack(int power, AbilityState state, int targetIndex)
        {
            ICombatant er = state.Performer;
            ICombatant ee = state.Target[targetIndex];

            bool restorative = false;

            if (!PhysicalHit(state, targetIndex))
                return;

            int bd = PhysicalBase(er);
            int dam = PhysicalDamage(bd, power, ee);

            dam = Critical(dam, state, targetIndex);
            dam = Berserk(dam, state);
            dam = RowCheck(dam, state, targetIndex);
            dam = Frog(dam, state);
            dam = Sadness(dam, ee);
            dam = Split(dam, state);
            dam = Barrier(dam, state, targetIndex);
            dam = MPTurbo(dam, state);
            dam = Mini(dam, state);
            dam = RandomVariation(dam);
            dam = LowerSanityCkeck(dam);
            dam = RunElementalChecks(dam, ref restorative, state, targetIndex);
            dam = UpperSanityCheck(dam);

            ee.AcceptDamage(er, dam);
        }

        public static void MagicSpell(int power, AbilityState state, int targetIndex)
        {
            ICombatant er = state.Performer;
            ICombatant ee = state.Target[targetIndex];

            if (!MagicHit(state, targetIndex))
                return;

            bool restorative = false;
            int bd = MagicalBase(er);
            int dam = MagicalDamage(bd, 4, ee);

            RunMagicModifiers(dam, ref restorative, state, targetIndex);

            if (restorative)
                dam = -dam;

            ee.AcceptDamage(er, dam);
        }
        public static void MagicChangeStatus()
        {
        }

        public static int RunMagicModifiers(int dam, ref bool restorative, AbilityState state, int targetIndex)
        {
            dam = Sadness(dam, state.Target[targetIndex]);
            dam = Split(dam, state);
            dam = Barrier(dam, state, targetIndex);
            dam = MPTurbo(dam, state);
            dam = RandomVariation(dam);
            dam = LowerSanityCkeck(dam);
            dam = RunElementalChecks(dam, ref restorative, state, targetIndex);
            dam = UpperSanityCheck(dam);

            return dam;
        }



        public static int PhysicalBase(ICombatant er)
        {
            return er.Atk + ((er.Atk + er.Level) / 32) * (er.Atk * er.Level / 32);
        }

        public static int PhysicalDamage(int bd, int power, ICombatant ee)
        {
            return (power * (512 - ee.Def) * bd) / (16 * 512);
        }

        public static bool PhysicalHit(AbilityState state, int targetIndex)
        {
            ICombatant er = state.Performer;
            ICombatant ee = state.Target[targetIndex];

            int hitp;

            if (ee.Absorbs(state.Elements) ||
                ee.Voids(state.Elements) ||
                ee.Death || ee.Sleep ||
                ee.Confusion || ee.Stop ||
                ee.Petrify || ee.Manipulate ||
                ee.Paralysed || ee.Peerless)
                hitp = 255;
            else 
            {
                hitp = ((er.Dexterity / 4) + state.HitP) 
                    + er.Defp
                    - ee.Defp;
                if (er.Fury)
                    hitp = hitp - hitp * 3 / 10;
            }

            // Sanity
            if (hitp < 1)
                hitp = 1;

            int lucky = Game.Random.Next(0, 100);

            // Lucky Hit
            if (lucky < Math.Floor(state.Performer.Luck / 4.0d))
                hitp = 255;
            // Lucky Evade
            else if (lucky < Math.Floor(ee.Luck / 4.0d))
                if (er is Ally && ee is Enemy)
                    hitp = 0;

            int r = Game.Random.Next(65536) * 99 / 65536 + 1;

            if (r < hitp)
                return true;
            else return false;

        }

       








        public static int MagicalBase(ICombatant er)
        {
            return 6 * (er.Mat + er.Level);
        }

        public static int MagicalDamage(int bd, int power, ICombatant ee)
        {
            return (power * (512 - ee.Def) * bd) / (16 * 512);
        }

        public static bool MagicHit(AbilityState state, int targetIndex)
        {
            if (state.HitP == 255)
                return true;
            if (state.Target[targetIndex].Absorbs(state.Elements))
                return true;
            if (state.Target[targetIndex].Voids(state.Elements))
                return true;
            if (state.Target[targetIndex].Death ||
                state.Target[targetIndex].Sleep ||
                state.Target[targetIndex].Confusion ||
                state.Target[targetIndex].Stop ||
                state.Target[targetIndex].Petrify ||
                state.Target[targetIndex].Paralysed ||
                state.Target[targetIndex].Peerless ||
                state.Target[targetIndex].Reflect)
                return true;

            int matp = state.HitP;

            if (state.Performer.Fury)
                matp = matp - matp * 3 / 10;

            if (Game.Random.Next(1, 101) > state.Target[targetIndex].MDefp)
                return false;

            int hitp = matp + state.Performer.Level - state.Target[targetIndex].Level / 2 - 1;

            if (Game.Random.Next(100) < hitp)
                return true;
            else return false;
        }



        public static int Critical(int dam, AbilityState state, int targetIndex)
        {
            ICombatant ee = state.Target[targetIndex];
            Ally er = state.Performer as Ally;

            if (er == null)
                return dam;

            int critp;

            if (er.LuckyGirl)
                critp = 255;
            else
            {
                critp = (er.Luck + er.Level - ee.Level) / 4;
                critp = critp + er.Weapon.CriticalPercent;
            }

            int r = (Game.Random.Next(65536) * 99 / 65536) + 1;

            if (r <= critp)
                dam = dam * 2;
            return dam;
        }

        public static int Berserk(int dam, AbilityState state)
        {
            if (state.Performer.Berserk)
                dam = dam * 15 / 10;
            return dam;
        }

        public static int RowCheck(int dam, AbilityState state, int targetIndex)
        {
            if (state.LongRange)
                return dam;
            if (state.Performer.BackRow || state.Target[targetIndex].BackRow)
                dam = dam / 2;

            return dam;
        }

        public static int Frog(int dam, AbilityState state)
        {
            if (state.Performer.Frog)
                dam = dam / 4;
            return dam;
        }



        public static int Sadness(int dam, ICombatant ee)
        {
            if (ee.Sadness)
                dam = dam * 7 / 10;
            return dam;
        }

        public static int Split(int dam, AbilityState state)
        {
            if (state.QuadraMagic)
                dam = dam / 2;
            else if (state.Target.Length > 1 && !state.NoSplit)
                dam = dam * 2 / 3;
            return dam;
        }

        public static int Barrier(int dam, AbilityState state, int targetIndex)
        {
            ICombatant ee = state.Target[targetIndex];
            switch (state.Type)
            {
                case AttackType.Magical:
                    if (ee.MBarrier) dam = dam / 2;
                    break;
                case AttackType.Physical:
                    if (ee.Barrier) dam = dam / 2;
                    break;
                default: break;
            }
            return dam;
        }

        public static int MPTurbo(int dam, AbilityState state)
        {
            dam = dam + (dam * 10 * state.MPTurboFactor) / 10;
            return dam;
        }
        public static int Mini(int dam, AbilityState state)
        {
            if (state.Performer.Small)
                dam = 0;
            return dam;
        }

        public static int RandomVariation(int dam)
        {
            dam = dam * (3841 + Game.Random.Next(256)) / 4096;
            return dam;
        }


        public static int RunElementalChecks(int dam, ref bool restorative, AbilityState state, int targetIndex)
        {
            bool checksDone = false;

            foreach (Element e in state.Elements)
                if (state.Target[targetIndex].Voids(e))
                {
                    dam = 0;
                    checksDone = true;
                    break;
                }
            if (!checksDone)
                foreach (Element e in state.Elements)
                    if (state.Target[targetIndex].Absorbs(e))
                    {
                        restorative = !restorative;
                        checksDone = true;
                        break;
                    }
            if (!checksDone)
                foreach (Element e in state.Elements)
                {
                    if (state.Target[targetIndex].Halves(e) && state.Target[targetIndex].Weak(e))
                        continue;
                    else if (state.Target[targetIndex].Halves(e))
                    {
                        dam = dam / 2;
                        break;
                    }
                    else if (state.Target[targetIndex].Weak(e))
                    {
                        dam = dam * 2;
                        break;
                    }
                }
            return dam;
        }

        public static int LowerSanityCkeck(int dam)
        {
            if (dam == 0)
                dam = 1;
            return dam;
        }

        public static int UpperSanityCheck(int dam)
        {
            if (dam > 9999)
                dam = 9999;
            return dam;
        }


    }
}
