using SmartSql.Abstractions.Logging.None;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using SmartSql.Common;
using System.Xml.Serialization;

namespace SmartSql.Abstractions.Logging
{
    public class LogManager
    {
        private static object syncObj = new object();
        private static ILoggerFactoryAdapter _loggerFactoryAdapter = null;
        private LogManager()
        { }
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
            try
            {
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SmartSqlLog));
                SmartSqlLog smartSqlLog = null;
                using (var configFile = FileLoader.Load("SmartSqlLog.xml"))
                {
                    smartSqlLog = xmlSerializer.Deserialize(configFile) as SmartSqlLog;
                }
                AssemblyName assemblyName = new AssemblyName { Name = smartSqlLog.LoggerFactoryAdapter.AssemblyName };
                Type loggerFactoryAdpterType = Assembly.Load(assemblyName)
                                               .GetType(smartSqlLog.LoggerFactoryAdapter.TypeName);
                _loggerFactoryAdapter = Activator.CreateInstance(loggerFactoryAdpterType) as ILoggerFactoryAdapter;
            }
            catch (Exception ex)
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
