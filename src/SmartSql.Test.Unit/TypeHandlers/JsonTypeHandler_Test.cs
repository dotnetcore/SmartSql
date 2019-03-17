using SmartSql.Test.Entities;
using SmartSql.TypeHandler;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class JsonTypeHandler_Test
    {
        [Fact]
        public void Create()
        {
            var jsonTypeHandlerGenericType = Assembly.Load("SmartSql.TypeHandler").GetType("SmartSql.TypeHandler.JsonTypeHandler`1");
            var userJsonTypeHandlerType = jsonTypeHandlerGenericType.MakeGenericType(typeof(User));
            var userJsonTypeHandler = Activator.CreateInstance(userJsonTypeHandlerType) as JsonTypeHandler<User>;

        }


    }
}
