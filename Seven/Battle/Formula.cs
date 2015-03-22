using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Atmosphere.Reverence.Seven.Asset;
using Atmosphere.Reverence.Seven.Battle;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal static class Formula
    {
        public static void PhysicalAttack(int power, Combatant source, Combatant target)
        {
            bool restorative = false;

            Element[] elements = new Element[0];

            if (source is Ally)
            {
                Ally ally = (Ally)source;

                elements = new Element[] { ally.Weapon.Element };
            }


            if (!PhysicalHit(source, target, elements))
            {
                return;
            }

            int bd = PhysicalBase(source);
            int dam = PhysicalDamage(bd, power, target);

            dam = Critical(dam, source, target);
            dam = Berserk(dam, source);
            dam = RowCheck(dam, source, target);
            dam = Frog(dam, target);
            dam = Sadness(dam, target);
           // dam = Split(dam, state);
            dam = Barrier(dam, target);
            dam = Mini(dam, source);
            dam = RandomVariation(dam);
            dam = LowerSanityCkeck(dam);
            dam = RunElementalChecks(dam, ref restorative, target, elements);
            dam = UpperSanityCheck(dam);

            target.AcceptDamage(source, AttackType.Physical,  dam);
        }

        public static void MagicSpell(int power, Combatant source, Combatant target, Spell spell, SpellModifiers modifiers)
        {
            if (!MagicHit(source, target, spell))
            {
                return;
            }

            bool restorative = false;
            int bd = MagicalBase(source);
            int dam = MagicalDamage(bd, 4, target);

            RunMagicModifiers(dam, ref restorative, target, spell, modifiers);

            if (restorative)
            {
                dam = -dam;
            }

            target.AcceptDamage(source, AttackType.Magical, dam);
        }

        public static void MagicChangeStatus()
        {
        }

        public static int RunMagicModifiers(int dam, ref bool restorative, Combatant target, Spell spell, SpellModifiers modifiers)
        {
            dam = Sadness(dam, target);
            dam = Split(dam, modifiers);
            dam = MBarrier(dam, target);
            dam = MPTurbo(dam, modifiers.MPTurboFactor);
            dam = RandomVariation(dam);
            dam = LowerSanityCkeck(dam);
            dam = RunElementalChecks(dam, ref restorative, target, spell.Element);
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

        public static bool PhysicalHit(Combatant source, Combatant target, Element[] elements)
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
                hitp = (source.Dexterity / 4) //+ state.HitP) 
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

        public static bool MagicHit(Combatant source, Combatant target, Spell spell)
        {
            if (spell.Matp == 255 ||
                target.Absorbs(spell.Element) ||
                target.Voids(spell.Element))
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

            int matp = spell.Matp;

            if (source.Fury)
            {
                matp = matp - matp * 3 / 10;
            }

            if (Seven.BattleState.Random.Next(1, 101) > target.MDefp)
            {
                return false;
            }

            int hitp = matp + source.Level - target.Level / 2 - 1;

            return Seven.BattleState.Random.Next(100) < hitp;
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
            if (source.Berserk) dam = dam * 15 / 10;

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
            if (source.Frog) dam = dam / 4;

            return dam;
        }



        public static int Sadness(int dam, Combatant ee)
        {
            if (ee.Sadness) dam = dam * 7 / 10;

            return dam;
        }

        public static int Split(int dam, SpellModifiers modifiers)
        {
            if (modifiers.QuadraMagic)
            {
                dam = dam / 2;
            }
            else if (modifiers.Alled)
            {
                dam = dam * 2 / 3;
            }
            return dam;
        }

        public static int Barrier(int dam, Combatant target)
        {
            if (target.Barrier) dam = dam / 2;
            
            return dam;
        }

        public static int MBarrier(int dam, Combatant target)
        {
            if (target.MBarrier) dam = dam / 2;

            return dam;
        }

        public static int MPTurbo(int dam, int mpTurboFactor)
        {
            dam = dam + (dam * 10 * mpTurboFactor) / 10;

            return dam;
        }
        public static int Mini(int dam, Combatant source)
        {
            if (source.Small) dam = 0;

            return dam;
        }

        public static int RandomVariation(int dam)
        {
            dam = dam * (3841 + Seven.BattleState.Random.Next(256)) / 4096;

            return dam;
        }


        public static int RunElementalChecks(int dam, ref bool restorative, Combatant target, IEnumerable<Element> attackElements)
        {
            bool checksDone = false;

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
                        restorative = !restorative;
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
            if (dam == 0) dam = 1;

            return dam;
        }

        public static int UpperSanityCheck(int dam)
        {
            if (dam > 9999) dam = 9999;

            return dam;
        }


    }
}
