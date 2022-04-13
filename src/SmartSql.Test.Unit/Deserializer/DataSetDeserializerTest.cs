﻿using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.Deserializer
{
    [Collection("GlobalSmartSql")]
    public class DataSetDeserializerTest
    {
        protected ISqlMapper SqlMapper { get; }

        public DataSetDeserializerTest(SmartSqlFixture smartSqlFixture)
        {
            SqlMapper = smartSqlFixture.SqlMapper;
        }

        [Fact]
        public void GetDataSet()
        {
            SqlMapper.Insert<AllPrimitive, long>(new AllPrimitive());

            var result = SqlMapper.GetDataSet(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetDataSet"
            });
            Assert.NotNull(result);
            Assert.Equal(2, result.Tables.Count);
        }


        [Fact]
        public async Task GetDataSetAsync()
        {
            var result = await SqlMapper.GetDataSetAsync(new RequestContext
            {
                Scope = nameof(AllPrimitive),
                SqlId = "GetDataSet"
            });
            Assert.NotNull(result);
            Assert.Equal(2, result.Tables.Count);
        }
    }
}