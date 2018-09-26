using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.DIExtension
{
    public class AssemblyAutoRegisterOptions
    {
        /// <summary>
        /// 获取 ISmartSqlMapper 实例函数
        /// </summary>
        public Func<IServiceProvider, ISmartSqlMapper> GetSmartSql { get; set; }
        /// <summary>
        /// 仓储接口程序集
        /// </summary>
        public string AssemblyString { get; set; }
        /// <summary>
        /// Scope模板
        /// 默认：I{Scope}Repository
        /// </summary>
        public string ScopeTemplate { get; set; }
        /// <summary>
        /// 仓储接口筛选器
        /// </summary>
        public Func<Type, bool> Filter { get; set; }

        public void UseTypeFilter<T>()
        {
            Filter = (type) =>
            {
                return typeof(T).IsAssignableFrom(type);
            };
        }
    }
}
