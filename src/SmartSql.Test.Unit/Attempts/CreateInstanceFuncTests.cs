using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using FluentAssertions;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Attempts;

public class CreateInstanceFuncTests
{
    private Func<object[], object> CreateViaIL(Type targetType)
    {
        var dyMethod = new DynamicMethod("NewInstance", targetType, new[] { CommonType.ObjectArray }, targetType, true);
        var ilGen = dyMethod.GetILGenerator();
        ilGen.New(targetType.GetConstructor(Type.EmptyTypes));
        ilGen.Return();
        return (Func<object[], object>)dyMethod.CreateDelegate(typeof(Func<object[], object>));
    }

    private Func<object[], object> CreateViaLinq(Type targetType)
    {
        var ctor = targetType.GetConstructor(Type.EmptyTypes);
        var param = Expression.Parameter(CommonType.ObjectArray, "args");
        return Expression.Lambda<Func<object[], object>>(Expression.New(ctor), param).Compile();
    }

    private Func<object[], object> CreateViaLinqWithArg(Type targetType)
    {
        var ctor = targetType.GetConstructor(new[] { CommonType.Int64 });
        var param = Expression.Parameter(CommonType.ObjectArray, "args");
        var arg = Expression.Unbox(Expression.ArrayIndex(param, Expression.Constant(0)), CommonType.Int64);
        return Expression.Lambda<Func<object[], object>>(Expression.New(ctor, arg), param).Compile();
    }

    [Fact]
    public void Should_CreateInstance_When_UsingLinqExpression()
    {
        var factory = CreateViaLinq(typeof(User));

        var user = factory(new object[] { 1 }) as User;

        user.Should().NotBeNull();
    }

    [Fact]
    public void Should_CreateInstanceWithArg_When_UsingLinqExpression()
    {
        var factory = CreateViaLinqWithArg(typeof(User));

        var user = factory(new object[] { 1L }) as User;

        user.Should().NotBeNull();
    }

    [Fact]
    public void Should_CreateInstance_When_UsingILEmit()
    {
        var factory = CreateViaIL(typeof(User));

        var user = factory(null) as User;

        user.Should().NotBeNull();
    }
}
