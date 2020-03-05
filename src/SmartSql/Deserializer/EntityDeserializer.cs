using SmartSql.Data;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql.Annotations;
using SmartSql.Configuration;
using SmartSql.CUD;
using SmartSql.Reflection;
using SmartSql.Reflection.EntityProxy;
using SmartSql.TypeHandlers;
using SmartSql.Utils;

namespace SmartSql.Deserializer
{
    public class EntityDeserializer : IDataReaderDeserializer, ISetupSmartSql
    {
        private ILogger<EntityDeserializer> _logger;

        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
        {
            return true;
        }

        public TResult ToSingle<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default;
            var deser = GetDeserialize<TResult>(executionContext);
            dataReader.Read();
            return deser(dataReader, executionContext.Request);
        }

        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var deser = GetDeserialize<TResult>(executionContext);
            while (dataReader.Read())
            {
                var result = deser(dataReader, executionContext.Request);
                var entity = result;
                list.Add(entity);
            }

            return list;
        }

        public async Task<TResult> ToSingleAsync<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (dataReader.HasRows)
            {
                var deser = GetDeserialize<TResult>(executionContext);
                await dataReader.ReadAsync();
                return deser(dataReader, executionContext.Request);
            }

            return default;
        }

        public async Task<IList<TResult>> ToListAsync<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (dataReader.HasRows)
            {
                var deser = GetDeserialize<TResult>(executionContext);
                while (await dataReader.ReadAsync())
                {
                    var result = deser(dataReader, executionContext.Request);
                    var entity = result;
                    list.Add(entity);
                }
            }

            return list;
        }

        private Func<DataReaderWrapper, AbstractRequestContext, TResult> GetDeserialize<TResult>(
            ExecutionContext executionContext)
        {
            return CacheUtil<TypeWrapper<EntityDeserializer, TResult>, DeserializeIdentity, Delegate>.GetOrAdd(
                    DeserializeIdentity.Of(executionContext),
                    _ => CreateDeserialize<TResult>(executionContext))
                as Func<DataReaderWrapper, AbstractRequestContext, TResult>;
        }

        private Delegate CreateDeserialize<TResult>(ExecutionContext executionContext)
        {
            var resultType = typeof(TResult);
            if (executionContext.Request.EnablePropertyChangedTrack == true)
            {
                if (resultType.GetProperties().Any(p => !p.SetMethod.IsVirtual))
                {
                    _logger.LogWarning(
                        $"Type:{resultType.FullName} contain Non-Virtual Method,can not be enhanced by EntityProxy!");
                }
                else
                {
                    resultType = EntityProxyCache<TResult>.ProxyType;
                }
            }

            var dataReader = executionContext.DataReaderWrapper;
            var resultMap = executionContext.Request.GetCurrentResultMap();

            var constructorMap = resultMap?.Constructor;
            var columns = Enumerable.Range(0, dataReader.FieldCount)
                .Select(i => new ColumnDescriptor
                {
                    Index = i,
                    ColumnName = dataReader.GetName(i),
                    PropertyName = executionContext.Request.AutoConverter.Convert(dataReader.GetName(i)),
                    FieldType = dataReader.GetFieldType(i)
                })
                .ToDictionary(col => col.ColumnName);

            var deserFunc = new DynamicMethod("Deserialize" + Guid.NewGuid().ToString("N"), resultType,
                new[] {DataType.DataReaderWrapper, RequestContextType.AbstractType}, resultType, true);
            var ilGen = deserFunc.GetILGenerator();
            ilGen.DeclareLocal(resultType); // return value
            ilGen.DeclareLocal(CommonType.Int32); // current column index
            ilGen.DeclareLocal(CommonType.String); // current column name
            ilGen.DeclareLocal(CommonType.String); // current Property.Name

            #region New

            ConstructorInfo resultCtor = null;
            if (constructorMap == null)
            {
                resultCtor = resultType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            }
            else
            {
                var ctorArgTypes = constructorMap.Args.Select(arg => arg.CSharpType).ToArray();
                resultCtor = resultType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ctorArgTypes, null);
                foreach (var arg in constructorMap.Args)
                {
                    var col = columns[arg.Column];
                    StoreLocalColumnIndex(ilGen, col.Index);
                    LoadPropertyValue(ilGen, executionContext, arg.CSharpType, col.FieldType, null);
                }
            }

            if (resultCtor == null)
            {
                throw new SmartSqlException(
                    $"No parameterless constructor defined for the target type: [{resultType.FullName}]");
            }

            ilGen.New(resultCtor);

            #endregion

            ilGen.StoreLocalVar(0);

            var ignoreDbNull = executionContext.SmartSqlConfig.Settings.IgnoreDbNull;

            ilGen.BeginExceptionBlock();
            foreach (var col in columns)
            {
                #region Ensure Property & TypeHanlder

                if (!ResolveProperty<TResult>(resultMap, resultType, col.Value, out var propertyHolder)
                )
                {
                    continue;
                }

                if (!propertyHolder.CanWrite)
                {
                    continue;
                }

                #endregion

                var columnDescriptor = col.Value;
                var colIndex = columnDescriptor.Index;
                StoreLocalColumnIndex(ilGen, colIndex);
                StoreLocalColumnName(ilGen, col.Value.ColumnName);
                StoreLocalPropertyName(ilGen, propertyHolder.Property.Name);
                var isDbNullLabel = ilGen.DefineLabel();

                var propertyType = propertyHolder.PropertyType;
                if (ignoreDbNull)
                {
                    ilGen.LoadArg(0);
                    LoadLocalColumnIndex(ilGen);
                    ilGen.Call(DataType.Method.IsDBNull);
                    ilGen.IfTrueS(isDbNullLabel);
                }

                ilGen.LoadLocalVar(0);
                LoadPropertyValue(ilGen, executionContext, propertyType, columnDescriptor.FieldType,
                    propertyHolder.TypeHandler);
                ilGen.Call(propertyHolder.SetMethod);
                if (ignoreDbNull)
                {
                    ilGen.MarkLabel(isDbNullLabel);
                }
            }

            ilGen.BeginCatchBlock(typeof(Exception));

            ilGen.LoadLocalVar(0);
            LoadLocalColumnIndex(ilGen);
            LoadLocalColumnName(ilGen);
            LoadLocalPropertyName(ilGen);
            ilGen.LoadArg(0);
            ilGen.EmitCall(OpCodes.Call, THROW_DESERIALIZE_EXCEPTION, null);
            ilGen.EndExceptionBlock();
            if (typeof(IEntityPropertyChangedTrackProxy).IsAssignableFrom(resultType))
            {
                ilGen.LoadLocalVar(0);
                ilGen.LoadInt32(1);
                var setEnableTrackMethod =
                    resultType.GetMethod(nameof(IEntityPropertyChangedTrackProxy.SetEnablePropertyChangedTrack));
                ilGen.Call(setEnableTrackMethod);
            }

            ilGen.LoadLocalVar(0);
            ilGen.Return();
            return deserFunc.CreateDelegate(typeof(Func<DataReaderWrapper, AbstractRequestContext, TResult>));
        }


        private static void StoreLocalColumnIndex(ILGenerator ilGen, int colIndex)
        {
            ilGen.LoadInt32(colIndex);
            ilGen.StoreLocalVar(1);
        }

        private static void LoadLocalColumnIndex(ILGenerator ilGen)
        {
            ilGen.LoadLocalVar(1);
        }

        private static void StoreLocalColumnName(ILGenerator ilGen, String colName)
        {
            ilGen.LoadString(colName);
            ilGen.StoreLocalVar(2);
        }

        private static void LoadLocalColumnName(ILGenerator ilGen)
        {
            ilGen.LoadLocalVar(2);
        }

        private static void StoreLocalPropertyName(ILGenerator ilGen, String propertyName)
        {
            ilGen.LoadString(propertyName);
            ilGen.StoreLocalVar(3);
        }

        private static void LoadLocalPropertyName(ILGenerator ilGen)
        {
            ilGen.LoadLocalVar(3);
        }

        private static MethodInfo THROW_DESERIALIZE_EXCEPTION =
            typeof(EntityDeserializer).GetMethod(nameof(ThrowDeserializeException));

        public static void ThrowDeserializeException(Exception ex, Object result, int columnIndex, String columnName,
            String propertyName, IDataReader dataReader)
        {
            var errorMsg =
                $"Deserialize Error,Entity:[{result?.GetType().FullName}] PropertyName:[{propertyName}] -> ColumnIndex:[{columnIndex}],ColumnName:[{columnName}] ,Invalid DataReader ColumnName:[{dataReader.GetName(columnIndex)}]!";
            throw new SmartSqlException(errorMsg, ex);
        }

        private static bool ResolveProperty<TResult>(ResultMap resultMap, Type resultType,
            ColumnDescriptor columnDescriptor
            , out PropertyHolder propertyHolder)
        {
            propertyHolder = null;
            if (resultMap?.Properties != null)
            {
                if (resultMap.Properties.TryGetValue(columnDescriptor.ColumnName, out var resultProperty))
                {
                    propertyHolder = new PropertyHolder
                    {
                        Property = resultType.GetProperty(resultProperty.Name),
                        TypeHandler = resultProperty.TypeHandler
                    };
                    return true;
                }
            }

            if (EntityMetaDataCache<TResult>.TryGetColumnByColumnName(columnDescriptor.ColumnName,
                out var columnAttribute))
            {
                propertyHolder = new PropertyHolder
                {
                    Property = columnAttribute.Property,
                    TypeHandler = columnAttribute.TypeHandler
                };
                return true;
            }

            if (EntityMetaDataCache<TResult>.TryGetColumnByPropertyName(columnDescriptor.PropertyName,
                out columnAttribute))
            {
                propertyHolder = new PropertyHolder
                {
                    Property = columnAttribute.Property,
                    TypeHandler = columnAttribute.TypeHandler
                };
                return true;
            }

            return false;
        }

        private void LoadPropertyValue(ILGenerator ilGen, ExecutionContext executionContext,
            Type propertyType, Type fieldType, String typeHandler)
        {
            var typeHandlerFactory = executionContext.SmartSqlConfig.TypeHandlerFactory;
            var propertyUnderType = (Nullable.GetUnderlyingType(propertyType) ?? propertyType);
            var isEnum = propertyUnderType.IsEnum;

            #region Check Enum

            if (isEnum)
            {
                typeHandlerFactory.TryRegisterEnumTypeHandler(propertyType, out _);
            }

            #endregion

            MethodInfo getValMethod = null;
            if (String.IsNullOrEmpty(typeHandler))
            {
                LoadTypeHandlerInvokeArgs(ilGen, propertyType);
                var mappedFieldType = fieldType;
                if (isEnum)
                {
                    mappedFieldType = AnyFieldTypeType.Type;
                }
                else if (propertyUnderType != fieldType)
                {
                    if (!typeHandlerFactory.TryGetTypeHandler(propertyType, fieldType, out _))
                    {
                        mappedFieldType = AnyFieldTypeType.Type;
                        if (!typeHandlerFactory.TryGetTypeHandler(propertyType, mappedFieldType, out _))
                        {
                            propertyType = CommonType.Object;
                        }
                    }
                }

                getValMethod = TypeHandlerCacheType.GetGetValueMethod(propertyType, mappedFieldType);
                ilGen.Call(getValMethod);
            }
            else
            {
                var typeHandlerField =
                    NamedTypeHandlerCache.GetTypeHandlerField(executionContext.SmartSqlConfig.Alias, typeHandler);
                ilGen.FieldGet(typeHandlerField);
                LoadTypeHandlerInvokeArgs(ilGen, propertyType);
                getValMethod = executionContext.SmartSqlConfig.TypeHandlerFactory.GetTypeHandler(typeHandler).GetType()
                    .GetMethod("GetValue");
                ilGen.Callvirt(getValMethod);
            }
        }

        private void LoadTypeHandlerInvokeArgs(ILGenerator ilGen, Type propertyType)
        {
            ilGen.LoadArg(0);
            LoadLocalColumnIndex(ilGen);
            ilGen.LoadType(propertyType);
        }

        public void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _logger = smartSqlBuilder.SmartSqlConfig.LoggerFactory.CreateLogger<EntityDeserializer>();
        }


        public struct DeserializeIdentity : IEquatable<DeserializeIdentity>
        {
            public String Alias { get; }
            public int ResultIndex { get; }
            public String RealSql { get; }
            private readonly int _hashCode;

            public static DeserializeIdentity Of(ExecutionContext executionContext)
            {
                return new DeserializeIdentity(executionContext.SmartSqlConfig.Alias,
                    executionContext.DataReaderWrapper.ResultIndex,
                    executionContext.Request.RealSql);
            }

            public DeserializeIdentity(String @alias, int resultIndex, String realSql)
            {
                Alias = alias;
                ResultIndex = resultIndex;
                RealSql = realSql;
                unchecked
                {
                    _hashCode = 17;
                    _hashCode = Alias?.GetHashCode() ?? 0;
                    _hashCode = (_hashCode * 23) ^ ResultIndex;
                    _hashCode = (_hashCode * 23) ^ (RealSql?.GetHashCode() ?? 0);
                }
            }

            public bool Equals(DeserializeIdentity other)
            {
                return Alias == other.Alias
                       && ResultIndex == other.ResultIndex
                       && RealSql == other.RealSql;
            }

            public override bool Equals(object obj)
            {
                return obj is DeserializeIdentity other && Equals(other);
            }

            public override int GetHashCode() => _hashCode;
        }


        public class ColumnDescriptor
        {
            public int Index { get; set; }
            public String ColumnName { get; set; }
            public Type FieldType { get; set; }
            public String PropertyName { get; set; }
        }
    }
}