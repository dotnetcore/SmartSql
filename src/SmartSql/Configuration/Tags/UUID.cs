using System;

namespace SmartSql.Configuration.Tags
{
    public class UUID : Tag
    {
        public String Format { get; set; }

        public override bool IsCondition(AbstractRequestContext context)
        {
            return true;
        }

        public override void BuildSql(AbstractRequestContext context)
        {
            if (String.IsNullOrEmpty(Format))
            {
                context.Parameters.TryAdd(Property, Guid.NewGuid());
            }
            else
            {
                context.Parameters.TryAdd(Property, Guid.NewGuid().ToString(Format));
            }
        }
    }
}