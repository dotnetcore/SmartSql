using SmartSql.Configuration;
using SmartSql.Configuration.Maps;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Abstractions.Config
{
    public enum EventType
    {
        SqlMapChangeed = 1,
        ConfigChanged = 2
    }
    public class OnChangedEventArgs : EventArgs
    {
        public SmartSqlMapConfig SqlMapConfig { get; set; }
        public SmartSqlMap SqlMap { get; set; }
        public EventType EventType { get; set; }
    }
    public delegate void OnChangedHandler(object sender, OnChangedEventArgs eventArgs);

    /// <summary>
    /// 配置文件加载器
    /// </summary>
    public interface IConfigLoader : IDisposable
    {
        event OnChangedHandler OnChanged;
        SmartSqlMapConfig SqlMapConfig { get; set; }
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <returns></returns>
        SmartSqlMapConfig Load();
    }



}
