using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmartSql.Reflection.TypeConstants
{
    public static class CommonType
    {
        public static readonly Type Object = typeof(object);
        public static readonly Type Int32 = typeof(int);
        public static readonly Type Int64 = typeof(long);
        public static readonly Type Boolean = typeof(bool);
        public static readonly Type String = typeof(string);
        public static readonly Type DateTime = typeof(DateTime);
        public static readonly Type Guid = typeof(Guid);
        public static readonly Type TimeSpan = typeof(TimeSpan);
        public static readonly Type Enum = typeof(Enum);
        public static readonly Type ObjectArray = typeof(object[]);
        public static readonly Type IEnumerable = typeof(IEnumerable);
        public static readonly Type GenericList = typeof(List<>);
        public static readonly Type Task = typeof(Task);
        public static readonly Type Void = typeof(void);
        public static readonly Type ValueTuple = typeof(ValueTuple);
        public static readonly Type Dictionary = typeof(IDictionary);
        public static readonly Type DictionaryStringObject = typeof(IDictionary<String, object>);

        public static bool IsValueTuple(Type type)
        {
            return type != null && type.ToString().StartsWith("System.ValueTuple");
        }
        public static MethodInfo GetValueTupleCreateMethod(Type valueTupleType)
        {
            return GetValueTupleCreateMethod(valueTupleType.GenericTypeArguments);
        }
        public static MethodInfo GetValueTupleCreateMethod(Type[] resultGenericTypeArguments)
        {
            return ValueTuple.GetMethods().First(m =>
             {
                 if (m.Name != "Create") { return false; }
                 return m.GetParameters().Length == resultGenericTypeArguments.Length;
             }).MakeGenericMethod(resultGenericTypeArguments);
        }
    }
}
