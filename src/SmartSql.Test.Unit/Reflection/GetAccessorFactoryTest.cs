using SmartSql.Reflection.PropertyAccessor;
using SmartSql.Test.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SmartSql.Reflection;
using Xunit;

namespace SmartSql.Test.Unit.Reflection
{
    public class GetAccessorFactoryTest
    {
        [Fact]
        public void Get_Id()
        {
            var obj = new AllPrimitive { };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(typeof(AllPrimitive), nameof(AllPrimitive.Id), out var get_Id);
            var id = get_Id(obj);
            Assert.Equal(id, obj.Id);
        }

        [Fact]
        public void Get_Int32()
        {
            var obj = new AllPrimitive { };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(typeof(AllPrimitive), nameof(AllPrimitive.Int32), out var get_Id);
            var id = get_Id(obj);
            Assert.Equal(id, obj.Int32);
        }

        [Fact]
        public void Get_String()
        {
            var obj = new AllPrimitive {String = "SmartSql"};
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(typeof(AllPrimitive), nameof(AllPrimitive.String), out var get_String);
            var str = get_String(obj);
            Assert.Equal(str, obj.String);
        }

        [Fact]
        public void Get_Nest1()
        {
            var obj = new
            {
                User = new
                {
                    Id = 1
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Id", out var getMethodImpl);
            var id = getMethodImpl(obj);
            Assert.Equal(id, obj.User.Id);
        }

        [Fact]
        public void Get_Nest1_Obj()
        {
            var obj = new
            {
                User = new
                {
                    Info = new
                    {
                        Id = 1
                    }
                }
            };

            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Info", out var getMethodImpl);
            var info = getMethodImpl(obj);
            Assert.Equal(info, obj.User.Info);
        }

        [Fact]
        public void Get_Nest2()
        {
            var obj = new
            {
                User = new
                {
                    Info = new
                    {
                        Id = 1
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Info.Id", out var getMethodImpl);
            var id = getMethodImpl(obj);
            Assert.Equal(id, obj.User.Info.Id);
        }

        [Fact]
        public void Get_Dictionary_Index_String()
        {
            var obj = new
            {
                User = new
                {
                    Items = new Dictionary<string, int>
                    {
                        {"Id", 1}
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Items[Id]", out var getMethodImpl);
            var id = getMethodImpl(obj);
            Assert.Equal(obj.User.Items["Id"], id);
        }

        [Fact]
        public void Get_Dictionary_Index()
        {
            var obj = new
            {
                User = new
                {
                    Items = new Dictionary<int, int>
                    {
                        {1, 1}
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Items[1]", out var getMethodImpl);
            var id = getMethodImpl(obj);
            Assert.Equal(obj.User.Items[1], id);
        }

        [Fact]
        public void Get_List_Index()
        {
            var obj = new
            {
                User = new
                {
                    Items = new List<String>
                    {
                        "SmartSql"
                    }
                }
            };
            IGetAccessorFactory getAccessorFactory = EmitGetAccessorFactory.Instance;
            getAccessorFactory.TryCreate(obj.GetType(), "User.Items[0]", out var getMethodImpl);
            var id = getMethodImpl(obj);
            Assert.Equal(obj.User.Items[0], id);
        }
    }
}