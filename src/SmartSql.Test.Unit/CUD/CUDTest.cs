using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.CUD
{
    public class CUDTest : AbstractXmlConfigBuilderTest
    {
        [Fact]
        public void Get()
        {
            AllPrimitive insertEntity = InsertReturnIdImpl(out long id);
            var entity = DbSession.GetById<AllPrimitive>(id);
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
            var recordsAffected = DbSession.Insert<AllPrimitive>(insertEntity);
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
            id = DbSession.Insert<AllPrimitive, long>(insertEntity);
            return insertEntity;
        }

        [Fact]
        public void Update()
        {
            AllPrimitive insertEntity = InsertReturnIdImpl(out long id);

            var recordsAffected = DbSession.Update<AllPrimitive>(new AllPrimitive
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
            AllPrimitive insertEntity = InsertReturnIdImpl(out long id);
            var recordsAffected = DbSession.DyUpdate<AllPrimitive>(new { Id = id, Boolean = true });
            Assert.NotEqual(0, recordsAffected);
        }

        [Fact]
        public void DeleteById()
        {
            AllPrimitive insertEntity = InsertReturnIdImpl(out long id);
            var recordsAffected = DbSession.DeleteById<AllPrimitive>(id);
            Assert.NotEqual(0, recordsAffected);
        }
        [Fact]
        public void DeleteMany()
        {
            InsertReturnIdImpl(out long id0);
            InsertReturnIdImpl(out long id1);
            InsertReturnIdImpl(out long id2);
            var recordsAffected = DbSession.DeleteMany<AllPrimitive,long>(new long[] { id0, id1, id2 });
            Assert.Equal(3, recordsAffected);
        }

    }
}
