using SmartSql.DataSource;
using SmartSql.DbSession;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Test.Unit
{
    public abstract class AbstractTest
    {
        protected String DbType => "SqlServer";
        protected String ConnectionString => "Data Source=.;Initial Catalog=SmartSqlTestDB;Integrated Security=True";
    }
}
