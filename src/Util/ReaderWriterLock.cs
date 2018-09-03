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

#if (!NETCF)
#define HAS_READERWRITERLOCK
#endif
#if NET_4_0 || MONO_4_0
#define HAS_READERWRITERLOCKSLIM
#endif

using System;

namespace log4net.Util
{
    /// <summary>
    /// Defines a lock that supports single writers and multiple readers.
    /// 定义一个支持单个写入器和多个读取器的锁。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>ReaderWriterLock</c> is used to synchronize access to a resource. 
    /// At any given time, it allows either concurrent read access for 
    /// multiple threads, or write access for a single thread. In a 
    /// situation where a resource is changed infrequently, a 
    /// <c>ReaderWriterLock</c> provides better throughput than a simple 
    /// one-at-a-time lock, such as <see cref="System.Threading.Monitor"/>.
    /// 
    /// ReaderWriterLock用于同步对资源的访问。
    /// 在任何给定的时间，它都允许对多个线程进行并发读访问，或者对单个线程进行写访问。
    /// 在资源很少更改的情况下，ReaderWriterLock比简单的一次一个锁提供更好的吞吐量，比如System.Threading.Monitor。
    /// </para>
    /// <para>
    /// If a platform does not support a <c>System.Threading.ReaderWriterLock</c> 
    /// implementation then all readers and writers are serialized. Therefore 
    /// the caller must not rely on multiple simultaneous readers.
    /// 
    /// 如果一个平台不支持System.Threading.ReaderWriterLock实现 然后所有的阅读器和写入器都被序列化。
    /// 因此，调用者不能依赖于多个并发阅读器。
    /// </para>
    /// </remarks>
    /// <author>Nicko Cadell</author>
    public sealed class ReaderWriterLock
	{
		#region Instance Constructors

		/// <summary>
		/// Constructor
		/// </summary>
		/// <remarks>
		/// <para>
		/// Initializes a new instance of the <see cref="ReaderWriterLock" /> class.
		/// </para>
		/// </remarks>
		public ReaderWriterLock()
		{

#if HAS_READERWRITERLOCK
#if HAS_READERWRITERLOCKSLIM
			m_lock = new System.Threading.ReaderWriterLockSlim(System.Threading.LockRecursionPolicy.SupportsRecursion);
#else
			m_lock = new System.Threading.ReaderWriterLock();
#endif
#endif
		}

        #endregion Private Instance Constructors

        #region Public Methods

        /// <summary>
        /// Acquires a reader lock。
        /// 获得读取锁。
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="AcquireReaderLock"/> blocks if a different thread has the writer lock, or if at least one thread is waiting for the writer lock.
        /// 如果另一个线程有写入器锁，或者至少有一个线程在等待写入器锁，则阻塞。
        /// </para>
        /// </remarks>
        public void AcquireReaderLock()
		{
#if HAS_READERWRITERLOCK
#if HAS_READERWRITERLOCKSLIM
                    // prevent ThreadAbort while updating state, see https://issues.apache.org/jira/browse/LOG4NET-443
                    try { } 
                    finally
                    {
                //尝试进入读取模式锁定状态，可以选择整数超时时间。
                m_lock.EnterReadLock();
                    }
#else
			m_lock.AcquireReaderLock(-1);
#endif
#else
			System.Threading.Monitor.Enter(this);
#endif
		}

        /// <summary>
        /// Decrements the lock count.
        /// 减少读取锁计数。
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="ReleaseReaderLock"/> decrements the lock count. When the count reaches zero, the lock is released.
        /// 减少读取锁计数。当计数为0时，锁被释放。
        /// </para>
        /// </remarks>
        public void ReleaseReaderLock()
		{
#if HAS_READERWRITERLOCK
#if HAS_READERWRITERLOCKSLIM
            //减少读取模式的递归计数，并在生成的计数为 0（零）时退出读取模式。
            m_lock.ExitReadLock();
#else
			m_lock.ReleaseReaderLock();

#endif
#else
			System.Threading.Monitor.Exit(this);
#endif
		}

        /// <summary>
        /// Acquires the writer lock.
        /// 获得写入锁。
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method blocks if another thread has a reader lock or writer lock.
        /// 如果另一个线程有读锁或写锁，此方法将阻塞。
        /// </para>
        /// </remarks>
        public void AcquireWriterLock()
		{
#if HAS_READERWRITERLOCK
#if HAS_READERWRITERLOCKSLIM
                    // prevent ThreadAbort while updating state, see https://issues.apache.org/jira/browse/LOG4NET-443
                    try { } 
                    finally
                    {
                //尝试进入写入模式锁定状态，可以选择超时时间。
                m_lock.EnterWriteLock();
                    }
#else
			m_lock.AcquireWriterLock(-1);
#endif
#else
			System.Threading.Monitor.Enter(this);
#endif
		}

        /// <summary>
        /// Decrements the lock count on the writer lock.
        /// 减少写入器锁计数。
        /// </summary>
        /// <remarks>
        /// <para>
        /// ReleaseWriterLock decrements the writer lock count. 
        /// When the count reaches zero, the writer lock is released.
        /// 释放写入器锁递减写入器锁计数。
        /// 当计数为0时，写入器锁被释放。
        /// </para>
        /// </remarks>
        public void ReleaseWriterLock()
		{
#if HAS_READERWRITERLOCK
#if HAS_READERWRITERLOCKSLIM
            //减少写入模式的递归计数，并在生成的计数为 0（零）时退出写入模式。
            m_lock.ExitWriteLock();
#else
			m_lock.ReleaseWriterLock();
#endif
#else
			System.Threading.Monitor.Exit(this);
#endif
		}

        #endregion Public Methods

        #region Private Members

#if HAS_READERWRITERLOCK
#if HAS_READERWRITERLOCKSLIM
        /// <summary>
        /// 表示用于管理资源访问的锁定状态，可实现多线程读取或进行独占式写入访问。
        /// 
        /// ReaderWriterLockSlim 类似于 ReaderWriterLock, ，只是简化了递归、 升级和降级锁定状态的规则。 
        /// ReaderWriterLockSlim可避免潜在的死锁的很多情况。 
        /// 此外，性能的 ReaderWriterLockSlim 明显优于 ReaderWriterLock。 
        /// ReaderWriterLockSlim 建议对于所有新的开发。
        /// </summary>
        private System.Threading.ReaderWriterLockSlim m_lock;
#else
		private System.Threading.ReaderWriterLock m_lock;
#endif

#endif

		#endregion
	}
}