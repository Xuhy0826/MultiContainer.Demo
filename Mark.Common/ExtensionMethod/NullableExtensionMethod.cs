using System;

namespace Mark.Common
{
	public static class NullableExtensionMethod
	{
		/// <summary>
		/// 可空decimal型格式化输出
		/// </summary>
		/// <param name="d"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ToString(this decimal? d, string format)
		{
			if (d.HasValue)
				return d.Value.ToString(format);
			return string.Empty;
		}

		/// <summary>
		/// 将Nullable<System.Decimal/>值舍入到指定小数位数
		/// </summary>
		/// <param name="d">可空的decimal</param>
		/// <param name="decimals"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException">decimals的值只能在0~28之间</exception>
		public static decimal? Round(this decimal? d, int decimals)
		{
			if (d.HasValue)
				return decimal.Round(d.Value, decimals);
			return null;
		}

		/// <summary>
		/// 可空double型格式化输出
		/// </summary>
		/// <param name="d"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ToString(this double? d, string format)
		{
			if (d.HasValue)
				return d.Value.ToString(format);
			return string.Empty;
		}

		/// <summary>
		/// 可空float型格式化输出
		/// </summary>
		/// <param name="f"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ToString(this float? f, string format)
		{
			if (f.HasValue)
				return f.Value.ToString(format);
			return string.Empty;
		}

		/// <summary>
		/// 可空int型格式化输出
		/// </summary>
		/// <param name="i"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ToString(this int? i, string format)
		{
			if (i.HasValue)
				return i.Value.ToString(format);
			return string.Empty;
		}

		/// <summary>
		/// 可空long型格式化输出
		/// </summary>
		/// <param name="l"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ToString(this long? l, string format)
		{
			if (l.HasValue)
				return l.Value.ToString(format);
			return string.Empty;
		}

		/// <summary>
		/// 可空DateTime型格式化输出
		/// </summary>
		/// <param name="dt"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string ToString(this DateTime? dt, string format)
		{
			if (dt.HasValue)
				return dt.Value.ToString(format);
			return string.Empty;
		}
	}
}
