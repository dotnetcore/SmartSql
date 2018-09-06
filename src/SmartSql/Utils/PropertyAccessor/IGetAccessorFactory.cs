using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Utils.PropertyAccessor
{

    public interface IGetAccessorFactory
    {
        Func<object, object> CreateGet(Type targetType, string propertyName, bool ignorePropertyCase);
    }
}
