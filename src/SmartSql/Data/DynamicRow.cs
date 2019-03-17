using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;

namespace SmartSql.Data
{
    public class DynamicRow : IDynamicMetaObjectProvider, IDictionary<String, object>
    {
        public object this[string key]
        {
            get => _columns.TryGetValue(key, out var index) ? _values[index] : null;
            set
            {
                if (_columns.TryGetValue(key, out var index))
                {
                    _values[index] = value;
                }
            }
        }

        public ICollection<string> Keys => _columns.Keys;

        public ICollection<object> Values => _values;

        public int Count => _values.Length;

        public bool IsReadOnly => true;
        private readonly IDictionary<string, int> _columns;
        private object[] _values;
        public DynamicRow(IDictionary<string, int> columns, object[] values)
        {
            _columns = columns;
            _values = values;
        }

        public void Add(string key, object value)
        {
            if (_columns.TryGetValue(key, out var index))
            {
                _values[index] = value;
            }
            else
            {
                _columns.Add(key, _columns.Count);
                Array.Resize(ref _values, _columns.Count);
                _values[_columns.Count] = value;
            }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _columns.Clear();
            for (var i = 0; i < _values.Length; i++)
            {
                _values[i] = null;
            }
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            if (TryGetValue(item.Key, out var val))
            {
                return val == item.Value;
            }
            return false;
        }

        public bool ContainsKey(string key)
        {
            return _columns.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            foreach (var kv in this)
            {
                array[arrayIndex++] = kv; 
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return _columns.Select(column => new KeyValuePair<string, object>(column.Key, _values[column.Value])).GetEnumerator();
        }

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new DynamicRowMetaObject(parameter, BindingRestrictions.Empty, this);
        }

        public bool Remove(string key)
        {
            return _columns.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return _columns.Remove(item.Key);
        }

        public bool TryGetValue(string key, out object value)
        {
            if (_columns.TryGetValue(key, out var index))
            {
                value = _values[index];
                return true;
            }
            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
