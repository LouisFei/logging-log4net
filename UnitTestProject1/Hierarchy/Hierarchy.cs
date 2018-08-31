using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using log4net.Repository;
using log4net;
using log4net.Config;

namespace UnitTestProject1.Hierarchy
{
    [TestClass]
    public class Hierarchy
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        [TestMethod]
        public void SetRepositoryPropertiesInConfigFile()
        {
            XmlDocument log4netConfig = new XmlDocument();
            log4netConfig.LoadXml(@"
                <log4net>
                  <property>
                    <key value=""two-plus-two"" />
                    <value value=""4"" />
                  </property>
                  <appender name=""StringAppender"" type=""log4net.Tests.Appender.StringAppender, log4net.Tests"">
                    <layout type=""log4net.Layout.SimpleLayout"" />
                  </appender>
                  <root>
                    <level value=""ALL"" />
                    <appender-ref ref=""StringAppender"" />
                  </root>
                </log4net>");

            ILoggerRepository rep = LogManager.CreateRepository(Guid.NewGuid().ToString());
            XmlConfigurator.Configure(rep, log4netConfig["log4net"]);

            Assert.AreEqual("4", rep.Properties["two-plus-two"]);
            Assert.IsNull(rep.Properties["one-plus-one"]);
        }
    }
}
