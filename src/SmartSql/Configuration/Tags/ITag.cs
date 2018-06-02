using SmartSql.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Configuration.Tags
{
    public interface ITag
    {
        TagType Type { get; }
        ITag Parent { get; set; }
        bool IsCondition(RequestContext context);
        void BuildSql(RequestContext context);
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
        Placeholder,
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
