using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public interface ITag
    {
        TagType Type { get; }
        String BuildSql(Object paramObj, String parameterPrefix);
    }

    public enum TagType
    {
        SqlText,
        IsEmpty,
        IsEqual,
        IsGreaterEqual,
        IsGreaterThan,
        IsLessEqual,
        IsLessThan,
        IsNotEmpty,
        IsNotEqual,
        IsNotNull,
        IsNull,
        Include,
        Switch,
        Case
    }
}
