using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using SmartSql.Abstractions;
using System.Collections;

namespace SmartSql.DyRepository
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private IDictionary<Type, object> cachedRepository = new Dictionary<Type, object>();

        private AssemblyBuilder assemblyBuilder;
        private ModuleBuilder moduleBuilder;
        private readonly IRepositoryBuilder RepositoryBuilder;

        public RepositoryFactory(IRepositoryBuilder RepositoryBuilder)
        {
            Init();
            this.RepositoryBuilder = RepositoryBuilder;
        }
        private void Init()
        {
            string assemblyName = "SmartSql.RepositoryImpl" + this.GetHashCode();
            assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = assemblyName
            }, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        public object CreateInstance(Type interfaceType, ISmartSqlMapper smartSqlMapper)
        {
            if (!cachedRepository.ContainsKey(interfaceType))
            {
                lock (this)
                {
                    var implType = RepositoryBuilder.BuildRepositoryImpl(interfaceType);
                    var paramTypes = new Type[] { typeof(ISmartSqlMapper) };
                    var ctorInfo = implType.GetConstructor(paramTypes);
                    var obj = ctorInfo.Invoke(new object[] { smartSqlMapper });
                    cachedRepository.Add(interfaceType, obj);
                }
            }
            return cachedRepository[interfaceType];
        }


        public T CreateInstance<T>(ISmartSqlMapper smartSqlMapper)
        {
            var interfaceType = typeof(T);
            return (T)CreateInstance(interfaceType, smartSqlMapper);
        }
    }
}
