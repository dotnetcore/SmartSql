using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.Reflection.ObjectFactoryBuilder
{
    public class ExpressionObjectFactoryBuilder : AbstractObjectFactoryBuilder
    {
        public override Func<object[], object> Build(Type targetType, Type[] ctorArgTypes)
        {
            var argsExpr = Expression.Parameter(CommonType.ObjectArray, "args");
            var parameterExprs = new List<Expression>();
            for (int i = 0; i < ctorArgTypes.Length; i++)
            {
                Type argType = ctorArgTypes[i];
                Expression argValExpr = Expression.ArrayIndex(argsExpr, Expression.Constant(i));
                if (argType.IsValueType)
                {
                    argValExpr = Expression.Unbox(argValExpr, argType);
                }
                else
                {
                    if (argType != CommonType.Object)
                    {
                        argValExpr = Expression.Convert(argValExpr, argType);
                    }
                }
                parameterExprs.Add(argValExpr);
            }
            var targetCtor = GetConstructor(targetType, ctorArgTypes);
            var newExpression = Expression.New(targetCtor, parameterExprs);
            return Expression.Lambda<Func<object[], object>>(newExpression, argsExpr).Compile();
        }
    }
}
