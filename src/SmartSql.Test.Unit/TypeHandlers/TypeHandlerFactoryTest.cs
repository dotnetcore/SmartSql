using System;
using System.Threading.Tasks;
using SmartSql.Test.Entities;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class TypeHandlerFactoryTest
    {
        TypeHandlerFactory typeHandlerFactory = new TypeHandlerFactory();
        Type enumType = typeof(NumericalEnum);

        [Fact]
        public void TestEnumType()
        {
            var typeHandler = typeHandlerFactory.GetTypeHandler(enumType);

            Assert.NotNull(typeHandler);
        }


        [Fact]
        public void TestConcurrentRegisterEnum()
        {
            var enumType = typeof(NumericalEnum);

            var taskMax = 200;
            var current = 0;
            var tasks = new Task[taskMax];
            while (current < taskMax)
            {
                var task = new Task(() =>
                {
                    var typeHandler = typeHandlerFactory.GetTypeHandler(enumType);
                });
                tasks[current] = task;
                task.Start();
                current++;
            }

            try
            {
                Task.WaitAll(tasks);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var typeHandler = typeHandlerFactory.GetTypeHandler(enumType);
                throw;
            }

        }
    }
}