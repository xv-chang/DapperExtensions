using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using DapperExtensions.FluentMap.Resolvers;
using Dapper;

namespace DapperExtensions
{
    public class InsertBuilder<T> : BaseBuilder<T>
    {
        private StringBuilder FieldBuilder = new StringBuilder();
        private StringBuilder ValueBuilder = new StringBuilder();
        public InsertBuilder(IDbConnection conn, T entity) : base(conn)
        {
            var properties = DefaultResolver.ResolveProperties(type, true);
            foreach (var p in properties)
            {
                var fieldName = DefaultResolver.ResolveColumnName(p);
                FieldBuilder.Append(FieldBuilder.Length > 0 ? $",{fieldName}" : fieldName);
                var paramterName = GetNewParamter();
                SetParamter(paramterName, p.GetValue(entity, null));
                ValueBuilder.Append(ValueBuilder.Length > 0 ? $",{paramterName}" : paramterName);
            }
        }
        public override int Execute()
        {
            var sql = $"INSERT {TableName}({FieldBuilder.ToString()})VALUES({ValueBuilder.ToString()})";
            return _conn.Execute(sql, GetParamters());
        }

    }
}
