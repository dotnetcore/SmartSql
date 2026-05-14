using FluentAssertions;
using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class SqlTextTests
{
    [Fact]
    public void Should_BuildSql_When_NoInSyntax()
    {
        var expected = "id=?id";
        SqlText sqlText = new SqlText(expected, "?");
        RequestContext requestContext = new RequestContext();

        sqlText.BuildSql(requestContext);

        var actual = requestContext.SqlBuilder.ToString();
        actual.Should().Be(expected);
    }

    [Fact]
    public void Should_ExpandInClause_When_ParameterIsArray()
    {
        SqlText sqlText = new SqlText("in ?Ids", "?");
        RequestContext requestContext = new RequestContext()
        {
            Request = new
            {
                Ids = new[] { 1, 2 }
            }
        };
        requestContext.SetupParameters();

        sqlText.BuildSql(requestContext);

        var actual = requestContext.SqlBuilder.ToString();
        actual.Should().Be("In (?Ids_0,?Ids_1)");
    }

    [Fact]
    public void Should_ExpandInClause_When_FollowedBySemicolon()
    {
        SqlText sqlText = new SqlText("in ?Ids;", "?");
        RequestContext requestContext = new RequestContext()
        {
            Request = new
            {
                Ids = new[] { 1, 2 }
            }
        };
        requestContext.SetupParameters();

        sqlText.BuildSql(requestContext);

        var actual = requestContext.SqlBuilder.ToString();
        actual.Should().Be("In (?Ids_0,?Ids_1);");
    }
}
