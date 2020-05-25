using System;

namespace Mark.Common
{/// <summary>
 /// 安全的null延迟赋值操作扩展方法
 /// </summary>
    public static class NullExtensionMethod
	{
		/// <summary>
		/// 安全的null延迟赋值操作，如果调用对象为null返回default()
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="t"></param>
		/// <param name="fn"></param>
		/// <returns></returns>
		public static U IfNotNull<T, U>(this T t, Func<T, U> fn)
		{
			return t != null ? fn(t) : default(U);
		}


		/// <summary>
		/// 安全的null延迟赋值操作，如果调用对象为null返回指定值
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="U"></typeparam>
		/// <param name="t"></param>
		/// <param name="fn"></param>
		/// <param name="customValue"></param>
		/// <returns></returns>
		public static U IfNotNull<T, U>(this T t, Func<T, U> fn, U customValue)
		{
			return t != null ? fn(t) : customValue;
		}
	}
}
