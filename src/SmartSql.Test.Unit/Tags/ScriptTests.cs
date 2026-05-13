using FluentAssertions;
using SmartSql.Configuration.Tags;
using SmartSql.Data;
using SmartSql.ScriptTag;
using System.Collections.Generic;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class ScriptTests
    {
        [Fact]
        public void Should_ReturnTrue_When_ScriptAndConditionMet()
        {
            Script script = new Script("Ids!=null and Ids.length==1 and Ids[0]==1");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] { 1 });
            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();

            var isCondition = script.IsCondition(requestContext);

            isCondition.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ScriptOrConditionMet()
        {
            Script script = new Script("Ids!=null || Name!=null or Names!=null");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] { 1 });
            sqlParams.TryAdd("Name", "SmartSql");
            sqlParams.TryAdd("Names", new string[] { "SmartSql" });
            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();

            var isCondition = script.IsCondition(requestContext);

            isCondition.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ScriptArrayIndexConditionMet()
        {
            SqlParameterCollection sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] { 1 });
            Script script = new Script("Ids[0]==1");
            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();

            var isCondition = script.IsCondition(requestContext);

            isCondition.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ScriptEqualityHolds()
        {
            Script script = new Script("1==1");

            var isCondition = script.IsCondition(new RequestContext { });

            isCondition.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ScriptGreaterThanConditionMet()
        {
            Script script = new Script("Ids!=null and Ids.length>0 and Ids.length gt 0");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] { 1 });

            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();

            var isCondition = script.IsCondition(requestContext);

            isCondition.Should().BeTrue();
        }

        [Fact]
        public void Should_ReturnTrue_When_ScriptLessThanConditionMet()
        {
            Script script = new Script("Ids!=null and Ids.length<2 and Ids.length lt 2");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] { 1 });

            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();

            var isCondition = script.IsCondition(requestContext);

            isCondition.Should().BeTrue();
        }
    }
}
