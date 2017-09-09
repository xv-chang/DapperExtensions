using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace DapperExtensions.FluentMap.Mapping
{
    public class EntityMap<T>: IEntityMap
    {
        public string TableName { get; private set; }
        public List<PropertyMap> PropertyMaps { set; get; } = new List<PropertyMap>();
        public List<ReferenceMap> ReferenceMaps { set; get; } = new List<ReferenceMap>();

        public PropertyMap Map<TValue>(Expression<Func<T, TValue>> expression)
        {
            var p = (PropertyInfo)ExpressionHelper.GetMemberInfo(expression);
            var pMap = new PropertyMap(p);
            //TODO 判断是否重复添加
            PropertyMaps.Add(pMap);
            return pMap;
        }

        public PropertyMap Id<TValue>(Expression<Func<T, TValue>> expression)
        {
            return Map(expression).Column("Id").IsKey().IsIdentity();
        }
        public void Table(string name)
        {
            TableName = name;
        }

        public ReferenceMap HasOne(Expression<Func<T, object>> expression)
        {
            var p = (PropertyInfo)ExpressionHelper.GetMemberInfo(expression);
            var rMap = new ReferenceMap(p);
            //TODO 判断是否重复添加
            ReferenceMaps.Add(rMap);
            return rMap;
        }

    }
    public interface IEntityMap
    {
         string TableName { get; }
         List<PropertyMap> PropertyMaps { get; }
         List<ReferenceMap> ReferenceMaps {  get; }
    }
}
