using SmartSql.Data;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace SmartSql.Deserializer
{
    public class EntityDeserializer : IDataReaderDeserializer
    {
        private readonly ConcurrentDictionary<String, Delegate> _deserCache = new ConcurrentDictionary<string, Delegate>();
        public TResult ToSinge<TResult>(ExecutionContext executionContext)
        {
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return default;
            var deser = GetDeserialize<TResult>(executionContext);
            dataReader.Read();
            return deser(dataReader);
        }

        public IList<TResult> ToList<TResult>(ExecutionContext executionContext)
        {
            var list = new List<TResult>();
            var dataReader = executionContext.DataReaderWrapper;
            if (!dataReader.HasRows) return list;
            var deser = GetDeserialize<TResult>(executionContext);
            while (dataReader.Read())
            {
                var result = deser(dataReader);
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
                return deser(dataReader);
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
                    var result = deser(dataReader);
                    var entity = result;
                    list.Add(entity);
                }
            }
            return list;
        }

        private Func<DataReaderWrapper, TReuslt> GetDeserialize<TReuslt>(ExecutionContext executionContext)
        {
            var key = GenerateKey(executionContext);
            if (!_deserCache.TryGetValue(key, out var deser))
            {
                lock (this)
                {
                    if (!_deserCache.TryGetValue(key, out deser))
                    {
                    deser = CreateDeserialize<TReuslt>(executionContext);
                        _deserCache.TryAdd(key, deser);
                    }
                }
            }
            return deser as Func<DataReaderWrapper, TReuslt>;
        }
        private Delegate CreateDeserialize<TReuslt>(ExecutionContext executionContext)
        {
            var resultType = typeof(TReuslt);
            var dataReader = executionContext.DataReaderWrapper;

            var resultMap = (executionContext.Request.ResultMap ?? executionContext.Request.MultipleResultMap?.Results.FirstOrDefault(m => m.Index == dataReader.ResultIndex)?.Map) ??
                            executionContext.Request.MultipleResultMap?.Root?.Map;
            var constructorMap = resultMap?.Constructor;
            var columns = Enumerable.Range(0, dataReader.FieldCount)
                .Select(i => new { Index = i, Name = dataReader.GetName(i) })
                .ToDictionary((col) => col.Name);

            var deserFunc = new DynamicMethod("Deserialize" + Guid.NewGuid().ToString("N"), resultType, new[] { DataType.DataReaderWrapper }, resultType, true);
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
                    LoadPropertyValue(ilGen, col.Index, arg.CSharpType);
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
                if (resultMap?.Properties != null && resultMap.Properties.TryGetValue(colName, out var result))
                {
                    propertyName = result.Name;
                }
                var property = resultType.GetProperty(propertyName);
                if (property == null) { continue; }
                if (!property.CanWrite) { continue; }
                var propertyType = property.PropertyType;
                ilGen.LoadLocalVar(0);
                #region Check Enum
                executionContext.SmartSqlConfig.TypeHandlerFactory.Get(propertyType);
                #endregion
                LoadPropertyValue(ilGen, colIndex, propertyType);

                ilGen.Call(property.SetMethod);
            }

            ilGen.LoadLocalVar(0);
            ilGen.Return();
            return deserFunc.CreateDelegate(typeof(Func<DataReaderWrapper, TReuslt>));
        }

        private void LoadPropertyValue(ILGenerator ilGen, int colIndex, Type propertyType)
        {
            ilGen.LoadArg(0);
            ilGen.LoadInt32(colIndex);
            var getValMethod = TypeHandlerCacheType.GetGetValueMethod(propertyType);
            ilGen.Call(getValMethod);
        }

        public String GenerateKey(ExecutionContext executionContext)
        {
            var statementKey = executionContext.Request.IsStatementSql ? executionContext.Request.FullSqlId : executionContext.Request.RealSql;
            return $"{statementKey}_{executionContext.Result.ResultType.FullName}";
        }

        private string GetColumnQueryString(IDataReader dataReader)
        {
            var columns = Enumerable.Range(0, dataReader.FieldCount)
                            .Select(i => $"({i}:{dataReader.GetName(i)}:{dataReader.GetFieldType(i).Name})");
            return String.Join("&", columns);
        }


    }
}
