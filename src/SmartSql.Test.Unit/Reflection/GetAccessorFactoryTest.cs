using SmartSql.Reflection.PropertyAccessor;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class GetAccessorFactoryTest
    {
        [Fact]
        public void Get_Id()
        {
            var obj = new AllPrimitive { };
            EmitGetAccessorFactory getAccessorFactory = new EmitGetAccessorFactory();
            var get_Id = getAccessorFactory.Create(typeof(AllPrimitive), nameof(AllPrimitive.Id));
            var id = get_Id(obj);
            Assert.Equal(id, obj.Id);
        }

        [Fact]
        public void Get_Int32()
        {
            var obj = new AllPrimitive { };
            EmitGetAccessorFactory getAccessorFactory = new EmitGetAccessorFactory();
            var get_Id = getAccessorFactory.Create(typeof(AllPrimitive), nameof(AllPrimitive.Int32));
            var id = get_Id(obj);
            Assert.Equal(id, obj.Int32);
        }

        [Fact]
        public void Get_String()
        {
            var obj = new AllPrimitive {String = "SmartSql"};
            EmitGetAccessorFactory getAccessorFactory = new EmitGetAccessorFactory();
            var get_String = getAccessorFactory.Create(typeof(AllPrimitive), nameof(AllPrimitive.String));
            var str = get_String(obj);
            Assert.Equal(str, obj.String);
        }
    }
}