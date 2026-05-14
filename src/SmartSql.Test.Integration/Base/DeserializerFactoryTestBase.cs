using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using SmartSql.Deserializer;
using SmartSql.Test.Integration.Fixtures;
using Xunit;

namespace SmartSql.Test.Integration.Base;

public abstract class DeserializerFactoryTestBase : IntegrationTestBase
{
    protected DeserializerFactoryTestBase(IDbTestFixture fixture) : base(fixture) { }

    [Fact]
    public void Should_ReturnCustomType_When_UsingCustomDeserializer()
    {
        var alias = $"DeserializerFactoryTest_{DbProvider}";
        var connectionString = Fixture.SmartSqlBuilder.SmartSqlConfig.Database.Write.ConnectionString;
        var builder = new SmartSqlBuilder()
            .UseDataSource(DbProvider, connectionString)
            .UseAlias(alias)
            .AddDeserializer(new CustomDeserializer())
            .Build();
        var uuidSql = DbProvider switch
        {
            "PostgreSql" => "SELECT gen_random_uuid()",
            "SqlServer" => "SELECT NEWID()",
            "MySql" or "MySqlConnector" => "SELECT uuid()",
            _ => "SELECT lower(hex(randomblob(4)) || '-' || hex(randomblob(2)) || '-' || '4' || substr(hex(randomblob(2)),2) || '-' || substr('89ab',abs(random()) % 4 + 1,1) || substr(hex(randomblob(2)),2) || '-' || hex(randomblob(6)))"
        };
        var result = builder.SqlMapper.QuerySingle<CustomResultType>(new RequestContext
        {
            RealSql = uuidSql
        });
        result.Should().NotBeNull();
        result.Value.Should().NotBeNullOrEmpty();
        builder.Dispose();
    }

    public class CustomResultType { public string Value { get; set; } = string.Empty; }

    public class CustomDeserializer : IDataReaderDeserializer
    {
        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
            => resultType == typeof(CustomResultType);

        public TResult ToSingle<TResult>(ExecutionContext executionContext)
        {
            var reader = executionContext.DataReaderWrapper;
            if (!reader.HasRows) return default;
            reader.Read();
            var value = reader.GetString(0);
            object result = new CustomResultType { Value = value };
            return (TResult)result;
        }

        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
            => throw new NotImplementedException();
        public Task<TResult> ToSingleAsync<TResult>(ExecutionContext executionContext)
            => throw new NotImplementedException();
        public Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
            => throw new NotImplementedException();
    }
}
