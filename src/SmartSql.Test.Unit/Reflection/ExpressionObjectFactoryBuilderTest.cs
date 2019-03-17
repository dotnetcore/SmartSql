using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Reflection.ObjectFactoryBuilder;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class ExpressionObjectFactoryBuilderTest : ObjectFactoryBuilderTest
    {
        private readonly ExpressionObjectFactoryBuilder _expressionObjectFactoryBuilder;
        public ExpressionObjectFactoryBuilderTest()
        {
            _expressionObjectFactoryBuilder = new ExpressionObjectFactoryBuilder();
        }
        [Fact]
        public void New()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, Type.EmptyTypes);
            var user = newFunc(null);
        }
        [Fact]
        public void New_Arg_Int64()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64 });
            var user = newFunc(new object[] { 3L });
        }
        [Fact]
        public void New_Arg_Int64_String()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64, CommonType.String });
            var user = newFunc(new object[] { 3L, "SmartSql" });
        }
        [Fact]
        public void New_Arg_Int64_String_Enum()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64, CommonType.String, typeof(UserStatus) });
            var user = newFunc(new object[] { 3L, "SmartSql", UserStatus.Ok });
        }
    }
}
