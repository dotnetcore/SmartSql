using System;
using System.Reflection;

namespace SmartSql.Reflection.EntityProxy
{
    public interface IEntityPropertyChangedTrackProxy
    {
        bool GetEnablePropertyChangedTrack();
        void SetEnablePropertyChangedTrack(bool enablePropertyChangedTrack);
        void OnPropertyChanged(string propName);
        int GetPropertyVersion(string propName);
        void ClearPropertyVersion();
    }
}