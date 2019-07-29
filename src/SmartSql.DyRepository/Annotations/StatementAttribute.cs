using SmartSql.DataSource;
using System;
using System.Data;

namespace SmartSql.DyRepository.Annotations
{
    /// <summary>
    /// SqlMap.Statement 映射
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class StatementAttribute : Attribute
    {
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
        /// <summary>
        /// 命令类型
        /// </summary>
        public CommandType CommandType { get; set; } = CommandType.Text;
        /// <summary>
        /// 数据源
        /// </summary>
        public DataSourceChoice SourceChoice { get; set; } = DataSourceChoice.Unknow;
        public bool EnablePropertyChangedTrack { get; set; }
        public String ReadDb { get; set; }
        public int CommandTimeout { get; set; }
    }
    /// <summary>
    /// 执行行为
    /// </summary>
    public enum ExecuteBehavior
    {
        /// <summary>
        /// 自动判断执行类型
        /// </summary>
        Auto = 0,
        /// <summary>
        /// 返回受影响行数
        /// </summary>
        Execute = 1,
        /// <summary>
        /// 返回结果的第一行第一列的值，主要用于返回主键
        /// </summary>
        ExecuteScalar = 2,
        /// <summary>
        /// 查询枚举对象，List
        /// </summary>
        Query = 3,
        /// <summary>
        /// 查询单个对象
        /// </summary>
        QuerySingle = 4,
        /// <summary>
        /// 返回DataTable
        /// </summary>
        GetDataTable = 5,
        /// <summary>
        /// 返回DataSet
        /// </summary>
        GetDataSet = 6,
    }
}