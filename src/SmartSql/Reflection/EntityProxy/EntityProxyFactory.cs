using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.Reflection.EntityProxy
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
            if (entityType.GetProperties().Any(p => !p.SetMethod.IsVirtual))
            {
                return entityType;
            }

            var virtualProperties = entityType.GetProperties().Where(p => p.SetMethod.IsVirtual).ToArray();

            string implName = $"{entityType.FullName}Proxy";
            var typeBuilder = _moduleBuilder.DefineType(implName, TypeAttributes.Public, entityType);
            typeBuilder.AddInterfaceImplementation(typeof(IEntityPropertyChangedTrackProxy));

            var changedVersionField =
                typeBuilder.DefineField("_changedVersion", DictionaryStringIntType.Type, FieldAttributes.Private);
            var enableTrackField = typeBuilder.DefineField("_enablePropertyChangedTrack", CommonType.Boolean,
                FieldAttributes.Private | FieldAttributes.HasDefault);

            BuildCtor(typeBuilder, entityType, changedVersionField, virtualProperties.Length);

            BuildGetEnableTrackMethod(typeBuilder, enableTrackField);
            BuildSetEnableTrackMethod(typeBuilder, enableTrackField);
            BuildClearPropertyVersionMethod(typeBuilder, changedVersionField);
            var onPropertyChangedMethodInfo =
                BuildOnPropertyChangedMethod(typeBuilder, enableTrackField, changedVersionField);
            BuildGetPropertyVersionMethod(typeBuilder, enableTrackField, changedVersionField);

            foreach (var propertyInfo in virtualProperties)
            {
                BuildSetMethod(typeBuilder, propertyInfo, onPropertyChangedMethodInfo);
            }

            return typeBuilder.CreateTypeInfo();
        }

        private MethodBuilder BuildSetEnableTrackMethod(TypeBuilder typeBuilder, FieldBuilder enableTrackField)
        {
            var setEnableTrackMethodBuilder = typeBuilder.DefineMethod(
                nameof(IEntityPropertyChangedTrackProxy.SetEnablePropertyChangedTrack),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
                CommonType.Void,
                new[] {CommonType.Boolean});
            var ilGen = setEnableTrackMethodBuilder.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.LoadArg(1);
            ilGen.FieldSet(enableTrackField);
            ilGen.Return();
            return setEnableTrackMethodBuilder;
        }

        private MethodBuilder BuildGetEnableTrackMethod(TypeBuilder typeBuilder, FieldBuilder enableTrackField)
        {
            var getEnableTrackMethodBuilder = typeBuilder.DefineMethod(
                nameof(IEntityPropertyChangedTrackProxy.GetEnablePropertyChangedTrack),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
                CommonType.Boolean,
                Type.EmptyTypes);
            var ilGen = getEnableTrackMethodBuilder.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.FieldGet(enableTrackField);
            ilGen.Return();
            return getEnableTrackMethodBuilder;
        }

        private MethodBuilder BuildClearPropertyVersionMethod(TypeBuilder typeBuilder, FieldBuilder changedVersionField)
        {
            var clearPropertyVersionBuilder = typeBuilder.DefineMethod(
                nameof(IEntityPropertyChangedTrackProxy.ClearPropertyVersion),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
                CommonType.Void,
                Type.EmptyTypes);
            var ilGen = clearPropertyVersionBuilder.GetILGenerator();
            ilGen.LoadArg(0);
            ilGen.FieldGet(changedVersionField);
            ilGen.Call(DictionaryStringIntType.Method.Clear);
            ilGen.Return();
            return clearPropertyVersionBuilder;
        }

        private void BuildCtor(TypeBuilder typeBuilder, Type entityType, FieldBuilder changedVersionField,
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
                ilGen.FieldSet(changedVersionField);

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

        private void BuildSetMethod(TypeBuilder typeBuilder, PropertyInfo propertyInfo,
            MethodInfo onPropertyChangedMethodInfo)
        {
            var setMethod = propertyInfo.SetMethod;
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
            ilGen.Call(onPropertyChangedMethodInfo);
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

        private MethodInfo BuildOnPropertyChangedMethod(TypeBuilder typeBuilder, FieldBuilder enableTrackField,
            FieldBuilder changedVersionField)
        {
            var onUpdatedBuilder = typeBuilder.DefineMethod(nameof(IEntityPropertyChangedTrackProxy.OnPropertyChanged),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot,
                CommonType.Void, new[] {CommonType.String});
            var ilGen = onUpdatedBuilder.GetILGenerator();
            var countLocalBuilder = ilGen.DeclareLocal(CommonType.Int32);

            CheckEnableTrack(enableTrackField, ilGen);

            ilGen.LoadArg(0);
            ilGen.FieldGet(changedVersionField);
            ilGen.LoadArg(1);
            ilGen.Emit(OpCodes.Ldloca_S, countLocalBuilder);
            ilGen.Call(DictionaryStringIntType.Method.TryGetValue);
            var containedLabel = ilGen.DefineLabel();
            ilGen.IfFalseS(containedLabel);

            ilGen.LoadArg(0);
            ilGen.FieldGet(changedVersionField);
            ilGen.LoadArg(1);
            ilGen.LoadLocalVar(0);
            ilGen.LoadInt32(1);
            ilGen.Add();
            ilGen.Call(DictionaryStringIntType.Method.IndexerSet);
            ilGen.Return();

            ilGen.MarkLabel(containedLabel);
            ilGen.LoadArg(0);
            ilGen.FieldGet(changedVersionField);
            ilGen.LoadArg(1);
            ilGen.LoadInt32(1);
            ilGen.Call(DictionaryStringIntType.Method.Add);
            ilGen.Return();
            return onUpdatedBuilder;
        }

        private static void CheckEnableTrack(FieldBuilder enableTrackField, ILGenerator ilGen,
            int? returnVal = null)
        {
            ilGen.LoadArg(0);
            ilGen.FieldGet(enableTrackField);
            var enableTrackLabel = ilGen.DefineLabel();
            ilGen.IfTrueS(enableTrackLabel);
            if (returnVal.HasValue)
            {
                ilGen.LoadInt32(returnVal.Value);
            }

            ilGen.Return();
            ilGen.MarkLabel(enableTrackLabel);
        }

        private void BuildGetPropertyVersionMethod(TypeBuilder typeBuilder, FieldBuilder enableTrackField,
            FieldBuilder changedVersionField)
        {
            var getStateMethodBuilder = typeBuilder.DefineMethod(
                nameof(IEntityPropertyChangedTrackProxy.GetPropertyVersion),
                MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig |
                MethodAttributes.Virtual | MethodAttributes.NewSlot,
                CommonType.Int32, new[] {CommonType.String});
            var ilGen = getStateMethodBuilder.GetILGenerator();
            var countLocalBuilder = ilGen.DeclareLocal(CommonType.Int32);

            CheckEnableTrack(enableTrackField, ilGen, 0);

            ilGen.LoadArg(0);
            ilGen.FieldGet(changedVersionField);
            ilGen.LoadArg(1);
            ilGen.Emit(OpCodes.Ldloca_S, countLocalBuilder);
            ilGen.Call(DictionaryStringIntType.Method.TryGetValue);

            var containedLabel = ilGen.DefineLabel();
            ilGen.IfTrueS(containedLabel);
            ilGen.LoadInt32(0);
            ilGen.Return();
            ilGen.MarkLabel(containedLabel);
            ilGen.LoadLocalVar(0);
            ilGen.Return();
        }
    }
}