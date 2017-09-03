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
        private SqlTranslator _sqlTranslator;
        public DeleteBuilder(IDbConnection conn)
        {
            _conn = conn;
            _sqlTranslator = new SqlTranslator();
        }
        public DeleteBuilder<T> Where(Expression<Func<T, object>> expression)
        {
            var sql= _sqlTranslator.VisitLambda(expression);
            return this;
        }

        public int Execute()
        {
            throw new NotImplementedException();
        }

    }
}
