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