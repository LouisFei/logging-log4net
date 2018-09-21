using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace UnitTestProject1.Util
{
    [TestClass]
    public class RandomStringPatternConverterTest
    {
        [TestMethod]
        public void TestConvert()
        {
            var converter = new RandomStringPatternConverter();

            // Check default string length
            StringWriter sw = new StringWriter();
            converter.Convert(sw, null);
            Assert.AreEqual(4, sw.ToString().Length, "Default string length should be 4");

            // Set string length to 7
            converter.Option = "7";
            converter.ActivateOptions();
            sw = new StringWriter();
            converter.Convert(sw, null);
            string string1 = sw.ToString();
            Assert.AreEqual(7, string1.Length, "string length should be 7");

            // Check for duplicate result
            sw = new StringWriter();
            converter.Convert(sw, null);
            string string2 = sw.ToString();
            Assert.IsTrue(string1 != string2, "strings should be different");
        }

        private class RandomStringPatternConverter
        {
            private object target = null;

            public RandomStringPatternConverter()
            {
                target = Utils.CreateInstance("log4net.Util.PatternStringConverters.RandomStringPatternConverter,log4net");
            }

            public string Option
            {
                get { return Utils.GetProperty(target, "Option") as string; }
                set { Utils.SetProperty(target, "Option", value); }
            }

            public void Convert(TextWriter writer, object state)
            {
                Utils.InvokeMethod(target, "Convert", writer, state);
            }

            public void ActivateOptions()
            {
                Utils.InvokeMethod(target, "ActivateOptions");
            }
        }

    }
}
