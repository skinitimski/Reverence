using System;
using SystemThread = System.Threading.Thread;

using NUnit.Framework;

using Atmosphere.Reverence.Seven.Test;

namespace Atmosphere.UnitTests.Seven
{
    [TestFixture]
    public partial class CharacterTest
    {
        [Test]
        public void LevelUpTest()
        {
            string cloud = LevelUpDemo.GetCharacterStats("Cloud", 45);
            string tifa = LevelUpDemo.GetCharacterStats("Tifa", 45);
            string barret = LevelUpDemo.GetCharacterStats("Barret", 45);

            string aeris = LevelUpDemo.GetCharacterStats("Aeris", 45);
            string nanaki = LevelUpDemo.GetCharacterStats("RedXIII", 45);
            string caitsith = LevelUpDemo.GetCharacterStats("CaitSith", 45);

            string yuffie = LevelUpDemo.GetCharacterStats("Yuffie", 45);            
            string vincent = LevelUpDemo.GetCharacterStats("Vincent", 45);
            string cid = LevelUpDemo.GetCharacterStats("Cid", 45);
            
        }
    }
}

