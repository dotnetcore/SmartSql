using System;
using System.Collections.Generic;
using System.Text;

namespace SmartSql.Reflection.ObjectFactoryBuilder
{
    public interface IObjectFactoryBuilder
    {
        Func<object[], object> GetObjectFactory(Type targetType, Type[] ctorArgTypes);
        Func<object[], object> Build(Type targetType, Type[] ctorArgTypes);
    }
}
