using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class OrderBy : Tag
    {
        public override string Prepend { get; set; } = " Order By ";

        public override bool IsCondition(AbstractRequestContext context)
        {
            Object reqVal = EnsurePropertyValue(context);

            if (reqVal is KeyValuePair<String, String>)
            {
                return true;
            }

            if (reqVal is IEnumerable<KeyValuePair<String, String>> orderBys)
            {
                return orderBys.Any();
            }

            return false;
        }

        public override void BuildSql(AbstractRequestContext context)
        {
            if (!IsCondition(context))
            {
                return;
            }

            Object reqVal = EnsurePropertyValue(context);

            context.SqlBuilder.Append(Prepend);

            if (reqVal is KeyValuePair<String, String> orderBy)
            {
                context.SqlBuilder.AppendFormat("{0} {1}", orderBy.Key, orderBy.Value);
                return;
            }


            var orderBys = (reqVal as IEnumerable<KeyValuePair<String, String>>).ToArray();

            for (var i = 0; i < orderBys.Length; i++)
            {
                orderBy = orderBys.ElementAt(i);
                context.SqlBuilder.AppendFormat("{0} {1}", orderBy.Key, orderBy.Value);
                if (i < (orderBys.Length - 1))
                {
                    context.SqlBuilder.Append(",");
                }
            }
        }

    }
}