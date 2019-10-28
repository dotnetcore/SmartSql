using SmartSql.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace System.Xml
{
    public static class XmlExtensions
    {
        public static bool TryGetValueAsString(this XmlAttributeCollection xmlAttributeCollection, string attrName,
            out string attrVal, Properties properties = null)
        {
            var attr = xmlAttributeCollection[attrName];
            if (attr == null)
            {
                attrVal = default;
                return false;
            }

            attrVal = attr.Value;
            if (properties != null)
            {
                attrVal = properties.GetPropertyValue(attr.Value);
            }

            return true;
        }

        public static bool TryGetValueAsBoolean(this XmlAttributeCollection xmlAttributeCollection, string attrName,
            out bool attrVal, Properties properties = null)
        {
            if (xmlAttributeCollection.TryGetValueAsString(attrName, out string attrValStr))
            {
                if (properties != null)
                {
                    attrValStr = properties.GetPropertyValue(attrValStr);
                }

                return Boolean.TryParse(attrValStr, out attrVal);
            }

            attrVal = default;
            return false;
        }

        public static bool TryGetValueAsInt32(this XmlAttributeCollection xmlAttributeCollection, string attrName,
            out int attrVal, Properties properties = null)
        {
            if (xmlAttributeCollection.TryGetValueAsString(attrName, out string attrValStr))
            {
                if (properties != null)
                {
                    attrValStr = properties.GetPropertyValue(attrValStr);
                }

                return Int32.TryParse(attrValStr, out attrVal);
            }

            attrVal = default;
            return false;
        }
    }
}