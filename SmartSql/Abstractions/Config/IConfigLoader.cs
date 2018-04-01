using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.SqlMap;
namespace SmartSql.Abstractions.Config
{
    /// <summary>
    /// 配置文件加载器
    /// </summary>
    public interface IConfigLoader : IDisposable
    {
        Action<ConfigChangedEvent> OnChanged { get; set; }
        SmartSqlMapConfig SqlMapConfig { get; }
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        SmartSqlMapConfig Load();
    }

    public class ConfigChangedEvent
    {
        public SmartSqlMapConfig SqlMapConfig { get; set; }
        public SmartSqlMap SqlMap { get; set; }
        public EventType EventType { get; set; }
    }
    public enum EventType
    {
        SqlMapChangeed = 1,
        ConfigChanged = 2
    }
}
