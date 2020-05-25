namespace Mark.Common
{/// <summary>
 /// 对象不存在异常
 /// </summary>
    public class NotFoundException : ExceptionBase
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public NotFoundException() { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message"></param>
		public NotFoundException(string message) : base(message) { }

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public NotFoundException(string message, System.Exception innerException) : base(message, innerException) { }
	}
}
