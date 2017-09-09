using DapperExtensions.FluentMap.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DapperExtensions.FluentMap.Resolvers
{
    public static class DefaultResolver
    {
        public static string ResolveTableName(Type type)
        {
            var entityMap = GetEntityMap(type);
            return entityMap.TableName;
        }
        public static IEnumerable<PropertyInfo> ResolveProperties(Type type, bool filterIdentity = false)
        {
            var entityMap = GetEntityMap(type);
            foreach (var propertyMap in entityMap.PropertyMaps)
            {
                if (!propertyMap.Ignored && (!filterIdentity || !propertyMap.Identity))
                {
                    yield return propertyMap.PropertyInfo;
                }
            }
        }
        public static PropertyInfo ResolveKeyProperty(Type type, out bool isIdentity)
        {
            var entityMap = GetEntityMap(type);
            var propertyMap = entityMap.PropertyMaps.FirstOrDefault(p => p.Key);
            isIdentity = propertyMap.Identity;
            return propertyMap.PropertyInfo;

        }
        public static string ResolveColumnName(PropertyInfo propertyInfo)
        {
            if (propertyInfo.DeclaringType != null)
            {
                var entityMap = GetEntityMap(propertyInfo.DeclaringType);
                var propertyMap = entityMap.PropertyMaps.FirstOrDefault(m => m.PropertyInfo.Name == propertyInfo.Name);
                return propertyMap.ColumnName;
            }
            throw new Exception($"属性{propertyInfo.Name}没有基类");

        }
        public static IEnumerable<string> ResolveColumnNames(Type type, IEnumerable<PropertyInfo> propertyInfos)
        {
            var entityMap = GetEntityMap(type);
            foreach (var property in propertyInfos)
            {
                var propertyMap = entityMap.PropertyMaps.FirstOrDefault(p => p.PropertyInfo.Name == property.Name);
                if (propertyMap != null && !propertyMap.Ignored)
                {
                    yield return propertyMap.ColumnName;
                }
            }

        }
        public static IEnumerable<string> ResolveColumnNames(Type type, bool filterIdentity = false)
        {
            var entityMap = GetEntityMap(type);
            foreach (var propertyMap in entityMap.PropertyMaps)
            {
                if (!propertyMap.Ignored && (!filterIdentity || !propertyMap.Identity))
                {
                    yield return propertyMap.ColumnName;
                }
            }
        }

        public static IEntityMap GetEntityMap(Type type)
        {
            IEntityMap entityMap;
            if (!MapConfig.EntityMaps.TryGetValue(type, out entityMap))
            {
                throw new Exception($"没有注册配置类{type.Name}");
            }
            return entityMap;
        }
    }
}
