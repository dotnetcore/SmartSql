using SmartSql.Configuration.Tags;
using Xunit;

namespace SmartSql.Test.Unit.Tags;

public class SqlTextTest
{
    [Fact]
    public void BuildSql()
    {
        var expected = "id=?id";
        SqlText sqlText = new SqlText(expected, "?");
        RequestContext requestContext = new RequestContext();
        sqlText.BuildSql(requestContext);
        var actual = requestContext.SqlBuilder.ToString();
        Assert.Equal(expected, actual);
    }
    
    [Fact]
    public void BuildSqlWithIn()
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
        Assert.Equal("In (?Ids_0,?Ids_1)", actual);
    }
    
    [Fact]
    public void BuildSqlWithInAndSemicolon()
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
        Assert.Equal("In (?Ids_0,?Ids_1);", actual);
    }
}