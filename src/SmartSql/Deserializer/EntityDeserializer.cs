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
using SmartSql.Configuration;
using SmartSql.Reflection;
using SmartSql.TypeHandlers;
using SmartSql.Utils;

namespace SmartSql.Deserializer
{
    public class EntityDeserializer : IDataReaderDeserializer
    {
        public bool CanDeserialize(ExecutionContext executionContext, Type resultType, bool isMultiple = false)
        {
            return true;
        }

        public TResult ToSinge<TResult>(ExecutionContext executionContext)
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

        public async Task<TResult> ToSingeAsync<TResult>(ExecutionContext executionContext)
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

        private Func<DataReaderWrapper, AbstractRequestContext, TResult> GetDeserialize<TResult>(ExecutionContext executionContext)
        {
            var key = GenerateKey(executionContext);
            return CacheUtil<TypeWrapper<EntityDeserializer, TResult>, String, Delegate>.GetOrAdd(key,
                   _ => CreateDeserialize<TResult>(executionContext))
                   as Func<DataReaderWrapper, AbstractRequestContext, TResult>;
        }
        private Delegate CreateDeserialize<TResult>(ExecutionContext executionContext)
        {
            var resultType = typeof(TResult);
            var dataReader = executionContext.DataReaderWrapper;

            var resultMap = executionContext.Request.GetCurrentResultMap();

            var constructorMap = resultMap?.Constructor;
            var columns = Enumerable.Range(0, dataReader.FieldCount)
                .Select(i => new { Index = i, Name = dataReader.GetName(i), FieldType = dataReader.GetFieldType(i) })
                .ToDictionary((col) => col.Name);

            var deserFunc = new DynamicMethod("Deserialize" + Guid.NewGuid().ToString("N"), resultType, new[] { DataType.DataReaderWrapper, RequestContextType.AbstractType }, resultType, true);
            var ilGen = deserFunc.GetILGenerator();
            ilGen.DeclareLocal(resultType);
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
                    LoadPropertyValue(ilGen, executionContext, col.Index, arg.CSharpType, col.FieldType, null);
                }
            }
            if (resultCtor == null)
            {
                throw new SmartSqlException($"No parameterless constructor defined for the target type: [{resultType.FullName}]");
            }
            ilGen.New(resultCtor);
            #endregion
            ilGen.StoreLocalVar(0);
            foreach (var col in columns)
            {
                var colName = col.Key;
                var propertyName = colName;
                var colIndex = col.Value.Index;
                var filedType = col.Value.FieldType;
                Property resultProperty = null;
                if (resultMap?.Properties != null && resultMap.Properties.TryGetValue(colName, out resultProperty))
                {
                    propertyName = resultProperty.Name;
                }
                var property = resultType.GetProperty(propertyName);
                if (property == null) { continue; }
                if (!property.CanWrite) { continue; }
                var propertyType = property.PropertyType;
                ilGen.LoadLocalVar(0);
                LoadPropertyValue(ilGen, executionContext, colIndex, propertyType, filedType, resultProperty);
                ilGen.Call(property.SetMethod);
            }

            ilGen.LoadLocalVar(0);
            ilGen.Return();
            return deserFunc.CreateDelegate(typeof(Func<DataReaderWrapper, AbstractRequestContext, TResult>));
        }
        private void LoadPropertyValue(ILGenerator ilGen, ExecutionContext executionContext, int colIndex, Type propertyType, Type fieldType, Property resultProperty)
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
            if (resultProperty?.Handler == null)
            {
                LoadTypeHandlerInvokeArgs(ilGen, colIndex, propertyType);
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
                            //throw new SmartSqlException($"Can not find TypeHandler:{nameof(ITypeHandler.PropertyType)}:{propertyType.FullName},{nameof(ITypeHandler.FieldType)}:{mappedFieldType.FullName}");
                        }
                    }
                }
                getValMethod = TypeHandlerCacheType.GetGetValueMethod(propertyType, mappedFieldType);
                ilGen.Call(getValMethod);
            }
            else
            {
                var typeHandlerField = NamedTypeHandlerCache.GetTypeHandlerField(executionContext.SmartSqlConfig.Alias, resultProperty.TypeHandler);
                ilGen.FieldGet(typeHandlerField);
                LoadTypeHandlerInvokeArgs(ilGen, colIndex, propertyType);
                getValMethod = resultProperty.Handler.GetType().GetMethod("GetValue");
                ilGen.Callvirt(getValMethod);
            }
        }

        private void LoadTypeHandlerInvokeArgs(ILGenerator ilGen, int colIndex, Type propertyType)
        {
            ilGen.LoadArg(0);
            ilGen.LoadInt32(colIndex);
            ilGen.LoadType(propertyType);
        }

        public String GenerateKey(ExecutionContext executionContext)
        {
            return
                $"Index:{executionContext.DataReaderWrapper.ResultIndex}_{(executionContext.Request.IsStatementSql ? executionContext.Request.FullSqlId : executionContext.Request.RealSql)}";
        }
    }
}
