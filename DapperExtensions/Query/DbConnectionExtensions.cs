using System.Data;

namespace DapperExtensions.Query
{
    public static class DbConnectionExtensions
    {
        public static QueryOver<T> CreateQuery<T>(this IDbConnection conn)
        {
            return new QueryOver<T>(conn);
        }
        public static SQLQuery<T> CreateSQL<T>(this IDbConnection conn, string sql)
        {
            return new SQLQuery<T>(conn, sql);
        }
    }
}
