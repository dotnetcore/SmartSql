using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.Abstractions
{
    /// <summary>
    /// SQL 命令执行器
    /// </summary>
    public interface ICommandExecuter
    {
        int ExecuteNonQuery();
        IDataReader ExecuteReader();
        IDataReader ExecuteReader(CommandBehavior behavior);
        object ExecuteScalar();
    }
}
