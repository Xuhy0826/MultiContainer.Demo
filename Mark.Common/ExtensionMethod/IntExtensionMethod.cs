using System;
using System.Collections.Generic;
using System.Text;

namespace Mark.Common.ExtensionMethod
{
    public static class IntExtensionMethod
    {
        /// <summary>
        /// 将int型转成枚举类
        /// </summary>
        /// <typeparam name="TEnum"></typeparam>
        /// <param name="intervalTypeVal"></param>
        /// <returns></returns>
        public static TEnum? ToEnum<TEnum>(this int intervalTypeVal) where TEnum : struct
        {
            try
            {
                var enumValue = (TEnum)System.Enum.ToObject(typeof(TEnum), intervalTypeVal);
                return enumValue;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
