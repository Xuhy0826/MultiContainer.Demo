namespace Mark.Common
{
    /// <summary>
    /// 自定义异常
    /// </summary>
    public class CustomException : ExceptionBase
    {
        public object MoreInfo { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public CustomException() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public CustomException(string message) : base(message) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        public CustomException(string message, object moreInfo) : base(message)
        {
            MoreInfo = moreInfo;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public CustomException(string message, System.Exception innerException) : base(message, innerException) { }
    }
}
