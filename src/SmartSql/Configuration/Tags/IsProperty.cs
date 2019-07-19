using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SmartSql.Configuration.Tags
{
    public class IsProperty : Tag, IPropertyChanged
    {
        public override bool IsCondition(AbstractRequestContext context)
        {
            var isCondition = context.Parameters.ContainsKey(Property);
            if (!isCondition)
            {
                return false;
            }

            return PropertyChangedUtil.IsCondition(this, context);
        }

        public PropertyChangedState PropertyChanged { get; set; }
    }
}