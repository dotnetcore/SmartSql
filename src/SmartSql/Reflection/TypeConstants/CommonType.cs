using System;
using System.Collections;
using System.Collections.Generic;
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
        public static readonly Type ArrayObject = typeof(object[]);
        public static readonly Type IEnumerable = typeof(IEnumerable);
        public static readonly Type GenericList = typeof(List<>);
        public static readonly Type Task = typeof(Task);
        public static readonly Type Void = typeof(void);
    }
}
