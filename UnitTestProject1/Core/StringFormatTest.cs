using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject1.Appender;
using log4net.Layout;
using log4net;
using log4net.Config;

namespace UnitTestProject1.Core
{
    [TestClass]
    public class StringFormatTest
    {
        [TestMethod]
        public void TestFormatString()
        {
            var stringAppender = new StringAppender();
            stringAppender.Layout = new PatternLayout("%message");

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            var log1 = LogManager.GetLogger(rep.Name, "TestFormatString");

            log1.Info("TestMessage");
            Assert.AreEqual("TestMessage", stringAppender.GetString(), "Test simple INFO event");
            stringAppender.Reset();

            // ***
            log1.DebugFormat("Before {0} After", "Middle");
            Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted DEBUG event");
            stringAppender.Reset();

            // ***
            log1.InfoFormat("Before {0} After", "Middle");
            Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted INFO event");
            stringAppender.Reset();

            // ***
            log1.WarnFormat("Before {0} After", "Middle");
            Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted WARN event");
            stringAppender.Reset();

            // ***
            log1.ErrorFormat("Before {0} After", "Middle");
            Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted ERROR event");
            stringAppender.Reset();

            // ***
            log1.FatalFormat("Before {0} After", "Middle");
            Assert.AreEqual("Before Middle After", stringAppender.GetString(), "Test simple formatted FATAL event");
            stringAppender.Reset();


            // ***
            log1.InfoFormat("Before {0} After {1}", "Middle", "End");
            Assert.AreEqual("Before Middle After End", stringAppender.GetString(), "Test simple formatted INFO event 2");
            stringAppender.Reset();


        }
    }
}
