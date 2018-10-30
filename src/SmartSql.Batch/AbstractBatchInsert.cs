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
            ColumnMappings = new Dictionary<String, ColumnMapping>(StringComparer.CurrentCulture);
        }
        public DbTable Table { get; set; }
        public ISmartSqlMapper SqlMapper { get; }
        public IDataSourceFilter DataSourceFilter => SqlMapper.SmartSqlOptions.DataSourceFilter;
        public IDbConnectionSessionStore SessionStore => SqlMapper.SmartSqlOptions.DbSessionStore;
        public IDictionary<String, ColumnMapping> ColumnMappings { get; private set; }

        public void Dispose()
        {
            SessionStore.Dispose();
        }

        protected virtual void InitColumnMappings()
        {
            if (ColumnMappings.Count == 0)
            {
                foreach (var dbColKV in Table.Columns)
                {
                    var dbCol = dbColKV.Value;
                    var colMapping = new ColumnMapping
                    {
                        Column = dbCol.Name,
                        Mapping = dbCol.Name
                    };
                    ColumnMappings.Add(colMapping.Mapping, colMapping);
                }
            }
        }

        public abstract void Insert();

        public abstract Task InsertAsync();

        public void AddColumnMapping(ColumnMapping columnMapping)
        {
            ColumnMappings.Add(columnMapping.Mapping, columnMapping);
        }

        public void ClearColumnMapping()
        {
            ColumnMappings.Clear();
        }
    }
}
