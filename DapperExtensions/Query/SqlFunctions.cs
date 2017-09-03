using System.Collections.Generic;

namespace DapperExtensions.Query
{
    public static class SqlFunctions
    {
        public static bool Like(this string str, string value, MatchMode matchMode = MatchMode.Any)
        {
            return true;
        }
        public static bool In<T>(this T obj, IEnumerable<T> array)
        {
            return true;
        }
        public static bool NotIn<T>(this T obj, IEnumerable<T> array)
        {
            return true;
        }
    }
}
