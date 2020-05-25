using System;
using System.Linq;

namespace Mark.Common
{
	/// <summary>
	/// Enum扩展方法类
	/// </summary>
	public static class EnumExtensionMethod
	{
		/// <summary>
		/// 取某个枚举对象的字符型Attribute
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetStringAttribute<T>(this Enum e) where T : EnumStringAttribute
		{
			if (e == null)
				return string.Empty;

			var attributes = (T[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(T), false);
			if (attributes.Count() > 0)
				return attributes.First().Value;

			return string.Empty;
		}

		/// <summary>
		/// 取某个对象的EnumText属性，如果没有设置EnumTextAttribute，则取枚举类的Name。
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetText(this Enum e)
		{
			if (e == null)
				return string.Empty;

			var attributes = (EnumTextAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(EnumTextAttribute), false);
			if (attributes.Count() > 0)
				return attributes.First().Value;

			return e.ToString();
		}

		/// <summary>
		/// 取某个枚举值。
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static object GetValue(this Enum e)
		{
			if (e == null)
				return null;

			//return e.GetType().InvokeMember(e.ToString(), BindingFlags.GetField, null, null, null).ToString();
			return e.GetType().GetField(e.ToString()).GetRawConstantValue();
		}

		/// <summary>
		/// 取某个对象的EnumIndex属性，如果没有设置EnumIndexAttribute，则取Value属性。
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static string GetIndex(this Enum e)
		{
			if (e == null)
				return string.Empty;

			var attributes = (EnumIndexAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(EnumIndexAttribute), false);
			if (attributes.Count() > 0)
				return attributes.First().Value;

			return e.GetValue().ToString();
		}

		/// <summary>
		/// 取某个对象的EnumIndex属性，如果没有设置EnumIsDisabledAttribute，返回False。
		/// </summary>
		/// <param name="e"></param>
		/// <returns></returns>
		public static bool IsDisabled(this Enum e)
		{
			if (e == null)
				return false;

			var attributes = (EnumDisabledAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(EnumDisabledAttribute), false);
			if (attributes.Count() > 0)
				return attributes.First().Value;

			return false;
		}
	}
}
