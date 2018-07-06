using System;
using System.Data;

namespace SmartSql.DyRepository
{
    public class StatementAttribute : Attribute
    {
        /// <summary>
        /// 定义 SmartSqlMap.Scope 该属性可选，默认使用仓储接口的Scope
        /// </summary>
        public string Scope { get; set; }
        /// <summary>
        /// 可选，默认使用函数名作为 Statement.Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 可选， 默认 Execute：Auto ，自动判断 执行类型
        /// </summary>
        public ExecuteBehavior Execute { get; set; } = ExecuteBehavior.Auto;
        /// <summary>
        /// 可选，当不使用 SmartSqlMap.Statement 时可直接定义 Sql
        /// </summary>
        public string Sql { get; set; }
    }
    /// <summary>
    /// 执行行为
    /// </summary>
    public enum ExecuteBehavior
    {
        Auto = 0,
        Execute = 1,
        ExecuteScalar = 2,
        Query = 3,
        QuerySingle = 4,
        GetDataTable = 5,
        GetDataSet = 6
    }
}