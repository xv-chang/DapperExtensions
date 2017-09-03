using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Linq.Expressions;

namespace DapperExtensions
{
    public static class Extensions
    {
        public static UpdateBuilder<T> Update<T>(this IDbConnection conn)
        {
            return new UpdateBuilder<T>(conn);
        }
        public static DeleteBuilder<T> Delete<T>(this IDbConnection conn)
        {
            return new DeleteBuilder<T>(conn);
        }
        public static InsertBuilder<T> Insert<T>(this IDbConnection conn,T entity)
        {
            return new InsertBuilder<T>(conn,entity);
        }

    }
}
