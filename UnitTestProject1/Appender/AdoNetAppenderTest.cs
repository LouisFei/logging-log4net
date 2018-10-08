using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net.Repository;
using log4net;
using log4net.Appender;
using log4net.Config;
using UnitTestProject1.Appender.AdoNet;
using System.Xml;
using System.Data;
using log4net.Core;

namespace UnitTestProject1.Appender
{
    [TestClass]
    public class AdoNetAppenderTest
    {
        [TestMethod]
        public void NoBufferingTest()
        {
            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

            AdoNetAppender adoNetAppender = new AdoNetAppender();
            adoNetAppender.BufferSize = -1;
            adoNetAppender.ConnectionType = "log4net.Tests.Appender.AdoNet.Log4NetConnection";
            adoNetAppender.ActivateOptions();

            BasicConfigurator.Configure(rep, adoNetAppender);

            ILog log = LogManager.GetLogger(rep.Name, "NoBufferingTest");
            log.Debug("Message");
            Assert.AreEqual(1, Log4NetCommand.MostRecentInstance.ExecuteNonQueryCount);
        }

        [TestMethod]
        public void BufferingTest()
        {
            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

            int bufferSize = 5;

            AdoNetAppender adoNetAppender = new AdoNetAppender();
            adoNetAppender.BufferSize = bufferSize;
            adoNetAppender.ConnectionType = "log4net.Tests.Appender.AdoNet.Log4NetConnection";
            adoNetAppender.ActivateOptions();

            BasicConfigurator.Configure(rep, adoNetAppender);

            ILog log = LogManager.GetLogger(rep.Name, "BufferingTest");
            for (int i = 0; i < bufferSize; i++)
            {
                log.Debug("Message");
                Assert.IsNull(Log4NetCommand.MostRecentInstance);
            }
            log.Debug("Message");
            Assert.AreEqual(bufferSize + 1, Log4NetCommand.MostRecentInstance.ExecuteNonQueryCount);
        }

        [TestMethod]
        public void WebsiteExample()
        {
            XmlDocument log4netConfig = new XmlDocument();
            #region Load log4netConfig
            log4netConfig.LoadXml(@"
                <log4net>
                <appender name=""AdoNetAppender"" type=""log4net.Appender.AdoNetAppender"">
                    <bufferSize value=""-1"" />
                    <connectionType value=""log4net.Tests.Appender.AdoNet.Log4NetConnection"" />
                    <connectionString value=""data source=.;initial catalog=SipscMemberLog;integrated security=false;persist security info=True;User ID=sml;Password=sml123"" />
                    <commandText value=""INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception)"" />
                    <parameter>
                        <parameterName value=""@log_date"" />
                        <dbType value=""DateTime"" />
                        <layout type=""log4net.Layout.RawTimeStampLayout"" />
                    </parameter>
                    <parameter>
                        <parameterName value=""@thread"" />
                        <dbType value=""String"" />
                        <size value=""255"" />
                        <layout type=""log4net.Layout.PatternLayout"">
                            <conversionPattern value=""%thread"" />
                        </layout>
                    </parameter>
                    <parameter>
                        <parameterName value=""@log_level"" />
                        <dbType value=""String"" />
                        <size value=""50"" />
                        <layout type=""log4net.Layout.PatternLayout"">
                            <conversionPattern value=""%level"" />
                        </layout>
                    </parameter>
                    <parameter>
                        <parameterName value=""@logger"" />
                        <dbType value=""String"" />
                        <size value=""255"" />
                        <layout type=""log4net.Layout.PatternLayout"">
                            <conversionPattern value=""%logger"" />
                        </layout>
                    </parameter>
                    <parameter>
                        <parameterName value=""@message"" />
                        <dbType value=""String"" />
                        <size value=""4000"" />
                        <layout type=""log4net.Layout.PatternLayout"">
                            <conversionPattern value=""%message"" />
                        </layout>
                    </parameter>
                    <parameter>
                        <parameterName value=""@exception"" />
                        <dbType value=""String"" />
                        <size value=""2000"" />
                        <layout type=""log4net.Layout.ExceptionLayout"" />
                    </parameter>
                </appender>
                <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""AdoNetAppender"" />
                  </root>  
                </log4net>");
            #endregion

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            XmlConfigurator.Configure(rep, log4netConfig["log4net"]);
            ILog log = LogManager.GetLogger(rep.Name, "WebsiteExample");
            log.Debug("Message");

            IDbCommand command = Log4NetCommand.MostRecentInstance;

            Assert.AreEqual(
                "INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception)",
                command.CommandText);

            Assert.AreEqual(6, command.Parameters.Count);

            IDbDataParameter param = (IDbDataParameter)command.Parameters["@message"];
            Assert.AreEqual("Message", param.Value);

            param = (IDbDataParameter)command.Parameters["@log_level"];
            Assert.AreEqual(Level.Debug.ToString(), param.Value);

            param = (IDbDataParameter)command.Parameters["@logger"];
            Assert.AreEqual("WebsiteExample", param.Value);

            param = (IDbDataParameter)command.Parameters["@exception"];
            Assert.IsTrue(string.IsNullOrEmpty((string)param.Value));
        }

    }
}
