using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using Dapper;
using System.Linq;

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
    public class SqlTranslator
    {

        public Dictionary<string, object> Params = new Dictionary<string, object>();
        private int _index = 0;
        public string PIndex
        {
            get { return $"p{_index++}"; }
        }

        public string VisitExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.OrElse:
                    return VisitAndOr((BinaryExpression)expression);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return VisitBinary((BinaryExpression)expression);
                case ExpressionType.Call:
                    return VisitMethodCall((MethodCallExpression)expression);
                case ExpressionType.New:
                    return VisitNew((NewExpression)expression);
                case ExpressionType.MemberAccess:
                    return VisitMemberAccess((MemberExpression)expression);
                default:
                    throw new NotImplementedException($"不支持类型{expression.NodeType.ToString()}");
            }
        }

        public string VisitLambda(LambdaExpression expression)
        {
            return VisitExpression(expression.Body);
        }

        private string VisitBinary(BinaryExpression expression)
        {
            //节点类型是字段或属性
            if (expression.Left.NodeType == ExpressionType.MemberAccess)
            {
                var op = string.Empty;
                switch (expression.NodeType)
                {
                    case ExpressionType.Equal:
                        op = "=";
                        break;
                    case ExpressionType.GreaterThan:
                        op = ">";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        op = ">=";
                        break;
                    case ExpressionType.LessThan:
                        op = "<";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        op = "<=";
                        break;
                    case ExpressionType.NotEqual:
                        op = "<>";
                        break;
                    default:
                        throw new NotImplementedException($"不支持类型{expression.NodeType.ToString()}");
                }
                var fKey = ((MemberExpression)expression.Left).Member.Name;
                var fVal = GetExpressValue(expression.Right);
                var p = PIndex;
                Params.Add(p, fVal);
                return $" {fKey}{op}@{p}";
            }
            return string.Empty;
        }

        private string VisitMemberAccess(MemberExpression expression)
        {
            return expression.Member.Name;
        }
        private object VisitConstant(ConstantExpression expression)
        {
            return expression.Value;
        }
        /// <summary>
        /// 处理调用方法 如 Contains
        /// </summary>
        /// <param name="expression"></param>
        private string VisitMethodCall(MethodCallExpression expression)
        {
            var methodName = expression.Method.Name;
            var args = expression.Arguments;
            switch (methodName)
            {
                case "Like":
                    {
                        var fKey = VisitMemberAccess((MemberExpression)args[0]);
                        var fval = GetExpressValue(args[1]);
                        var mode = (MatchMode)GetExpressValue(args[2]);
                        var p = PIndex;
                        switch (mode)
                        {
                            case MatchMode.Any:
                                Params.Add(p, $"%{fval}%");
                                break;
                            case MatchMode.Start:
                                Params.Add(p, $"{fval}%");
                                break;
                            case MatchMode.End:
                                Params.Add(p, $"%{fval}");
                                break;
                            default:
                                break;
                        }
                        return $" {fKey} like @{p}";
                    }
                case "In":
                    {
                        var fKey = VisitMemberAccess((MemberExpression)args[0]);
                        var fval = VisitNewArrayInit((NewArrayExpression)args[1]);
                        if (fval.Length > 0)
                        {
                            var inParams = new StringBuilder();
                            foreach (var item in fval)
                            {
                                var p = PIndex;
                                Params.Add(p, fval);
                                inParams.Append($"@{p},");
                            }
                            return $" {fKey} in ({inParams.ToString().TrimEnd(',')})";
                        }
                        return string.Empty;
                    }
                case "NotIn":
                    {
                        var fKey = VisitMemberAccess((MemberExpression)args[0]);
                        var fval = VisitNewArrayInit((NewArrayExpression)args[1]);
                        if (fval.Length > 0)
                        {
                            var inParams = new StringBuilder();
                            foreach (var item in fval)
                            {
                                var p = PIndex;
                                Params.Add(p, fval);
                                inParams.Append($"@{p},");
                            }
                            return $" {fKey} not in ({inParams.ToString().TrimEnd(',')})";
                        }
                        return string.Empty;
                    }
                default:
                    throw new NotImplementedException($"不支持方法{methodName}");
            }
        }

        private string VisitAndOr(BinaryExpression andOr)
        {
            var op = andOr.NodeType == ExpressionType.AndAlso ? " AND " : " OR ";
            //处理左边表达式
            var leftSql = VisitExpression(andOr.Left);
            //处理右边表达式
            var rightSql = VisitExpression(andOr.Right);
            return $"({leftSql} {op} {rightSql})";
        }

        private string VisitNew(NewExpression expression)
        {
            StringBuilder builder = new StringBuilder();
            if (expression.Members.Count > 0)
            {
                foreach (var member in expression.Members)
                {
                    builder.Append(member.Name + ",");
                }
            }
            return builder.ToString().TrimEnd(',');
        }
        private object[] VisitNewArrayInit(NewArrayExpression expression)
        {
            var val = Expression.Lambda<Func<object[]>>(expression).Compile()();
            return val;
        }

        private object GetExpressValue(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
            {
                return VisitConstant((ConstantExpression)expression);
            }
            return Expression.Lambda<Func<object>>(expression).Compile()();
        }

    }
    public enum MatchMode
    {
        Any,
        Start,
        End
    }
    public class SqlBuilder
    {
        public SqlBuilder()
        {
            SelectBuilder = new StringBuilder();
            WhereBuilder = new StringBuilder();
            OrderBuilder = new StringBuilder();
            Parameters = new DynamicParameters();
        }
        private int pIndex = 0;
        public DynamicParameters Parameters;
        public Type Type { set; get; }
        private string BasicTemplate => "SELECT {0} FROM {1} {2} {3} {4} {5}";
        private string CustomTemplate => "{0} {1} {2} {3} {4}";
        private string CountTemplate => "SELECT COUNT(1) FROM {0} {1}";
        public bool HasWhere
        {
            get
            {
                return WhereBuilder.Length > 0;
            }
        }
        public StringBuilder WhereBuilder { set; get; }
        public bool HasOrder
        {
            get
            {
                return OrderBuilder.Length > 0;
            }
        }
        public StringBuilder OrderBuilder { set; get; }

        public bool HasSelect
        {
            get
            {
                return SelectBuilder.Length > 0;
            }
        }
        public StringBuilder SelectBuilder { set; get; }
        public string TableName { set; get; }
        public int SkipNum { set; get; }
        public int TakeNum { set; get; }
        private string TakeSQL => TakeNum > 0 ? $" LIMIT {TakeNum}" : string.Empty;
        private string SkipSQL => SkipNum > 0 ? $" OFFSET {SkipNum}" : string.Empty;
        private string TableSQL => TableName;
        public string SelectSQL
        {
            get
            {
                if (!HasSelect)
                {
                    SelectBuilder.Append(string.Join(",", Type.GetProperties().Select(p => p.Name)));
                }
                return SelectBuilder.ToString();
            }
        }
        public string WhereSQL => WhereBuilder.ToString();
        public string OrderSQL => OrderBuilder.ToString();
        public string CountSQL
        {
            get
            {
                return string.Format(CountTemplate, TableSQL, WhereSQL);
            }
        }
        public string BasicSQL
        {
            get
            {
                return string.Format(BasicTemplate, SelectSQL, TableSQL, WhereSQL, OrderSQL, TakeSQL, SkipSQL);
            }
        }

        public string CustomSQL
        {
            get
            {

                return string.Format(CustomSQL, SelectSQL, WhereSQL, OrderSQL, TakeSQL, SkipSQL);
            }
        }
        public bool IsDesc { set; get; }
        private string GenerateParameter()
        {
            return $"p{pIndex++}";
        }
        public void AddOrder(string name, bool desc)
        {
            var sort = desc ? " DESC" : "ASC";
            OrderBuilder.Append(HasOrder ? $",{name } {sort}" : $" ORDER BY {name} {sort}");
        }

        public void SetParamter(string key, object val)
        {
            Parameters.Add(key, val);
        }
        public void SetParamters(Dictionary<string, object> paramsDic)
        {
            foreach (var item in paramsDic)
            {
                Parameters.Add(item.Key, item.Value);
            }
        }
        public void AppendSelect(string sql)
        {
            SelectBuilder.Append(sql);
        }
        public void AppendAnd(string sql)
        {
            WhereBuilder.Append(HasWhere ? $" AND {sql}" : $" WHERE {sql}");
        }
        public void AppendOr(string sql)
        {
            WhereBuilder.Append(HasWhere ? $" OR {sql}" : $" WHERE {sql}");
        }
        public void AppendOrder(string sql)
        {
            OrderBuilder.Append(HasOrder ? $",{sql}" : $" ORDER BY {sql}");
        }

    }

    public enum SqlUseType
    {
        Select,
        And,
        Or,
        OrderBy,
        GroupBy
    }

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

    public static class DbConnectionExtensions
    {
        public static QueryOver<T> CreateQuery<T>(this IDbConnection conn)
        {
            return new QueryOver<T>(conn);
        }
        public static QueryOver<T> CreateQuery<T>(this IDbConnection conn, string tableName)
        {
            return new QueryOver<T>(conn, tableName);
        }
        public static SQLQuery<T> CreateSQL<T>(this IDbConnection conn, string sql)
        {
            return new SQLQuery<T>(conn, sql);
        }
    }
}
