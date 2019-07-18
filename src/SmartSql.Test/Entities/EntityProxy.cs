using System.Collections.Generic;
using SmartSql.Reflection.EntityProxy;

namespace SmartSql.Test.Entities
{
    public class EntityProxy : Entity, IEntityPropertyChangedTrackProxy
    {
        private Dictionary<string, int> _changedVersion;

        public EntityProxy()
        {
            _changedVersion = new Dictionary<string, int>(2);
        }


        public bool EnablePropertyChangedTrack { get; set; }

        public void OnPropertyChanged(string propName)
        {
            if (!EnablePropertyChangedTrack)
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
            if (!EnablePropertyChangedTrack)
            {
                return 0;
            }

            return _changedVersion.TryGetValue(propName, out var count) ? count : 0;
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