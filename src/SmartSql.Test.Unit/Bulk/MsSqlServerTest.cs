using System;
using Xunit;
using SmartSql.Bulk.MsSqlServer;
using SmartSql.Bulk;
using System.Threading.Tasks;
using SmartSql.DataSource;
using SmartSql.DbSession;
using SmartSql.Test.Entities;

namespace SmartSql.Test.Unit.Bulk
{
    public class MsSqlServerFixture
    {
        public IDbSessionFactory DbSessionFactory { get; }

        public MsSqlServerFixture()
        {
            DbSessionFactory = new SmartSqlBuilder()
                .UseDataSource(DbProvider.MS_SQLSERVER,
                    "Data Source=.;Initial Catalog=SmartSqlTestDB;Integrated Security=True")
                .UseAlias("MsSqlServer-Bulk")
                .AddTypeHandler(new Configuration.TypeHandler()
                {
                    Name = "Json",
                    HandlerType = typeof(TypeHandler.JsonTypeHandler)
                })
                .Build().GetDbSessionFactory();
        }
    }

    public class MsSqlServerTest : IClassFixture<MsSqlServerFixture>
    {
        private IDbSessionFactory _dbSessionFactory;

        public MsSqlServerTest(MsSqlServerFixture serverFixture)
        {
            _dbSessionFactory = serverFixture.DbSessionFactory;
        }

        [Fact]
        public void Insert()
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                var data = dbSession.GetDataTable(new RequestContext
                {
                    RealSql = "Select Top(100) * From T_AllPrimitive Order By Id Desc"
                });
                data.TableName = "T_AllPrimitive";
                IBulkInsert bulkInsert = new BulkInsert(dbSession);
                bulkInsert.Table = data;
                bulkInsert.Insert();
            }
        }


        [Fact]
        public void InsertByList()
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                var list = dbSession.Query<AllPrimitive>(new RequestContext
                {
                    RealSql = "Select Top(100) * From T_AllPrimitive Order By Id Desc"
                });
                IBulkInsert bulkInsert = new BulkInsert(dbSession);

                bulkInsert.Insert(list);
            }
        }

        [Fact]
        public async Task InsertAsync()
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                var data = await dbSession.GetDataTableAsync(new RequestContext
                {
                    RealSql = "Select Top(100) * From T_AllPrimitive Order By Id Desc"
                });
                data.TableName = "T_AllPrimitive";
                IBulkInsert bulkInsert = new BulkInsert(dbSession);
                bulkInsert.Table = data;
                await bulkInsert.InsertAsync();
            }
        }

        [Fact]
        public async Task InsertByListAsync()
        {
            using (var dbSession = _dbSessionFactory.Open())
            {
                var list = await dbSession.QueryAsync<AllPrimitive>(new RequestContext
                {
                    RealSql = "Select Top(100) * From T_AllPrimitive Order By Id Desc"
                });
                IBulkInsert bulkInsert = new BulkInsert(dbSession);
                await bulkInsert.InsertAsync<AllPrimitive>(list);
            }
        }
    }
}