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
using System.Text;

using log4net.Core;
using log4net.Util;

namespace log4net.Layout
{
    /// <summary>
    /// Extract the value of a property from the <see cref="LoggingEvent"/>
    /// 从<see cref="LoggingEvent"/>中提取属性值
    /// </summary>
    /// <remarks>
    /// <para>
    /// Extract the value of a property from the <see cref="LoggingEvent"/>
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    public class RawPropertyLayout : IRawLayout
	{
		#region Constructors

		/// <summary>
		/// Constructs a RawPropertyLayout
		/// </summary>
		public RawPropertyLayout()
		{
		}

		#endregion

		private string m_key;

        /// <summary>
        /// The name of the value to lookup in the LoggingEvent Properties collection.
        /// 要在LoggingEvent属性集合中查找的值的名称。
        /// </summary>
        /// <value>
        /// Value to lookup in the LoggingEvent Properties collection
        /// 在LoggingEvent属性集合中查找的值
        /// </value>
        /// <remarks>
        /// <para>
        /// String name of the property to lookup in the <see cref="LoggingEvent"/>.
        /// 要在<see cref="LoggingEvent"/>中查找的属性的字符串名称。
        /// </para>
        /// </remarks>
        public string Key
		{
			get { return m_key; }
			set { m_key = value; }
		}

        #region Implementation of IRawLayout

        /// <summary>
        /// Lookup the property for <see cref="Key"/>
        /// 查找属性<see cref="Key"/>
        /// </summary>
        /// <param name="loggingEvent">The event to format</param>
        /// <returns>returns property value</returns>
        /// <remarks>
        /// <para>
        /// Looks up and returns the object value of the property named <see cref="Key"/>. 
        /// 查找并返回名为<see cref="Key"/>的属性的对象值。
        /// If there is no property defined with than name then <c>null</c> will be returned.
        /// 如果没有定义属性，则返回<c>null</c>。查找并返回名为<see cref="Key"/>的属性的对象值。
        /// </para>
        /// </remarks>
        public virtual object Format(LoggingEvent loggingEvent)
		{
			return loggingEvent.LookupProperty(m_key);
		}

		#endregion
	}
}
