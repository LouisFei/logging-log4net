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

using System;
using System.Globalization;

using log4net.Layout;
using log4net.Core;
using log4net.Util;

namespace log4net.Appender
{
    /// <summary>
    /// Appends logging events to the console.
    /// 把日志输出到控制台。
    /// </summary>
    /// <remarks>
    /// <para>
    /// ConsoleAppender appends log events to the standard output stream or the error output stream using a layout specified by the user.
    /// ConsoleAppender使用用户指定的布局将日志事件附加到标准输出流或错误输出流。
    /// </para>
    /// <para>
    /// By default, all output is written to the console's standard output stream.
    /// 默认情况下，所有输出都写入控制台的标准输出流。
    /// The <see cref="Target"/> property can be set to direct the output to the error stream.
    /// 可以将目标属性设置为将输出指向错误流。
    /// </para>
    /// <para>
    /// NOTE: This appender writes each message to the <c>System.Console.Out</c> or 
    /// <c>System.Console.Error</c> that is set at the time the event is appended.
    /// 注意:此附加器将每个消息写入事件附加时设置的System.Console.Out或System.Console.Error。
    /// Therefore it is possible to programmatically redirect the output of this appender (for example NUnit does this to capture program output). 
    /// 因此，可以通过编程方式重定向此appender的输出(例如NUnit这样做是为了捕获程序输出)。
    /// While this is the desired behavior of this appender it may have security implications in your application. 
    /// 虽然这是该附件的期望行为，但它可能对应用程序的安全性有影响。
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public class ConsoleAppender : AppenderSkeleton
	{
		#region Public Instance Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleAppender" /> class.
		/// </summary>
		/// <remarks>
		/// The instance of the <see cref="ConsoleAppender" /> class is set up to write 
		/// to the standard output stream.
		/// </remarks>
		public ConsoleAppender() 
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleAppender" /> class
		/// with the specified layout.
		/// </summary>
		/// <param name="layout">the layout to use for this appender</param>
		/// <remarks>
		/// The instance of the <see cref="ConsoleAppender" /> class is set up to write 
		/// to the standard output stream.
		/// </remarks>
		[Obsolete("Instead use the default constructor and set the Layout property")]
		public ConsoleAppender(ILayout layout) : this(layout, false)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConsoleAppender" /> class
		/// with the specified layout.
		/// </summary>
		/// <param name="layout">the layout to use for this appender</param>
		/// <param name="writeToErrorStream">flag set to <c>true</c> to write to the console error stream</param>
		/// <remarks>
		/// When <paramref name="writeToErrorStream" /> is set to <c>true</c>, output is written to
		/// the standard error output stream.  Otherwise, output is written to the standard
		/// output stream.
		/// </remarks>
		[Obsolete("Instead use the default constructor and set the Layout & Target properties")]
		public ConsoleAppender(ILayout layout, bool writeToErrorStream) 
		{
			Layout = layout;
			m_writeToErrorStream = writeToErrorStream;
		}

        #endregion Public Instance Constructors

        #region Public Instance Properties

        /// <summary>
        /// Target is the value of the console output stream.
        /// 目标是控制台输出流的值。
        /// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
        /// 目标要么是Console.Out，要么是Console.Error。
        /// </summary>
        /// <value>
        /// Target is the value of the console output stream.
        /// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
        /// </value>
        /// <remarks>
        /// <para>
        /// Target is the value of the console output stream.
        /// This is either <c>"Console.Out"</c> or <c>"Console.Error"</c>.
        /// </para>
        /// </remarks>
        virtual public string Target
		{
			get { return m_writeToErrorStream ? ConsoleError : ConsoleOut; }
			set
			{
				string v = value.Trim();

				if (SystemInfo.EqualsIgnoringCase(ConsoleError, v))
				{
					m_writeToErrorStream = true;
				} 
				else 
				{
					m_writeToErrorStream = false;
				}
			}
		}

		#endregion Public Instance Properties

		#region Override implementation of AppenderSkeleton

		/// <summary>
		/// This method is called by the <see cref="M:AppenderSkeleton.DoAppend(LoggingEvent)"/> method.
		/// </summary>
		/// <param name="loggingEvent">The event to log.</param>
		/// <remarks>
		/// <para>
		/// Writes the event to the console.
		/// </para>
		/// <para>
		/// The format of the output will depend on the appender's layout.
		/// </para>
		/// </remarks>
		override protected void Append(LoggingEvent loggingEvent) 
		{
#if NETCF_1_0
			// Write to the output stream
			Console.Write(RenderLoggingEvent(loggingEvent));
#else
			if (m_writeToErrorStream)
			{
				// Write to the error stream
				Console.Error.Write(RenderLoggingEvent(loggingEvent));
			}
			else
			{
				// Write to the output stream
				Console.Write(RenderLoggingEvent(loggingEvent));
			}
#endif
		}

		/// <summary>
		/// This appender requires a <see cref="Layout"/> to be set.
		/// </summary>
		/// <value><c>true</c></value>
		/// <remarks>
		/// <para>
		/// This appender requires a <see cref="Layout"/> to be set.
		/// </para>
		/// </remarks>
		override protected bool RequiresLayout
		{
			get { return true; }
		}

		#endregion Override implementation of AppenderSkeleton

		#region Public Static Fields

		/// <summary>
		/// The <see cref="ConsoleAppender.Target"/> to use when writing to the Console 
		/// standard output stream.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="ConsoleAppender.Target"/> to use when writing to the Console 
		/// standard output stream.
		/// </para>
		/// </remarks>
		public const string ConsoleOut = "Console.Out";

		/// <summary>
		/// The <see cref="ConsoleAppender.Target"/> to use when writing to the Console 
		/// standard error output stream.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="ConsoleAppender.Target"/> to use when writing to the Console 
		/// standard error output stream.
		/// </para>
		/// </remarks>
		public const string ConsoleError = "Console.Error";

		#endregion Public Static Fields

		#region Private Instances Fields

		private bool m_writeToErrorStream = false;

		#endregion Private Instances Fields
	}
}
