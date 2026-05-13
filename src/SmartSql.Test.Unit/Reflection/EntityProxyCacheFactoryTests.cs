using FluentAssertions;
using SmartSql.Reflection.EntityProxy;
using SmartSql.Test.Unit.TestEntities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class EntityProxyCacheFactoryTests
    {
        [Fact]
        public void Should_TrackPropertyChanged_When_EnablePropertyChangedTrackIsTrue()
        {
            var entity = EntityProxyCache<Entity>.CreateInstance();
            var entityProxy = entity as IEntityPropertyChangedTrackProxy;

            var state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            state.Should().Be(0);

            entity.Id = 1;
            state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            state.Should().Be(0);

            entityProxy.SetEnablePropertyChangedTrack(true);

            entity.Id = 1;
            entityProxy.GetPropertyVersion(nameof(Entity.Id)).Should().Be(1);

            entity.Deleted = true;
            entityProxy.GetPropertyVersion(nameof(Entity.Deleted)).Should().Be(1);
        }

        [Fact]
        public void Should_TrackVersionIncrementally_When_CreatedWithConstructorArgs()
        {
            var entity = EntityProxyCache<Entity>.CreateInstance(1);

            entity.Id.Should().Be(1);

            var entityProxy = entity as IEntityPropertyChangedTrackProxy;

            var state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            state.Should().Be(0);

            entityProxy.SetEnablePropertyChangedTrack(true);

            entity.Id = 1;
            state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            state.Should().Be(1);

            entity.Id = 1;

            entityProxy.GetPropertyVersion(nameof(Entity.Id)).Should().Be(2);
        }
    }
}
