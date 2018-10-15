using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using System.IO;
using log4net.Repository;
using log4net;
using log4net.Config;
using log4net.Layout.Pattern;
using log4net.Core;
using UnitTestProject1.Appender;
using log4net.Util;

namespace UnitTestProject1.Util
{
    [TestClass]
    public class PatternConverterTest
    {
        [TestMethod]
        public void PatternLayoutConverterProperties()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.LoadXml(@"
                <log4net>
                  <appender name=""StringAppender"" type=""UnitTestProject1.Appender.StringAppender, UnitTestProject1"">
                    <layout type=""log4net.Layout.PatternLayout"">
                        <converter>
                            <name value=""propertyKeyCount"" />
                            <type value=""UnitTestProject1.Util.PropertyKeyCountPatternLayoutConverter, UnitTestProject1"" />
                            <property>
                                <key value=""one-plus-one"" />
                                <value value=""2"" />
                            </property>
                            <property>
                               <key value=""two-plus-two"" />
                               <value value=""4"" />
                            </property> 
                        </converter>
                        <conversionPattern value=""%propertyKeyCount"" />
                    </layout>
                  </appender>
                  <root>
                    <level value=""ALL"" />                  
                    <appender-ref ref=""StringAppender"" />
                  </root>  
                </log4net>");

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            XmlConfigurator.Configure(rep, log4netConfig["log4net"]);

            ILog log = LogManager.GetLogger(rep.Name, "PatternLayoutConverterProperties");
            log.Debug("Message");

            PropertyKeyCountPatternLayoutConverter converter = PropertyKeyCountPatternLayoutConverter.MostRecentInstance;
            Assert.AreEqual(2, converter.Properties.Count);
            Assert.AreEqual("4", converter.Properties["two-plus-two"]);

            StringAppender appender = (StringAppender)LogManager.GetRepository(rep.Name).GetAppenders()[0];
            Assert.AreEqual("2", appender.GetString());
        }

        [TestMethod]
        public void PatternConverterProperties()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.LoadXml(@"
                <log4net>
                  <appender name=""PatternStringAppender"" type=""UnitTestProject1.Util.PatternStringAppender, UnitTestProject1"">
                    <layout type=""log4net.Layout.SimpleLayout"" />
                    <setting>
                        <converter>
                            <name value=""propertyKeyCount"" />
                            <type value=""UnitTestProject1.Util.PropertyKeyCountPatternConverter, UnitTestProject1"" />
                            <property>
                                <key value=""one-plus-one"" />
                                <value value=""2"" />
                            </property>
                            <property>
                               <key value=""two-plus-two"" />
                               <value value=""4"" />
                            </property> 
                        </converter>
                        <conversionPattern value=""%propertyKeyCount"" />
                    </setting>
                  </appender>
                  <root>
                    <level value=""ALL"" />                  
                    <appender-ref ref=""PatternStringAppender"" />
                  </root>  
                </log4net>");

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            XmlConfigurator.Configure(rep, log4netConfig["log4net"]);

            ILog log = LogManager.GetLogger(rep.Name, "PatternConverterProperties");
            log.Debug("Message");

            PropertyKeyCountPatternConverter converter = PropertyKeyCountPatternConverter.MostRecentInstance;
            Assert.AreEqual(2, converter.Properties.Count);
            Assert.AreEqual("4", converter.Properties["two-plus-two"]);

            PatternStringAppender appender = (PatternStringAppender)LogManager.GetRepository(rep.Name).GetAppenders()[0];
            Assert.AreEqual("2", appender.Setting.Format());
        }

    }

    public class PropertyKeyCountPatternLayoutConverter : PatternLayoutConverter
    {
        private static PropertyKeyCountPatternLayoutConverter mostRecentInstance;

        public PropertyKeyCountPatternLayoutConverter()
        {
            mostRecentInstance = this;
        }

        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            writer.Write(Properties.GetKeys().Length);
        }

        public static PropertyKeyCountPatternLayoutConverter MostRecentInstance
        {
            get { return mostRecentInstance; }
        }
    }

    public class PropertyKeyCountPatternConverter : PatternConverter
    {
        private static PropertyKeyCountPatternConverter mostRecentInstance;

        public PropertyKeyCountPatternConverter()
        {
            mostRecentInstance = this;
        }

        protected override void Convert(TextWriter writer, object state)
        {
            writer.Write(Properties.GetKeys().Length);
        }

        public static PropertyKeyCountPatternConverter MostRecentInstance
        {
            get { return mostRecentInstance; }
        }
    }

    public class PatternStringAppender : StringAppender
    {
        private static PatternStringAppender mostRecentInstace;

        private PatternString setting;

        public PatternStringAppender()
        {
            mostRecentInstace = this;
        }

        public PatternString Setting
        {
            get { return setting; }
            set { setting = value; }
        }

        public static PatternStringAppender MostRecentInstace
        {
            get { return mostRecentInstace; }
        }
    }

}
