using System;

namespace SmartSql.DyRepository.Annotations
{
    /// <summary>
    /// 函数参数特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
    public class ParamAttribute : Attribute
    {
        public ParamAttribute(string name)
        {
            Name = name;
        }
        /// <summary>
        /// DbDataParameter.Name
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// TypeHandler Name
        /// </summary>
        public String TypeHandler { get; set; }
    }
}