using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DapperExtensions.FluentMap.Mapping
{
    public class ReferenceMap
    {
        public ReferenceMap(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// 属性
        /// </summary>
        public PropertyInfo PropertyInfo { get; private set; }
        /// <summary>
        /// 要关联的表
        /// </summary>
        public string TableName { set; private get; }
        /// <summary>
        /// 当前表的外键
        /// </summary>
        public string ColumnName { get; private set; }

        public ReferenceMap Table(string name)
        {
            TableName = name;
            return this;
        }
        public ReferenceMap Column(string name)
        {
            ColumnName = name;
            return this;
        }

    }
}
