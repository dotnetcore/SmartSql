using SmartSql.Abstractions;
using SmartSql.Abstractions.DataSource;
using SmartSql.Abstractions.DbSession;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Batch
{
    public abstract class AbstractBatchInsert : IBatchInsert
    {
        public AbstractBatchInsert(ISmartSqlMapper sqlMapper)
        {
            SqlMapper = sqlMapper;
        }
        public DbTable Table { get; set; }
        public ISmartSqlMapper SqlMapper { get; }
        public IDataSourceFilter DataSourceFilter => SqlMapper.SmartSqlOptions.DataSourceFilter;
        public IDbConnectionSessionStore SessionStore => SqlMapper.SmartSqlOptions.DbSessionStore;

        public void Dispose()
        {
            SessionStore.Dispose();
        }

        public abstract void Insert();

        public abstract Task InsertAsync();
    }
}
