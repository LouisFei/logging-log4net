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

using log4net.Util;

namespace log4net
{
    /// <summary>
    /// The log4net Thread Context.
    /// log4net线程上下文。
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>ThreadContext</c> provides a location for thread specific debugging information to be stored.
    /// ThreadContext为存储特定于线程的调试信息提供了一个位置。
    /// The <c>ThreadContext</c> properties override any <see cref="GlobalContext"/> properties with the same name.
    /// ThreadContext属性覆盖所有同名的GlobalContext属性。
    /// </para>
    /// <para>
    /// The thread context has a properties map and a stack.
    /// 线程上下文有一个属性映射和一个堆栈。
    /// The properties and stack can be included in the output of log messages. 
    /// 属性和堆栈可以包含在日志消息的输出中。
    /// The <see cref="log4net.Layout.PatternLayout"/> supports selecting and outputting these properties.
    /// PatternLayout支持选择和输出这些属性。
    /// </para>
    /// <para>
    /// The Thread Context provides a diagnostic context for the current thread. 
    /// 线程上下文为当前线程提供诊断上下文。
    /// This is an instrument for distinguishing interleaved log output from different sources. 
    /// 这是一种区分不同来源的交错日志输出的工具。
    /// Log output is typically interleaved when a server handles multiple clients near-simultaneously.
    /// 当服务器几乎同时处理多个客户机时，日志输出通常是交叉的。
    /// </para>
    /// <para>
    /// The Thread Context is managed on a per thread basis.
    /// 线程上下文是在每个线程的基础上管理的。
    /// </para>
    /// </remarks>
    /// <example>Example of using the thread context properties to store a username.
    /// <code lang="C#">
    /// ThreadContext.Properties["user"] = userName;
    ///	log.Info("This log message has a ThreadContext Property called 'user'");
    /// </code>
    /// </example>
    /// <example>Example of how to push a message into the context stack
    /// <code lang="C#">
    ///	using(ThreadContext.Stacks["NDC"].Push("my context message"))
    ///	{
    ///		log.Info("This log message has a ThreadContext Stack message that includes 'my context message'");
    ///	
    ///	} // at the end of the using block the message is automatically popped 
    /// </code>
    /// </example>
    /// <threadsafety static="true" instance="true" />
    /// <author>Nicko Cadell</author>
    public sealed class ThreadContext
	{
		#region Private Instance Constructors

		/// <summary>
		/// Private Constructor. 
		/// </summary>
		/// <remarks>
		/// <para>
		/// Uses a private access modifier to prevent instantiation of this class.
		/// </para>
		/// </remarks>
		private ThreadContext()
		{
		}

		#endregion Private Instance Constructors

		#region Public Static Properties

		/// <summary>
		/// The thread properties map
		/// </summary>
		/// <value>
		/// The thread properties map
		/// </value>
		/// <remarks>
		/// <para>
		/// The <c>ThreadContext</c> properties override any <see cref="GlobalContext"/>
		/// properties with the same name.
		/// </para>
		/// </remarks>
		public static ThreadContextProperties Properties
		{
			get { return s_properties; }
		}

		/// <summary>
		/// The thread stacks
		/// </summary>
		/// <value>
		/// stack map
		/// </value>
		/// <remarks>
		/// <para>
		/// The thread local stacks.
		/// </para>
		/// </remarks>
		public static ThreadContextStacks Stacks
		{
			get { return s_stacks; }
		}

		#endregion Public Static Properties

		#region Private Static Fields

		/// <summary>
		/// The thread context properties instance
		/// </summary>
		private readonly static ThreadContextProperties s_properties = new ThreadContextProperties();

		/// <summary>
		/// The thread context stacks instance
		/// </summary>
		private readonly static ThreadContextStacks s_stacks = new ThreadContextStacks(s_properties);

		#endregion Private Static Fields
	}
}
