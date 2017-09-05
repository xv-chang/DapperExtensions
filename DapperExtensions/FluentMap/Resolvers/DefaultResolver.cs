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
            IEntityMap entityMap;
            if (MapConfig.EntityMaps.TryGetValue(type, out entityMap))
            {
                if (entityMap != null)
                {
                    return entityMap.TableName;
                }
            }
            throw new Exception($"没有注册配置类{type.Name}的表名");
        }
        public static IEnumerable<PropertyInfo> ResolveProperties(Type type, bool filterIdentity = false)
        {
            IEntityMap entityMap;
            if (MapConfig.EntityMaps.TryGetValue(type, out entityMap))
            {
                foreach (var property in type.GetProperties())
                {
                    var propertyMap = entityMap.PropertyMaps.FirstOrDefault(p => p.PropertyInfo.Name == property.Name);
                    if (propertyMap != null && !propertyMap.Ignored && (!filterIdentity || !propertyMap.Identity))
                    {
                        yield return property;
                    }
                }
            }
            throw new Exception($"没有注册配置类{type.Name}");
        }
        public static PropertyInfo ResolveKeyProperty(Type type, out bool isIdentity)
        {
            IEntityMap entityMap;
            if (MapConfig.EntityMaps.TryGetValue(type, out entityMap))
            {
                var propertyMap = entityMap.PropertyMaps.FirstOrDefault(p => p.Key);
                isIdentity = propertyMap.Identity;
                return propertyMap.PropertyInfo;
            }
            throw new Exception($"没有找到{type.Name}的主键");
        }
        public static string ResolveColumnName(PropertyInfo propertyInfo)
        {
            if (propertyInfo.DeclaringType != null)
            {
                IEntityMap entityMap;
                if (MapConfig.EntityMaps.TryGetValue(propertyInfo.DeclaringType, out entityMap))
                {
                    if (entityMap != null)
                    {
                        var propertyMap = entityMap.PropertyMaps.FirstOrDefault(m => m.PropertyInfo.Name == propertyInfo.Name);
                        return propertyMap.ColumnName;
                    }
                }
            }
            throw new Exception($"没有配置列名:{propertyInfo.Name}");
        }
        public static IEnumerable<string> ResolveColumnNames(Type type, IEnumerable<PropertyInfo> propertyInfos)
        {
            IEntityMap entityMap;
            if (MapConfig.EntityMaps.TryGetValue(type, out entityMap))
            {
                foreach (var property in propertyInfos)
                {
                    var propertyMap = entityMap.PropertyMaps.FirstOrDefault(p => p.PropertyInfo.Name == property.Name);
                    if (propertyMap != null && !propertyMap.Ignored)
                    {
                        yield return propertyMap.ColumnName;
                    }
                }
            }
            throw new Exception($"没有注册配置类{type.Name}");
        }
        public static IEnumerable<string> ResolveColumnNames(Type type, bool filterIdentity = false)
        {
            IEntityMap entityMap;
            if (MapConfig.EntityMaps.TryGetValue(type, out entityMap))
            {
                foreach (var property in type.GetProperties())
                {
                    var propertyMap = entityMap.PropertyMaps.FirstOrDefault(p => p.PropertyInfo.Name == property.Name);
                    if (propertyMap != null && !propertyMap.Ignored && (!filterIdentity || !propertyMap.Identity))
                    {
                        yield return propertyMap.ColumnName;
                    }
                }
            }
            throw new Exception($"没有注册配置类{type.Name}");
        }
    }
}
