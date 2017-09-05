using System;
using System.Collections.Generic;
using System.Text;
using Dapper;
using System.Linq;

namespace DapperExtensions
{
    public class SqlBuilder
    {
        public IEnumerable<string> DefaultColumns { get; set; }
        private string BasicTemplate => "SELECT {0} FROM {1} {2} {3} {4} {5}";
        private string CustomTemplate => "{0} {1} {2} {3} {4}";
        private string CountTemplate => "SELECT COUNT(1) FROM {0} {1}";
        public StringBuilder WhereBuilder = new StringBuilder();
        public StringBuilder OrderBuilder = new StringBuilder();
        public StringBuilder SelectBuilder = new StringBuilder();
        public string TableName { get; set; }
        public int SkipNum { set; get; }
        public int TakeNum { set; get; }
        private string TakeSQL => TakeNum > 0 ? $" LIMIT {TakeNum}" : string.Empty;
        private string SkipSQL => SkipNum > 0 ? $" OFFSET {SkipNum}" : string.Empty;
        public string SelectSQL
        {
            get
            {
                if (SelectBuilder.Length == 0)
                {
                    SelectBuilder.Append(string.Join(",", DefaultColumns));
                }
                return SelectBuilder.ToString();
            }
        }
        public string CountSQL
        {
            get
            {
                return string.Format(CountTemplate, TableName, WhereBuilder.ToString());
            }
        }
        public string BasicSQL
        {
            get
            {
                return string.Format(BasicTemplate, SelectSQL, TableName, WhereBuilder.ToString(), OrderBuilder.ToString(), TakeSQL, SkipSQL);
            }
        }

        public string CustomSQL
        {
            get
            {
                return string.Format(CustomSQL, SelectSQL, WhereBuilder.ToString(), OrderBuilder.ToString(), TakeSQL, SkipSQL);
            }
        }
        public void AddOrder(string name, bool desc)
        {
            var sort = desc ? " DESC" : "ASC";
            OrderBuilder.Append(OrderBuilder.Length > 0 ? $",{name } {sort}" : $" ORDER BY {name} {sort}");
        }
        public void AppendSelect(string sql)
        {
            SelectBuilder.Append(SelectBuilder.Length > 0 ? $",{sql}" : sql);
        }
        public void AppendAnd(string sql)
        {
            WhereBuilder.Append(WhereBuilder.Length > 0 ? $" AND {sql}" : $" WHERE {sql}");
        }
        public void AppendOr(string sql)
        {
            WhereBuilder.Append(WhereBuilder.Length > 0 ? $" OR {sql}" : $" WHERE {sql}");
        }
        public void AppendOrder(string sql)
        {
            OrderBuilder.Append(OrderBuilder.Length > 0 ? $",{sql}" : $" ORDER BY {sql}");
        }
    }
}
