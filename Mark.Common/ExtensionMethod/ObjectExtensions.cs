using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Mark.Common.ExtensionMethod
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// 数据塑性（获取指定的字段的值）
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">要进行塑型的对象</param>
        /// <param name="fields">要获取这个对象中的哪些字段（属性）</param>
        /// <returns></returns>
        public static ExpandoObject ShapeData<TSource>(this TSource source, string fields)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var expandoObj = new ExpandoObject();

            //如果传入的fields是空的，就将属性全部返回
            if (string.IsNullOrWhiteSpace(fields))
            {
                var propertyInfos =
                    typeof(TSource).GetProperties(BindingFlags.IgnoreCase | BindingFlags.Public |
                                                  BindingFlags.Instance);
                foreach (var propertyInfo in propertyInfos)
                {
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)expandoObj).Add(propertyInfo.Name, propertyValue);
                }
            }
            else
            {
                var fieldsAfterSplit = fields.Split(",");
                foreach (var field in fieldsAfterSplit)
                {
                    var propertyName = field.Trim();

                    var propertyInfo = typeof(TSource).GetProperty(propertyName,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    if (propertyInfo == null)
                    {
                        throw new Exception($"在{typeof(TSource)}上没有找到{propertyName}这个属性");
                    }
                    var propertyValue = propertyInfo.GetValue(source);
                    ((IDictionary<string, object>)expandoObj).Add(propertyInfo.Name, propertyValue);
                }
            }

            return expandoObj;
        }
    }
}
