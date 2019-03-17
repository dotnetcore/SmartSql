using SmartSql.Reflection.ObjectFactoryBuilder;
using SmartSql.Test.Entities;
using System;
using SmartSql.Reflection.TypeConstants;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class EmitObjectFactoryBuilderTest : ObjectFactoryBuilderTest
    {
        private readonly EmitObjectFactoryBuilder _emitObjectFactoryBuilder;
        public EmitObjectFactoryBuilderTest()
        {
            _emitObjectFactoryBuilder = new EmitObjectFactoryBuilder();
        }
        [Fact]
        public void New()
        {
            var newFunc = _emitObjectFactoryBuilder.GetObjectFactory(UserType, Type.EmptyTypes);
            var user = newFunc(null);
        }
        [Fact]
        public void New_Arg_Int64()
        {
            var newFunc = _emitObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64 });
            var user = newFunc(new object[] { 3L });
        }
        [Fact]
        public void New_Arg_Int64_String()
        {
            var newFunc = _emitObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64, CommonType.String });
            var user = newFunc(new object[] { 3L, "SmartSql" });
        }
        [Fact]
        public void New_Arg_Int64_String_Enum()
        {
            var newFunc = _emitObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64, CommonType.String, typeof(UserStatus) });
            var user = newFunc(new object[] { 3L, "SmartSql", UserStatus.Ok });
        }
    }
}
