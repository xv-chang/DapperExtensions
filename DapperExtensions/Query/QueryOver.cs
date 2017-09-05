using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Dapper;
using DapperExtensions.FluentMap.Resolvers;
using DapperExtensions;

namespace DapperExtensions.Query
{
    public class QueryOver<T>
    {
        private readonly SqlTranslator<T> _sqlTranslator;
        private readonly SqlBuilder _sqlBuilder;
        private readonly IDbConnection _conn;
        public QueryOver(IDbConnection conn)
        {
            var type = typeof(T);
            _sqlTranslator = new SqlTranslator<T>();
            _sqlBuilder = _sqlBuilder = new SqlBuilder()
            {
                TableName = DefaultResolver.ResolveTableName(type),
                DefaultColumns = DefaultResolver.ResolveColumnNames(type)
            };
            _conn = conn;
        }
        public QueryOver<T> Select(Expression<Func<T, object>> expression)
        {
            var sql = _sqlTranslator.VisitExpression(expression);
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
            var sql = _sqlTranslator.VisitExpression(expression);
            _sqlBuilder.AppendAnd(sql);
            return this;
        }
        public QueryOver<T> Or(Expression<Func<T, bool>> expression)
        {
            var sql = _sqlTranslator.VisitExpression(expression);
            _sqlBuilder.AppendOr(sql);
            return this;
        }

        public QueryOver<T> OrderBy(Expression<Func<T, object>> expression)
        {
            var sql = _sqlTranslator.VisitExpression(expression);
            _sqlBuilder.AppendOrder($"{sql} ASC");
            return this;
        }
        public QueryOver<T> DescOrderBy(Expression<Func<T, object>> expression)
        {

            var sql = _sqlTranslator.VisitExpression(expression);
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
            return _conn.Query<T>(_sqlBuilder.BasicSQL, GetParameters()).AsList();
        }
        private DynamicParameters GetParameters()
        {
            var paramters = new DynamicParameters();
            foreach (var item in _sqlTranslator.Params)
            {
                paramters.Add(item.Key, item.Value);
            }
            return paramters;
        }
    }
}
