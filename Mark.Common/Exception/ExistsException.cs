namespace Mark.Common
{/// <summary>
 /// 对象已存在异常
 /// </summary>
    public class ExistsException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public ExistsException() { }

		/// <summary>
		/// 对象已存在异常
		/// </summary>
		/// <param name="message"></param>
		public ExistsException(string message) : base(message) { }

		/// <summary>
		/// 对象已存在异常
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ExistsException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
