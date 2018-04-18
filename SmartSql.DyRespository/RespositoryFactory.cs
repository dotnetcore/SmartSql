using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using SmartSql.Abstractions;
using System.Collections;

namespace SmartSql.DyRespository
{
    public class RespositoryFactory : IRespositoryFactory
    {
        private IDictionary<Type, object> cachedRespository = new Dictionary<Type, object>();

        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
        private readonly IRespositoryBuilder respositoryBuilder;

        public RespositoryFactory(IRespositoryBuilder respositoryBuilder)
        {
            Init();
            this.respositoryBuilder = respositoryBuilder;
        }
        private void Init()
        {
            string assemblyName = "SmartSql.RespositoryImpl" + this.GetHashCode();
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = assemblyName
            }, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        public object CreateInstance(Type interfaceType, ISmartSqlMapper smartSqlMapper)
        {
            if (!cachedRespository.ContainsKey(interfaceType))
            {
                lock (this)
                {
                    var implType = respositoryBuilder.BuildRespositoryImpl(interfaceType);
                    var paramTypes = new Type[] { typeof(ISmartSqlMapper) };
                    var ctorInfo = implType.GetConstructor(paramTypes);
                    var obj = ctorInfo.Invoke(new object[] { smartSqlMapper });
                    cachedRespository.Add(interfaceType, obj);
                }
            }
            return cachedRespository[interfaceType];
        }


        public T CreateInstance<T>(ISmartSqlMapper smartSqlMapper)
        {
            var interfaceType = typeof(T);
            return (T)CreateInstance(interfaceType, smartSqlMapper);
        }
    }
}
