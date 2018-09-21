using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using log4net.Util;
using System.IO;
using System.Configuration;
using System.Reflection;

namespace UnitTestProject1.Util
{
    [TestClass]
    public class PatternStringTest : MarshalByRefObject
    {
        [TestMethod]
        public void TestEnvironmentFolderPathPatternConverter()
        {
            string[] specialFolderNames = Enum.GetNames(typeof(Environment.SpecialFolder));

            foreach (string specialFolderName in specialFolderNames)
            {
                string pattern = "%envFolderPath{" + specialFolderName + "}";

                PatternString patternString = new PatternString(pattern);

                string evaluatedPattern = patternString.Format();

                Environment.SpecialFolder specialFolder =
                    (Environment.SpecialFolder)Enum.Parse(typeof(Environment.SpecialFolder), specialFolderName);

                Assert.AreEqual(Environment.GetFolderPath(specialFolder), evaluatedPattern);
            }
        }

        [TestMethod]
        public void TestAppSettingPathConverter()
        {
            string configurationFileContent = @"
<configuration>
  <appSettings>
    <add key=""TestKey"" value = ""TestValue"" />
  </appSettings>
</configuration>
";
            string configurationFileName = null;
            AppDomain appDomain = null;
            try
            {
                configurationFileName = CreateTempConfigFile(configurationFileContent);
                appDomain = CreateConfiguredDomain("AppSettingsTestDomain", configurationFileName);

                PatternStringTest pst = (PatternStringTest)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, this.GetType().FullName);
                pst.TestAppSettingPathConverterInConfiguredDomain();
            }
            finally
            {
                if (appDomain != null) AppDomain.Unload(appDomain);
                if (configurationFileName != null) File.Delete(configurationFileName);
            }
        }

        public void TestAppSettingPathConverterInConfiguredDomain()
        {
            string pattern = "%appSetting{TestKey}";
            PatternString patternString = new PatternString(pattern);
            string evaluatedPattern = patternString.Format();
            string appSettingValue = ConfigurationManager.AppSettings["TestKey"];
            Assert.AreEqual("TestValue", appSettingValue, "Expected configuration file to contain a key TestKey with the value TestValue");
            Assert.AreEqual(appSettingValue, evaluatedPattern, "Evaluated pattern expected to be identical to appSetting value");

            string badPattern = "%appSetting{UnknownKey}";
            patternString = new PatternString(badPattern);
            evaluatedPattern = patternString.Format();
            Assert.AreEqual("(null)", evaluatedPattern, "Evaluated pattern expected to be \"(null)\" for non-existent appSettings key");
        }

        private static string CreateTempConfigFile(string configurationFileContent)
        {
            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, configurationFileContent);
            return fileName;
        }

        private static AppDomain CreateConfiguredDomain(string domainName, string configurationFileName)
        {
            AppDomainSetup ads = new AppDomainSetup();
            ads.ApplicationBase = AppDomain.CurrentDomain.BaseDirectory;
            ads.ConfigurationFile = configurationFileName;
            AppDomain ad = AppDomain.CreateDomain(domainName, null, ads);
            return ad;
        }


    }
}
