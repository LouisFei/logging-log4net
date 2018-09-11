using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UnitTestProject1.Appender;
using log4net.Layout;
using log4net;
using log4net.Config;
using log4net.Util;
using log4net.Layout.Pattern;
using System.IO;
using log4net.Core;

namespace UnitTestProject1.Layout
{
    [TestClass]
    public class PatternLayoutTest
    {
        [TestMethod]
        public void TestThreadPropertiesPattern()
        {
            var stringAppender = new StringAppender();
            stringAppender.Layout = new PatternLayout("%property{prop1}");

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            var log1 = LogManager.GetLogger(rep.Name, "TestThreadPropertiesPattern");
            log1.Info("TestMessage");
            Assert.AreEqual(SystemInfo.NullText, stringAppender.GetString(), "Test no thread properties value set");

            stringAppender.Reset();
            ThreadContext.Properties["prop1"] = "val1";
            log1.Info("TestMessage");
            Assert.AreEqual("val1", stringAppender.GetString(), "Test thread properties value set");

            stringAppender.Reset();
            ThreadContext.Properties.Remove("prop1");
            log1.Info("TestMessage");
            Assert.AreEqual(SystemInfo.NullText, stringAppender.GetString(), "Test thread properties value removed");
        }

        [TestMethod]
        public void TestStackTracePattern()
        {
            var stringAppender = new StringAppender();
            stringAppender.Layout = new PatternLayout("%stacktrace{2}");

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            var log1 = LogManager.GetLogger(rep.Name, "TestStackTracePattern");

            log1.Info("TestMessage");
            StringAssert.EndsWith(stringAppender.GetString(), "PatternLayoutTest.TestStackTracePattern", "stack trace value set");
            //stringAppender.Reset();
        }

        [TestMethod]
        public void TestGlobalPropertiesPattern()
        {
            var stringAppender = new StringAppender();
            stringAppender.Layout = new PatternLayout("%property{prop1}");

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            var log1 = LogManager.GetLogger(rep.Name, "TestGlobalPropertiesPattern");
            log1.Info("TestMessage");
            Assert.AreEqual(SystemInfo.NullText, stringAppender.GetString(), "Test no global properties value set");

            stringAppender.Reset();
            GlobalContext.Properties["prop1"] = "val1";
            log1.Info("TestMessage");
            Assert.AreEqual("val1", stringAppender.GetString(), "Test global properties value set");

            stringAppender.Reset();
            GlobalContext.Properties.Remove("prop1");
            log1.Info("TestMessage");
            Assert.AreEqual(SystemInfo.NullText, stringAppender.GetString(), "Test global properties value removed");
        }

        [TestMethod]
        public void TestAddingCustomPattern()
        {
            var stringAppender = new StringAppender();
            var layout = new PatternLayout();

            layout.AddConverter("TestAddingCustomPattern", typeof(TestMessagePatternConverter));
            layout.ConversionPattern = "%TestAddingCustomPattern";
            layout.ActivateOptions();

            stringAppender.Layout = layout;

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            var log1 = LogManager.GetLogger(rep.Name, "TestAddingCustomPattern");
            log1.Info("TestMessage");
            Assert.AreEqual("TestMessage", stringAppender.GetString(), "%TestAddingCustomPattern not registered");

        }

        [TestMethod]
        public void TestNamedPatternConverterWidthoutPrecisionShouldReturnFullName()
        {
            var stringAppender = new StringAppender();
            var layout = new PatternLayout();

            layout.AddConverter("message-as-name", typeof(MessageAsNamePatternConverter));
            layout.ConversionPattern = "%message-as-name";
            layout.ActivateOptions();

            stringAppender.Layout = layout;

            var rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            var log1 = LogManager.GetLogger(rep.Name, "TestAddingCustomPattern");

            log1.Info("NoDots");
            Assert.AreEqual("NoDots", stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();

            log1.Info("One.Dot");
            Assert.AreEqual("One.Dot", stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();

            log1.Info("Tw.o.Dots");
            Assert.AreEqual("Tw.o.Dots", stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();

            log1.Info("TrailingDot.");
            Assert.AreEqual("TrailingDot.", stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();

            log1.Info(".LeadingDot");
            Assert.AreEqual(".LeadingDot", stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();

            // empty string and other evil combinations as tests for of-by-one mistakes in index calculations
            log1.Info(string.Empty);
            Assert.AreEqual(string.Empty, stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();

            log1.Info(".");
            Assert.AreEqual(".", stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();

            log1.Info("x");
            Assert.AreEqual("x", stringAppender.GetString(), "%message-as-name not registered");
            stringAppender.Reset();
        }



        /// <summary>
		/// Converter to include event message
		/// </summary>
		private class TestMessagePatternConverter : PatternLayoutConverter
        {
            /// <summary>
            /// Convert the pattern to the rendered message
            /// </summary>
            /// <param name="writer"><see cref="TextWriter" /> that will receive the formatted result.</param>
            /// <param name="loggingEvent">the event being logged</param>
            /// <returns>the relevant location information</returns>
            protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
            {
                loggingEvent.WriteRenderedMessage(writer);
            }
        }

        private class MessageAsNamePatternConverter : NamedPatternConverter
        {
            protected override string GetFullyQualifiedName(LoggingEvent loggingEvent)
            {
                return loggingEvent.MessageObject.ToString();
            }
        }
    }
}
