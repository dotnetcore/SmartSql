using System;

namespace SmartSql.DyRepository.Annotations
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
    public class CacheAttribute : Attribute
    {
        public CacheAttribute(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public String Id { get; }
        public String Type { get; }
        public String[] FlushOnExecutes { get; set; }

        /// <summary>
        /// Unit seconds
        /// </summary>
        public int FlushInterval { get; set; }

        public int CacheSize { get; set; } = 100;
    }
}