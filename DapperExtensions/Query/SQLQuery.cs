using System.Collections.Generic;
using System.Data;
using Dapper;

namespace DapperExtensions.Query
{
    public class SQLQuery<T>
    {
        private readonly SqlBuilder _sqlBuilder;
        private readonly IDbConnection _conn;

        public SQLQuery(IDbConnection conn, string selectSQL)
        {
            _sqlBuilder = new SqlBuilder();
            _sqlBuilder.AppendSelect(selectSQL);
            _conn = conn;
        }
        public SQLQuery<T> Where(string whereSQL)
        {
            return And(whereSQL);
        }
        public SQLQuery<T> And(string whereSQL)
        {
            _sqlBuilder.AppendAnd(whereSQL);
            return this;
        }
        public SQLQuery<T> Or(string whereSQL)
        {
            _sqlBuilder.AppendOr(whereSQL);
            return this;
        }
        public SQLQuery<T> OrderBy(string orderSql)
        {
            _sqlBuilder.AppendOrder(orderSql);
            return this;
        }
        public SQLQuery<T> Skip(int num)
        {
            _sqlBuilder.SkipNum = num;
            return this;
        }
        public SQLQuery<T> Take(int num)
        {
            _sqlBuilder.TakeNum = num;
            return this;
        }
        public List<T> ToList(object param = null)
        {
            return _conn.Query<T>(_sqlBuilder.CustomSQL, param).AsList();
        }
        //public Page<T> ToPage(object param = null, int pageIndex = 1, int pageSize = 10)
        //{
        //    _sqlBuilder.SkipNum = (pageIndex - 1) * pageSize;
        //    _sqlBuilder.TakeNum = pageSize;
        //    using (var mulit = _conn.QueryMultiple($"{_sqlBuilder.CountSQL};{_sqlBuilder.CustomSQL}", param))
        //    {
        //        var count = mulit.Read<int>().Single();
        //        var records = mulit.Read<T>().ToList();
        //        var paging = new Paging
        //        {
        //            PageIndex = pageIndex,
        //            PageSize = pageSize,
        //            Total = count
        //        };
        //        return new Page<T>() { Paging = paging, Records = records };
        //    }
        //}
    }
}
