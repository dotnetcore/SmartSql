using FluentAssertions;
using System.Collections.Generic;
using System.Diagnostics;
using SmartSql.Bulk;
using SmartSql.Test.Entities;
using Xunit;
using Xunit.Abstractions;


namespace SmartSql.Test.Integration.Bulk;

public class BulkTests
{
    private readonly ITestOutputHelper _output;

    public BulkTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void Should_MatchRowCount_When_ToDataTable()
    {
        var list = new List<AllPrimitive>();
        for (int i = 0; i < 100000; i++)
        {
            list.Add(new AllPrimitive
            {
                Id = i
            });
        }

        var watch = Stopwatch.StartNew();
        var dataTable = list.ToDataTable();
        _output.WriteLine($"ToDataTable taken :{watch.ElapsedMilliseconds}");
        watch.Stop();
        dataTable.Rows.Count.Should().Be(list.Count);
    }
}
