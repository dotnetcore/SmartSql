using System;

namespace SmartSql.Annotations
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScopeAttribute : Attribute
    {
        public ScopeAttribute(string scope)
        {
            Scope = scope;
        }

        public string Scope { get; }
    }
}