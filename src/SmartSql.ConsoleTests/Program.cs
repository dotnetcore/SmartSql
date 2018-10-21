using SmartSql.Abstractions;
using SmartSql.ZooKeeperConfig;
using System;
using Microsoft.Extensions.Logging;
using System.Linq;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using SmartSql.Options;
using Microsoft.Extensions.Options;
using SmartSql.Abstractions.Config;

namespace SmartSql.ConsoleTests
{
    class Program
    {
        static ILoggerFactory loggerFactory = new LoggerFactory();
        static void Main(string[] args)
        {

            loggerFactory.AddConsole(LogLevel.Trace);
            loggerFactory.AddDebug(LogLevel.Trace);
            //OptionsConfig_Test();
            Console.WriteLine("---------------------");
            LocalFileConfigLoader_Test();

            Console.WriteLine("Hello World!");
            Console.ReadLine();

        }

        private static void OptionsConfig_Test()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                             .AddJsonFile("SmartSqlConfig.json", false, true);

            var configuration = builder.Build();
            var services = new ServiceCollection();
            services.AddOptions();
            services.Configure<SmartSqlConfigOptions>(configuration);
            services.AddSingleton<ILoggerFactory>(loggerFactory);
            services.AddSmartSqlOption();

            var serviceProvider = services.BuildServiceProvider();
            var _smartSqlMapper = serviceProvider.GetRequiredService<ISmartSqlMapper>();
        }

        static void LocalFileConfigLoader_Test()
        {

            var sqlMapper = new SmartSqlMapper(loggerFactory);
            while (true)
            {
                string ipt = Console.ReadLine();
                var list = sqlMapper.Query<T_Entity>(new RequestContext
                {
                    Scope = "Entity",
                    SqlId = "Query",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                });
            }


        }

        static void ZooKeeperConfigLoader_Test()
        {
            string connStr = "192.168.31.103:2181";//192.168.31.103:2181 192.168.1.5:2181,192.168.1.5:2182,192.168.1.5:2183

            string configPath = "/Config/App1/SmartSqlMapConfig.xml";
            var configLoader = new ZooKeeperConfigLoader(configPath, connStr);

            var SqlMapper = new SmartSqlMapper(new SmartSqlOptions
            {
                ConfigPath = configPath,
                ConfigLoader = configLoader
            });

            int i = 0;
            for (i = 0; i < 10; i++)
            {
                Console.ReadLine();
                var list = SqlMapper.Query<T_Entity>(new RequestContext
                {
                    Scope = "Entity",
                    SqlId = "Query",
                    Request = new { Ids = new long[] { 1, 2, 3, 4 } }
                });
                Console.WriteLine($"{list.Count()}");
            }
        }
    }


    /// <summary>
    ///T_Entity
    /// </summary>	
    public class T_Entity 
    {


        private long PrivatePro { get; set; } = 88888;
        /// <summary>
        /// FLong
        /// </summary>		
        public virtual long FLong { get; set; }

        /// <summary>
        /// FDecimal
        /// </summary>		
        public virtual decimal FDecimal { get; set; }
        /// <summary>
        /// FNullDecimal
        /// </summary>		
        public virtual decimal? FNullDecimal { get; set; }

        /// <summary>
        /// FBool
        /// </summary>		
        public virtual bool FBool { get; set; }

        /// <summary>
        /// FNullBool
        /// </summary>		
        public virtual bool? FNullBool { get; set; }

        /// <summary>
        /// FTimestamp
        /// </summary>		
        ///public virtual DateTime FTimestamp { get; set; }

        /// <summary>
        /// Status
        /// </summary>		
        public virtual EntityStatus Status { get; set; }

        /// <summary>
        /// NullStatus
        /// </summary>		
        public virtual EntityStatus? NullStatus { get; set; }

        /// <summary>
        /// CreationTime
        /// </summary>		
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// LastUpdateTime
        /// </summary>		
        public virtual DateTime? LastUpdateTime { get; set; }

        public virtual String JustMeString { get; set; }
    }

    public enum EntityStatus
    {
        Ok = 1
    }


}