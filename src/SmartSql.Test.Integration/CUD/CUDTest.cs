using System;
using System.Collections.Generic;
using SmartSql.Test.Entities;
using Xunit;

namespace SmartSql.Test.Integration.CUD
{
    public class CUDTest : IntegrationTestBase
    {
        public CUDTest(SmartSqlFixture fixture) : base(fixture) { }

        [Fact]
        public void Get()
        {
            InsertReturnIdImpl(out long id);
            var entity = SqlMapper.GetById<AllPrimitive, long>(id);
            Assert.NotNull(entity);
        }

        [Fact]
        public void GetWithPropertyTrack()
        {
            InsertReturnIdImpl(out long id);
            var entity = SqlMapper.GetById<AllPrimitive, long>(id, true);
            Assert.NotNull(entity);
        }

        [Fact]
        public void Insert()
        {
            var insertEntity = new AllPrimitive
            {
                String = "Insert",
                DateTime = DateTime.Now
            };
            var recordsAffected = SqlMapper.Insert<AllPrimitive>(insertEntity);
            Assert.NotEqual(0, recordsAffected);
        }

        [Fact]
        public void Insert_Return_Id()
        {
            AllPrimitive insertEntity = InsertReturnIdImpl(out long id);
            Assert.NotEqual(0, id);
            Assert.Equal(id, insertEntity.Id);
        }

        private AllPrimitive InsertReturnIdImpl(out long id)
        {
            var insertEntity = new AllPrimitive
            {
                String = "Insert",
                DateTime = DateTime.Now
            };
            id = SqlMapper.Insert<AllPrimitive, long>(insertEntity);
            return insertEntity;
        }

        [Fact]
        public void Update()
        {
            InsertReturnIdImpl(out long id);
            var recordsAffected = SqlMapper.Update<AllPrimitive>(new AllPrimitive
            {
                Id = id,
                String = "Update",
                Boolean = true,
                DateTime = DateTime.Now
            });
            Assert.NotEqual(0, recordsAffected);
        }

        [Fact]
        public void DyUpdate()
        {
            InsertReturnIdImpl(out long id);
            var recordsAffected = SqlMapper.DyUpdate<AllPrimitive>(new { Id = id, Boolean = true });
            Assert.NotEqual(0, recordsAffected);
        }

        [Fact]
        public void DyUpdate_Dic()
        {
            InsertReturnIdImpl(out long id);
            var recordsAffected = SqlMapper.DyUpdate<AllPrimitive>(new Dictionary<String, object>
            {
                {"Id", id},
                {"Boolean", true}
            });
            Assert.NotEqual(0, recordsAffected);
        }

        [Fact]
        public void DeleteById()
        {
            InsertReturnIdImpl(out long id);
            var recordsAffected = SqlMapper.DeleteById<AllPrimitive, long>(id);
            Assert.NotEqual(0, recordsAffected);
        }

        [Fact]
        public void DeleteMany()
        {
            InsertReturnIdImpl(out long id0);
            InsertReturnIdImpl(out long id1);
            InsertReturnIdImpl(out long id2);
            var recordsAffected = SqlMapper.DeleteMany<AllPrimitive, long>(new long[] { id0, id1, id2 });
            Assert.Equal(3, recordsAffected);
        }

        [Fact]
        public void UpdateByTrack()
        {
            InsertReturnIdImpl(out long id0);
            var entity = SqlMapper.GetById<AllPrimitive, long>(id0, true);
            entity.String = "Updated";
            SqlMapper.Update(entity);
        }
    }
}
