using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

using UnitTestProject1.Appender;

using log4net.Layout;
using log4net.Core;
using log4net;
using log4net.Repository;
using log4net.Config;

namespace UnitTestProject1.Layout
{
    [TestClass]
    public class XmlLayoutTest
    {
        [TestMethod]
        public void TestBasicEventLogging()
        {
            TextWriter writer = new StringWriter();
            XmlLayout layout = new XmlLayout();
            var ed= new LoggingEventData();
            ed.Domain = "tests";
            ed.ExceptionString = "";
            ed.Identity = "testRunner";
            ed.Level = Level.Info;
            ed.LocationInfo = new LocationInfo(GetType());
            ed.LoggerName = "testLogger";
            ed.Message = "testMessage";
            ed.ThreadName = "testThreadName";
            ed.TimeStampUtc = DateTime.Now.ToUniversalTime();
            ed.UserName = "testRunner";
            ed.Properties = new log4net.Util.PropertiesDictionary();

            layout.Format(writer, new LoggingEvent(ed));

            /*
                <event logger="testLogger" timestamp="2018-09-07T14:23:41.5453462+08:00" level="INFO" thread="testThreadName" domain="tests" identity="testRunner" username="testRunner">
	                <message>testMessage</message>
                </event>
             */

            Assert.IsTrue(writer.ToString().Contains("testMessage"));

        }

        [TestMethod]
        public void TestCDATAEscaping()
        {
            TextWriter writer = new StringWriter();
            XmlLayout layout = new XmlLayout();
            var ed = new LoggingEventData();
            ed.Domain = "tests";
            ed.ExceptionString = "";
            ed.Identity = "testRunner";
            ed.Level = Level.Info;
            ed.LocationInfo = new LocationInfo(GetType());
            ed.LoggerName = "testLogger";
            ed.Message = "testMessage";
            ed.ThreadName = "testThreadName";
            ed.TimeStampUtc = DateTime.Now.ToUniversalTime();
            ed.UserName = "testRunner";
            ed.Properties = new log4net.Util.PropertiesDictionary();

            //The &'s trigger the use of a cdata block
            ed.Message = "&&&&&&&Escape this ]]>. End here.";

            layout.Format(writer, new LoggingEvent(ed));

            /*
             <event logger="testLogger" timestamp="2018-09-07T15:52:57.7039085+08:00" level="INFO" thread="testThreadName" domain="tests" identity="testRunner" username="testRunner"><message><![CDATA[&&&&&&&Escape this ]]>]]<![CDATA[>. End here.]]></message></event>
             */

            Assert.IsTrue(writer.ToString().Contains("<![CDATA[&&&&&&&Escape this ]]>]]<![CDATA[>. End here.]]>"));
        }

        [TestMethod]
        public void TestBase64EventLogging()
        {
            TextWriter writer = new StringWriter();
            XmlLayout layout = new XmlLayout();
            var ed = new LoggingEventData();
            ed.Domain = "tests";
            ed.ExceptionString = "";
            ed.Identity = "testRunner";
            ed.Level = Level.Info;
            ed.LocationInfo = new LocationInfo(GetType());
            ed.LoggerName = "testLogger";
            ed.Message = "testMessage";
            ed.ThreadName = "testThreadName";
            ed.TimeStampUtc = DateTime.Now.ToUniversalTime();
            ed.UserName = "testRunner";
            ed.Properties = new log4net.Util.PropertiesDictionary();

            ed.Message = "Base64EncodeMessage";
            layout.Base64EncodeMessage = true;

            layout.Format(writer, new LoggingEvent(ed));

            /*
             <event logger="testLogger" timestamp="2018-09-07T17:34:09.3690143+08:00" level="INFO" thread="testThreadName" domain="tests" identity="testRunner" username="testRunner"><message>QmFzZTY0RW5jb2RlTWVzc2FnZQ==</message></event>
             */

            Assert.IsTrue(writer.ToString().Contains("QmFzZTY0RW5jb2RlTWVzc2FnZQ=="));
        }

        [TestMethod]
        public void TestPropertyEventLogging()
        {
            StringAppender stringAppender = new StringAppender();
            stringAppender.Layout = new XmlLayout();

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            ILog log1 = LogManager.GetLogger(rep.Name, "TestThreadProperiesPattern");

            var ed = new LoggingEventData()
            {
                Domain = "tests",
                ExceptionString = "",
                Identity = "testRunner",
                Level = Level.Info,
                LocationInfo = new LocationInfo(GetType()),
                LoggerName = "testLogger",
                Message = "testMessage",
                ThreadName = "testThreadName",
                TimeStampUtc = DateTime.Now.ToUniversalTime(),
                UserName = "testRunner",
                Properties = new log4net.Util.PropertiesDictionary()
            };
            ed.Properties["Property1"] = "prop1";
            log1.Logger.Log(new LoggingEvent(ed));

            Assert.IsTrue(stringAppender.GetString().Contains("Property1") && stringAppender.GetString().Contains("prop1"));
        }

        [TestMethod]
        public void TestPropertyEventLogging2()
        {
            StringAppender stringAppender = new StringAppender();
            stringAppender.Layout = new PatternLayout("%property{Property1}");

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, stringAppender);

            ILog log1 = LogManager.GetLogger(rep.Name, "TestPropertyEventLogging2");

            var ed = new LoggingEventData()
            {
                Domain = "tests",
                ExceptionString = "",
                Identity = "testRunner",
                Level = Level.Info,
                LocationInfo = new LocationInfo(GetType()),
                LoggerName = "testLogger",
                Message = "testMessage",
                ThreadName = "testThreadName",
                TimeStampUtc = DateTime.Now.ToUniversalTime(),
                UserName = "testRunner",
                Properties = new log4net.Util.PropertiesDictionary()
            };
            ed.Properties["Property1"] = "prop1";

            log1.Logger.Log(new LoggingEvent(ed));

            Assert.AreEqual("prop1", stringAppender.GetString());
        }

    }
}
