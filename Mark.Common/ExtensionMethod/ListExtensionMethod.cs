using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mark.Common
{
	public static class ListExtensionMethod
	{
		/// <summary>
		/// 串联IList&lt;T&gt;成字符串
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="separator">分隔符</param>
		/// <param name="wrap">是否在字符串两端添加分隔符</param>
		/// <param name="includeEmpty">是否包含空字符串</param>
		/// <returns></returns>
		public static string JoinToString<T>(this IEnumerable<T> list, string separator, bool wrap = false, bool includeEmpty = false)
		{
			if (list == null || list.Count() == 0)
				return string.Empty;

			StringBuilder sb = new StringBuilder();
			if (wrap)
			{
				foreach (T t in list)
				{
					string s = string.Empty;
					if (t != null)
						s = t.ToString();
					if (includeEmpty || !string.IsNullOrEmpty(s))
						sb.Append(s + separator);
				}
				if (sb.Length > 0)
					sb.Insert(0, separator);
			}
			else
			{
				foreach (T t in list)
				{
					string s = string.Empty;
					if (t != null)
						s = t.ToString();

					if (includeEmpty || !string.IsNullOrEmpty(s))
						sb.Append(separator + s);
				}
				if (sb.Length > 0)
					sb.Remove(0, separator.Length);
			}
			return sb.ToString();
		}

		/// <summary>
		/// 转换string集合为int集合
		/// </summary>
		/// <param name="list"></param>
		/// <returns></returns>
		public static IList<int> ToInt32List(this IList<string> list)
		{
			IList<int> list_int = new List<int>();

			if (list == null || list.Count() == 0)
				return list_int;

			foreach (string str in list)
			{
				int i;
				if (int.TryParse(str, out i))
					list_int.Add(i);
			}

			return list_int;
		}

		/// <summary>
		/// 追加一个List，不创建新的List
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="appendList"></param>
		public static void Append<T>(this IList<T> list, IList<T> appendList)
		{
			foreach (T t in appendList)
				list.Add(t);
		}

		/// <summary>
		/// 添加项，仅在IList中没有此项时
		/// </summary>
		/// <param name="list"></param>
		/// <param name="value"></param>
		public static void AddWhenNotExisted<T>(this IList<T> list, T value)
		{
			if (!list.Contains(value))
				list.Add(value);
		}

		/// <summary>
		/// 复制集合，引用类型只是新建集合后把原集合中对象添加进新集合，并未复制对象
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <returns></returns>
		public static IList<T> Clone<T>(this IList<T> list)
		{
			IList<T> newList = new List<T>();
			if (list != null)
			{
				foreach (T t in list)
					newList.Add(t);
			}

			return newList;
		}
		
		/// <summary>
		/// IList集合有没有内容
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsNullOrNoItems<T>(this IList<T> list)
		{
			if (list == null || list.Count <= 0)
				return true;
			return false;
		}
	}
}
