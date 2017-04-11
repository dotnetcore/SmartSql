using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DataAccess
{
    public class DataAccessGeneric<TEntity> : DataAccess
        where TEntity : class
    {
        public DataAccessGeneric(String smartSqlMapConfigPath = "SmartSqlMapConfig.config") : base(smartSqlMapConfigPath)
        {

        }
        protected override void InitScope()
        {
            Scope = typeof(TEntity).Name;
        }
    }
}
