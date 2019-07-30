using System;
using System.Reflection;

namespace SmartSql.Configuration.Tags
{
    public class Now : Tag
    {
        public String Kind { get; set; }

        public override bool IsCondition(AbstractRequestContext context)
        {
            return true;
        }

        public override void BuildSql(AbstractRequestContext context)
        {
            if (!String.IsNullOrEmpty(Prepend))
            {
                context.SqlBuilder.Append(Prepend);
            }

            context.SqlBuilder.AppendFormat("{0}{1}",
                context.ExecutionContext.SmartSqlConfig.Database.DbProvider.ParameterPrefix, Property);
            context.Parameters.TryAdd(Property, GetNow());
        }

        private DateTime GetNow()
        {
            if (String.IsNullOrEmpty(Kind))
            {
                return DateTime.Now;
            }

            switch (Kind.ToUpper())
            {
                case "UTC":
                {
                    return DateTime.UtcNow;
                }

                default:
                {
                    return DateTime.Now;
                }
            }
        }
    }
}