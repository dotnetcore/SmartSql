using System;
using System.Reflection.Emit;
using FluentAssertions;
using SmartSql.Reflection.TypeConstants;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Attempts;

public class ILBuilderTests
{
    [Fact]
    public void Should_CreateInstance_When_UsingDynamicMethod()
    {
        var targetType = typeof(User);
        var dyMethod = new DynamicMethod("NewUser", targetType, new[] { CommonType.ObjectArray }, targetType, true);

        var ilGen = dyMethod.GetILGenerator();
        ilGen.New(targetType.GetConstructor(Type.EmptyTypes));
        ilGen.Return();

        var factory = (Func<object[], object>)dyMethod.CreateDelegate(typeof(Func<object[], object>));
        var user = factory(null) as User;

        user.Should().NotBeNull();
    }
}
