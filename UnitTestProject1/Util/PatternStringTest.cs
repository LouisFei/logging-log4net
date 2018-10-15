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
                //创建临时配置文件。
                configurationFileName = CreateTempConfigFile(configurationFileContent);
                //创建配置应用程序域
                appDomain = CreateConfiguredDomain("AppSettingsTestDomain", configurationFileName);

                //通过反射机制创建当前类的对象实例。
                PatternStringTest pst = (PatternStringTest)appDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, this.GetType().FullName);
                //调用对象方法
                pst.TestAppSettingPathConverterInConfiguredDomain();
            }
            finally
            {
                if (appDomain != null) AppDomain.Unload(appDomain);
                if (configurationFileName != null) File.Delete(configurationFileName);
            }
        }

        /// <summary>
        /// 测试AppSetting路径转换器（在配置的应用程序域中）
        /// </summary>
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

        /// <summary>
        /// 创建临时配置文件
        /// </summary>
        /// <param name="configurationFileContent"></param>
        /// <returns></returns>
        private static string CreateTempConfigFile(string configurationFileContent)
        {
            string fileName = Path.GetTempFileName();
            File.WriteAllText(fileName, configurationFileContent);
            return fileName;
        }

        /// <summary>
        /// 创建配置应用程序域
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="configurationFileName"></param>
        /// <returns></returns>
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
