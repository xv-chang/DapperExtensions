using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Data;
using DapperExtensions.FluentMap.Resolvers;
using Dapper;

namespace DapperExtensions
{
    public class DeleteBuilder<T> : BaseBuilder<T>
    {

        private StringBuilder WhereBuilder { set; get; }
        public DeleteBuilder(IDbConnection conn) : base(conn)
        {

        }
        public DeleteBuilder<T> Where(Expression<Func<T, bool>> expression)
        {
            var sql = _sqlTranslator.VisitExpression(expression);
            WhereBuilder.Append(WhereBuilder.Length > 0 ? $" AND {sql}" : $" WHERE {sql}");
            return this;
        }
        public override int Execute()
        {
            var sql = $"DELETE FROM {TableName}{WhereBuilder.ToString()}";
            return _conn.Execute(sql, GetParamters());
        }

    }
}
