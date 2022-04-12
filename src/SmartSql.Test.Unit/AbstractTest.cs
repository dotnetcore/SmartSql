using SmartSql.DataSource;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit
{
    public abstract class AbstractTest
    {
        protected String DbType => "MySql";
        protected String ConnectionString => "server=localhost;uid=root;pwd=root;database=SmartSqlTestDB";
    }
}
