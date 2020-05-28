using System;
using System.Collections.Generic;
using System.Linq;
using Demo.User.Domain.Model.Dto;
using Mark.Common.Services;

namespace Demo.User.API.Services
{
    public class PropertyMappingService : IPropertyMappingService
    {
        private readonly Dictionary<string, PropertyMappingValue> _userPropertyMapping =
            new Dictionary<string, PropertyMappingValue>(StringComparer.OrdinalIgnoreCase)
            {
                {nameof(UserDto.Id), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.Id)}) },
                {nameof(UserDto.Gender), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.Gender)}) },
                {nameof(UserDto.No), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.No)}) },
                {nameof(UserDto.Level), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.Level) }) },
                {nameof(UserDto.Name), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.Name)}) },
                {nameof(UserDto.TelNumber), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.TelNumber)}) },
                {nameof(UserDto.IsVip), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.VipFlag)}) },
                {nameof(UserDto.Age), new PropertyMappingValue(new List<string>{nameof(Domain.Aggregate.User.BirthDay)}, true) },
                
            };

        private readonly IList<IPropertyMapping> _propertyMappings = new List<IPropertyMapping>();

        public PropertyMappingService()
        {
            _propertyMappings.Add(new PropertyMapping<UserDto, Domain.Aggregate.User>(_userPropertyMapping));
        }

        public Dictionary<string, PropertyMappingValue> GetPropertyMapping<TSource, TDestination>()
        {
            var matchingMapping = _propertyMappings.OfType<PropertyMapping<TSource, TDestination>>();
            var propertyMappings = matchingMapping.ToList();
            if (propertyMappings.Count == 1)
            {
                return propertyMappings.First().MappingDictionary;
            }
            throw new Exception($"无法找到唯一的映射关系：{typeof(TSource)}, {typeof(TDestination)}");
        }

        /// <summary>
        /// 判断传入的orderby是否合法
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDestination"></typeparam>
        /// <param name="fields">【例】：传入格式如“orderby age ”</param>
        /// <returns></returns>
        public bool ValidMappingExistsFor<TSource, TDestination>(string fields)
        {
            if (string.IsNullOrWhiteSpace(fields))
            {
                return true;
            }

            var propertyMapping = GetPropertyMapping<TSource, TDestination>();

            var fieldAfterSplit = fields.Split(",");

            if (fieldAfterSplit == null) return false;
            foreach (var field in fieldAfterSplit)
            {
                var trimmedField = field.Trim();
                var indexOfFirstSpace = trimmedField.IndexOf(" ", StringComparison.Ordinal);
                var propertyName = indexOfFirstSpace == -1
                    ? trimmedField
                    : trimmedField.Remove(indexOfFirstSpace);

                if (!propertyMapping.ContainsKey(propertyName))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
