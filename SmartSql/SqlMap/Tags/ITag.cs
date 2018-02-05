using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.SqlMap.Tags
{
    public interface ITag
    {
        TagType Type { get; }
        String BuildSql(RequestContext context);
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
        IsTrue,
        IsFalse,
        IsProperty,
        Include,
        Switch,
        SwitchCase,
        SwitchDefault,
        Dynamic,
        Where,
        Set,
        For,
        Env
    }
}
