using System.Collections.Generic;
using SmartSql.Reflection.Proxy;

namespace SmartSql.Test.Entities
{
    public class EntityProxy : Entity, IEntityProxy
    {
        private Dictionary<string, int> _updateStates;

        public EntityProxy()
        {
            _updateStates = new Dictionary<string, int>(2);
        }


        public bool EnableTrack { get; set; }

        public void OnUpdated(string propName)
        {
            if (!EnableTrack)
            {
                return;
            }

            if (_updateStates.TryGetValue(propName, out var count))
            {
                _updateStates[propName] = count + 1;
            }
            else
            {
                _updateStates.Add(propName, 1);
            }
        }

        public int GetState(string propName)
        {
            if (!EnableTrack)
            {
                return 0;
            }

            return _updateStates.TryGetValue(propName, out var count) ? count : 0;
        }


        public override long Id
        {
            set
            {
                base.Id = value;
                OnUpdated(nameof(Id));
            }
        }

        public override string Name
        {
            set
            {
                base.Name = value;
                OnUpdated(nameof(Name));
            }
        }
    }
}