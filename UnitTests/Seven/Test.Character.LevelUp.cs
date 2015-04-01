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
            LevelUpDemo.GetCharacterStats("Cloud", 99);
        }
    }
}

