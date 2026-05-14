using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FluentAssertions;
using SmartSql.ConfigBuilder;
using SmartSql.Configuration;
using SmartSql.Exceptions;
using Xunit;

namespace SmartSql.Test.Unit.ConfigBuilder;

public class XmlConfigBuilderEdgeTests
{
    #region Missing Database Node

    [Fact]
    public void Should_ThrowException_When_DatabaseNodeMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Database*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_DbProviderNodeMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*DbProvider*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_WriteDataSourceMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Database>
    <DbProvider Name=""SQLite""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Write*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region TypeHandler Edge Cases

    [Fact]
    public void Should_ThrowException_When_TypeHandlerTypeMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <TypeHandlers>
    <TypeHandler Name=""TestHandler""/>
  </TypeHandlers>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Type*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_GenericTypeHandlerPropertyTypeMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <TypeHandlers>
    <TypeHandler Name=""TestHandler"" Type=""SmartSql.TypeHandler.ListTypeHandler`1, SmartSql""/>
  </TypeHandlers>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>();
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region TagBuilder Edge Cases

    [Fact]
    public void Should_ThrowException_When_TagBuilderNameMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <TagBuilders>
    <TagBuilder Type=""SmartSql.Configuration.Tags.IsNotEmpty, SmartSql""/>
  </TagBuilders>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Name*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_TagBuilderTypeMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <TagBuilders>
    <TagBuilder Name=""CustomTag""/>
  </TagBuilders>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Type*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region IdGenerator Edge Cases

    [Fact]
    public void Should_ThrowException_When_IdGeneratorNameMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <IdGenerators>
    <IdGenerator Type=""SnowflakeId""/>
  </IdGenerators>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Name*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_IdGeneratorTypeMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <IdGenerators>
    <IdGenerator Name=""TestId""/>
  </IdGenerators>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Type*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region SmartSqlMap Edge Cases

    [Fact]
    public void Should_ThrowException_When_SmartSqlMapPathMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <SmartSqlMaps>
    <SmartSqlMap Type=""File""/>
  </SmartSqlMaps>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Path*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_SmartSqlMapTypeMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <SmartSqlMaps>
    <SmartSqlMap Path=""Test.xml""/>
  </SmartSqlMaps>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Type*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region AutoConverter Edge Cases

    [Fact]
    public void Should_ThrowException_When_AutoConverterNameMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <AutoConverters>
    <AutoConverter>
      <Tokenizer Name=""PascalCase""/>
      <Converter Name=""UpperCase""/>
    </AutoConverter>
  </AutoConverters>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Name*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_AutoConverterTokenizerMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <AutoConverters>
    <AutoConverter Name=""TestConverter"">
      <Converter Name=""UpperCase""/>
    </AutoConverter>
  </AutoConverters>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>()
                .WithMessage("*Tokenizer*");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ThrowException_When_AutoConverterConverterMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <AutoConverters>
    <AutoConverter Name=""TestConverter"">
      <Tokenizer Name=""PascalCase""/>
    </AutoConverter>
  </AutoConverters>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<SmartSqlException>();
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region ReadDataSource Edge Cases

    [Fact]
    public void Should_ThrowException_When_ReadDataSourceNameMissing()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
    <Property Name=""ReadConnStr"" Value=""Data Source=:memory:Read""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
    <Read ConnectionString=""${ReadConnStr}"" Weight=""100""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<Exception>();
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region DbProvider Edge Cases

    [Fact]
    public void Should_ThrowException_When_DbProviderCannotBeInitialized()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().Throw<Exception>();
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region Properties with Various Types

    [Fact]
    public void Should_ParsePropertiesWithNumericValues()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""IntValue"" Value=""42""/>
    <Property Name=""DoubleValue"" Value=""3.14""/>
    <Property Name=""BoolValue"" Value=""true""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""Data Source=:memory:""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var config = configBuilder.Build();

            config.Properties.GetPropertyValue("${IntValue}").Should().Be("42");
            config.Properties.GetPropertyValue("${DoubleValue}").Should().Be("3.14");
            config.Properties.GetPropertyValue("${BoolValue}").Should().Be("true");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_ParseSimplePropertyReferences()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""DbPath"" Value=""/tmp/data/db""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${DbPath}/test.db""/>
  </Database>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var config = configBuilder.Build();

            config.Properties.GetPropertyValue("${DbPath}").Should().Be("/tmp/data/db");
            config.Database.Write.ConnectionString.Should().Be("/tmp/data/db/test.db");
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion

    #region Empty/Optional Sections

    [Fact]
    public void Should_BuildConfig_When_TagBuildersSectionIsEmpty()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <TagBuilders/>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().NotThrow();
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    [Fact]
    public void Should_BuildConfig_When_AutoConvertersSectionIsEmpty()
    {
        var xml = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<SmartSqlMapConfig xmlns=""http://SmartSql.net/schemas/SmartSqlMapConfig.xsd"">
  <Properties>
    <Property Name=""ConnectionString"" Value=""Data Source=:memory:""/>
  </Properties>
  <Database>
    <DbProvider Name=""SQLite""/>
    <Write Name=""WriteDB"" ConnectionString=""${ConnectionString}""/>
  </Database>
  <AutoConverters/>
</SmartSqlMapConfig>";
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_config_{Guid.NewGuid()}.xml");
        File.WriteAllText(tempPath, xml);

        try
        {
            var configBuilder = new XmlConfigBuilder(ResourceType.File, tempPath);

            var act = () => configBuilder.Build();

            act.Should().NotThrow();
        }
        finally
        {
            File.Delete(tempPath);
        }
    }

    #endregion
}
