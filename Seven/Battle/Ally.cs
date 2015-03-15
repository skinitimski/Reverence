using System;

namespace Atmosphere.Reverence.Seven.Battle
{
    internal class Ally
    {
        public static int Attack(Character c)
        {
            return c.Strength + c.Weapon.Attack;
        }
        public static int AttackPercent(Character c)
        {
            return c.Weapon.AttackPercent;
        }
        public static int Defense(Character c)
        {
            return c.Vitality + c.Armor.Defense;
        }
        public static int DefensePercent(Character c)
        {
            return (c.Dexterity / 4) + c.Armor.DefensePercent;
        }
        public static int MagicAttack(Character c)
        {
            return c.Magic;
        }
        public static int MagicDefense(Character c)
        {
            return c.Spirit + c.Armor.MagicDefense;
        }
        public static int MagicDefensePercent(Character c)
        {
            return c.Armor.MagicDefensePercent;
        }
    }
}

