using System;
using System.Collections.Generic;
using System.Reflection;

namespace SmartSql.Reflection.TypeConstants
{
    public static class DictionaryStringIntType
    {
        public static readonly Type Type = typeof(Dictionary<String, int>);

        public static class Method
        {
            public static readonly MethodInfo TryGetValue = Type.GetMethod(nameof(Dictionary<String, int>.TryGetValue));

            public static readonly MethodInfo IndexerSet = Type.GetMethod("set_Item");
            public static readonly MethodInfo Add = Type.GetMethod(nameof(Dictionary<String, int>.Add));
            public static readonly MethodInfo Clear = Type.GetMethod(nameof(Dictionary<String, int>.Clear));
        }

        public static class Ctor
        {
            public static readonly ConstructorInfo Capacity = Type.GetConstructor(new Type[] {CommonType.Int32});
        }
    }
}