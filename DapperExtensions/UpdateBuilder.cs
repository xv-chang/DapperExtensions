using System;
using System.Collections.Generic;
using System.Text;
using System.Linq.Expressions;
using System.Data;

namespace DapperExtensions
{
    public class UpdateBuilder<T>
    {
        private IDbConnection _conn;
        private string _whereSQL;
        public UpdateBuilder(IDbConnection conn)
        {
            _conn = conn;
        }
        public UpdateBuilder<T> Where(Expression<Func<T, object>> expression)
        {
            return this;
        }
        public UpdateBuilder<T> SetField(Expression<Func<T,object>> expression)
        {
            return this;
        }
        public UpdateBuilder<T> WithValue<TValue>(TValue val)
        {
            return this;
        }
        public int Execute()
        {
            throw new NotImplementedException();
        }

    }
}
