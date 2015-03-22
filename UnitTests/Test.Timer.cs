using System;
using SystemThread = System.Threading.Thread;

using NUnit.Framework;

using Atmosphere.Reverence.Time;

namespace Atmosphere.UnitTests
{
    [TestFixture]
    public partial class Test
    {
        [Test]
        public void ClockTest()
        {
//            TestClock(new Clock());
//            TestClock(new Clock(Clock.TICKS_PER_MS / 2));
//            TestClock(new Clock(Clock.TICKS_PER_MS * 2));
//            TestClock(new Clock(Clock.TICKS_PER_MS * 2, 1000, true));

            //                                              100 (100 ms)
            //                                             3000 (3 s)
            //                                           120000 (2 m)
            //                                        + 3600000 (1 h)

            Clock clock = new Clock(Clock.TICKS_PER_MS, 3723100, false);
            
            Console.WriteLine(clock.ToString());

            Assert.AreEqual(1, clock.Hours);
            Assert.AreEqual(2, clock.Minutes);
            Assert.AreEqual(3, clock.Seconds);
            Assert.AreEqual(100, clock.Milliseconds);
        }


        private void TestClock(Clock clock)
        {
            // Assumes the clock is started. Clocks are started at instantiation by default.

            long elapsed1, elapsed2, elapsed3;

            int msWait = (int)(20L * Clock.TICKS_PER_MS / clock.TicksPer);
            
            elapsed1 = clock.TotalMilliseconds;
            SystemThread.Sleep(msWait);
            elapsed2 = clock.TotalMilliseconds;
            Assert.IsTrue(elapsed1 < elapsed2);


            // Now let's pause and make sure time stops
            Assert.IsTrue(clock.Pause());
            Assert.IsFalse(clock.Pause());            
            elapsed1 = clock.TotalMilliseconds;
            SystemThread.Sleep(msWait);
            elapsed2 = clock.TotalMilliseconds;            
            Assert.AreEqual(elapsed1, elapsed2);


            // Now let's unpause and make sure time flows again
            elapsed3 = clock.TotalMilliseconds;
            Assert.IsTrue(clock.Unpause());
            Assert.IsFalse(clock.Unpause());            
            SystemThread.Sleep(msWait);
            elapsed1 = clock.TotalMilliseconds;
            SystemThread.Sleep(msWait);
            elapsed2 = clock.TotalMilliseconds;
            Assert.IsTrue(elapsed1 < elapsed2);
            Assert.IsTrue(elapsed1 > elapsed3);
            Assert.IsTrue(elapsed2 > elapsed3);
            
            

            // Now let's reset, restart, and make sure time has reset
            elapsed3 = clock.TotalMilliseconds;
            Assert.IsTrue(clock.Reset(true));
            Assert.IsTrue(clock.TotalMilliseconds < elapsed3);
            SystemThread.Sleep(msWait);
            Assert.AreNotEqual(0, clock.TotalMilliseconds);
            elapsed1 = clock.TotalMilliseconds;
            SystemThread.Sleep(msWait);
            elapsed2 = clock.TotalMilliseconds;            
            Assert.IsTrue(elapsed1 < elapsed2);


            // Now let's reset, without restarting, and make sure time stops and we're at 0
            Assert.IsFalse(clock.Reset(false));
            SystemThread.Sleep(msWait);
            Assert.AreEqual(0, clock.TotalMilliseconds);
            SystemThread.Sleep(msWait);
            Assert.AreEqual(0, clock.TotalMilliseconds);
        }
        
        [Test]
        public void TimerTest()
        {
            //TestClock(new Timer(500));
            //TestTimer(new Timer(500, false));
            
            //TestClock(new Timer(500));
            //TestTimer(new Timer(500, false));
        }

        /// <summary>
        /// Tests the given timer. Must have not been started.
        /// </summary>
        private void TestTimer(Timer timer)
        {
            long elapsed1, elapsed2;
            
            int halfWait = (int)(timer.Timeout / 2);
            
            Assert.AreEqual(timer.TotalMilliseconds, 0);
            Assert.IsFalse(timer.IsUp);


            timer.Unpause();      

            SystemThread.Sleep(halfWait);
            Assert.IsFalse(timer.IsUp);
            SystemThread.Sleep(halfWait);
            Assert.IsTrue(timer.IsUp);
            Assert.IsTrue(timer.TotalMilliseconds >= timer.Timeout);
            
            
            timer.Reset();
            Assert.IsFalse(timer.IsUp);            
            SystemThread.Sleep(halfWait);
            Assert.IsFalse(timer.IsUp);            
            SystemThread.Sleep(halfWait);
            Assert.IsTrue(timer.IsUp);
            
            
            timer.Reset(false);
            Assert.AreEqual(timer.TotalMilliseconds, 0);
        }
    }
}

