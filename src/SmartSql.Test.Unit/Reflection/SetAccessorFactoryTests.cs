using FluentAssertions;
using SmartSql.Reflection.PropertyAccessor;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class SetAccessorFactoryTests
    {
        [Fact]
        public void Should_SetValue_When_AccessingIdProperty()
        {
            var obj = new AllPrimitive { };
            ISetAccessorFactory setAccessorFactory = EmitSetAccessorFactory.Instance;

            var set_Id = setAccessorFactory.Create(typeof(AllPrimitive), nameof(AllPrimitive.Id));
            set_Id(obj, 1L);

            obj.Id.Should().Be(1L);
        }

        [Fact]
        public void Should_SetValue_When_AccessingInt32Property()
        {
            var obj = new AllPrimitive { };
            ISetAccessorFactory setAccessorFactory = EmitSetAccessorFactory.Instance;

            var set_Int32 = setAccessorFactory.Create(typeof(AllPrimitive), nameof(AllPrimitive.Int32));
            set_Int32(obj, 1);

            obj.Int32.Should().Be(1);
        }
    }
}
