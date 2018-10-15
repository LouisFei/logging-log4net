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

namespace UnitTestProject1.Demo
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void PatternLayoutConverterProperties()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.LoadXml(@"
                <log4net>
                  <appender name=""StringAppender"" type=""UnitTestProject1.Appender.StringAppender, UnitTestProject1"">
                    <layout type=""UnitTestProject1.Demo.BusinessIDPatternLayout"">
                        <conversionPattern value=""%date [%t]%-5level %c [%businessID] %n"" />
                    </layout>
                  </appender>
                  <root>
                    <level value=""ALL"" />                  
                    <appender-ref ref=""StringAppender"" />
                  </root>  
                </log4net>");

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            XmlConfigurator.Configure(rep, log4netConfig["log4net"]);

            ILog log = LogManager.GetLogger(rep.Name, "TestBusinessId");
            log.Info(new BusinessIDLog() { ID = "123456", BusinessType = "orderId" });
            //2018-10-15 10:29:19,131 [12]INFO  TestBusinessId [ businessID:123456, businessType:orderId] 

            StringAppender appender = (StringAppender)LogManager.GetRepository(rep.Name).GetAppenders()[0];
            Assert.IsTrue(appender.GetString().Contains("TestBusinessId [ businessID:123456, businessType:orderId]"));
        }
    }
}
