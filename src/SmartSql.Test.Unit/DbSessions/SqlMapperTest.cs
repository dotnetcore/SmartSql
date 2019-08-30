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
        public  void QuerySingleDynamic()
        {
            var list =  SqlMapper.QuerySingleDynamic(new RequestContext
            {
                RealSql = "SELECT Top (1) T.* From T_AllPrimitive T With(NoLock)"
            });

            Assert.NotNull(list);
        }
        [Fact]
        public  void QueryDynamic()
        {
            var list =  SqlMapper.QueryDynamic(new RequestContext
            {
                RealSql = "SELECT Top (5) T.* From T_AllPrimitive T With(NoLock)"
            });

            Assert.NotNull(list);
        }
        [Fact]
        public  void QueryDictionary()
        {
            var list =  SqlMapper.QueryDictionary(new RequestContext
            {
                RealSql = "SELECT Top (5) T.* From T_AllPrimitive T With(NoLock)"
            });

            Assert.NotNull(list);
        }
        [Fact]
        public  void QuerySingleDictionary()
        {
            var list =  SqlMapper.QuerySingleDictionary(new RequestContext
            {
                RealSql = "SELECT Top (1) T.* From T_AllPrimitive T With(NoLock)"
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
                EnablePropertyChangedTrack = true,
                RealSql = "SELECT Top (1) T.* From T_AllPrimitive T With(NoLock)"
            });
            var entityProxy = entity as IEntityPropertyChangedTrackProxy;
            Assert.NotNull(entityProxy);

            var state = entityProxy.GetPropertyVersion(nameof(AllPrimitive.String));
            Assert.Equal(0, state);
            entity.String = "Updated";
            state = entityProxy.GetPropertyVersion(nameof(AllPrimitive.String));
            Assert.Equal(1, state);

            SqlMapper.Update(entity);
        }

        [Fact]
        public void DbNullToDefaultEntity()
        {
            var entity = SqlMapper.QuerySingle<IgnoreDbNullEntity>(new RequestContext
            {
                RealSql = "SELECT Top (1) T.* From T_IgnoreDbNullEntity T"
            });
            Assert.Equal(0, entity.DbNullId);
        }
    }
}