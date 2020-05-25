using System;
using System.Collections.Generic;

namespace Mark.Common
{
	/// <summary>
	/// 泛型枚举静态帮助类
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class Enum<T> where T : struct
	{
		/// <summary>
		/// 将枚举转换成IEnumerable&lt;aT&gt;   
		/// </summary>
		/// <returns></returns>
		/// <exception cref="NotSupportedException"></exception>
		public static IEnumerable<T> AsEnumerable()
		{
			Type enumType = typeof(T);

			if (!enumType.IsEnum)
				throw new NotSupportedException(string.Format("{0}必须为枚举类型。", enumType));

			EnumQuery<T> query = new EnumQuery<T>();
			return query;
		}
		
	}

	class EnumQuery<T> : IEnumerable<T>
	{
		private List<T> list;

		#region IEnumerable<T> Members

		public IEnumerator<T> GetEnumerator()
		{
			Array values = Enum.GetValues(typeof(T));
			list = new List<T>(values.Length);
			foreach (var value in values)
			{
				list.Add((T)value);
			}

			return list.GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
