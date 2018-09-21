using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using log4net;
using log4net.Appender;
using log4net.Layout;
using log4net.Config;

namespace UnitTestProject1.Appender
{
    [TestClass]
    public class TraceAppenderTest
    {
        [TestMethod]
        public void DefaultCategoryTest()
        {
            var categoryTraceListener = new CategoryTraceListener();
            Trace.Listeners.Clear();
            Trace.Listeners.Add(categoryTraceListener);

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

            var traceAppender = new TraceAppender();
            traceAppender.Layout = new SimpleLayout();
            traceAppender.ActivateOptions();

            BasicConfigurator.Configure(rep, traceAppender);

            ILog log = LogManager.GetLogger(rep.Name, GetType());
            log.Debug("Message");

            Assert.AreEqual(
                GetType().ToString(),
                categoryTraceListener.Category);
        }

        [TestMethod]
        public void MethodNameCategoryTest()
        {
            var categoryTraceListener = new CategoryTraceListener();
            Trace.Listeners.Clear();
            Trace.Listeners.Add(categoryTraceListener);

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            var traceAppender = new TraceAppender();
            var methodLayout = new PatternLayout("%method");
            methodLayout.ActivateOptions();

            traceAppender.Category = methodLayout;
            traceAppender.Layout = new SimpleLayout();
            traceAppender.ActivateOptions();

            BasicConfigurator.Configure(rep, traceAppender);

            ILog log = LogManager.GetLogger(rep.Name, GetType());
            log.Debug("Message");

            Assert.AreEqual(
                System.Reflection.MethodInfo.GetCurrentMethod().Name,
                categoryTraceListener.Category);
        }
    }

    public class CategoryTraceListener : TraceListener
    {
        private string lastCategory;

        public override void Write(string message)
        {
            // empty
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        public override void Write(string message, string category)
        {
            lastCategory = category;
            base.Write(message, category);
        }

        public string Category
        {
            get { return lastCategory; }
        }
    }

}
