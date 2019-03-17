using SmartSql.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using SmartSql.Reflection;

namespace SmartSql.DataSource
{
    public class DbProviderManager
    {
        private IDictionary<string, DbProvider> _dbProviders = new Dictionary<string, DbProvider>(StringComparer.CurrentCultureIgnoreCase);

        public static readonly DbProvider SQLSERVER_DBPROVIDER = new DbProvider
        {
            Name = "SqlServer",
            ParameterPrefix = "@",
            Type = "System.Data.SqlClient.SqlClientFactory,System.Data.SqlClient",
            SelectAutoIncrement = ";Select Scope_Identity();"
        };
        public static readonly DbProvider POSTGRESQL_DBPROVIDER = new DbProvider
        {
            Name = "PostgreSql",
            ParameterPrefix = "@",
            Type = "Npgsql.NpgsqlFactory,Npgsql",
            SelectAutoIncrement = "Returning *;"
        };
        public static DbProviderManager Instance = new DbProviderManager();
        private DbProviderManager()
        {
            _dbProviders.Add("SqlServer", SQLSERVER_DBPROVIDER);
            _dbProviders.Add("PostgreSql", POSTGRESQL_DBPROVIDER);
            _dbProviders.Add("MySql", new DbProvider
            {
                Name = "MySql",
                ParameterPrefix = "?",
                Type = "MySql.Data.MySqlClient.MySqlClientFactory,MySql.Data",
                SelectAutoIncrement = ";Select Last_Insert_Id();"
            });
            _dbProviders.Add("Oracle", new DbProvider
            {
                Name = "Oracle",
                ParameterPrefix = ":",
                Type = "Oracle.ManagedDataAccess.Client.OracleClientFactory,Oracle.ManagedDataAccess",
                SelectAutoIncrement = ""
            });
            _dbProviders.Add("SQLite", new DbProvider
            {
                Name = "SQLite",
                ParameterPrefix = "$",
                Type = "System.Data.SQLite.SQLiteFactory,System.Data.SQLite",
                SelectAutoIncrement = ""
            });
        }
        public void Add(DbProvider dbProvider)
        {
            _dbProviders.Add(dbProvider.Name, dbProvider);
        }
        public bool Remove(string dbProviderName)
        {
            return _dbProviders.Remove(dbProviderName);
        }

        public bool TryGet(string dbProviderName, out DbProvider dbProvider)
        {
            if (!_dbProviders.TryGetValue(dbProviderName, out dbProvider))
            {
                return false;
            }
            if (dbProvider.Factory == null)
            {
                dbProvider.Factory = GetDbProviderFactory(dbProvider.Type);
            }
            return true;
        }

        public bool TryInit(ref DbProvider dbProvider)
        {
            if (!TryGet(dbProvider.Name, out var dbProviderCache))
            {
                return false;
            }
            if (string.IsNullOrEmpty(dbProvider.ParameterPrefix))
            {
                dbProvider.ParameterPrefix = dbProviderCache.ParameterPrefix;
            }
            if (String.IsNullOrEmpty(dbProvider.SelectAutoIncrement))
            {
                dbProvider.SelectAutoIncrement = dbProviderCache.SelectAutoIncrement;
            }
            dbProvider.Type = dbProviderCache.Type;
            dbProvider.Factory = dbProviderCache.Factory;
            var cmdBuilder = dbProvider.Factory.CreateCommandBuilder();
            dbProvider.ParameterNamePrefix = cmdBuilder.QuotePrefix;
            dbProvider.ParameterNameSuffix = cmdBuilder.QuoteSuffix;
            cmdBuilder.Dispose();
            return true;
        }

        public DbProviderFactory GetDbProviderFactory(String typeString)
        {
            return TypeUtils.GetType(typeString)
                .GetField("Instance")
                .GetValue(null) as DbProviderFactory;

        }
    }
}
