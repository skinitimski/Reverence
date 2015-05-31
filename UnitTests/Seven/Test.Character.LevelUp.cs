using System;
using System.Text;
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
            string cloud = LevelUpDemo.GetCharacterStats("Cloud", 53);
            string tifa = LevelUpDemo.GetCharacterStats("Tifa", 46);
            string barret = LevelUpDemo.GetCharacterStats("Barret", 44);

            string aeris = LevelUpDemo.GetCharacterStats("Aeris", 48);
            string nanaki = LevelUpDemo.GetCharacterStats("RedXIII", 46);
            string caitsith = LevelUpDemo.GetCharacterStats("CaitSith", 43);

            string yuffie = LevelUpDemo.GetCharacterStats("Yuffie", 44);            
            string vincent = LevelUpDemo.GetCharacterStats("Vincent", 47);
            string cid = LevelUpDemo.GetCharacterStats("Cid", 47);

            StringBuilder all = new StringBuilder();
            all.AppendLine(cloud);
            all.AppendLine(tifa);
            all.AppendLine(barret);
            all.AppendLine(aeris);
            all.AppendLine(nanaki);
            all.AppendLine(caitsith);
            all.AppendLine(yuffie);
            all.AppendLine(vincent);
            all.AppendLine(cid);

            string a = all.ToString();
        }
    }
}

