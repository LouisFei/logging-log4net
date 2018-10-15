using log4net.Layout;
using System;

namespace UnitTestProject1.Demo
{
    //接管格式化，它不是全局的，而是当前实例局部的。

    public class BusinessIDPatternLayout : PatternLayout
    {
        public BusinessIDPatternLayout()
        {
            this.AddConverter("businessID", typeof(BusinessIDPatternConvert));
        }
    }
}
