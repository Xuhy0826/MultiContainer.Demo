using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace Mark.Common
{
    public class BindComboxEnumType<T>
    {
        public string Name { get; set; }

        public string TypeValue { get; set; }

        public object ActualValue { get; set; }

        static BindComboxEnumType()
        {
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                if (!field.IsSpecialName)
                    BindComboxEnumType<T>.BindTypes.Add(new BindComboxEnumType<T>()
                    {
                        Name = BindComboxEnumType<T>.GetDescriptionByEnum(field),
                        TypeValue = field.GetRawConstantValue().ToString(),
                        ActualValue = field.GetRawConstantValue()
                    });
            }
        }

        public static List<BindComboxEnumType<T>> BindTypes { get; } = new List<BindComboxEnumType<T>>();

        public static string GetDescriptionByEnum(FieldInfo enumField)
        {
            object[] customAttributes = enumField.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (customAttributes.Length == 0)
                return string.Empty;
            return ((DescriptionAttribute)customAttributes[0]).Description;
        }
    }
}
