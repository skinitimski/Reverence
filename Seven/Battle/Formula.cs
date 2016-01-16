using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal delegate int DamageFormula(Combatant source, Combatant target, AbilityModifiers modifiers);

    internal static class Formula
    {
        public static int RunPhysicalModifiers(int dam, Combatant source, Combatant target, IEnumerable<Element> elements)
        {
            dam = Formula.Critical(dam, source, target);
            dam = Formula.Berserk(dam, source);
            dam = Formula.RowCheck(dam, source, target);
            dam = Formula.Frog(dam, target);
            dam = Formula.Sadness(dam, target);
            dam = Formula.Barrier(dam, target);
            dam = Formula.Mini(dam, source);
            dam = Formula.RandomVariation(dam);
            dam = Formula.LowerSanityCkeck(dam);
            dam = Formula.RunElementalChecks(dam, target, elements);
            dam = Formula.UpperSanityCheck(dam);

            return dam;
        }
        
        public static int RunMagicModifiers(int dam, Combatant target, IEnumerable<Element> elements, AbilityModifiers modifiers)
        {
            dam = Sadness(dam, target);
            dam = Split(dam, modifiers);
            dam = MBarrier(dam, target);
            dam = MPTurbo(dam, modifiers);
            dam = RandomVariation(dam);
            dam = LowerSanityCkeck(dam);
            dam = RunElementalChecks(dam, target, elements);
            dam = UpperSanityCheck(dam);

            return dam;
        }
        
        public static int RunCureModifiers(int dam, Combatant target, IEnumerable<Element> elements, AbilityModifiers modifiers)
        {
            dam = Split(dam, modifiers);
            dam = MBarrier(dam, target);
            dam = MPTurbo(dam, modifiers);
            dam = RandomVariation(dam);
            dam = LowerSanityCkeck(dam);
            dam = RunElementalChecks(dam, target, elements);
            dam = UpperSanityCheck(dam);
            
            return dam;
        }

        public static int PhysicalBase(Combatant er)
        {
            return er.Atk + ((er.Atk + er.Level) / 32) * (er.Atk * er.Level / 32);
        }

        public static int PhysicalDamage(int bd, int power, Combatant ee)
        {
            return (power * (512 - ee.Def) * bd) / (16 * 512);
        }

        public static bool PhysicalHit(int atkp, Combatant source, Combatant target, IEnumerable<Element> elements)
        {
            int hitp;

            if (target.Absorbs(elements) ||
                target.Voids(elements) ||
                target.Death || 
                target.Sleep ||
                target.Confusion || 
                target.Stop ||
                target.Petrify || 
                target.Manipulate ||
                target.Paralysed || 
                target.Peerless)
            {
                hitp = 255;
            }
            else
            {
                hitp = (source.Dexterity / 4) + atkp
                    + source.Defp
                    - target.Defp;

                if (source.Fury)
                {
                    hitp = hitp - hitp * 3 / 10;
                }
            }

            // Sanity
            if (hitp < 1)
            {
                hitp = 1;
            }

            int lucky = Seven.BattleState.Random.Next(0, 100);

            // Lucky Hit
            if (lucky < Math.Floor(source.Luck / 4.0d))
            {
                hitp = 255;
            }
            // Lucky Evade
            else if (lucky < Math.Floor(target.Luck / 4.0d))
            {
                if (source is Ally && target is Enemy)
                {
                    hitp = 0;
                }
            }

            int r = Seven.BattleState.Random.Next(65536) * 99 / 65536 + 1;

            return r < hitp;
        }

        public static int MagicalBase(Combatant er)
        {
            return 6 * (er.Mat + er.Level);
        }

        public static int MagicalDamage(int bd, int power, Combatant ee)
        {
            return (power * (512 - ee.Def) * bd) / (16 * 512);
        }

        public static bool MagicHit(Combatant source, Combatant target, int matp, IEnumerable<Element> elements)
        {
            if (matp == 255 ||
                target.Absorbs(elements) ||
                target.Voids(elements))
            {
                return true;
            }
            if (target.Death ||
                target.Sleep ||
                target.Confusion ||
                target.Stop ||
                target.Petrify ||
                target.Paralysed ||
                target.Peerless ||
                target.Reflect)
            {
                return true;
            }

            if (source.Fury)
            {
                matp = matp - matp * 3 / 10;
            }

            // Magic Defense Percent
            if (Seven.BattleState.Random.Next(1, 101) < target.MDefp)
            {
                return false;
            }

            int hitp = matp + source.Level - target.Level / 2 - 1;

            return Seven.BattleState.Random.Next(100) < hitp;
        }

        public static bool StatusHit(Combatant source, Combatant target, int odds, IEnumerable<Status> statuses, AbilityModifiers modifiers)
        {
            // auto hit conditions

            if (odds >= 100)
            {
                return true;
            }            
            if (statuses.Count() == 1 && statuses.Contains(Status.Frog) && target.Frog)
            {
                return true;
            }
            if (statuses.Count() == 1 && statuses.Contains(Status.Small) && target.Small)
            {
                return true;
            }
            if (target is Ally && statuses.Any(s => new Status[] {
                Status.Haste,
                Status.Berserk,
                Status.Shield
            }.Contains(s)))
            {
                return true;
            }

            odds = MPTurbo(odds, modifiers);

            odds = Split(odds, modifiers);

            odds -= 1;

            return Seven.BattleState.Random.Next(99) < odds;
        }

        public static int Critical(int dam, Combatant source, Combatant target)
        {
            Combatant ee = target;
            Ally er = source as Ally;

            if (er == null)
            {
                return dam;
            }

            int critp;

            if (er.LuckyGirl)
            {
                critp = 255;
            }
            else
            {
                critp = (er.Luck + er.Level - ee.Level) / 4;
                critp = critp + er.Weapon.CriticalPercent;
            }

            int r = (Seven.BattleState.Random.Next(65536) * 99 / 65536) + 1;

            if (r <= critp)
            {
                dam = dam * 2;
            }

            return dam;
        }

        public static int Berserk(int dam, Combatant source)
        {
            if (source.Berserk)
            {
                dam = dam * 15 / 10;
            }

            return dam;
        }

        public static int RowCheck(int dam, Combatant source, Combatant target)
        {
            if (source.LongRange)
            {
                return dam;
            }
            if (source.BackRow || target.BackRow)
            {
                dam = dam / 2;
            }

            return dam;
        }

        public static int Frog(int dam, Combatant source)
        {
            if (source.Frog)
            {
                dam = dam / 4;
            }

            return dam;
        }

        public static int Sadness(int dam, Combatant ee)
        {
            if (ee.Sadness)
            {
                dam = dam * 7 / 10;
            }

            return dam;
        }

        public static int Split(int dam, AbilityModifiers modifiers)
        {
            if (modifiers.QuadraMagic)
            {
                dam = dam / 2;
            }
            else if (modifiers.Alled && !modifiers.NoSplit)
            {
                dam = dam * 2 / 3;
            }

            return dam;
        }

        public static int QuadraMagic(int dam, AbilityModifiers modifiers)
        {
            if (modifiers.QuadraMagic)
            {
                dam = dam / 2;
            }

            return dam;
        }

        public static int Barrier(int dam, Combatant target)
        {
            if (target.Barrier)
            {
                dam = dam / 2;
            }
            
            return dam;
        }

        public static int MBarrier(int dam, Combatant target)
        {
            if (target.MBarrier)
            {
                dam = dam / 2;
            }

            return dam;
        }

        public static int MPTurbo(int dam, AbilityModifiers modifiers)
        {
            dam = dam + (dam * 10 * modifiers.MPTurboFactor) / 10;

            return dam;
        }

        public static int Mini(int dam, Combatant source)
        {
            if (source.Small)
            {
                dam = 0;
            }

            return dam;
        }

        public static int RandomVariation(int dam)
        {
            dam = dam * (3841 + Seven.BattleState.Random.Next(256)) / 4096;

            return dam;
        }

        public static int RunElementalChecks(int dam, Combatant target, IEnumerable<Element> attackElements)
        {
            bool checksDone = false;

            if (attackElements.Contains(Element.Restorative))
            {
                dam = -dam;
            }

            foreach (Element e in attackElements)
            {
                if (target.Voids(e))
                {
                    dam = 0;
                    checksDone = true;
                    break;
                }
            }

            if (!checksDone)
            {
                foreach (Element e in attackElements)
                {
                    if (target.Absorbs(e))
                    {
                        dam = -dam;
                        checksDone = true;
                        break;
                    }
                }
            }

            if (!checksDone)
            {
                foreach (Element e in attackElements)
                {
                    if (target.Halves(e) && target.Weak(e))
                    {
                        continue;
                    }
                    else if (target.Halves(e))
                    {
                        dam = dam / 2;
                        break;
                    }
                    else if (target.Weak(e))
                    {
                        dam = dam * 2;
                        break;
                    }
                }
            }

            return dam;
        }

        public static int LowerSanityCkeck(int dam)
        {
            if (dam == 0)
            {
                dam = 1;
            }

            return dam;
        }

        public static int UpperSanityCheck(int dam)
        {
            if (dam > 9999)
            {
                dam = 9999;
            }

            return dam;
        }


    }
}
