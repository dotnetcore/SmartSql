using System;
using SmartSql.Reflection.Proxy;
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
            var entityProxy = entity as IEntityProxy;

            var state = entityProxy.GetState(nameof(Entity.Id));
            Assert.Equal(0, state);

            entity.Id = 1;
            state = entityProxy.GetState(nameof(Entity.Id));
            Assert.Equal(0, state);

            entityProxy.EnableTrack = true;
            entity.Id = 1;
            Assert.Equal(1, entityProxy.GetState(nameof(Entity.Id)));
        }

        [Fact]
        public void TestCtor()
        {
            var entity = EntityProxyCache<Entity>.CreateInstance(1);

            Assert.Equal(1, entity.Id);

            var entityProxy = entity as IEntityProxy;

            var state = entityProxy.GetState(nameof(Entity.Id));
            Assert.Equal(0, state);
            
            entityProxy.EnableTrack = true;
            
            entity.Id = 1;
            state = entityProxy.GetState(nameof(Entity.Id));
            Assert.Equal(1, state);

            entity.Id = 1;

            Assert.Equal(2, entityProxy.GetState(nameof(Entity.Id)));
        }
    }
}