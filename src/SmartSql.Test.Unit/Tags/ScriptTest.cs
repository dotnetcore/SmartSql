using SmartSql.Data;
using SmartSql.ScriptTag;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Tags
{
    public class ScriptTest
    {
        [Fact]
        public void And()
        {
            Script script = new Script("Ids!=null and Ids.length==1 and Ids[0]==1");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] {1});
            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();
            var isCondition = script.IsCondition(requestContext);
            Assert.True(isCondition);
        }

        [Fact]
        public void Or()
        {
            Script script = new Script("Ids!=null || Name!=null or Names!=null");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] {1});
            sqlParams.TryAdd("Name", "SmartSql");
            sqlParams.TryAdd("Names", new string[] {"SmartSql"});
            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();
            var isCondition = script.IsCondition(requestContext);
            Assert.True(isCondition);
        }

        [Fact]
        public void ArrayIndex()
        {
            SqlParameterCollection sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] {1});
            Script script = new Script("Ids[0]==1");
            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();
            var isCondition = script.IsCondition(requestContext);
            Assert.True(isCondition);
        }

        [Fact]
        public void Eq()
        {
            Script script = new Script("1==1");
            var isCondition = script.IsCondition(new RequestContext { });
            Assert.True(isCondition);
        }


        [Fact]
        public void GreatThen()
        {
            Script script = new Script("Ids!=null and Ids.length>0 and Ids.length gt 0");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] {1});

            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();
            var isCondition = script.IsCondition(requestContext);
            Assert.True(isCondition);
        }

        [Fact]
        public void LessThen()
        {
            Script script = new Script("Ids!=null and Ids.length<2 and Ids.length lt 2");
            var sqlParams = new SqlParameterCollection();
            sqlParams.TryAdd("Ids", new long[] {1});

            var requestContext = new RequestContext
            {
                Request = sqlParams
            };
            requestContext.SetupParameters();
            var isCondition = script.IsCondition(requestContext);
            Assert.True(isCondition);
        }
    }
}