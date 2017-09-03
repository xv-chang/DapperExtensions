using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Dapper;

namespace DapperExtensions.Query
{
    public class QueryOver<T>
    {
        private readonly SqlTranslator _sqlTranslator;
        private readonly SqlBuilder _sqlBuilder;
        private readonly IDbConnection _conn;
        public QueryOver(IDbConnection conn)
        {
            _sqlTranslator = new SqlTranslator();
            _sqlBuilder = new SqlBuilder
            {
                Type = typeof(T),
                TableName = typeof(T).Name
            };
            _conn = conn;
        }

        public QueryOver(IDbConnection conn, string tableName)
        {
            _sqlTranslator = new SqlTranslator();
            _sqlBuilder = new SqlBuilder
            {
                Type = typeof(T),
                TableName = tableName
            };
            _conn = conn;
        }

        public QueryOver<T> Select(Expression<Func<T, object>> expression)
        {
            var sql = _sqlTranslator.VisitLambda(expression);
            _sqlBuilder.AppendSelect(sql);
            return this;
        }
        public QueryOver<T> Where(Expression<Func<T, bool>> expression)
        {
            return And(expression);
        }
        public QueryOver<T> WhereIF(bool flag, Expression<Func<T, bool>> expression)
        {
            if (flag)
                return And(expression);
            return this;
        }
        public QueryOver<T> And(Expression<Func<T, bool>> expression)
        {
            var sql = _sqlTranslator.VisitLambda(expression);
            _sqlBuilder.AppendAnd(sql);
            return this;
        }
        public QueryOver<T> Or(Expression<Func<T, bool>> expression)
        {
            var sql = _sqlTranslator.VisitLambda(expression);
            _sqlBuilder.AppendOr(sql);
            return this;
        }

        public QueryOver<T> OrderBy(Expression<Func<T, object>> expression)
        {
            var sql = _sqlTranslator.VisitLambda(expression);
            _sqlBuilder.AppendOrder($"{sql} ASC");
            return this;
        }
        public QueryOver<T> DescOrderBy(Expression<Func<T, object>> expression)
        {

            var sql = _sqlTranslator.VisitLambda(expression);
            _sqlBuilder.AppendOrder($"{sql} DESC");
            return this;
        }
        public QueryOver<T> OrderBy(string name)
        {
            _sqlBuilder.AppendOrder($"{name} ASC");
            return this;
        }

        public QueryOver<T> DescOrderBy(string name)
        {
            _sqlBuilder.AppendOrder($"{name} DESC");
            return this;
        }
        public QueryOver<T> Skip(int num)
        {
            _sqlBuilder.SkipNum = num;
            return this;
        }
        public QueryOver<T> Take(int num)
        {
            _sqlBuilder.TakeNum = num;
            return this;
        }
        public List<T> ToList()
        {
            _sqlBuilder.SetParamters(_sqlTranslator.Params);
            return _conn.Query<T>(_sqlBuilder.BasicSQL, _sqlBuilder.Parameters).AsList();
        }
        //public Page<T> ToPage(int pageIndex = 1, int pageSize = 10)
        //{
        //    _sqlBuilder.SkipNum = (pageIndex - 1) * pageSize;
        //    _sqlBuilder.TakeNum = pageSize;
        //    _sqlBuilder.SetParamters(_sqlTranslator.Params);
        //    using (var mulit = _conn.QueryMultiple($"{_sqlBuilder.CountSQL};{_sqlBuilder.BasicSQL}", _sqlBuilder.Parameters))
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
