using FluentAssertions;
using SmartSql.Test.Unit.TestEntities;
using SmartSql.TypeHandlers;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SmartSql.Test.Unit.TypeHandlers
{
    public class TypeHandlerFactoryTests
    {
        private readonly TypeHandlerFactory _typeHandlerFactory = new TypeHandlerFactory();
        private readonly Type _enumType = typeof(NumericalEnum);

        [Fact]
        public void Should_ResolveEnumTypeHandler_When_EnumTypeRequested()
        {
            var typeHandler = _typeHandlerFactory.GetTypeHandler(_enumType);

            typeHandler.Should().BeOfType<EnumTypeHandler<NumericalEnum>>();
        }

        [Fact]
        public void Should_BeThreadSafe_When_ConcurrentTypeHandlerResolution()
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
