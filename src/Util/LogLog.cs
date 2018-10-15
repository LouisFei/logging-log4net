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
using System.Collections;
#if !NETSTANDARD1_3
using System.Configuration;
#endif
using System.Diagnostics;

namespace log4net.Util
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    public delegate void LogReceivedEventHandler(object source, LogReceivedEventArgs e);

    /// <summary>
    /// Outputs log statements from within the log4net assembly.
    /// 从log4net程序集中输出日志语句。
    /// </summary>
    /// <remarks>
    /// <para>
    /// Log4net components cannot make log4net logging calls. 
    /// However, it is sometimes useful for the user to learn about what log4net is doing.
    /// Log4net组件不能执行Log4net日志调用。
    /// 然而，对于用户来说，了解log4net在做什么有时是有用的。
    /// </para>
    /// <para>
    /// All log4net internal debug calls go to the standard output stream
    /// whereas internal error messages are sent to the standard error output stream.
    /// 所有log4net内部调试调用都被发送到标准输出流，而内部错误消息被发送到标准错误输出流。
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public sealed class LogLog
	{
        /// <summary>
        /// The event raised when an internal message has been received.
        /// 内部消息接收事件
        /// </summary>
        public static event LogReceivedEventHandler LogReceived;

        private readonly Type source;
        private readonly DateTime timeStampUtc;
        private readonly string prefix;
        private readonly string message;
        private readonly Exception exception;

        /// <summary>
        /// The Type that generated the internal message.
        /// </summary>
	    public Type Source
	    {
	        get { return source; }
	    }

        /// <summary>
        /// The DateTime stamp of when the internal message was received.
        /// </summary>
	    public DateTime TimeStamp
	    {
            get { return timeStampUtc.ToLocalTime(); }
	    }

        /// <summary>
        /// The UTC DateTime stamp of when the internal message was received.
        /// </summary>
        public DateTime TimeStampUtc
        {
            get { return timeStampUtc; }
	    }

        /// <summary>
        /// A string indicating the severity of the internal message.
        /// </summary>
        /// <remarks>
        /// "log4net: ", 
        /// "log4net:ERROR ", 
        /// "log4net:WARN "
        /// </remarks>
	    public string Prefix
	    {
	        get { return prefix; }
	    }

        /// <summary>
        /// The internal log message.
        /// </summary>
	    public string Message
	    {
	        get { return message; }
	    }

        /// <summary>
        /// The Exception related to the message.
        /// </summary>
        /// <remarks>
        /// Optional. Will be null if no Exception was passed.
        /// </remarks>
	    public Exception Exception
	    {
	        get { return exception; }
	    }

        /// <summary>
        /// Formats Prefix, Source, and Message in the same format as the value
        /// sent to Console.Out and Trace.Write.
        /// </summary>
        /// <returns></returns>
	    public override string ToString()
	    {
            return Prefix + Source.Name + ": " + Message;
	    }

	    #region Private Instance Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LogLog" /> class. 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
	    public LogLog(Type source, string prefix, string message, Exception exception)
	    {
            timeStampUtc = DateTime.UtcNow;
	        
            this.source = source;
	        this.prefix = prefix;
	        this.message = message;
	        this.exception = exception;
	    }

		#endregion Private Instance Constructors

		#region Static Constructor

		/// <summary>
		/// Static constructor that initializes logging by reading 
		/// settings from the application configuration file.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <c>log4net.Internal.Debug</c> application setting
		/// controls internal debugging. This setting should be set
		/// to <c>true</c> to enable debugging.
		/// </para>
		/// <para>
		/// The <c>log4net.Internal.Quiet</c> application setting
		/// suppresses all internal logging including error messages. 
		/// This setting should be set to <c>true</c> to enable message
		/// suppression.
		/// </para>
		/// </remarks>
		static LogLog()
		{
#if !NETCF
			try
			{
				InternalDebugging = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Debug"), false);
				QuietMode = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Quiet"), false);
				EmitInternalMessages = OptionConverter.ToBoolean(SystemInfo.GetAppSetting("log4net.Internal.Emit"), true);
			}
			catch(Exception ex)
			{
				// If an exception is thrown here then it looks like the config file does not
				// parse correctly.
				//
				// We will leave debug OFF and print an Error message
				Error(typeof(LogLog), "Exception while reading ConfigurationSettings. Check your .config file is well formed XML.", ex);
			}
#endif
		}

        #endregion Static Constructor

        #region Public Static Properties

        /// <summary>
        /// Gets or sets a value indicating whether log4net internal logging is enabled or disabled.
        /// 获取或设置一个值，该值指示是否启用或禁用log4net内部日志记录。
        /// </summary>
        /// <value>
        /// <c>true</c> if log4net internal logging is enabled, otherwise <c>false</c>.
        /// 如果log4net内部日志记录被启用，则为true，否则为false
        /// </value>
        /// <remarks>
        /// <para>
        /// When set to <c>true</c>, internal debug level logging will be displayed.
        /// 当设置为true时，将显示内部调试级别的日志记录。
        /// </para>
        /// <para>
        /// This value can be set by setting the application setting <c>log4net.Internal.Debug</c> in the application configuration file.
        /// 这个值可以通过在应用程序配置文件设置应用程序设置<c>log4net.Internal.Debug</c>来设置。
        /// </para>
        /// <para>
        /// The default value is <c>false</c>, i.e. debugging is disabled.
        /// 默认值<c>false</c>，即禁用调试。
        /// </para>
        /// </remarks>
        /// <example>
        /// <para>
        /// The following example enables internal debugging using the application configuration file :
        /// </para>
        /// <code lang="XML" escaped="true">
        /// <configuration>
        ///		<appSettings>
        ///			<add key="log4net.Internal.Debug" value="true" />
        ///		</appSettings>
        /// </configuration>
        /// </code>
        /// </example>
        public static bool InternalDebugging
		{
			get { return s_debugEnabled; }
			set { s_debugEnabled = value; }
		}

        /// <summary>
        /// Gets or sets a value indicating whether log4net should generate no output from internal logging, not even for errors. 
        /// 获取或设置一个值，该值指示log4net是否不应该从内部日志记录生成任何输出，甚至对于错误也不应该。
        /// </summary>
        /// <value>
        /// <c>true</c> if log4net should generate no output at all from internal logging, otherwise <c>false</c>.
        /// 如果log4net不应该从内部日志中生成任何输出，则为true，否则为false。
        /// </value>
        /// <remarks>
        /// <para>
        /// When set to <c>true</c> will cause internal logging at all levels to be suppressed. 
        /// 当设置为true时，将导致所有级别的内部日志记录被抑制。
        /// This means that no warning or error reports will be logged. 
        /// 这意味着不会记录任何警告或错误报告。
        /// This option overrides the <see cref="InternalDebugging"/> setting and disables all debug also.
        /// </para>
        /// <para>This value can be set by setting the application setting <c>log4net.Internal.Quiet</c> in the application configuration file.
        /// </para>
        /// <para>
        /// The default value is <c>false</c>, i.e. internal logging is not disabled.
        /// 默认值为false，即不禁用内部日志记录。
        /// </para>
        /// </remarks>
        /// <example>
        /// The following example disables internal logging using the application configuration file :
        /// <code lang="XML" escaped="true">
        /// <configuration>
        ///		<appSettings>
        ///			<add key="log4net.Internal.Quiet" value="true" />
        ///		</appSettings>
        /// </configuration>
        /// </code>
        /// </example>
        public static bool QuietMode
		{
			get { return s_quietMode; }
			set { s_quietMode = value; }
		}

        /// <summary>
        /// 发出内部消息
        /// </summary>
        public static bool EmitInternalMessages
        {
            get { return s_emitInternalMessages; }
            set { s_emitInternalMessages = value; }
        }

		#endregion Public Static Properties

		#region Public Static Methods

        /// <summary>
        /// Raises the LogReceived event when an internal messages is received.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="prefix"></param>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public static void OnLogReceived(Type source, string prefix, string message, Exception exception)
        {
            if (LogReceived != null)
            {
                LogReceived(null, new LogReceivedEventArgs(new LogLog(source, prefix, message, exception)));
            }
        }

	    /// <summary>
		/// Test if LogLog.Debug is enabled for output.
		/// </summary>
		/// <value>
		/// <c>true</c> if Debug is enabled
		/// </value>
		/// <remarks>
		/// <para>
		/// Test if LogLog.Debug is enabled for output.
		/// </para>
		/// </remarks>
		public static bool IsDebugEnabled
		{
			get { return s_debugEnabled && !s_quietMode; }
		}

        /// <summary>
        /// Writes log4net internal debug messages to the standard output stream.
        /// 将log4net内部调试消息写入标准输出流。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// <para>
        ///	All internal debug messages are prepended with the string "log4net: ".
        /// </para>
        /// </remarks>
        public static void Debug(Type source, string message) 
		{
			if (IsDebugEnabled) 
			{
                if (EmitInternalMessages)
                {
                    EmitOutLine(PREFIX + message);
                }

                OnLogReceived(source, PREFIX, message, null);
			}
		}

		/// <summary>
		/// Writes log4net internal debug messages to the 
		/// standard output stream.
		/// </summary>
        /// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="exception">An exception to log.</param>
		/// <remarks>
		/// <para>
		///	All internal debug messages are prepended with 
		///	the string "log4net: ".
		/// </para>
		/// </remarks>
		public static void Debug(Type source, string message, Exception exception) 
		{
			if (IsDebugEnabled) 
			{
                if (EmitInternalMessages)
                {
                    EmitOutLine(PREFIX + message);
                    if (exception != null)
                    {
                        EmitOutLine(exception.ToString());
                    }
                }

                OnLogReceived(source, PREFIX, message, exception);
			}
		}
  
		/// <summary>
		/// Test if LogLog.Warn is enabled for output.
		/// </summary>
		/// <value>
		/// <c>true</c> if Warn is enabled
		/// </value>
		/// <remarks>
		/// <para>
		/// Test if LogLog.Warn is enabled for output.
		/// </para>
		/// </remarks>
		public static bool IsWarnEnabled
		{
			get { return !s_quietMode; }
		}

		/// <summary>
		/// Writes log4net internal warning messages to the 
		/// standard error stream.
		/// </summary>
        /// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <remarks>
		/// <para>
		///	All internal warning messages are prepended with 
		///	the string "log4net:WARN ".
		/// </para>
		/// </remarks>
		public static void Warn(Type source, string message) 
		{
			if (IsWarnEnabled)
			{
                if (EmitInternalMessages)
                {
                    EmitErrorLine(WARN_PREFIX + message);
                }

                OnLogReceived(source, WARN_PREFIX, message, null);
			}
		}  

		/// <summary>
		/// Writes log4net internal warning messages to the 
		/// standard error stream.
		/// </summary>
        /// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="exception">An exception to log.</param>
		/// <remarks>
		/// <para>
		///	All internal warning messages are prepended with 
		///	the string "log4net:WARN ".
		/// </para>
		/// </remarks>
		public static void Warn(Type source, string message, Exception exception) 
		{
			if (IsWarnEnabled)
			{
                if (EmitInternalMessages)
                {
                    EmitErrorLine(WARN_PREFIX + message);
                    if (exception != null)
                    {
                        EmitErrorLine(exception.ToString());
                    }
                }

                OnLogReceived(source, WARN_PREFIX, message, exception);
			}
		} 

		/// <summary>
		/// Test if LogLog.Error is enabled for output.
		/// </summary>
		/// <value>
		/// <c>true</c> if Error is enabled
		/// </value>
		/// <remarks>
		/// <para>
		/// Test if LogLog.Error is enabled for output.
		/// </para>
		/// </remarks>
		public static bool IsErrorEnabled
		{
			get { return !s_quietMode; }
		}

        /// <summary>
        /// Writes log4net internal error messages to the standard error stream.
        /// 将log4net内部错误消息写入标准错误流。
        /// </summary>
        /// <param name="source">The Type that generated this message.</param>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// <para>
        ///	All internal error messages are prepended with 
        ///	the string "log4net:ERROR ".
        /// </para>
        /// </remarks>
        public static void Error(Type source, string message) 
		{
			if (IsErrorEnabled)
			{
                if (EmitInternalMessages)
                {
                    EmitErrorLine(ERR_PREFIX + message);
                }

                OnLogReceived(source, ERR_PREFIX, message, null);
			}
		}  

		/// <summary>
		/// Writes log4net internal error messages to the 
		/// standard error stream.
		/// </summary>
        /// <param name="source">The Type that generated this message.</param>
		/// <param name="message">The message to log.</param>
		/// <param name="exception">An exception to log.</param>
		/// <remarks>
		/// <para>
		///	All internal debug messages are prepended with 
		///	the string "log4net:ERROR ".
		/// </para>
		/// </remarks>
		public static void Error(Type source, string message, Exception exception) 
		{
			if (IsErrorEnabled)
			{
                if (EmitInternalMessages)
                {
                    EmitErrorLine(ERR_PREFIX + message);
                    if (exception != null)
                    {
                        EmitErrorLine(exception.ToString());
                    }
                }

                OnLogReceived(source, ERR_PREFIX, message, exception);
			}
		}

        #endregion Public Static Methods

        /// <summary>
        /// Writes output to the standard output stream.  
        /// 将输出写入标准输出流。
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// <para>
        /// Writes to both Console.Out and System.Diagnostics.Trace.
        /// Note that the System.Diagnostics.Trace is not supported
        /// on the Compact Framework.
        /// </para>
        /// <para>
        /// If the AppDomain is not configured with a config file then
        /// the call to System.Diagnostics.Trace may fail. This is only
        /// an issue if you are programmatically creating your own AppDomains.
        /// </para>
        /// </remarks>
        private static void EmitOutLine(string message)
		{
			try
			{
#if NETCF
				Console.WriteLine(message);
				//System.Diagnostics.Debug.WriteLine(message);
#else
				Console.Out.WriteLine(message);
				Trace.WriteLine(message);
#endif
			}
			catch
			{
				// Ignore exception, what else can we do? Not really a good idea to propagate back to the caller
			}
		}

        /// <summary>
        /// Writes output to the standard error stream.  
        /// 将输出写入标准错误流。
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <remarks>
        /// <para>
        /// Writes to both Console.Error and System.Diagnostics.Trace.
        /// Note that the System.Diagnostics.Trace is not supported
        /// on the Compact Framework.
        /// </para>
        /// <para>
        /// If the AppDomain is not configured with a config file then
        /// the call to System.Diagnostics.Trace may fail. This is only
        /// an issue if you are programmatically creating your own AppDomains.
        /// </para>
        /// </remarks>
        private static void EmitErrorLine(string message)
		{
			try
			{
#if NETCF
				Console.WriteLine(message);
				//System.Diagnostics.Debug.WriteLine(message);
#else
				Console.Error.WriteLine(message);
				Trace.WriteLine(message);
#endif
			}
			catch
			{
				// Ignore exception, what else can we do? Not really a good idea to propagate back to the caller
			}
		}

		#region Private Static Fields

		/// <summary>
		///  Default debug level
		/// </summary>
		private static bool s_debugEnabled = false;

		/// <summary>
		/// In quietMode not even errors generate any output.
		/// </summary>
		private static bool s_quietMode = false;

        /// <summary>
        /// 发出内部消息
        /// </summary>
        private static bool s_emitInternalMessages = true;

        /// <summary>
        /// 前缀
        /// </summary>
		private const string PREFIX			= "log4net: ";
        /// <summary>
        /// 报错前缀
        /// </summary>
		private const string ERR_PREFIX		= "log4net:ERROR ";
        /// <summary>
        /// 警告前缀
        /// </summary>
		private const string WARN_PREFIX	= "log4net:WARN ";

        #endregion Private Static Fields

        /// <summary>
        /// Subscribes to the LogLog.LogReceived event and stores messages to the supplied IList instance.
        /// 订阅LogLog.logreceive事件并将消息存储到提供的IList实例。
        /// </summary>
        public class LogReceivedAdapter : IDisposable
        {
            private readonly IList items;
            private readonly LogReceivedEventHandler handler;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="items"></param>
            public LogReceivedAdapter(IList items)
            {
                this.items = items;

                handler = new LogReceivedEventHandler(LogLog_LogReceived);

                LogReceived += handler;
            }

            void LogLog_LogReceived(object source, LogReceivedEventArgs e)
            {
                items.Add(e.LogLog);
            }

            /// <summary>
            /// 
            /// </summary>
            public IList Items
            {
                get { return items; }
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                LogReceived -= handler;
            }
        }
	}

    /// <summary>
    /// 
    /// </summary>
    public class LogReceivedEventArgs : EventArgs
    {
        private readonly LogLog loglog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loglog"></param>
        public LogReceivedEventArgs(LogLog loglog)
        {
            this.loglog = loglog;
        }

        /// <summary>
        /// 
        /// </summary>
        public LogLog LogLog
        {
            get { return loglog; }
        }
    }
}
