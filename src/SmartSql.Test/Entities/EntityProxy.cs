using System.Collections.Generic;
using SmartSql.Reflection.EntityProxy;

namespace SmartSql.Test.Entities
{
    public class EntityProxy : Entity, IEntityPropertyChangedTrackProxy
    {
        private Dictionary<string, int> _changedVersion;

        public EntityProxy(bool enablePropertyChangedTrack)
        {
            _enablePropertyChangedTrack = enablePropertyChangedTrack;
            _changedVersion = new Dictionary<string, int>(2);
        }

        private bool _enablePropertyChangedTrack;

        public bool GetEnablePropertyChangedTrack()
        {
            return _enablePropertyChangedTrack;
        }

        public void SetEnablePropertyChangedTrack(bool enablePropertyChangedTrack)
        {
            _enablePropertyChangedTrack = enablePropertyChangedTrack;
        }

        public void OnPropertyChanged(string propName)
        {
            if (!_enablePropertyChangedTrack)
            {
                return;
            }

            if (_changedVersion.TryGetValue(propName, out var count))
            {
                _changedVersion[propName] = count + 1;
            }
            else
            {
                _changedVersion.Add(propName, 1);
            }
        }

        public int GetPropertyVersion(string propName)
        {
            if (!_enablePropertyChangedTrack)
            {
                return 0;
            }

            return _changedVersion.TryGetValue(propName, out var count) ? count : 0;
        }

        public void ClearPropertyVersion()
        {
            _changedVersion.Clear();
        }


        public override long Id
        {
            set
            {
                base.Id = value;
                OnPropertyChanged(nameof(Id));
            }
        }

        public override string Name
        {
            set
            {
                base.Name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }
}