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
using System.IO;

namespace log4net.ObjectRenderer
{
    /// <summary>
    /// Implement this interface in order to render objects as strings.
    /// 实现此接口以将对象呈现为字符串。
    /// 将告诉logger如何把一个对象转化为一个字符串记录到日志里。
    /// </summary>
    /// <remarks>
    /// <para>
    /// Certain types require special case conversion to
    /// string form. This conversion is done by an object renderer.
    /// Object renderers implement the <see cref="IObjectRenderer"/>
    /// interface.
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public interface IObjectRenderer
	{
        /// <summary>
        /// Render the object <paramref name="obj"/> to a string
        /// 将对象<paramref name="obj"/>渲染到一个字符串
        /// </summary>
        /// <param name="rendererMap">The map used to lookup renderers.用于查找呈现程序的映射</param>
        /// <param name="obj">The object to render</param>
        /// <param name="writer">The writer to render to.对象渲染到的目的写入器</param>
        /// <remarks>
        /// <para>
        /// Render the object <paramref name="obj"/> to a string.
        /// </para>
        /// <para>
        /// The <paramref name="rendererMap"/> parameter is provided to lookup and render other objects. 
        /// <paramref name="rendererMap"/>参数用于查找和呈现其他对象。
        /// This is very useful where <paramref name="obj"/> contains nested objects of unknown type. 
        /// 当<paramref name="obj"/>包含未知类型的嵌套对象时，这非常有用。
        /// The <see cref="M:RendererMap.FindAndRender(object, TextWriter)"/> method can be used to render these objects.
        /// RendererMap.FindAndRender(object, TextWriter)方法可用于呈现这些对象。
        /// </para>
        /// </remarks>
        void RenderObject(RendererMap rendererMap, object obj, TextWriter writer);
	}
}
