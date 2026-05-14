using System.Xml;
using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.DataSource;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class SqlMapBuilderTests
{
    private static SmartSqlConfig CreateSmartSqlConfig()
    {
        return new SmartSqlConfig
        {
            Database = new Database
            {
                DbProvider = DbProviderManager.SQLITE_DBPROVIDER
            }
        };
    }

    private static XmlDocument LoadTestXml(string fileName)
    {
        var xml = new XmlDocument();
        xml.Load(fileName);
        return xml;
    }

    [Fact]
    public void Should_ParseScope_When_XmlHasScope()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-Simple.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        sqlMap.Scope.Should().Be("TestScope");
    }

    [Fact]
    public void Should_ParseStatements_When_XmlHasStatements()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-Simple.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        sqlMap.Statements.Should().ContainKey("TestScope.GetAll");
        sqlMap.Statements.Should().ContainKey("TestScope.GetById");
        sqlMap.Statements["TestScope.GetAll"].Id.Should().Be("GetAll");
        sqlMap.Statements["TestScope.GetById"].Id.Should().Be("GetById");
    }

    [Fact]
    public void Should_RegisterSqlMapInConfig_When_Built()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-Simple.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        config.SqlMaps.Should().ContainKey("TestScope");
        config.SqlMaps["TestScope"].Should().BeSameAs(sqlMap);
    }

    [Fact]
    public void Should_ParseSqlTags_When_StatementHasContent()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-Simple.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var statement = sqlMap.Statements["TestScope.GetAll"];
        statement.SqlTags.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_ParseDynamicTags_When_StatementHasWhereClause()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-DynamicTags.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var statement = sqlMap.Statements["DynamicScope.QueryUsers"];
        statement.SqlTags.Should().NotBeEmpty();
    }

    [Fact]
    public void Should_ParseCaches_When_XmlHasCaches()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        sqlMap.Caches.Should().ContainKey("CacheScope.TestCache");
        var cache = sqlMap.Caches["CacheScope.TestCache"];
        cache.Type.Should().Be("Fifo");
    }

    [Fact]
    public void Should_ParseCacheFlushInterval_When_CacheHasFlushInterval()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var cache = sqlMap.Caches["CacheScope.TestCache"];
        cache.FlushInterval.Should().NotBeNull();
        cache.FlushInterval.Hours.Should().Be(1);
        cache.FlushInterval.Minutes.Should().Be(30);
    }

    [Fact]
    public void Should_ParseCacheFlushOnExecute_When_CacheHasFlushOnExecute()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var cache = sqlMap.Caches["CacheScope.TestCache"];
        cache.FlushOnExecutes.Should().ContainSingle();
        cache.FlushOnExecutes[0].Statement.Should().Be("CacheScope.InsertUser");
    }

    [Fact]
    public void Should_ParseResultMaps_When_XmlHasResultMaps()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        sqlMap.ResultMaps.Should().ContainKey("CacheScope.UserResult");
        var resultMap = sqlMap.ResultMaps["CacheScope.UserResult"];
        resultMap.Properties.Should().ContainKey("Id");
        resultMap.Properties.Should().ContainKey("UserName");
    }

    [Fact]
    public void Should_ParseResultMapPropertyMapping_When_ColumnDiffersFromProperty()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var resultMap = sqlMap.ResultMaps["CacheScope.UserResult"];
        var nameProp = resultMap.Properties["UserName"];
        nameProp.Name.Should().Be("Name");
        nameProp.Column.Should().Be("UserName");
    }

    [Fact]
    public void Should_ParseParameterMaps_When_XmlHasParameterMaps()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        sqlMap.ParameterMaps.Should().ContainKey("CacheScope.UserParameter");
        var paramMap = sqlMap.ParameterMaps["CacheScope.UserParameter"];
        paramMap.Parameters.Should().ContainKey("Id");
        paramMap.Parameters.Should().ContainKey("Name");
    }

    [Fact]
    public void Should_ParseParameterDbType_When_ParameterHasDbType()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var paramMap = sqlMap.ParameterMaps["CacheScope.UserParameter"];
        paramMap.Parameters["Id"].DbType.Should().Be(System.Data.DbType.Int64);
    }

    [Fact]
    public void Should_ParseMultipleResultMaps_When_XmlHasMultipleResultMaps()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        sqlMap.MultipleResultMaps.Should().ContainKey("CacheScope.UserMultipleResult");
        var mResultMap = sqlMap.MultipleResultMaps["CacheScope.UserMultipleResult"];
        mResultMap.Results.Should().HaveCount(2);
        mResultMap.Root.Should().NotBeNull();
        mResultMap.Root.MapId.Should().Be("CacheScope.UserResult");
    }

    [Fact]
    public void Should_ParseStatementCacheRef_When_StatementHasCacheAttribute()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var statement = sqlMap.Statements["CacheScope.GetAll"];
        statement.CacheId.Should().Be("CacheScope.TestCache");
    }

    [Fact]
    public void Should_ParseStatementResultMapRef_When_StatementHasResultMapAttribute()
    {
        var config = CreateSmartSqlConfig();
        var xml = LoadTestXml("TestSqlMap-CacheAndMaps.xml");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);
        var sqlMap = sqlMapBuilder.Build();

        var statement = sqlMap.Statements["CacheScope.GetAll"];
        statement.ResultMapId.Should().Be("CacheScope.UserResult");
    }

    [Fact]
    public void Should_Throw_When_XmlHasNoScope()
    {
        var config = CreateSmartSqlConfig();
        var xml = new XmlDocument();
        xml.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                     "<SmartSqlMap xmlns=\"http://SmartSql.net/schemas/SmartSqlMap.xsd\">" +
                     "<Statements><Statement Id=\"Test\">SELECT 1</Statement></Statements>" +
                     "</SmartSqlMap>");

        var sqlMapBuilder = new SqlMapBuilder(xml, config);

        var act = () => sqlMapBuilder.Build();

        act.Should().Throw<SmartSqlException>();
    }

    [Fact]
    public void Should_ReuseExistingSqlMap_When_SameScopeAlreadyRegistered()
    {
        var config = CreateSmartSqlConfig();
        var xml1 = LoadTestXml("TestSqlMap-Simple.xml");

        var builder1 = new SqlMapBuilder(xml1, config);
        builder1.Build();

        config.SqlMaps.Should().ContainKey("TestScope");
        config.SqlMaps["TestScope"].Statements.Should().ContainKey("TestScope.GetAll");

        var xml2 = new XmlDocument();
        xml2.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?>" +
                     "<SmartSqlMap Scope=\"TestScope\" xmlns=\"http://SmartSql.net/schemas/SmartSqlMap.xsd\">" +
                     "<Statements><Statement Id=\"NewStatement\">SELECT 2</Statement></Statements>" +
                     "</SmartSqlMap>");

        var builder2 = new SqlMapBuilder(xml2, config);
        var sqlMap = builder2.Build();

        config.SqlMaps["TestScope"].Should().BeSameAs(sqlMap);
        sqlMap.Statements.Should().ContainKey("TestScope.NewStatement");
        sqlMap.Statements.Should().ContainKey("TestScope.GetAll");
    }
}
