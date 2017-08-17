using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Data;

namespace DapperExtensions
{
    public class DeleteBuilder<T>
    {
        private IDbConnection _conn;
        private string _whereSQL;
        public DeleteBuilder(IDbConnection conn)
        {
            _conn = conn;
        }
        public DeleteBuilder<T> Where(Expression<Func<T, object>> expression)
        {
            return this;
        }

        public int Execute()
        {
            throw new NotImplementedException();
        }

    }
}
