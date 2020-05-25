using System;

namespace Mark.Common
{
	/// <summary>
	/// 枚举Text属性
	/// </summary>
	public class EnumStringAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="value"></param>
		public EnumStringAttribute(string value)
		{
			Value = value;
		}

		/// <summary>
		/// 值
		/// </summary>
		public string Value { get; protected set; }
	}

	/// <summary>
	/// 枚举Text属性
	/// </summary>
	public class EnumTextAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="value"></param>
		public EnumTextAttribute(string value)
		{
			Value = value;
		}

		/// <summary>
		/// 值
		/// </summary>
		public string Value { get; private set; }
	}

	/// <summary>
	/// 枚举Disabled属性
	/// </summary>
	public class EnumDisabledAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="value"></param>
		public EnumDisabledAttribute(bool value)
		{
			Value = value;
		}

		/// <summary>
		/// 值
		/// </summary>
		public bool Value { get; private set; }
	}

	/// <summary>
	/// 枚举Index属性
	/// </summary>
	public class EnumIndexAttribute : Attribute
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="value"></param>
		public EnumIndexAttribute(string value)
		{
			Value = value;
		}

		/// <summary>
		/// 值
		/// </summary>
		public string Value { get; private set; }
	}
}
