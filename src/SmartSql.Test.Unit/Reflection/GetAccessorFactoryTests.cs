using FluentAssertions;
using SmartSql.Reflection;
using SmartSql.Reflection.PropertyAccessor;
using SmartSql.Test.Unit.TestEntities;
using System;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class GetAccessorFactoryTests
    {
        [Fact]
        public void Should_GetValue_When_AccessingIdProperty()
        {
            var obj = new AllPrimitive { };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(typeof(AllPrimitive), nameof(AllPrimitive.Id), out var get_Id);

            var id = get_Id(obj);

            id.Should().Be(obj.Id);
        }

        [Fact]
        public void Should_GetValue_When_AccessingInt32Property()
        {
            var obj = new AllPrimitive { };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(typeof(AllPrimitive), nameof(AllPrimitive.Int32), out var get_Id);

            var id = get_Id(obj);

            id.Should().Be(obj.Int32);
        }

        [Fact]
        public void Should_GetValue_When_AccessingStringProperty()
        {
            var obj = new AllPrimitive { String = "SmartSql" };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(typeof(AllPrimitive), nameof(AllPrimitive.String), out var get_String);

            var str = get_String(obj);

            str.Should().Be(obj.String);
        }

        [Fact]
        public void Should_GetNestedValue_When_AccessingOneLevelNestedProperty()
        {
            var obj = new
            {
                User = new
                {
                    Id = 1
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Id", out var getMethodImpl);

            var id = getMethodImpl(obj);

            id.Should().Be(obj.User.Id);
        }

        [Fact]
        public void Should_GetNestedObject_When_AccessingNestedObjectProperty()
        {
            var obj = new
            {
                User = new
                {
                    Info = new
                    {
                        Id = 1
                    }
                }
            };

            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Info", out var getMethodImpl);

            var info = getMethodImpl(obj);

            info.Should().Be(obj.User.Info);
        }

        [Fact]
        public void Should_GetNestedValue_When_AccessingTwoLevelNestedProperty()
        {
            var obj = new
            {
                User = new
                {
                    Info = new
                    {
                        Id = 1
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Info.Id", out var getMethodImpl);

            var id = getMethodImpl(obj);

            id.Should().Be(obj.User.Info.Id);
        }

        [Fact]
        public void Should_GetValue_When_AccessingDictionaryWithStringKey()
        {
            var obj = new
            {
                User = new
                {
                    Items = new Dictionary<string, int>
                    {
                        {"Id", 1}
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Items[Id]", out var getMethodImpl);

            var id = getMethodImpl(obj);

            id.Should().Be(obj.User.Items["Id"]);
        }

        [Fact]
        public void Should_GetValue_When_AccessingDictionaryWithIntKey()
        {
            var obj = new
            {
                User = new
                {
                    Items = new Dictionary<int, int>
                    {
                        {1, 1}
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Items[1]", out var getMethodImpl);

            var id = getMethodImpl(obj);

            id.Should().Be(obj.User.Items[1]);
        }

        [Fact]
        public void Should_GetValue_When_AccessingListByIndex()
        {
            var obj = new
            {
                User = new
                {
                    Items = new List<String>
                    {
                        "SmartSql"
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Items[0]", out var getMethodImpl);

            var id = getMethodImpl(obj);

            id.Should().Be(obj.User.Items[0]);
        }
    }
}
