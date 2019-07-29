using System;
using SmartSql.Reflection.EntityProxy;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class EntityProxyCacheFactoryTest
    {
        [Fact]
        public void Test()
        {
            var entity = EntityProxyCache<Entity>.CreateInstance();
            var entityProxy = entity as IEntityPropertyChangedTrackProxy;

            var state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            Assert.Equal(0, state);

            entity.Id = 1;
            state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            Assert.Equal(0, state);

            entityProxy.SetEnablePropertyChangedTrack(true);
            
            entity.Id = 1;
            Assert.Equal(1, entityProxy.GetPropertyVersion(nameof(Entity.Id)));
            

            entity.Deleted = true;
            Assert.Equal(1, entityProxy.GetPropertyVersion(nameof(Entity.Deleted)));
        }

        [Fact]
        public void TestCtor()
        {
            var entity = EntityProxyCache<Entity>.CreateInstance(1);

            Assert.Equal(1, entity.Id);

            var entityProxy = entity as IEntityPropertyChangedTrackProxy;

            var state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            Assert.Equal(0, state);
            
            entityProxy.SetEnablePropertyChangedTrack(true);
            
            entity.Id = 1;
            state = entityProxy.GetPropertyVersion(nameof(Entity.Id));
            Assert.Equal(1, state);

            entity.Id = 1;

            Assert.Equal(2, entityProxy.GetPropertyVersion(nameof(Entity.Id)));
        }
        
    }
}