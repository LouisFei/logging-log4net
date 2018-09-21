#region Apache License
//
// Licensed to the Apache Software Foundation (ASF) under one or more 
// contributor license agreements. See the NOTICE file distributed with
// this work for additional information regarding copyright ownership. 
// The ASF licenses this file to you under the Apache License, Version 2.0
// (the "License"); you may not use this file except in compliance with 
// the License. You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using log4net.Filter;
using log4net.Layout;
using log4net.Core;

namespace log4net.Appender
{
    /// <summary>
    /// Implement this interface for your own strategies for printing log statements.
    /// 为打印日志语句的策略实现此接口。
    /// 附着器。
    /// 可以控制日志内容的输出目的地。
    /// appender的类别有：Console（控制台）File（文件）JDBC、JMS等等，
    /// logger可以通过方法logger.addAppender(appender);配置多个appender，
    /// 对logger来说，每个有效的日志请求结果都将输出到logger本身以及父logger的appender上。
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementors should consider extending the <see cref="AppenderSkeleton"/>
    /// class which provides a default implementation of this interface.
    /// 实现者应该考虑扩展AppenderSkeleton类，它提供这个接口的默认实现。
    /// </para>
    /// <para>
    /// Appenders can also implement the <see cref="IOptionHandler"/> interface. Therefore
    /// they would require that the <see cref="M:IOptionHandler.ActivateOptions()"/> method
    /// be called after the appenders properties have been configured.
    /// Appenders也可以实现IOptionHandler接口。因此他们需要在配置appenders属性后调用IOptionHandler.ActivateOptions()方法。
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public interface IAppender
	{
        /// <summary>
        /// Closes the appender and releases resources.
        /// 关闭appender并释放资源。
        /// </summary>
        /// <remarks>
        /// <para>
        /// Releases any resources allocated within the appender such as file handles, network connections, etc.
        /// 释放appender中分配的任何资源，如文件句柄、网络连接等。
        /// </para>
        /// <para>
        /// It is a programming error to append to a closed appender.
        /// 附加到一个关闭的附加器是一个编程错误。
        /// </para>
        /// </remarks>
        void Close();

        /// <summary>
        /// Log the logging event in Appender specific way.
        /// 以Appender特定的方式记录日志事件。
        /// </summary>
        /// <param name="loggingEvent">The event to log</param>
        /// <remarks>
        /// <para>
        /// This method is called to log a message into this appender.
        /// 调用此方法将消息记录到此appender。
        /// </para>
        /// </remarks>
        void DoAppend(LoggingEvent loggingEvent);

		/// <summary>
		/// Gets or sets the name of this appender.
		/// </summary>
		/// <value>The name of the appender.</value>
		/// <remarks>
		/// <para>The name uniquely identifies the appender.</para>
		/// </remarks>
		string Name { get; set; }
	}
}
