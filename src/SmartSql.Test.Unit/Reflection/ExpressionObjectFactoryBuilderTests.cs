using FluentAssertions;
using SmartSql.Reflection.ObjectFactoryBuilder;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Unit.TestEntities;
using System;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class ExpressionObjectFactoryBuilderTests : ObjectFactoryBuilderTests
    {
        private readonly ExpressionObjectFactoryBuilder _expressionObjectFactoryBuilder;

        public ExpressionObjectFactoryBuilderTests()
        {
            _expressionObjectFactoryBuilder = new ExpressionObjectFactoryBuilder();
        }

        [Fact]
        public void Should_CreateInstance_When_ParameterlessConstructorUsed()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, Type.EmptyTypes);

            var user = newFunc(null);

            user.Should().NotBeNull();
        }

        [Fact]
        public void Should_CreateInstance_When_Int64ConstructorUsed()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64 });

            var user = newFunc(new object[] { 3L });

            user.Should().NotBeNull();
        }

        [Fact]
        public void Should_CreateInstance_When_Int64AndStringConstructorUsed()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64, CommonType.String });

            var user = newFunc(new object[] { 3L, "SmartSql" });

            user.Should().NotBeNull();
        }

        [Fact]
        public void Should_CreateInstance_When_Int64StringAndEnumConstructorUsed()
        {
            var newFunc = _expressionObjectFactoryBuilder.GetObjectFactory(UserType, new Type[] { CommonType.Int64, CommonType.String, typeof(UserStatus) });

            var user = newFunc(new object[] { 3L, "SmartSql", UserStatus.Ok });

            user.Should().NotBeNull();
        }
    }
}
