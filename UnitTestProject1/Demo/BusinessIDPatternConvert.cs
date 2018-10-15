using log4net.Layout.Pattern;
using System;
using log4net.Core;
using System.IO;

namespace UnitTestProject1.Demo
{
    //每一个特殊的格式符都是一个converter。
    public class BusinessIDPatternConvert : PatternLayoutConverter
    {
        protected override void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            var businessID = loggingEvent.MessageObject as BusinessIDLog;
            if (businessID == null)
            {
                return;
            }

            writer.Write($" businessID:{businessID.ID}, businessType:{businessID.BusinessType}");
        }
    }


}
