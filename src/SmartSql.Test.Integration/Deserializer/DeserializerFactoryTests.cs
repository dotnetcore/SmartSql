using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSql.Deserializer;
using Xunit;

namespace SmartSql.Test.Integration.Deserializer;

public class DeserializerFactoryTests : IntegrationTestBase
{
    public DeserializerFactoryTests(SmartSqlFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnCustomType_When_UsingCustomDeserializer()
    {
        var builder = new SmartSqlBuilder()
            .UseDataSource("MySql", "server=localhost;uid=root;pwd=root;database=SmartSqlTestDB")
            .UseAlias("DeserializerFactoryTest")
            .AddDeserializer(new CustomDeserializer())
            .Build();
        var result = builder.SqlMapper.QuerySingle<CustomResultType>(new RequestContext
        {
            RealSql = "Select uuid()"
        });
        result.Should().NotBeNull();
        result.Id.Should().NotBe(Guid.Empty);
    }

    public class CustomResultType
    {
        public Guid Id { get; set; }
    }

    public class CustomDeserializer : IDataReaderDeserializer
    {
        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
        {
            return resultType == typeof(CustomResultType);
        }

        public TResult ToSingle<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default(TResult);
            dataReader.Read();
            var id = dataReader.GetGuid(0);
            object result = new CustomResultType { Id = id };
            return (TResult)result;
        }

        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> ToSingleAsync<TResult>(ExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            throw new NotImplementedException();
        }
    }
}
