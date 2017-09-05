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
                return ((MemberExpression)expr).Member;
            throw new ArgumentException("expression 不是 MemberExpression 类型");
        }
        public static object GetExpressValue(Expression expression)
        {
            if (expression.NodeType == ExpressionType.Constant)
                return VisitConstant((ConstantExpression)expression);
            return Expression.Lambda<Func<object>>(expression).Compile()();
        }
        private static object VisitConstant(ConstantExpression expression) => expression.Value;

    }
}
