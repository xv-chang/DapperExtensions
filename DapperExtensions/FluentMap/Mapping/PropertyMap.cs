using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace DapperExtensions.FluentMap.Mapping
{
    public class PropertyMap
    {
        public PropertyMap(PropertyInfo propertyInfo)
        {
            PropertyInfo = propertyInfo;
        }
        public string ColumnName { get; private set; }

        /// <summary>
        /// 忽略
        /// </summary>
        public bool Ignored { get; private set; }
        /// <summary>
        /// 主键
        /// </summary>
        public bool Key { get; private set; }
        /// <summary>
        /// 自增
        /// </summary>
        public bool Identity { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }

        public PropertyMap Column(string name)
        {
            ColumnName = name;
            return this;
        }
        public PropertyMap Ignore()
        {
            Ignored = true;
            return this;
        }
        public PropertyMap IsKey()
        {
            Key = true;
            return this;
        }
        public PropertyMap IsIdentity()
        {
            Identity = true;
            return this;
        }
    }
}
