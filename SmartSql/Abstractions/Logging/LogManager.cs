using SmartSql.Logging.None;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SmartSql.Common;
using System.Xml.Serialization;
using System.IO;

namespace SmartSql.Abstractions.Logging
{
    public class LogManager
    {
        private static object syncObj = new object();
        private static ILoggerFactoryAdapter _loggerFactoryAdapter = null;
        private LogManager() { }
        public static ILoggerFactoryAdapter Adapter
        {
            get
            {
                if (_loggerFactoryAdapter == null)
                {
                    lock (syncObj)
                    {
                        if (_loggerFactoryAdapter == null)
                        {
                            LoadLoggerFactoryAdapter();
                        }
                    }
                }
                return _loggerFactoryAdapter;
            }
        }


        private static void LoadLoggerFactoryAdapter()
        {
            var fileInfo = FileLoader.GetInfo("SmartSqlLog.xml");
            if (fileInfo.Exists)
            {
                using (var configFile = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlLog));
                    SmartSqlLog smartSqlLog = xmlSerializer.Deserialize(configFile) as SmartSqlLog;
                    AssemblyName assemblyName = new AssemblyName { Name = smartSqlLog.LoggerFactoryAdapter.AssemblyName };
                    Type loggerFactoryAdpterType = Assembly.Load(assemblyName)
                                                   .GetType(smartSqlLog.LoggerFactoryAdapter.TypeName);
                    _loggerFactoryAdapter = Activator.CreateInstance(loggerFactoryAdpterType) as ILoggerFactoryAdapter;
                }
            }
            else
            {
                _loggerFactoryAdapter = new NoneLoggerFactoryAdapter();
            }
        }

        public static ILog GetLogger(Type type)
        {
            return Adapter.GetLogger(type);
        }
        public static ILog GetLogger(string name)
        {
            return Adapter.GetLogger(name);
        }


    }
}
