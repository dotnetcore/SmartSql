using System;
using System.Reflection;

namespace SmartSql.Reflection.EntityProxy
{
    public interface IEntityPropertyChangedTrackProxy
    {
        bool EnablePropertyChangedTrack { get; set; }
        void OnPropertyChanged(string propName);
        int GetPropertyVersion(string propName);
    }
}