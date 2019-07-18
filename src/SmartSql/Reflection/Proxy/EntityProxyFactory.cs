using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.Reflection.Proxy
{
    public class EntityProxyFactory
    {
        private AssemblyBuilder _assemblyBuilder;
        private ModuleBuilder _moduleBuilder;

        public static EntityProxyFactory Instance = new EntityProxyFactory();

        public EntityProxyFactory()
        {
            Init();
        }

        private void Init()
        {
            string assemblyName = "SmartSql.EntityProxy";
            _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
            {
                Name = assemblyName
            }, AssemblyBuilderAccess.Run);
            _moduleBuilder = _assemblyBuilder.DefineDynamicModule(assemblyName + ".dll");
        }

        public Type CreateProxyType(Type entityType)
        {
            string implName = $"{entityType.FullName}_Proxy_{Guid.NewGuid():N}";
            var typeBuilder = _moduleBuilder.DefineType(implName, TypeAttributes.Public, entityType);
            typeBuilder.AddInterfaceImplementation(typeof(IEntityProxy));
            var updateStatesField =
                typeBuilder.DefineField("_updateStates", DictionaryStringIntType.Type, FieldAttributes.Private);
            var properties = entityType.GetProperties();
            BuildCtor(typeBuilder, entityType, updateStatesField, properties.Length);
            var onUpdatedMethodInfo = BuildOnUpdatedMethod(typeBuilder, updateStatesField);
            BuildGetStateMethod(typeBuilder, updateStatesField);

            foreach (var propertyInfo in properties)
            {
                BuildSetMethod(typeBuilder, propertyInfo, onUpdatedMethodInfo);
            }

            return typeBuilder.CreateTypeInfo();
        }

        private void BuildCtor(TypeBuilder typeBuilder, Type entityType, FieldBuilder updateStatesField,
            int propertyCount)
        {
            var entityCtors = entityType.GetConstructors();
            foreach (var entityCtor in entityCtors)
            {
                var paramTypes = entityCtor.GetParameters().Select(p => p.ParameterType).ToArray();
                var ctorBuilder = typeBuilder.DefineConstructor(
                    entityCtor.Attributes,
                    entityCtor.CallingConvention, paramTypes);
                var ilGen = ctorBuilder.GetILGenerator();

                ilGen.LoadArg(0);
                ilGen.LoadInt32(propertyCount);
                ilGen.New(DictionaryStringIntType.Ctor.Capacity);
                ilGen.FieldSet(updateStatesField);
                
                ilGen.LoadArg(0);
                if (paramTypes.Length > 0)
                {
                    for (int argIndex = 1; argIndex <= paramTypes.Length; argIndex++)
                    {
                        ilGen.LoadArg(argIndex);
                    }
                }
                ilGen.Call(entityCtor);
                ilGen.Return();
            }
        }

        private void BuildSetMethod(TypeBuilder typeBuilder, PropertyInfo propertyInfo, MethodInfo onUpdatedMethodInfo)
        {
            var setMethod = propertyInfo.SetMethod;
            if (!setMethod.IsVirtual)
            {
                return;
            }

            var paramTypes = setMethod.GetParameters().Select(p => p.ParameterType).ToArray();
            var getMethodBuilder = typeBuilder.DefineMethod(setMethod.Name,
                setMethod.Attributes ^ MethodAttributes.NewSlot,
                setMethod.ReturnType, paramTypes);
            var ilGen = getMethodBuilder.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.LoadArg(1);
            ilGen.Call(setMethod);

            ilGen.LoadArg(0);
            ilGen.LoadString(propertyInfo.Name);
            ilGen.Call(onUpdatedMethodInfo);
            ilGen.Return();
        }

        private void BuildGetMethod(TypeBuilder typeBuilder, PropertyInfo propertyInfo)
        {
            var getMethod = propertyInfo.GetGetMethod();
            var getMethodBuilder = typeBuilder.DefineMethod(getMethod.Name,
                getMethod.Attributes ^ MethodAttributes.NewSlot,
                getMethod.ReturnType, Type.EmptyTypes);
            var ilGen = getMethodBuilder.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.Call(getMethod);
            ilGen.Return();
        }

        private MethodInfo BuildOnUpdatedMethod(TypeBuilder typeBuilder, FieldBuilder updateStatesField)
        {
            var onUpdatedBuilder = typeBuilder.DefineMethod("OnUpdated",
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot,
                CommonType.Void, new[] {CommonType.String});
            var ilGen = onUpdatedBuilder.GetILGenerator();
            var countLocalBuilder = ilGen.DeclareLocal(CommonType.Int32);
            ilGen.LoadArg(0);
            ilGen.FieldGet(updateStatesField);
            ilGen.LoadArg(1);
            ilGen.Emit(OpCodes.Ldloca_S, countLocalBuilder);
            ilGen.Call(DictionaryStringIntType.Method.TryGetValue);
            var containedLabel = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brfalse_S, containedLabel);

            ilGen.LoadArg(0);
            ilGen.FieldGet(updateStatesField);
            ilGen.LoadArg(1);
            ilGen.LoadLocalVar(0);
            ilGen.LoadInt32(1);
            ilGen.Add();
            ilGen.Call(DictionaryStringIntType.Method.IndexerSet);
            ilGen.Return();

            ilGen.MarkLabel(containedLabel);
            ilGen.LoadArg(0);
            ilGen.FieldGet(updateStatesField);
            ilGen.LoadArg(1);
            ilGen.LoadInt32(1);
            ilGen.Call(DictionaryStringIntType.Method.Add);
            ilGen.Return();
            return onUpdatedBuilder;
        }

        private void BuildGetStateMethod(TypeBuilder typeBuilder, FieldBuilder updateStatesField)
        {
            var getStateMethodBuilder = typeBuilder.DefineMethod("GetState",
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot,
                CommonType.Int32, new[] {CommonType.String});
            var ilGen = getStateMethodBuilder.GetILGenerator();
            var countLocalBuilder = ilGen.DeclareLocal(CommonType.Int32);
            ilGen.LoadArg(0);
            ilGen.FieldGet(updateStatesField);
            ilGen.LoadArg(1);
            ilGen.Emit(OpCodes.Ldloca_S, countLocalBuilder);
            ilGen.Call(DictionaryStringIntType.Method.TryGetValue);

            var containedLabel = ilGen.DefineLabel();
            ilGen.Emit(OpCodes.Brtrue_S, containedLabel);
            ilGen.LoadInt32(0);
            ilGen.Return();
            ilGen.MarkLabel(containedLabel);
            ilGen.LoadLocalVar(0);
            ilGen.Return();
        }
    }
}