using System;
using System.IO;

using NUnit.Framework;

using Atmosphere.EnemyMechanicsParser;

namespace Atmosphere.Reverence.UnitTests
{
    [TestFixture]
    public class EnemyMechanicsParserTest
    {
        [Test]
        public void TestCreateEnemy()
        {
            String data = File.ReadAllText("../../TestData/enemy1.txt");

            Enemy enemy = Enemy.Create(data);
            
            Console.WriteLine(enemy);

            Assert.AreEqual("Land Worm", enemy.name);

            Assert.AreEqual("22", enemy.lvl);
            Assert.AreEqual("1500", enemy.hp);
            Assert.AreEqual("80", enemy.mp);
            Assert.AreEqual("400", enemy.exp);
            Assert.AreEqual("40", enemy.ap);
            Assert.AreEqual("256", enemy.gil);
            
            Assert.AreEqual(1, enemy.weak.Count);
            Assert.AreEqual(2, enemy.half.Count);
            Assert.AreEqual(0, enemy.@void.Count);
            Assert.AreEqual(1, enemy.absorb.Count);
            Assert.AreEqual(2, enemy.immunities.Count);
            
            Assert.Contains("Ice", enemy.weak);
            Assert.Contains("Fire", enemy.half);
            Assert.Contains("Gravity", enemy.half);
            Assert.Contains("Earth", enemy.absorb);
            Assert.Contains("Confusion", enemy.immunities);
            Assert.Contains("Frog", enemy.immunities);

            Assert.AreEqual("60", enemy.atk);
            Assert.AreEqual("80", enemy.def);
            Assert.AreEqual("1", enemy.defp);
            Assert.AreEqual("68", enemy.dex);
            Assert.AreEqual("42", enemy.mat);
            Assert.AreEqual("230", enemy.mdf);
            Assert.AreEqual("0", enemy.lck);

            Assert.Equals("Fire Fang", enemy.morph);
        }
    }
}

