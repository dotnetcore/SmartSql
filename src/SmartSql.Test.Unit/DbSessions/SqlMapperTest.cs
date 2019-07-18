using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SmartSql.Reflection.EntityProxy;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Unit.DbSessions
{
    [Collection("GlobalSmartSql")]
    public class SqlMapperTest
    {
        protected ISqlMapper SqlMapper { get; }

        public SqlMapperTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public async Task QueryAsync()
        {
            var list = await SqlMapper.QueryAsync<dynamic>(new RequestContext
            {
                RealSql = "SELECT Top (5) T.* From T_AllPrimitive T With(NoLock)"
            });

            Assert.NotNull(list);
        }

        [Fact]
        public void Query()
        {
            var list = SqlMapper.Query<AllPrimitive>(new RequestContext
            {
                RealSql = "SELECT Top (5) T.* From T_AllPrimitive T With(NoLock)"
            });

            Assert.NotNull(list);
        }

        [Fact]
        public void QueryEnableTrack()
        {
            var entity = SqlMapper.QuerySingle<AllPrimitive>(new RequestContext
            {
                EnableTrack = true,
                RealSql = "SELECT Top (1) T.* From T_AllPrimitive T With(NoLock)"
            });
            var entityProxy = entity as IEntityPropertyChangedTrackProxy;
            Assert.NotNull(entityProxy);

            var state = entityProxy.GetPropertyVersion(nameof(AllPrimitive.String));
            Assert.Equal(0, state);
            entity.String = "Updated";
            state = entityProxy.GetPropertyVersion(nameof(AllPrimitive.String));
            Assert.Equal(1, state);
        }
    }
}