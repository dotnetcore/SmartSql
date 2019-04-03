using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Attempts
{
    public class CreateInstanceFunc_Test
    {
        public Func<object[], object> ILGenerator(Type _targetType)
        {
            DynamicMethod dyMethod = new DynamicMethod("NewGenericList", _targetType, new Type[] { CommonType.ObjectArray }, _targetType, true);
            var ilGen = dyMethod.GetILGenerator();
            var newCtor = _targetType.GetConstructor(Type.EmptyTypes);
            ilGen.New(newCtor);
            ilGen.Return();
            return (Func<object[], object>)dyMethod.CreateDelegate(typeof(Func<object[], object>));
        }

        public Func<object[], object> LinqExpression(Type _targetType)
        {
            var newCtor = _targetType.GetConstructor(Type.EmptyTypes);
            var argValsExpr = Expression.Parameter(CommonType.ObjectArray, "argVals");
            var newExpression = Expression.New(newCtor);
            return Expression.Lambda<Func<object[], object>>(newExpression, argValsExpr).Compile();
        }

        public Func<object[], object> LinqExpression_Arg(Type _targetType)
        {
            var newCtor = _targetType.GetConstructor(new Type[] { CommonType.Int64 });
            var argValsExpr = Expression.Parameter(CommonType.ObjectArray, "argVals");
            var arg1Expr = Expression.ArrayIndex(argValsExpr, Expression.Constant(0));
            var unBoxArg1Expr = Expression.Unbox(arg1Expr, CommonType.Int64);
            var newExpression = Expression.New(newCtor, unBoxArg1Expr);
            return Expression.Lambda<Func<object[], object>>(newExpression, argValsExpr).Compile();
        }

        [Fact]
        public void LinqTest()
        {
            var user = LinqExpression(typeof(User))(new object[] { 1 });
        }
        [Fact]
        public void LinqArgTest()
        {
            var user = LinqExpression_Arg(typeof(User))(new object[] { 1L });
        }

    }
}
