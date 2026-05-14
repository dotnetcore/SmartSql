using System;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using SmartSql.Configuration;
using SmartSql.DataSource;
using SmartSql.DbSession;
using Xunit;

namespace SmartSql.Test.Unit.DbSession
{
    public class DbSessionFactoryTests
    {
        private static DbProvider CreateSqliteDbProvider()
        {
            return new DbProvider
            {
                Name = DbProvider.SQLITE,
                ParameterPrefix = "@",
                Type = "Microsoft.Data.Sqlite.SqliteFactory,Microsoft.Data.Sqlite",
                Factory = SqliteFactory.Instance
            };
        }

        private static SmartSqlConfig CreateTestConfig()
        {
            var dbProvider = CreateSqliteDbProvider();
            return new SmartSqlConfig
            {
                Database = new Database
                {
                    DbProvider = dbProvider,
                    Write = new WriteDataSource
                    {
                        Name = "Write",
                        ConnectionString = "Data Source=:memory:",
                        DbProvider = dbProvider
                    }
                }
            };
        }

        [Fact]
        public void Should_StoreConfig_When_Constructed()
        {
            var config = CreateTestConfig();

            var factory = new DbSessionFactory(config);

            factory.SmartSqlConfig.Should().BeSameAs(config);
        }

        [Fact]
        public void Should_CreateSession_When_OpenCalled()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);

            var session = factory.Open();

            session.Should().NotBeNull();
            session.Should().BeAssignableTo<IDbSession>();
        }

        [Fact]
        public void Should_FireOpenedEvent_When_OpenCalled()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);
            IDbSession openedSession = null;
            factory.Opened += (sender, args) => openedSession = args.DbSession;

            var session = factory.Open();

            openedSession.Should().BeSameAs(session);
        }

        [Fact]
        public void Should_NotFireOpenedEvent_When_NoSubscribers()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);

            var act = () => factory.Open();

            act.Should().NotThrow();
        }

        [Fact]
        public void Should_CreateSessionWithDataSource_When_OpenWithDataSourceCalled()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);
            var dataSource = new WriteDataSource
            {
                Name = "CustomWrite",
                ConnectionString = "Data Source=:memory:",
                DbProvider = CreateSqliteDbProvider()
            };

            var session = factory.Open(dataSource);

            session.Should().NotBeNull();
            session.DataSource.Should().BeSameAs(dataSource);
        }

        [Fact]
        public void Should_CreateSessionWithConnectionString_When_OpenWithConnectionStringCalled()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);

            var session = factory.Open("Data Source=:memory:");

            session.Should().NotBeNull();
            session.DataSource.Should().NotBeNull();
            session.DataSource.ConnectionString.Should().Be("Data Source=:memory:");
        }

        [Fact]
        public void Should_SetWriteDataSourceName_When_OpenWithConnectionStringCalled()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);

            var session = factory.Open("Data Source=:memory:");

            session.DataSource.Name.Should().Be("Write");
        }

        [Fact]
        public void Should_PreserveDbProvider_When_OpenWithConnectionStringCalled()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);

            var session = factory.Open("Data Source=:memory:");

            session.DataSource.DbProvider.Should().BeSameAs(config.Database.DbProvider);
        }

        [Fact]
        public void Should_CreateUniqueSessions_When_OpenCalledMultipleTimes()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);

            var session1 = factory.Open();
            var session2 = factory.Open();

            session1.Id.Should().NotBe(session2.Id);
        }

        [Fact]
        public void Should_FireOpenedEventWithSender_When_OpenCalled()
        {
            var config = CreateTestConfig();
            var factory = new DbSessionFactory(config);
            object eventSender = null;
            factory.Opened += (sender, args) => eventSender = sender;

            factory.Open();

            eventSender.Should().BeSameAs(factory);
        }
    }
}
