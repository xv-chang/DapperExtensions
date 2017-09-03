using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;

namespace DapperExtensions.Query
{
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
}
