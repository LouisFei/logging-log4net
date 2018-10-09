using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net.Util;
using log4net.Core;

namespace UnitTestProject1.Util
{
    [TestClass]
    public class CyclicBufferTest
    {
        [TestMethod]
        public void TestConstructorSize0()
        {
            CyclicBuffer cb = null;
            try
            {
                cb = new CyclicBuffer(0);
            }
            catch
            {                
            }
            
            Assert.IsNull(cb);
        }

        [TestMethod]
        public void TestSize1()
        {
            CyclicBuffer cb = new CyclicBuffer(1);

            Assert.AreEqual(0, cb.Length, "空缓冲区的长度应该为0"); //Empty Buffer should have length 0
            Assert.AreEqual(1, cb.MaxSize, "缓冲区的最大大小应该是1"); //Buffer should have max size 1

            LoggingEvent event1 = new LoggingEvent(null, null, null, null, null, null);
            LoggingEvent event2 = new LoggingEvent(null, null, null, null, null, null);

            LoggingEvent discardedEvent = cb.Append(event1);

            Assert.IsNull(discardedEvent, "在缓冲区满之前，不应该丢弃任何事件"); //No event should be discarded untill the buffer is full
            Assert.AreEqual(1, cb.Length, "缓冲区的长度应该为1"); //Buffer should have length 1
            Assert.AreEqual(1, cb.MaxSize, "缓冲区的大小应该仍然是1"); //Buffer should still have max size 1


            discardedEvent = cb.Append(event2);

            Assert.AreSame(event1, discardedEvent, "现在可以预期event1将被丢弃");//Expect event1 to now be discarded
            Assert.AreEqual(1, cb.Length, "缓冲区的长度应该仍然为1");//Buffer should still have length 1
            Assert.AreEqual(1, cb.MaxSize, "缓冲区的大小应该仍然是1");//Buffer should really still have max size 1

            LoggingEvent[] discardedEvents = cb.PopAll();

            Assert.AreEqual(1, discardedEvents.Length, "取出事件的长度应该是1"); //Poped events length should be 1
            Assert.AreSame(event2, discardedEvents[0], "预计event2现在会被取出"); //Expect event2 to now be popped
            Assert.AreEqual(0, cb.Length, "缓冲区的长度应该回到0"); //Buffer should be back to length 0
            Assert.AreEqual(1, cb.MaxSize, "缓冲区的大小应该仍然是1");//Buffer should really really still have max size 1
        }


        [TestMethod]
        public void TestSize2()
        {
            CyclicBuffer cb = new CyclicBuffer(2);

            Assert.AreEqual(0, cb.Length, "Empty Buffer should have length 0");
            Assert.AreEqual(2, cb.MaxSize, "Buffer should have max size 2");

            LoggingEvent event1 = new LoggingEvent(null, null, null, null, null, null);
            LoggingEvent event2 = new LoggingEvent(null, null, null, null, null, null);
            LoggingEvent event3 = new LoggingEvent(null, null, null, null, null, null);

            LoggingEvent discardedEvent;

            discardedEvent = cb.Append(event1);
            Assert.IsNull(discardedEvent, "No event should be discarded after append 1");
            discardedEvent = cb.Append(event2);
            Assert.IsNull(discardedEvent, "No event should be discarded after append 2");

            discardedEvent = cb.Append(event3);
            Assert.AreSame(event1, discardedEvent, "Expect event1 to now be discarded");

            discardedEvent = cb.PopOldest();
            Assert.AreSame(event2, discardedEvent, "Expect event2 to now be discarded");

            LoggingEvent[] discardedEvents = cb.PopAll();

            Assert.AreEqual(1, discardedEvents.Length, "Poped events length should be 1");
            Assert.AreSame(event3, discardedEvents[0], "Expect event3 to now be popped");
            Assert.AreEqual(0, cb.Length, "Buffer should be back to length 0");
            Assert.AreEqual(2, cb.MaxSize, "Buffer should really really still have max size 2");
        }
    }
}
