using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class Tests
    {
        public void CreateInstance_Linq()
        {
            var userType = typeof(User);

            var funParamTypes = typeof(Object[]);
            var paramExp = Expression.Parameter(funParamTypes, "args");

            Expression.ArrayIndex(paramExp, Expression.Constant(0));

            var ctorTypes = new Type[] { CommonType.Int32, CommonType.String };
            var userCtor = userType.GetConstructor(ctorTypes);
        }
        [Fact]
        public void BuildNewUser_Func_IL()
        {
            var _targetType = typeof(User);
            DynamicMethod dyMethod = new DynamicMethod("NewGenericList", _targetType, new Type[] { CommonType.ObjectArray }, _targetType, true);

            var ilGen = dyMethod.GetILGenerator();
            var newCtor = _targetType.GetConstructor(Type.EmptyTypes);
            ilGen.New(newCtor);
            ilGen.Return();
            //var funcType = Expression.GetFuncType(CommonType.ArrayObject,_targetType);
            var func = (Func<object[], object>)dyMethod.CreateDelegate(typeof(Func<object[], object>));
            var user = func(null) as User;
            Assert.NotNull(user);
        }
    }
}
