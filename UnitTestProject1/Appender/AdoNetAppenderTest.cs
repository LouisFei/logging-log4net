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
using log4net.Util;
using log4net.Layout;

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
            adoNetAppender.BufferSize = -1; //缓冲区大小，只有日志记录超过设定值才会一块写入到数据库。
            adoNetAppender.ConnectionType = "UnitTestProject1.Appender.AdoNet.Log4NetConnection";
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
            adoNetAppender.ConnectionType = "UnitTestProject1.Appender.AdoNet.Log4NetConnection";
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
                    <connectionType value=""UnitTestProject1.Appender.AdoNet.Log4NetConnection"" />
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

        [TestMethod]
        public void BufferingWebsiteExample()
        {
            XmlDocument log4netConfig = new XmlDocument();
            #region Load log4netConfig
            log4netConfig.LoadXml(@"
                <log4net>
                <appender name=""AdoNetAppender"" type=""log4net.Appender.AdoNetAppender"">
                    <bufferSize value=""2"" />
                    <connectionType value=""UnitTestProject1.Appender.AdoNet.Log4NetConnection"" />
                    <connectionString value=""data source=[database server];initial catalog=[database name];integrated security=false;persist security info=True;User ID=[user];Password=[password]"" />
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

            for (int i = 0; i < 3; i++)
            {
                log.Debug("Message");
            }

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

        [TestMethod]
        public void NullPropertyXmlConfig()
        {
            XmlDocument log4netConfig = new XmlDocument();
            #region Load log4netConfig
            log4netConfig.LoadXml(@"
                <log4net>
                <appender name=""AdoNetAppender"" type=""log4net.Appender.AdoNetAppender"">
                    <bufferSize value=""-1"" />
                    <connectionType value=""UnitTestProject1.Appender.AdoNet.Log4NetConnection"" />
                    <connectionString value=""data source=[database server];initial catalog=[database name];integrated security=false;persist security info=True;User ID=[user];Password=[password]"" />
                    <commandText value=""INSERT INTO Log ([ProductId]) VALUES (@productId)"" />
                    <parameter>
                        <parameterName value=""@productId"" />
                        <dbType value=""String"" />
                        <size value=""50"" />
                        <layout type="" log4net.Layout.RawPropertyLayout"">
                           <key value=""ProductId"" />
                        </layout>
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
            ILog log = LogManager.GetLogger(rep.Name, "NullPropertyXmlConfig");

            log.Debug("Message");

            IDbCommand command = Log4NetCommand.MostRecentInstance;
            IDbDataParameter param = (IDbDataParameter)command.Parameters["@productId"];

            Assert.AreNotEqual(SystemInfo.NullText, param.Value);
            Assert.AreEqual(DBNull.Value, param.Value);
        }

        [TestMethod]
        public void NullPropertyProgmaticConfig()
        {
            AdoNetAppender appender = new AdoNetAppender();
            appender.ConnectionType = typeof(Log4NetConnection).FullName;
            appender.BufferSize = -1;
            appender.CommandText = "INSERT INTO Log ([productId]) VALUES (@productId)";
            appender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@productId",
                DbType = DbType.String,
                Size = 50,
                Layout = new RawPropertyLayout() { Key = "ProductId" }
            });

            appender.ActivateOptions();

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            BasicConfigurator.Configure(rep, appender);
            ILog log = LogManager.GetLogger(rep.Name, "NullPropertyProgmaticConfig");

            log.Debug("Message");

            IDbCommand command = Log4NetCommand.MostRecentInstance;
            IDbDataParameter param = (IDbDataParameter)command.Parameters["@productId"];

            Assert.AreNotEqual(SystemInfo.NullText, param.Value);
            Assert.AreEqual(DBNull.Value, param.Value);
        }

        [TestMethod]
        public void ReallyAdoNetAppenderByCode()
        {
            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

            #region AdoNetAppender & AdoNetAppenderParameter
            AdoNetAppender adoAppender = new AdoNetAppender();
            adoAppender.Name = "AdoNetAppender";
            adoAppender.CommandType = CommandType.Text;
            adoAppender.BufferSize = 1; //被设置为小于或等于1的值，则不会发生缓冲。
            adoAppender.ConnectionType = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            adoAppender.ConnectionString = "Data Source=192.168.0.214;Initial Catalog=SipscMemberLog;Integrated Security=False;Persist Security Info=False;User ID=sml;Password=sml123;";
            adoAppender.CommandText = @"INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message],[Exception]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception)";
            //日志记录时间：RawTimeStampLayout为默认的时间输出格式。
            adoAppender.AddParameter(new AdoNetAppenderParameter() {
                ParameterName = "@log_date",
                DbType = DbType.DateTime,
                Layout = new RawTimeStampLayout()
            });
            //线程号
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@thread",
                Size = 255, //长度不可以省略，否则不会输出。
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread"))
            });
            //日志等级
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@log_level",
                Size = 50,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level"))
            });
            //日志记录类名称
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@logger",
                DbType = DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger"))
            });
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@message",
                DbType = DbType.String,
                Size = 4000,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message"))
            });            
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@exception",
                DbType = DbType.String,
                Size = 2000,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%exception"))
            });
            #endregion

            adoAppender.ActivateOptions();
            BasicConfigurator.Configure(rep, adoAppender);
            ILog log = LogManager.GetLogger(rep.Name, "ReallyAdoNetAppenderByCode");

            log.Info($"Message {Guid.NewGuid().ToString()}");
            log.Error($"出错啦 {Guid.NewGuid().ToString()}", new Exception("模拟异常"));

            Assert.IsTrue(true);
        }

        [TestMethod]
        public void ReallyAdoNetAppenderWithPropertyByCode()
        {
            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());

            #region AdoNetAppender & AdoNetAppenderParameter
            AdoNetAppender adoAppender = new AdoNetAppender();
            adoAppender.Name = "AdoNetAppender";
            adoAppender.CommandType = CommandType.Text;
            adoAppender.BufferSize = 1; //被设置为小于或等于1的值，则不会发生缓冲。
            adoAppender.ConnectionType = "System.Data.SqlClient.SqlConnection, System.Data, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            adoAppender.ConnectionString = "Data Source=192.168.0.214;Initial Catalog=SipscMemberLog;Integrated Security=False;Persist Security Info=False;User ID=sml;Password=sml123;";
            adoAppender.CommandText = @"INSERT INTO Log ([Date],[Thread],[Level],[Logger],[Message],[Exception],[Prop1],[Prop2],[Prop3]) VALUES (@log_date, @thread, @log_level, @logger, @message, @exception, @prop1, @prop2, @prop3)";
            //日志记录时间：RawTimeStampLayout为默认的时间输出格式。
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@log_date",
                DbType = DbType.DateTime,
                Layout = new RawTimeStampLayout()
            });
            //线程号
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@thread",
                Size = 255, //长度不可以省略，否则不会输出。
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%thread"))
            });
            //日志等级
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@log_level",
                Size = 50,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%level"))
            });
            //日志记录类名称
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@logger",
                DbType = DbType.String,
                Size = 255,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%logger"))
            });
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@message",
                DbType = DbType.String,
                Size = 4000,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%message"))
            });
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@exception",
                DbType = DbType.String,
                Size = 2000,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%exception"))
            });

            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@prop1",
                DbType = DbType.String,
                Size = 50,
                Layout = new Layout2RawLayoutAdapter(new PatternLayout("%property{prop1}"))
            });
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@prop2",
                DbType = DbType.Int32,
                //Size = 50,
                Layout = new RawPropertyLayout() { Key = "prop2" }
            });
            adoAppender.AddParameter(new AdoNetAppenderParameter()
            {
                ParameterName = "@prop3",
                DbType = DbType.Guid,
                Layout = new GuidPropertyLayout()
            });
            #endregion

            adoAppender.ActivateOptions();
            BasicConfigurator.Configure(rep, adoAppender);
            ILog log = LogManager.GetLogger(rep.Name, "ReallyAdoNetAppenderByCode");

            //log.Info($"Message {Guid.NewGuid().ToString()}");
            //log.Error($"出错啦 {Guid.NewGuid().ToString()}", new Exception("模拟异常"));

            var logEventData = new LoggingEventData()
            {
                TimeStampUtc = DateTime.UtcNow,
                Level = Level.Info,
                Message = $"test-{Guid.NewGuid()}",
                Properties = new PropertiesDictionary()
            };
            logEventData.Properties["prop1"] = "prop111";
            logEventData.Properties["prop2"] = 111;

            log.Logger.Log(new LoggingEvent(logEventData));

            Assert.IsTrue(true);
        }


    }

    public class GuidPropertyLayout : IRawLayout
    {
        public object Format(LoggingEvent loggingEvent)
        {
            return Guid.NewGuid();
        }
    }
}
