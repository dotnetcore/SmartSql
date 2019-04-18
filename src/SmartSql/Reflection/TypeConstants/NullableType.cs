using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SmartSql.Reflection.TypeConstants
{
    public static class NullableType<TTarget> where TTarget : struct
    {
        public static Type TargetType = typeof(TTarget);
        public static readonly Type Type = typeof(TTarget?);

        public static readonly ConstructorInfo Ctor = Type.GetConstructor(new Type[] { TargetType });
    }
}
