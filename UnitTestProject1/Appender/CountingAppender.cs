using log4net.Appender;
using log4net.Core;

namespace UnitTestProject1.Appender
{
    /// <summary>
    /// Implements an Appender for test purposes that counts the number of output calls to <see cref="Append" />.
    /// ʵ��һ�����ڲ���Ŀ�ĵĸ��������ø���������Ҫ���ӵ�������õ�������
    /// AppenderSkeleton�����ࡣ
    /// </summary>
    /// <remarks>
    /// This appender is used in the unit tests.
    /// ������������ڵ�Ԫ���ԡ�
    /// </remarks>
    /// <author>Nicko Cadell</author>
    /// <author>Gert Driesen</author>
    public class CountingAppender : AppenderSkeleton
	{
		#region Public Instance Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CountingAppender" /> class.
		/// </summary>
		public CountingAppender()
		{
			m_counter = 0;
		}
		#endregion Public Instance Constructors

		#region Public Instance Properties
		/// <summary>
		/// Returns the number of times <see cref="Append" /> has been called.
		/// </summary>
		/// <value>
		/// The number of times <see cref="Append" /> has been called.
		/// </value>
		public int Counter
		{
			get { return m_counter; }
		}
		#endregion Public Instance Properties

		/// <summary>
		/// Reset the counter to zero
		/// </summary>
		public void ResetCounter()
		{
			m_counter = 0;
		}

        #region Override implementation of AppenderSkeleton
        /// <summary>
        /// Registers how many times the method has been called.
        /// AppenderSkeleton������Ӧ��ʵ�����������ִ��ʵ�ʵ���־��¼��
        /// </summary>
        /// <param name="logEvent">The logging event.</param>
        protected override void Append(LoggingEvent logEvent)
		{
			m_counter++;
		}
		#endregion Override implementation of AppenderSkeleton

		#region Private Instance Fields
		/// <summary>
		/// The number of times <see cref="Append" /> has been called.
		/// </summary>
		private int m_counter;
		#endregion Private Instance Fields
	}
}