using System;
using System.Text;

using Atmosphere.Reverence.Seven;

namespace Atmosphere.Reverence.Seven.Test
{
    public static class LevelUpDemo
    {
        public static string GetCharacterStats(string character, int level)
        {
            Data data = new Data(typeof(Seven).Assembly);

            Party p = new Party(data);

            Character c = (Character)typeof(Party).GetProperty(character).GetValue(p, null);

            while (c.Level < level)
            {
                c.GainExperience(c.ExpToNextLevel + 10);
            }

            return c.ToString();
        }
    }
}

