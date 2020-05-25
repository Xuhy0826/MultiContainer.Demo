using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Mark.Common
{
	public static class StringExtensionMethod
	{
		/// <summary>
		/// 替换空字符串
		/// </summary>
		/// <param name="str">原始字符串</param>
		/// <param name="strReplace">替换字符串</param>
		/// <returns></returns>
		public static string IfNullOrEmpty(this string str, string strReplace)
		{
			if (string.IsNullOrEmpty(str))
				return strReplace;
			return str;
		}

		/// <summary>
		/// 将字符串转化成SQL语句的LIKE查询条件%str%
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string ToSQLLikeQuery(this string str)
		{
			if (!str.StartsWith("%"))
				str = "%" + str;
			if (!str.EndsWith("%"))
				str = str + "%";
			return str;
		}

		/// <summary>
		/// MD5加密
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string Md5Encryt(this string str)
		{
			StringBuilder sb = new StringBuilder();
			using (MD5 md5 = MD5.Create())
			{
				byte[] buffers = System.Text.Encoding.UTF8.GetBytes(str + "Y0826_smartspyeye");
				byte[] bytes = md5.ComputeHash(buffers);
				md5.Clear();
				for (int i = 0; i < bytes.Length; i++)
				{
					sb.Append(bytes[i].ToString("x2"));
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// SQL语句动态拼WHERE关键字
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string AppendWhereWhenNull(this string str)
		{
            if (str == "")
                return str;
			int index = str.IndexOf("WHERE");
			if (index < 0)
				return "WHERE " + str;
			return str;
		}


        #region 字符串拆分
        /// <summary>
        /// 从头截取文本，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="s"></param>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetSubString(this string str, int length)
        {
            string temp = str;
            int j = 0;
            int k = 0;
            for (int i = 0; i < temp.Length; i++)
            {
                if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
                {
                    j += 2;
                }
                else
                {
                    j += 1;
                }
                if (j <= length)
                {
                    k += 1;
                }
                if (j > length)
                {
                    return temp.Substring(0, k) + "..";
                }
            }
            return temp;
        }

        /// <summary>
        /// 从末尾截取文本，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="s"></param>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetEndSubString(this string str, int length)
        {
            int j = 0;
            int k = 0;
            string temp = str;
            StringBuilder sb = new StringBuilder();

            for (int i = str.Length; i > str.Length - length - 1; i--)
            {
                if (Regex.IsMatch(temp.Substring(i - 1, 1), @"[\u4e00-\u9fa5]+"))
                {
                    j += 2;
                }
                else
                {
                    j += 1;
                }
                if (j <= length)
                {
                    k += 1;
                }
                if (j > length)
                {
                    return temp.Substring(str.Length - k, k);
                }
            }
            return temp;
        }


        /// <summary>
        /// 将字符串按分隔符拆分成一组数字
        /// </summary>
        /// <param name="str"></param>
        /// <param name="separator">分隔符</param>
        /// <param name="allowRepeat">是否允许重复值</param>
        /// <returns></returns>
        public static IList<int> SplitInt(this string str, string separator, bool allowRepeat = false)
		{
			IList<int> list = new List<int>();
			if (!string.IsNullOrEmpty(str))
			{
				string[] ss = Regex.Split(str, separator);
				foreach (string s in ss)
				{
					if (string.IsNullOrEmpty(s.Trim()))
						continue;

					int i;
					if (!int.TryParse(s.Trim(), out i))
						continue;

					if (!list.Contains(i))
						list.Add(i);
					else
					{
						if (allowRepeat)
							list.Add(i);
					}
				}
			}
			return list;
		}

		/// <summary>
		/// 将字符串按分隔符拆分
		/// </summary>
		/// <param name="str"></param>
		/// <param name="separator">分隔符</param>
		/// <param name="includeEmpty">是否包含空字符串</param>
		/// <param name="trim">是否清除两端空格</param>
		/// <param name="allowRepeat">是否允许重复值</param>
		/// <returns></returns>
		public static IList<string> SplitString(this string str, string separator, bool includeEmpty = false, bool trim = true, bool allowRepeat = false)
		{
			IList<string> list = new List<string>();
			if (!string.IsNullOrEmpty(str))
			{
				string[] ss = Regex.Split(str, separator);
				foreach (string s in ss)
				{
					if (!includeEmpty && string.IsNullOrEmpty(s.Trim()))
						continue;

					string temp = s;
					if (trim)
						temp = s.Trim();

					if (!list.Contains(temp))
						list.Add(temp);
					else
					{
						if (allowRepeat)
							list.Add(temp);
					}
				}
			}
			return list;
		}
        /// <summary>
        /// 移除字符串末尾指定字符
        /// </summary>
        /// <param name="str">需要移除的字符串</param>
        /// <param name="value">指定字符</param>
        /// <returns>移除后的字符串</returns>
        public static string RemoveLastChar(this string str, string value)
        {
            int finder = str.LastIndexOf(value, StringComparison.Ordinal);
            if (finder != -1)
            {
                return str.Substring(0, finder);
            }
            return str;
        }
        #endregion

        #region 解析字符串为int、decimal、DateTime、Enum

        /// <summary>
        /// 解析字符串为Guid?
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Guid? ToGuid(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;
			Guid v;
			if (Guid.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为Guid,如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static Guid ToGuidReq(this string t)
		{
			Guid? guid = t.ToGuid();
			if (!guid.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为GUid类型。", t));
			return guid.Value;
		}

		/// <summary>
		/// 解析字符串为Int32?
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static int? ToInt32(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;

			int v;
			if (int.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为Int32，如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static int ToInt32Req(this string t)
		{
			int? i = t.ToInt32();
			if (!i.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为Int32类型。", t));
			return i.Value;
		}

		/// <summary>
		/// 解析字符串为Int32
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static int ToInt32OrDefault(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return default(int);

			int v;
			if (int.TryParse(t.Trim(), out v))
				return v;
			return default(int);
		}

		/// <summary>
		/// 解析字符串为Int32
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static int ToInt32OrDefault(this string t, int defaultValue)
		{
			if (string.IsNullOrEmpty(t))
				return defaultValue;

			int v;
			if (int.TryParse(t.Trim(), out v))
				return v;
			return defaultValue;
		}

        /// <summary>
		/// 解析字符串为Int64?
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static long? ToInt64(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;

			long v;
			if (long.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为Int64，如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static long ToInt64Req(this string t)
		{
			long? l = t.ToInt64();
			if (!l.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为Int64类型。", t));
			return l.Value;
		}

		/// <summary>
		/// 解析字符串为Int64
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static long ToInt64OrDefault(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return default(long);

			long v;
			if (long.TryParse(t.Trim(), out v))
				return v;
			return default(long);
		}

		/// <summary>
		/// 解析字符串为Int64
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static long ToInt64OrDefault(this string t, long defaultValue)
		{
			if (string.IsNullOrEmpty(t))
				return defaultValue;

			long v;
			if (long.TryParse(t.Trim(), out v))
				return v;
			return defaultValue;
		}


		/// <summary>
		/// 解析字符串为decimal?
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static decimal? ToDecimal(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;

			decimal v;
			if (decimal.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为decimal，如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static decimal ToDecimalReq(this string t)
		{
			decimal? d = t.ToDecimal();
			if (!d.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为decimal类型。", t));
			return d.Value;
		}

		/// <summary>
		/// 解析字符串为decimal
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static decimal ToDecimalOrDefault(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return default(decimal);

			decimal v;
			if (decimal.TryParse(t.Trim(), out v))
				return v;
			return default(decimal);
		}

		/// <summary>
		/// 解析字符串为decimal
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static decimal ToDecimalOrDefault(this string t, decimal defaultValue)
		{
			if (string.IsNullOrEmpty(t))
				return defaultValue;

			decimal v;
			if (decimal.TryParse(t.Trim(), out v))
				return v;
			return defaultValue;
		}

		/// <summary>
		/// 解析字符串为double?
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static double? ToDouble(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;

			double v;
			if (double.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为double，如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static double ToDoubleReq(this string t)
		{
			double? d = t.ToDouble();
			if (!d.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为double类型。", t));
			return d.Value;
		}

		/// <summary>
		/// 解析字符串为double
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static double ToDoubleOrDefault(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return default(double);

			double v;
			if (double.TryParse(t.Trim(), out v))
				return v;
			return default(double);
		}

		/// <summary>
		/// 解析字符串为double
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static double ToDoubleOrDefault(this string t, double defaultValue)
		{
			if (string.IsNullOrEmpty(t))
				return defaultValue;

			double v;
			if (double.TryParse(t.Trim(), out v))
				return v;
			return defaultValue;
		}

		/// <summary>
		/// 解析字符串为float?
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float? ToFloat(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;

			float v;
			if (float.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为float，如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static float ToFloatReq(this string t)
		{
			float? f = t.ToFloat();
			if (!f.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为float类型。", t));
			return f.Value;
		}

		/// <summary>
		/// 解析字符串为float
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static float ToFloatOrDefault(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return default(float);

			float v;
			if (float.TryParse(t.Trim(), out v))
				return v;
			return default(float);
		}

		/// <summary>
		/// 解析字符串为float
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static float ToFloatOrDefault(this string t, float defaultValue)
		{
			if (string.IsNullOrEmpty(t))
				return defaultValue;

			float v;
			if (float.TryParse(t.Trim(), out v))
				return v;
			return defaultValue;
		}

		/// <summary>
		/// 解析字符串为DateTime?
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static DateTime? ToDateTime(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;

			DateTime v;
			if (DateTime.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为DateTime，如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static DateTime ToDateTimeReq(this string t)
		{
			DateTime? d = t.ToDateTime();
			if (!d.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为DateTime类型。", t));
			return d.Value;
		}

		/// <summary>
		/// 解析字符串为DateTime
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static DateTime ToDateTimeOrDefault(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return default(DateTime);

			DateTime v;
			if (DateTime.TryParse(t.Trim(), out v))
				return v;
			return default(DateTime);
		}

		/// <summary>
		/// 解析字符串为DateTime
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static DateTime ToDateTimeOrDefault(this string t, DateTime defaultValue)
		{
			if (string.IsNullOrEmpty(t))
				return defaultValue;

			DateTime v;
			if (DateTime.TryParse(t.Trim(), out v))
				return v;
			return defaultValue;
		}

		/// <summary>
		/// 解析字符串为bool?
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool? ToBool(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return null;

			bool v;
			if (bool.TryParse(t.Trim(), out v))
				return v;
			return null;
		}

		/// <summary>
		/// 解析字符串为bool，如无法解析抛出异常
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static bool ToBoolReq(this string t)
		{
			bool? b = t.ToBool();
			if (!b.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为bool类型。", t));
			return b.Value;
		}

		/// <summary>
		/// 解析字符串为bool
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static bool ToBoolOrDefault(this string t)
		{
			if (string.IsNullOrEmpty(t))
				return default(bool);

			bool v;
			if (bool.TryParse(t.Trim(), out v))
				return v;
			return default(bool);
		}

		/// <summary>
		/// 解析字符串为bool
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static bool ToBoolOrDefault(this string t, bool defaultValue)
		{
			if (string.IsNullOrEmpty(t))
				return defaultValue;

			bool v;
			if (bool.TryParse(t.Trim(), out v))
				return v;
			return defaultValue;
		}


		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，不在枚举类定义的值无法解析
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static T? ToEnum<T>(this string t) where T : struct
		{
			if (!typeof(T).IsEnum)
				throw new NotSupportedException(string.Format("{0}必须为枚举类型。", typeof(T)));

			//使用Enum.Parse时，如果解析一个未在枚举中定义的值，如8这个值
			//这时虽然Enum.IsDefined(type, 8)和Enum.IsDefined(type, "8")都返回false
			//但是使用Enum.Parse却可以解析出值8，用IsDefined可以限定值有效

			if (string.IsNullOrEmpty(t))
				return null;

			//if (Enum.IsDefined(typeof(T), t))
			//    return (T)Enum.Parse(typeof(T), t);
			//return null;

			//try { return (T)Enum.Parse(typeof(T), t); }
			//catch { }

			foreach (T value in Enum.GetValues(typeof(T)))
			{
				//Name相等或值相等
				if (value.ToString() == t.Trim()
					|| typeof(T).GetField(value.ToString()).GetRawConstantValue().ToString() == t.Trim())
					return value;
			}
			return null;
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，未在枚举类定义的值无法解析，如无法解析抛出异常
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static T ToEnumReq<T>(this string t) where T : struct
		{
			T? e = t.ToEnum<T>();
			if (!e.HasValue)
				throw new ArgumentException(string.Format("无法将值“{0}”解析为“{1}”枚举类型。", t, typeof(T)));
			return e.Value;
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，未在枚举类定义的值无法解析
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		public static T ToEnumOrDefault<T>(this string t) where T : struct
		{
			return t.ToEnumOrDefault(default(T));
		}

		/// <summary>
		/// 解析字符串为Enum，与Enum.Parse不同，未在枚举类定义的值无法解析
		/// </summary>
		/// <param name="t"></param>
		/// <param name="defaultValue"></param>
		/// <returns></returns>
		public static T ToEnumOrDefault<T>(this string t, T defaultValue) where T : struct
		{
			T? e = t.ToEnum<T>();

			if (e.HasValue)
				return e.Value;

			return defaultValue;

			//foreach (T value in Enum.GetValues(typeof(T)))
			//{
			//    if (value.ToString() == t
			//        || typeof(T).GetField(value.ToString()).GetRawConstantValue().ToString() == t)
			//        return value;
			//}
			//return defaultValue;
		}


		/// <summary>
		/// 根据枚举Text解析为Enum，如找不到抛出异常（Text可能重复，此方法返回第一个找到的值）
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="t"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		public static T ToEnumReqByText<T>(this string t) where T : struct
		{
			T? e = t.ToEnum<T>();
			if (!e.HasValue)
				throw new ArgumentException(string.Format("无法找到枚举类型“{1}”文本为“{0}”的值。", t, typeof(T)));
			return e.Value;
		}

		#endregion

		/// <summary>
		/// string.Substring方法的补充，截取字符串前会先判断截取位置及长度是否有效
		/// 不会抛出异常：System.ArgumentOutOfRangeException : Index and length must refer to a location within the string.
		/// </summary>
		/// <param name="str"></param>
		/// <param name="startIndex"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string SmartSubstring(this string str, int startIndex, int length)
		{
			if (string.IsNullOrEmpty(str))
				return string.Empty;

			if (startIndex < 0 || length < 1)
				return string.Empty;

			if (startIndex > str.Length - 1)
				return string.Empty;

			if (startIndex + length > str.Length)
				length = str.Length - startIndex;

			return str.Substring(startIndex, length);
		}

		/// <summary>
		/// 取字符串的字节长度
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static int GetByteLength(this string str)
		{
			int byteLength = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (Convert.ToInt32(str[i]) > 255)
					byteLength = byteLength + 2;
				else
					byteLength++;
			}
			return byteLength;
		}

		/// <summary> 
		/// 将字符串转为拼音首字母缩写，如有英文字符会保留
		/// </summary> 
		/// <param name="str">字符串</param>
		/// <returns></returns>
		public static string ToPinyinAbbr(this string str)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i <= str.Length - 1; i++)
				sb.Append(GetFirstPinyin(str.Substring(i, 1)));
			return sb.ToString();
		}

		/// <summary>
		/// 取一个汉字的拼音首字母，如果是英文则直接返回大写形式
		/// </summary>
		/// <param name="cnChar">单个汉字</param>
		/// <returns></returns>
		private static string GetFirstPinyin(string cnChar)
		{
			long l;
			byte[] bytes = Encoding.Default.GetBytes(cnChar);

			//如果是字母，则直接返回
			if (bytes.Length == 1)
				return cnChar.ToUpper();
			else
			{
				// get the array of byte from the single char 
				int i1 = (short)(bytes[0]);
				int i2 = (short)(bytes[1]);
				l = i1 * 256 + i2;
			}

			//expresstion
			//table of the constant list
			// 'A'; //45217..45252
			// 'B'; //45253..45760
			// 'C'; //45761..46317
			// 'D'; //46318..46825
			// 'E'; //46826..47009
			// 'F'; //47010..47296
			// 'G'; //47297..47613

			// 'H'; //47614..48118
			// 'J'; //48119..49061
			// 'K'; //49062..49323
			// 'L'; //49324..49895
			// 'M'; //49896..50370
			// 'N'; //50371..50613
			// 'O'; //50614..50621
			// 'P'; //50622..50905
			// 'Q'; //50906..51386

			// 'R'; //51387..51445
			// 'S'; //51446..52217
			// 'T'; //52218..52697
			//没有U,V
			// 'W'; //52698..52979
			// 'X'; //52980..53640
			// 'Y'; //53689..54480
			// 'Z'; //54481..55289

			// iCnChar match the constant
			if ((l >= 45217) && (l <= 45252))
				return "A";
			else if ((l >= 45253) && (l <= 45760))
				return "B";
			else if ((l >= 45761) && (l <= 46317))
				return "C";
			else if ((l >= 46318) && (l <= 46825))
				return "D";
			else if ((l >= 46826) && (l <= 47009))
				return "E";
			else if ((l >= 47010) && (l <= 47296))
				return "F";
			else if ((l >= 47297) && (l <= 47613))
				return "G";
			else if ((l >= 47614) && (l <= 48118))
				return "H";
			else if ((l >= 48119) && (l <= 49061))
				return "J";
			else if ((l >= 49062) && (l <= 49323))
				return "K";
			else if ((l >= 49324) && (l <= 49895))
				return "L";
			else if ((l >= 49896) && (l <= 50370))
				return "M";
			else if ((l >= 50371) && (l <= 50613))
				return "N";
			else if ((l >= 50614) && (l <= 50621))
				return "O";
			else if ((l >= 50622) && (l <= 50905))
				return "P";
			else if ((l >= 50906) && (l <= 51386))
				return "Q";
			else if ((l >= 51387) && (l <= 51445))
				return "R";
			else if ((l >= 51446) && (l <= 52217))
				return "S";
			else if ((l >= 52218) && (l <= 52697))
				return "T";
			else if ((l >= 52698) && (l <= 52979))
				return "W";
			else if ((l >= 52980) && (l <= 53640))
				return "X";
			else if ((l >= 53689) && (l <= 54480))
				return "Y";
			else if ((l >= 54481) && (l <= 55289))
				return "Z";
			else return ("?");
		}

        /// <summary>

        /// 将整数转为大写的中文数字

        /// </summary>

        /// <param name="ni_intInput"></param>

        /// <returns></returns>

        public static string ToCNUpperCase(this int ni_intInput)
        {
            string tstrRet = "";
            int tintInput;
            int tintRemainder, tintDigitPosIndex = 0;
            int tintLoopX = 0;
            string[] tastrNumCNChar = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string[] tastrDigitPosCNChar = new string[] { "", "十", "佰", "仟", "万", "亿" };
            tintInput = ni_intInput;
            tintLoopX = 0;
            while (tintInput / 10 > 0 || tintInput > 0)
            {
                tintRemainder = (tintInput % 10);
                if (tintLoopX == 5)//十万
                    tintDigitPosIndex = 1;
                else if (tintLoopX == 8)//亿
                    tintDigitPosIndex = 5;
                else if (tintLoopX == 9)//十亿
                    tintDigitPosIndex = 1;
                //end if
                if (tintRemainder > 0)
                    tstrRet = tastrNumCNChar[tintRemainder] + tastrDigitPosCNChar[tintDigitPosIndex] + tstrRet;
                else
                    tstrRet = tastrNumCNChar[tintRemainder] + tstrRet; ;
                tintDigitPosIndex += 1;
                tintLoopX += 1;
                tintInput /= 10;
            }
            tstrRet = System.Text.RegularExpressions.Regex.Replace(tstrRet, "零零*零*", "零");
            return tstrRet;
        }

        /// <summary>
        /// 将整数转为小写的中文数字
        /// </summary>
        /// <param name="ni_intInput"></param>
        /// <returns></returns>
        public static string ToCNLowerCase(this int ni_intInput)
        {
            string tstrRet = "";
            int tintInput;
            int tintRemainder, tintDigitPosIndex = 0;
            int tintLoopX = 0;
            string[] tastrNumCNChar = new string[] { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string[] tastrDigitPosCNChar = new string[] { "", "十", "百", "千", "万", "亿" };
            tintInput = ni_intInput;
            tintLoopX = 0;
            while (tintInput / 10 > 0 || tintInput > 0)
            {
                tintRemainder = (tintInput % 10);
                if (tintLoopX == 5)//十万
                    tintDigitPosIndex = 1;
                else if (tintLoopX == 8)//亿
                    tintDigitPosIndex = 5;
                else if (tintLoopX == 9)//十亿
                    tintDigitPosIndex = 1;
                if (tintRemainder > 0)
                    tstrRet = tastrNumCNChar[tintRemainder] + tastrDigitPosCNChar[tintDigitPosIndex] + tstrRet;
                else
                    tstrRet = tastrNumCNChar[tintRemainder] + tstrRet; ;
                tintDigitPosIndex += 1;
                tintLoopX += 1;
                tintInput /= 10;
            }
            tstrRet = System.Text.RegularExpressions.Regex.Replace(tstrRet, "零零*零*", "零");
            return tstrRet;
        }
    }
}
