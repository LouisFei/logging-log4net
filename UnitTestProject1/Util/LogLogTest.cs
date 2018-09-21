using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using log4net.Util;
using System.Collections;

namespace UnitTestProject1.Util
{
    [TestClass]
    public class LogLogTest
    {
        [TestMethod]
        public void TestTraceListenerCounter()
        {
            var traceListenerCounter = new TraceListernerCounter();

            Trace.Listeners.Clear();
            Trace.Listeners.Add(traceListenerCounter);

            Trace.Write("Hello");
            Trace.Write("World");

            Assert.AreEqual(2, traceListenerCounter.Count);
        }

        [TestMethod]
        public void TestEmitInternalMessages()
        {
            var traceListenerCounter = new TraceListernerCounter();

            Trace.Listeners.Clear();
            Trace.Listeners.Add(traceListenerCounter);

            LogLog.Error(GetType(), "Hello");
            LogLog.Error(GetType(), "World");
            Trace.Flush();

            Assert.AreEqual(2, traceListenerCounter.Count);

            try
            {
                LogLog.EmitInternalMessages = false;

                LogLog.Error(GetType(), "Hello");
                LogLog.Error(GetType(), "World");
                Assert.AreEqual(2, traceListenerCounter.Count);
            }
            finally
            {
                LogLog.EmitInternalMessages = true;
            }
        }

        [TestMethod]
        public void TestLogReceivedAdapter()
        {
            ArrayList messages = new ArrayList();

            using (new LogLog.LogReceivedAdapter(messages))
            {
                LogLog.Debug(GetType(), "Won't be recorded.");
                LogLog.Error(GetType(), "This will be recorded.");
                LogLog.Error(GetType(), "This will be recorded.");
            }

            Assert.AreEqual(2, messages.Count);
        }

    }

    public class TraceListernerCounter : TraceListener
    {
        private int count = 0;

        public override void Write(string message)
        {
            count++;
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public void Reset()
        {
            count = 0;
        }

        public int Count
        {
            get { return count; }
        }
    }
}
