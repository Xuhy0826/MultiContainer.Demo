namespace Mark.Common
{
	/// <summary>
	/// 无权访问异常
	/// </summary>
	public class AcessForbiddenException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public AcessForbiddenException() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message"></param>
		public AcessForbiddenException(string message) : base(message) { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public AcessForbiddenException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
