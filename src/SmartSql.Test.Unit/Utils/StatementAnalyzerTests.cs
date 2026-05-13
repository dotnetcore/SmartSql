using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils;

public class StatementAnalyzerTests
{
    private readonly StatementAnalyzer _analyzer = new();

    [Fact]
    public void Should_ReturnInsert_When_SqlStartsWithInsert()
    {
        var result = _analyzer.Analyse("INSERT INTO Users (Id) VALUES (1)");

        result.Should().Be(StatementType.Insert);
    }

    [Fact]
    public void Should_ReturnSelect_When_SqlStartsWithSelect()
    {
        var result = _analyzer.Analyse("SELECT * FROM Users");

        result.Should().Be(StatementType.Select);
    }

    [Fact]
    public void Should_ReturnUpdate_When_SqlStartsWithUpdate()
    {
        var result = _analyzer.Analyse("UPDATE Users SET Name = 'test'");

        result.Should().Be(StatementType.Update);
    }

    [Fact]
    public void Should_ReturnDelete_When_SqlStartsWithDelete()
    {
        var result = _analyzer.Analyse("DELETE FROM Users WHERE Id = 1");

        result.Should().Be(StatementType.Delete);
    }

    [Fact]
    public void Should_ReturnMultipleTypes_When_MultipleStatements()
    {
        var result = _analyzer.Analyse("INSERT INTO T (Id) VALUES (1); SELECT * FROM T");

        result.Should().HaveFlag(StatementType.Insert);
        result.Should().HaveFlag(StatementType.Select);
    }

    [Fact]
    public void Should_ReturnUnknown_When_UnrecognizedSql()
    {
        var result = _analyzer.Analyse("EXEC sp_some_proc");

        result.Should().Be(StatementType.Unknown);
    }
}
