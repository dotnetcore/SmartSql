using System;
using System.Collections.Generic;
using System.Text;
using SmartSql.Utils;

namespace SmartSql.DIExtension
{
    public class AssemblyAutoRegisterOptions : TypeScanOptions
    {
        /// <summary>
        /// 实例别名
        /// </summary>
        public String SmartSqlAlias { get; set; }
        
        /// <summary>
        /// Scope模板
        /// 默认：I{Scope}Repository
        /// </summary>
        public string ScopeTemplate { get; set; }
    }
}