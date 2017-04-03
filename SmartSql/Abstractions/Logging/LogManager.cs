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
        private static ILoggerAdapter _loggerAdapter = null;
        private LogManager()
        { }
        public static ILoggerAdapter Adapter
        {
            get
            {
                if (_loggerAdapter == null)
                {
                    lock (syncObj)
                    {
                        if (_loggerAdapter == null)
                        {
                            LoadLoggerFactoryAdapter();
                        }
                    }
                }
                return _loggerAdapter;
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
                AssemblyName assemblyName = new AssemblyName { Name = smartSqlLog.LoggerAdapter.AssemblyName };
                Type loggerAdpterType = Assembly.Load(assemblyName)
                                               .GetType(smartSqlLog.LoggerAdapter.TypeName);
                _loggerAdapter = Activator.CreateInstance(loggerAdpterType) as ILoggerAdapter;
            }
            catch (Exception ex)
            {
                _loggerAdapter = new NoneLoggerAdapter();
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
