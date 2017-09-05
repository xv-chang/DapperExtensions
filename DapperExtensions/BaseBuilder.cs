using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Data;
using DapperExtensions.FluentMap.Resolvers;
using Dapper;

namespace DapperExtensions
{
    public class BaseBuilder<T>
    {
        public IDbConnection _conn;
        public Type type = typeof(T);
        public SqlTranslator<T> _sqlTranslator;
        public string TableName { set; get; }

        public BaseBuilder(IDbConnection conn)
        {
            _conn = conn;
            _sqlTranslator = new SqlTranslator<T>();
        }
        public void SetParamter(string key,object val)
        {
             _sqlTranslator.Params.Add(key, val);
        }
        public string GetNewParamter()
        {
            return _sqlTranslator.PIndex;
        }
        public DynamicParameters GetParamters()
        {
            DynamicParameters parameters=new DynamicParameters();
            foreach (var item in _sqlTranslator.Params)
            {
                parameters.Add(item.Key, item.Value);
            }
            return parameters;
        }
        public virtual int Execute()
        {
            throw new NotImplementedException();
        }

     

    }

}
