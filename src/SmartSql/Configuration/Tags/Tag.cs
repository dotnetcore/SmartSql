using SmartSql.Abstractions;
using SmartSql.Configuration.Statements;
using System;
using System.Collections.Generic;

namespace SmartSql.Configuration.Tags
{
    public abstract class Tag : ITag
    {
        public virtual String Prepend { get; set; }
        public String Property { get; set; }
        public abstract TagType Type { get; }
        public IList<ITag> ChildTags { get; set; }
        public ITag Parent { get; set; }
        public Statement Statement { get; set; }

        public abstract bool IsCondition(RequestContext context);
        public virtual void BuildSql(RequestContext context)
        {
            if (IsCondition(context))
            {
                context.Sql.Append(" ");
                if (!context.IgnorePrepend)
                {
                    context.Sql.Append(Prepend);
                }
                else
                {
                    context.IgnorePrepend = false;
                }
                context.Sql.Append(" ");
                BuildChildSql(context);
            }
        }

        public virtual void BuildChildSql(RequestContext context)
        {
            if (ChildTags != null && ChildTags.Count > 0)
            {
                foreach (var childTag in ChildTags)
                {
                    childTag.BuildSql(context);
                }
            }
        }
        protected virtual String GetDbProviderPrefix(RequestContext context)
        {
            return context.SmartSqlContext.DbPrefix;
        }

        protected virtual Object GetPropertyValue(RequestContext context)
        {
            return context.RequestParameters.GetValue(Property);
        }
    }
}
