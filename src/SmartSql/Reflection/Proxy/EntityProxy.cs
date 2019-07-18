using System;
using System.Reflection;

namespace SmartSql.Reflection.Proxy
{
    public class PropertyUpdatedEventArgs : EventArgs
    {
        public PropertyInfo Property { get; set; }
        public object Old { get; set; }
        public object New { get; set; }
    }

    public interface IEntityProxy
    {
        bool EnableTrack { get; set; }
        void OnUpdated(string propName);
        int GetState(string propName);
    }
}