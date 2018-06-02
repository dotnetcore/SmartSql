using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.DataSource
{
    public interface IDataSourceGroup
    {
        IWriteDataSource Write { get; set; }
        IEnumerable<IReadDataSource> Reads { get; set; }
    }
}
