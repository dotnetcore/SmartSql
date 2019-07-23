using SmartSql.Reflection.PropertyAccessor;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class SetAccessorFactoryTest
    {
        [Fact]
        public void Set_Id()
        {
            var obj = new AllPrimitive { };
            ISetAccessorFactory setAccessorFactory = EmitSetAccessorFactory.Instance;
            var set_Id = setAccessorFactory.Create(typeof(AllPrimitive), nameof(AllPrimitive.Id));
            set_Id(obj, 1L);
            Assert.Equal(1L, obj.Id);
        }

        [Fact]
        public void Set_Int32()
        {
            var obj = new AllPrimitive { };
            ISetAccessorFactory setAccessorFactory = EmitSetAccessorFactory.Instance;
            var set_Int32 = setAccessorFactory.Create(typeof(AllPrimitive), nameof(AllPrimitive.Int32));
            set_Int32(obj, 1);
            Assert.Equal(1, obj.Int32);
        }

    }
}
