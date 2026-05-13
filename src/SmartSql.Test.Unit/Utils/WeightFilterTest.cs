using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSql.DataSource;
using SmartSql.Utils;
using Xunit;

namespace SmartSql.Test.Unit.Utils
{
    public class WeightFilterTest
    {
        [Fact]
        public void Elect()
        {
            var readDataSources = Enumerable.Range(1, 100).Select(m => new ReadDataSource { Name = m.ToString(), Weight = m });
            var inWeightSources = readDataSources.Select(readDataSource => new WeightFilter<ReadDataSource>.WeightSource
            {
                Source = readDataSource,
                Weight = readDataSource.Weight
            });
            WeightFilter<ReadDataSource> weightFilter = new WeightFilter<ReadDataSource>();
            var choiced = weightFilter.Elect(inWeightSources);
        }
    }
}
