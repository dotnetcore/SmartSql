using System;
using System.Collections;
using System.Collections.Generic;
using SmartSql.Exceptions;

namespace SmartSql.Reflection
{
    public class PropertyTokenizer : IEnumerator<PropertyTokenizer>
    {
        private const char DELIMITER = '.';
        private const char INDEX_OPEN = '[';
        private const char INDEX_CLOSE = ']';

        public bool IsFirst { get; private set; } = true;
        public String FullName { get; }
        public String Name { get; }
        public String Children { get; }
        public String Index { get; }
        public AccessMode Mode { get; }

        public PropertyTokenizer(string fullName)
        {
            FullName = fullName;
            Mode = AccessMode.Get;
            var delimiterIdx = FullName.IndexOf(DELIMITER);
            if (delimiterIdx > -1)
            {
                Name = FullName.Substring(0, delimiterIdx);
                Children = FullName.Substring(delimiterIdx + 1);
            }
            else
            {
                Name = FullName;
                Children = null;
            }

            var openIdx = Name.IndexOf(INDEX_OPEN);
            if (openIdx > -1)
            {
                Mode = AccessMode.IndexerGet;
                var closeIdx = Name.IndexOf(INDEX_CLOSE);
                Index = Name.Substring(openIdx + 1, closeIdx - openIdx - 1);
                Name = Name.Substring(0, openIdx);
            }
        }

        public bool MoveNext()
        {
            if (IsFirst)
            {
                IsFirst = false;
                Current = this;
                return true;
            }

            if (!String.IsNullOrEmpty(Current.Children))
            {
                Current = new PropertyTokenizer(Current.Children);
                return true;
            }

            return false;
        }

        public void Reset()
        {
            Current = this;
        }

        public PropertyTokenizer Current { get; private set; }


        object IEnumerator.Current => Current;

        public void Dispose()
        {
        }
    }

    public enum AccessMode
    {
        Get = 1,
        Set = 2,
        IndexerGet = 3,
        IndexerSet = 4,
    }
}