﻿#region Apache License
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
using log4net.Util;

namespace log4net.Util
{
    /// <summary>
    /// Contain the information obtained when parsing formatting modifiers in conversion modifiers.
    /// 包含转换修饰符中解析格式化修饰符时获得的信息。
    /// </summary>
    /// <remarks>
    /// <para>
    /// Holds the formatting information extracted from the format string by the <see cref="PatternParser"/>. 
    /// 保存模式解析器从格式字符串中提取的格式信息。
    /// This is used by the <see cref="PatternConverter"/> objects when rendering the output.
    /// 在呈现输出时，PatternConverter对象使用了这个方法。
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public class FormattingInfo
	{
		#region Public Instance Constructors

		/// <summary>
		/// Defaut Constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="FormattingInfo" /> class.
		/// </para>
		/// </remarks>
		public FormattingInfo() 
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="FormattingInfo" /> class with the specified parameters.
		/// </para>
		/// </remarks>
		public FormattingInfo(int min, int max, bool leftAlign) 
		{
			m_min = min;
			m_max = max;
			m_leftAlign = leftAlign;
		}

		#endregion Public Instance Constructors

		#region Public Instance Properties

		/// <summary>
		/// Gets or sets the minimum value.
		/// </summary>
		/// <value>
		/// The minimum value.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets or sets the minimum value.
		/// </para>
		/// </remarks>
		public int Min
		{
			get { return m_min; }
			set { m_min = value; }
		}

		/// <summary>
		/// Gets or sets the maximum value.
		/// </summary>
		/// <value>
		/// The maximum value.
		/// </value>
		/// <remarks>
		/// <para>
		/// Gets or sets the maximum value.
		/// </para>
		/// </remarks>
		public int Max
		{
			get { return m_max; }
			set { m_max = value; }
		}

        /// <summary>
        /// Gets or sets a flag indicating whether left align is enabled or not.
        /// 获取或设置一个标志，该标志指示是否启用左对齐。
        /// </summary>
        /// <value>
        /// A flag indicating whether left align is enabled or not.
        /// 指示是否启用左对齐的标志。
        /// </value>
        /// <remarks>
        /// <para>
        /// Gets or sets a flag indicating whether left align is enabled or not.
        /// 获取或设置一个标志，该标志指示是否启用左对齐。
        /// </para>
        /// </remarks>
        public bool LeftAlign
		{
			get { return m_leftAlign; }
			set { m_leftAlign = value; }
		}

		#endregion Public Instance Properties

		#region Private Instance Fields

		private int m_min = -1;
		private int m_max = int.MaxValue;
		private bool m_leftAlign = false;

		#endregion Private Instance Fields
	}
}
