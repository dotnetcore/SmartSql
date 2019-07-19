using System;
using System.Reflection;

namespace SmartSql.Configuration.Tags
{
    public interface IPropertyChanged
    {
        String Property { get; set; }

        /// <summary>
        /// 验证属性值是否变更过
        ///  true : 如果属性值没有变更，IsCondition 直接返回 false,否则返回 true
        ///  false: 如果属性值有变更，IsCondition 直接返回 false,否则返回 true
        /// </summary>
        PropertyChangedState PropertyChanged { get; set; }
    }

    public enum PropertyChangedState
    {
        Ignore = 0,
        Changed = 1,
        Unchanged = 2
    }
}