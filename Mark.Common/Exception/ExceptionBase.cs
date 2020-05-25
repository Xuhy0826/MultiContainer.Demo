using System;

namespace Mark.Common
{
	/// <summary>
	/// 异常基类
	/// </summary>
	public abstract class ExceptionBase : Exception
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ExceptionBase() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常信息</param>
		public ExceptionBase(string message) : base(message) { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message">异常信息</param>
		/// <param name="innerException">内部异常</param>
		public ExceptionBase(string message, Exception innerException) : base(message, innerException) { }

	}
}
