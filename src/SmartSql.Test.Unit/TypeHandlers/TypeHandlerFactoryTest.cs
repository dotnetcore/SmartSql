using System;
using System.Threading.Tasks;
using SmartSql.Test.Entities;
using SmartSql.TypeHandlers;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class TypeHandlerFactoryTest
    {
        private readonly TypeHandlerFactory _typeHandlerFactory = new TypeHandlerFactory();
        private readonly Type _enumType = typeof(NumericalEnum);

        [Fact]
        public void GetEnumTypeHandler()
        {
            var typeHandler = _typeHandlerFactory.GetTypeHandler(_enumType);
            Assert.Equal(typeof(EnumTypeHandler<NumericalEnum>), typeHandler.GetType());
        }

        [Fact]
        public void ConcurrentGetEnumTypeHandler()
        {
            var taskMax = 200;
            var current = 0;
            var tasks = new Task[taskMax];
            while (current < taskMax)
            {
                var task = new Task(() =>
                {
                    var typeHandler = _typeHandlerFactory.GetTypeHandler(_enumType);
                });
                tasks[current] = task;
                task.Start();
                current++;
            }

            Task.WaitAll(tasks);
        }
    }
}