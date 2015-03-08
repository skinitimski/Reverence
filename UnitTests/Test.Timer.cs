using System;
using SystemThread = System.Threading.Thread;

using NUnit.Framework;

using Atmosphere.BattleSimulator;

namespace UnitTests
{
    [TestFixture]
    public partial class Test
    {
        [Test]
        public void ClockTest()
        {
            Clock clock = new Clock();
            long elapsed1, elapsed2;

            elapsed1 = clock.Elapsed;
            SystemThread.Sleep(2);
            elapsed2 = clock.Elapsed;
            Assert.IsTrue(elapsed1 < elapsed2);

            Assert.IsTrue(clock.Pause());
            Assert.IsFalse(clock.Pause());
            
            elapsed1 = clock.Elapsed;
            SystemThread.Sleep(2);
            elapsed2 = clock.Elapsed;            
            Assert.AreEqual(elapsed1, elapsed2);

            Assert.IsTrue(clock.Unpause());
            Assert.IsFalse(clock.Unpause());
            
            elapsed1 = clock.Elapsed;
            SystemThread.Sleep(2);
            elapsed2 = clock.Elapsed;
            Assert.IsTrue(elapsed1 < elapsed2);
        }
        
        [Test]
        public void TimerTest()
        {
            Timer timer = new Timer(500, false);
            
            TestTimer(timer);
        }
        
        [Test]
        public void ScaledTimerTest()
        {
            Timer timer;
            
            timer = new ScaledTimer(500, Clock.TICKS_PER_MS, false);            
            TestTimer(timer);
            
            //timer = new ScaledTimer(1000, Clock.TICKS_PER_MS / 4, false);            
            //TestTimer(timer);
        }

        /// <summary>
        /// Tests the given timer. Must have not been started.
        /// </summary>
        private void TestTimer(Timer timer)
        {
            long elapsed1, elapsed2;
            
            Assert.AreEqual(timer.Elapsed, 0);
            Assert.IsFalse(timer.IsUp);
            
            timer.Unpause();

            int halfWait = timer.Timeout / 2;
            
            
            SystemThread.Sleep(halfWait);
            Assert.IsFalse(timer.IsUp);
            SystemThread.Sleep(halfWait);
            Assert.IsTrue(timer.IsUp);
            Assert.IsTrue(timer.Elapsed >= timer.Timeout);
            
            
            timer.Reset();
            Assert.IsFalse(timer.IsUp);            
            SystemThread.Sleep(halfWait);
            Assert.IsFalse(timer.IsUp);            
            SystemThread.Sleep(halfWait);
            Assert.IsTrue(timer.IsUp);
            
            
            timer.Reset(false);
            Assert.AreEqual(timer.Elapsed, 0);
        }
    }
}

