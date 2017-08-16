using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;


namespace DapperExtensions
{
    public static class ExpressionHelper
    {
        public static MemberInfo GetMemberInfo(LambdaExpression expression)
        {
            var expr = ((LambdaExpression)expression).Body;
            if (expr.NodeType == ExpressionType.MemberAccess)
            {
                return ((MemberExpression)expr).Member;
            }
            throw new ArgumentException("expression 不是 MemberExpression 类型");
        }
    }
}
