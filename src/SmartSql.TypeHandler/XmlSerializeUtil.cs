using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace SmartSql.TypeHandler
{
    public class XmlSerializeUtil
    {
        public static object Deserialize(string xmlString, Type targetType)
        {
            using (StringReader stringReader = new StringReader(xmlString))
            {
                XmlSerializer xmldes = new XmlSerializer(targetType);
                return xmldes.Deserialize(stringReader);
            }
        }

        public static string Serializer(object obj)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                XmlSerializer xml = new XmlSerializer(obj.GetType());
                xml.Serialize(mStream, obj);
                StreamReader streamReader = new StreamReader(mStream);
                return streamReader.ReadToEnd();
            }
        }
    }
}
