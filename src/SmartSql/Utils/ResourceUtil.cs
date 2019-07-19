using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using SmartSql.ConfigBuilder;
using SmartSql.Exceptions;

namespace SmartSql.Utils
{
    public class ResourceUtil
    {
        public static string BaseDirectory { get; } = AppDomain.CurrentDomain.BaseDirectory;

        public static XmlDocument LoadAsXml(ResourceType resourceType, string resourcePath)
        {
            switch (resourceType)
            {
                case ResourceType.File:
                    {
                        return LoadFileAsXml(resourcePath);
                    }
                case ResourceType.Embedded:
                    {
                        return LoadEmbeddedAsXml(resourcePath);
                    }
                case ResourceType.Uri:
                    {
                        var resourceUri = new Uri(resourcePath);
                        return LoadUriAsXml(resourceUri);
                    }
                default:
                    {
                        throw new SmartSqlException($"can not support ResourceType:{resourceType}");
                    }
            }
        }
        
        public static bool FileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(BaseDirectory, filePath);
            }

            return File.Exists(filePath);
        }

        public static XmlDocument LoadFileAsXml(string filePath)
        {
            if (!File.Exists(filePath))
            {
                filePath = Path.Combine(BaseDirectory, filePath);
            }
            using (var xmlReader = new XmlTextReader(filePath))
            {
                var xml = new XmlDocument();
                xml.Load(xmlReader);
                return xml;
            }
        }

        public static XmlDocument LoadUriAsXml(Uri uri)
        {
            var xml = new XmlDocument();
            xml.Load(uri.AbsoluteUri);
            return xml;
        }

        public static XmlDocument LoadEmbeddedAsXml(string embeddedResource)
        {
            var names = embeddedResource.Split(',');
            if (names.Length != 2)
            {
                throw new SmartSqlException($"EmbeddedResource:{embeddedResource} format error.");
            }
            var resourcePath = names[0];
            var assemblyStr = names[1];
            var resourceAssembly = Assembly.Load(assemblyStr);
            using (var resourceStream = resourceAssembly.GetManifestResourceStream(resourcePath))
            {
                var xml = new XmlDocument();
                xml.Load(resourceStream ?? throw new InvalidOperationException());
                return xml;
            }
        }
    }
}