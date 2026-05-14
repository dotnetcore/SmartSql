using FluentAssertions;
using SmartSql.Configuration;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.DataSource;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class IsPropertyTests
    {
        private RequestContext CreateContext(string property, object value)
        {
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd(property, value);
            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();
            return context;
        }

        private RequestContext CreateEmptyContext()
        {
            var sqlParams = new SqlParameterCollection();
            var context = new RequestContext
            {
                Request = sqlParams
            };
            context.SetupParameters();
            return context;
        }

        private static TestableRequestContext CreateTestableContext(int version, string property, object value)
        {
            var context = new TestableRequestContext { PropertyVersion = version };
            context.ExecutionContext = new ExecutionContext
            {
                SmartSqlConfig = new SmartSqlConfig
                {
                    Database = new Database
                    {
                        DbProvider = new DbProvider { ParameterPrefix = "@" }
                    }
                }
            };
            context.SetupParameters();
            if (property != null)
            {
                context.Parameters.TryAdd(property, value);
            }
            return context;
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyExistsAndPropertyChangedIsIgnore()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Ignore };
            var context = CreateContext("Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyDoesNotExist()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Ignore };
            var context = CreateEmptyContext();

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyExistsAndPropertyChangedIsChangedAndVersionIsPositive()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Changed };
            var context = CreateTestableContext(3, "Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyExistsAndPropertyChangedIsChangedAndVersionIsZero()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Changed };
            var context = CreateTestableContext(0, "Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyExistsAndPropertyChangedIsUnchangedAndVersionIsZero()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Unchanged };
            var context = CreateTestableContext(0, "Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyExistsAndPropertyChangedIsUnchangedAndVersionIsPositive()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Unchanged };
            var context = CreateTestableContext(5, "Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        [Fact]
        public void Should_ReturnTrue_When_PropertyExistsAndPropertyChangedIsChangedAndVersionIsMinusOne()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Changed };
            var context = CreateTestableContext(-1, "Name", "SmartSql");

            var result = tag.IsCondition(context);

            result.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnFalse_When_PropertyDoesNotExistRegardlessOfPropertyChanged()
        {
            var tag = new IsProperty { Property = "Name", PropertyChanged = PropertyChangedState.Changed };
            var context = CreateTestableContext(3, null, null);

            var result = tag.IsCondition(context);

            result.Should().BeFalse();
        }

        private class TestableRequestContext : RequestContext<object>
        {
            public int PropertyVersion { get; set; }

            public override int GetPropertyVersion(string propName)
            {
                return PropertyVersion;
            }
        }
    }
}
