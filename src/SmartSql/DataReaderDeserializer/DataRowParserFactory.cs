using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using SmartSql.Abstractions;
using SmartSql.Utils;
using System.Globalization;
using SmartSql.Exceptions;

namespace SmartSql.DataReaderDeserializer
{
    public class DataRowParserFactory
    {
        private readonly IDictionary<string, Func<IDataReader, RequestContext, object>> _cachedDeserializer = new Dictionary<string, Func<IDataReader, RequestContext, object>>();

        public DataRowParserFactory()
        {
        }

        private string GetColumnKey(IDataReader dataReader)
        {
            var columns = Enumerable.Range(0, dataReader.FieldCount)
                            .Select(i => $"({i}:{dataReader.GetName(i)}:{dataReader.GetFieldType(i).Name})");
            return String.Join("&", columns);
        }

        public Func<IDataReader, RequestContext, object> GetParser(IDataReaderWrapper dataReader, RequestContext requestContext, Type targetType)
        {
            string key = $"{requestContext.StatementKey}_{GetColumnKey(dataReader)}_{targetType.FullName}";
            if (!_cachedDeserializer.ContainsKey(key))
            {
                lock (this)
                {
                    if (!_cachedDeserializer.ContainsKey(key))
                    {
                        Func<IDataReader, RequestContext, object> deser = null;
                        if (targetType.IsValueType || targetType == TypeUtils.StringType)
                        {
                            deser = CreateValueTypeParserImpl(dataReader, requestContext, targetType);
                        }
                        else
                        {
                            deser = CreateParserImpl(dataReader, requestContext, targetType);
                        }
                        _cachedDeserializer.Add(key, deser);
                    }
                }
            }
            return _cachedDeserializer[key];
        }

        private MethodInfo GetRealValueMethod(Type valueType)
        {
            var typeCode = Type.GetTypeCode(valueType);
            switch (typeCode)
            {
                case TypeCode.Boolean:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetBoolean");
                    }
                case TypeCode.Byte:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetByte");
                    }
                case TypeCode.Char:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetChar");
                    }
                case TypeCode.DateTime:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetDateTime");
                    }
                case TypeCode.Decimal:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetDecimal");
                    }
                case TypeCode.Double:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetDouble");
                    }
                case TypeCode.Single:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetFloat");
                    }
                case TypeCode.Int16:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetInt16");
                    }
                case TypeCode.Int32:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetInt32");
                    }
                case TypeCode.Int64:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetInt64");
                    }
                case TypeCode.String:
                    {
                        return TypeUtils.DataRecordType.GetMethod("GetString");
                    }
                default:
                    {
                        if (valueType == typeof(Guid))
                        {
                            return TypeUtils.DataRecordType.GetMethod("GetGuid");
                        }
                        return null;
                    }
            }
        }

        private Func<IDataReader, RequestContext, object> CreateValueTypeParserImpl(IDataReader dataReader, RequestContext context, Type targetType)
        {
            var nullUnderType = Nullable.GetUnderlyingType(targetType);
            var realType = nullUnderType ?? targetType;
            int valIndex = 0;
            if (realType.IsEnum)
            {
                return (dr, ctx) =>
                {
                    var val = dr.GetValue(valIndex);
                    if (val is float || val is double || val is decimal)
                    {
                        val = Convert.ChangeType(val, Enum.GetUnderlyingType(realType), CultureInfo.InvariantCulture);
                    }
                    return val is DBNull ? null : Enum.ToObject(realType, val);
                };
            }
            return (dr, ctx) =>
            {
                var val = dr.GetValue(valIndex);
                if (val != null && val.GetType() != realType)
                {
                    return Convert.ChangeType(val, realType);
                }
                return val is DBNull ? null : val;
            };
        }

        private Func<IDataReader, RequestContext, object> CreateParserImpl(IDataReaderWrapper dataReader, RequestContext context, Type targetType)
        {
            var resultMap = context.ResultMap ?? context.MultipleResultMap?.Results.FirstOrDefault(m => m.Index == dataReader.ResultIndex)?.Map;
            if (resultMap == null)
            {
                resultMap = context.MultipleResultMap?.Root?.Map;
            }
            var constructorMap = resultMap?.Constructor;

            var columns = Enumerable.Range(0, dataReader.FieldCount)
                            .Select(i => new { Index = i, Name = dataReader.GetName(i) })
                            .ToDictionary((col) => col.Name);

            var dynamicMethod = new DynamicMethod("Deserialize" + Guid.NewGuid().ToString("N"), targetType, new[] { TypeUtils.DataReaderType, TypeUtils.RequestContextType }, targetType, true);
            var iLGenerator = dynamicMethod.GetILGenerator();
            iLGenerator.DeclareLocal(targetType);
            ConstructorInfo targetCtor = null;
            if (constructorMap == null)
            {
                targetCtor = targetType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            }
            else
            {
                var ctorArgTypes = constructorMap.Args.Select(arg => arg.ArgType).ToArray();
                targetCtor = targetType.GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ctorArgTypes, null);
                //load arg value
                foreach (var arg in constructorMap.Args)
                {
                    var col = columns[arg.Column];
                    EmitLoadColVal(iLGenerator, dataReader, col.Index, arg.TypeHandler, arg.ArgType, null);
                }
            }

            if (targetCtor == null)
                throw new SmartSqlException($"No parameterless constructor defined for the target type: \"{targetType.FullName}\"");

            iLGenerator.Emit(OpCodes.Newobj, targetCtor); // [target]
            iLGenerator.Emit(OpCodes.Stloc_0);

            foreach (var col in columns)
            {
                var colName = col.Key;
                var colIndex = col.Value.Index;
                var result = resultMap?.Properties?.FirstOrDefault(r => r.Column == colName);
                string propertyName = result != null ? result.Name : colName;
                var property = targetType.GetProperty(propertyName);
                if (property == null) { continue; }
                if (!property.CanWrite) { continue; }
                Type propertyType = property.PropertyType;
                Label isDbNullLabel = iLGenerator.DefineLabel();
                EmitLoadColVal(iLGenerator, dataReader, colIndex, result?.TypeHandler, propertyType, isDbNullLabel);
                iLGenerator.Emit(OpCodes.Call, property.SetMethod);// stack is empty
                iLGenerator.MarkLabel(isDbNullLabel);
            }

            iLGenerator.Emit(OpCodes.Ldloc_0);// stack is [rval]
            iLGenerator.Emit(OpCodes.Ret);

            var funcType = System.Linq.Expressions.Expression.GetFuncType(TypeUtils.DataReaderType, TypeUtils.RequestContextType, targetType);
            return (Func<IDataReader, RequestContext, object>)dynamicMethod.CreateDelegate(funcType);
        }

        private void EmitLoadColVal(ILGenerator iLGenerator, IDataReader dataReader, int colIndex, string typeHandler, Type toType, Label? isDbNullLabel)
        {
            var fieldType = dataReader.GetFieldType(colIndex);
            if (!String.IsNullOrEmpty(typeHandler))
            {
                if (isDbNullLabel != null)
                {
                    iLGenerator.Emit(OpCodes.Ldloc_0);// [target]
                }
                EmitGetTypeHanderValue(iLGenerator, colIndex, typeHandler, toType);
                if (toType.IsValueType)
                {
                    iLGenerator.Emit(OpCodes.Unbox_Any, toType);
                }
            }
            else
            {
                var nullUnderType = Nullable.GetUnderlyingType(toType);
                var realType = nullUnderType ?? toType;
                if (nullUnderType != null || realType == TypeUtils.StringType)
                {
                    iLGenerator.Emit(OpCodes.Ldarg_0);// [dataReader]
                    EmitUtils.LoadInt32(iLGenerator, colIndex);// [dataReader][index]
                    iLGenerator.Emit(OpCodes.Call, TypeUtils.IsDBNullMethod);//[isDbNull-value]
                    if (isDbNullLabel != null)
                    {
                        iLGenerator.Emit(OpCodes.Brtrue, isDbNullLabel.Value);//[empty]
                    }
                    else
                    {
                        iLGenerator.Emit(OpCodes.Ldnull);
                    }
                }
                var getRealValueMethod = GetRealValueMethod(fieldType);
                if (isDbNullLabel != null)
                {
                    iLGenerator.Emit(OpCodes.Ldloc_0);// [target]
                }
                if (realType.IsEnum)
                {
                    EmitUtils.TypeOf(iLGenerator, realType);//[target][enumType]
                }

                #region GetValue

                iLGenerator.Emit(OpCodes.Ldarg_0);// [dataReader]
                EmitUtils.LoadInt32(iLGenerator, colIndex);// [dataReader][index]
                if (getRealValueMethod != null)
                {
                    iLGenerator.Emit(OpCodes.Call, getRealValueMethod);//[prop-value]
                }
                else
                {
                    iLGenerator.Emit(OpCodes.Call, TypeUtils.GetValueMethod_DataRecord);//[prop-value]
                    if (fieldType.IsValueType)
                    {
                        iLGenerator.Emit(OpCodes.Unbox_Any, fieldType);
                    }
                }

                #endregion GetValue

                //[target][property-Value]
                if (realType.IsEnum)
                {
                    //[target][propertyType-enumType][property-Value]
                    EmitConvertEnum(iLGenerator, fieldType);
                    iLGenerator.Emit(OpCodes.Unbox_Any, realType);
                    //[target][property-Value(enum-value)]
                }
                else if (fieldType != realType)
                {
                    EmitUtils.ChangeType(iLGenerator, fieldType, realType);
                }
                if (nullUnderType != null)
                {
                    iLGenerator.Emit(OpCodes.Newobj, toType.GetConstructor(new[] { nullUnderType }));
                }
            }
        }

        private static readonly MethodInfo _getTypeHandlerMethod = TypeUtils.RequestContextType.GetMethod("GetTypeHandler");

        private void EmitGetTypeHanderValue(ILGenerator iLGenerator, int colIndex, string typeHandlerName, Type propertyType)
        {
            iLGenerator.Emit(OpCodes.Ldarg_1);// [RequestContext]
            iLGenerator.Emit(OpCodes.Ldstr, typeHandlerName);// [RequestContext][typeHandlerName]
            iLGenerator.Emit(OpCodes.Call, _getTypeHandlerMethod);//[ITypeHandler]
            iLGenerator.Emit(OpCodes.Ldarg_0);//[typeHandler][dataReader]
            EmitUtils.LoadInt32(iLGenerator, colIndex);// [typeHandler][dataReader][index]
            EmitUtils.TypeOf(iLGenerator, propertyType);//[typeHandler][dataReader][index][propertyType]
            iLGenerator.Emit(OpCodes.Call, TypeUtils.GetValueMethod_TypeHandler);// [property-Value]
        }

        private void EmitConvertEnum(ILGenerator iLGenerator, Type valueType)
        {
            MethodInfo enumConvertMethod;
            if (valueType == TypeUtils.StringType)
            {
                enumConvertMethod = typeof(Enum).GetMethod("Parse", new Type[] { TypeUtils.TypeType, TypeUtils.StringType });
            }
            else
            {
                enumConvertMethod = typeof(Enum).GetMethod("ToObject", new Type[] { TypeUtils.TypeType, valueType });
            }
            iLGenerator.Emit(OpCodes.Call, enumConvertMethod);
        }
    }
}