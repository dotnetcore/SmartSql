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
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="smartSqlMapper">smartSqlMapper</param>
        /// <returns></returns>
        SmartSqlMapConfig Load(String path, ISmartSqlMapper smartSqlMapper);
    }
}
