using System;
using System.Collections.Generic;
using SmartSql.Configuration;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.TypeHandlers
{
    public class TypeHandlerFactory
    {
        private static readonly Type ENUM_TYPE_HANDLER_TYPE = typeof(EnumTypeHandler<>);
        private static readonly Type NULLABLE_ENUM_TYPE_HANDLER_TYPE = typeof(NullableEnumTypeHandler<>);
        private static readonly UnknownTypeHandler UNKNOWN_TYPE_HANDLER = new UnknownTypeHandler();
        private readonly Dictionary<string, ITypeHandler> _nameHandlerMap = new Dictionary<string, ITypeHandler>();

        private readonly ITypeHandlerBuilder _typeHandlerBuilder = new TypeHandlerBuilder();

        private readonly Dictionary<Type, IDictionary<Type, ITypeHandler>> _typeHandlerMap =
            new Dictionary<Type, IDictionary<Type, ITypeHandler>>();

        public TypeHandlerFactory()
        {
            ITypeHandler handler = null;

            #region Boolean

            handler = new BooleanTypeHandler();
            Register(handler);
            handler = new BooleanCharTypeHandler();
            Register(handler);
            handler = new BooleanStringTypeHandler();
            Register(handler);
            handler = new BooleanAnyTypeHandler();
            Register(handler);
            handler = new NullableBooleanTypeHandler();
            Register(handler);
            handler = new NullableBooleanCharTypeHandler();
            Register(handler);
            handler = new NullableBooleanStringTypeHandler();
            Register(handler);
            handler = new NullableBooleanAnyTypeHandler();
            Register(handler);

            #endregion

            #region Byte

            handler = new ByteTypeHandler();
            Register(handler);
            handler = new ByteAnyTypeHandler();
            Register(handler);
            handler = new NullableByteTypeHandler();
            Register(handler);
            handler = new NullableByteAnyTypeHandler();
            Register(handler);

            #endregion

            #region Char

            handler = new CharTypeHandler();
            Register(handler);
            handler = new CharAnyTypeHandler();
            Register(handler);
            handler = new NullableCharTypeHandler();
            Register(handler);
            handler = new NullableCharAnyTypeHandler();
            Register(handler);

            #endregion

            #region DateTime

            handler = new DateTimeTypeHandler();
            Register(handler);
            handler = new DateTimeStringTypeHandler();
            Register(handler);
            handler = new DateTimeAnyTypeHandler();
            Register(handler);
            handler = new NullableDateTimeTypeHandler();
            Register(handler);
            handler = new NullableDateTimeStringTypeHandler();
            Register(handler);
            handler = new NullableDateTimeAnyTypeHandler();
            Register(handler);

            #endregion

            #region Decimal

            handler = new DecimalTypeHandler();
            Register(handler);
            handler = new DecimalAnyTypeHandler();
            Register(handler);
            handler = new NullableDecimalTypeHandler();
            Register(handler);
            handler = new NullableDecimalAnyTypeHandler();
            Register(handler);

            #endregion

            #region Double

            handler = new DoubleTypeHandler();
            Register(handler);
            handler = new DoubleAnyTypeHandler();
            Register(handler);
            handler = new NullableDoubleTypeHandler();
            Register(handler);
            handler = new NullableDoubleAnyTypeHandler();
            Register(handler);

            #endregion

            #region Int16

            handler = new Int16TypeHandler();
            Register(handler);
            handler = new Int16ByteTypeHandler();
            Register(handler);
            handler = new Int16AnyTypeHandler();
            Register(handler);
            handler = new NullableInt16TypeHandler();
            Register(handler);
            handler = new NullableInt16AnyTypeHandler();
            Register(handler);

            #endregion

            #region Int32

            handler = new Int32TypeHandler();
            Register(handler);
            handler = new Int32ByteTypeHandler();
            Register(handler);
            handler = new Int32Int16TypeHandler();
            Register(handler);
            handler = new Int32Int64TypeHandler();
            Register(handler);
            handler = new Int32AnyTypeHandler();
            Register(handler);
            handler = new NullableInt32TypeHandler();
            Register(handler);
            handler = new NullableInt32AnyTypeHandler();
            Register(handler);

            #endregion

            #region Int64

            handler = new Int64TypeHandler();
            Register(handler);
            handler = new Int64ByteTypeHandler();
            Register(handler);
            handler = new Int64Int16TypeHandler();
            Register(handler);
            handler = new Int64Int32TypeHandler();
            Register(handler);
            handler = new Int64AnyTypeHandler();
            Register(handler);
            handler = new NullableInt64TypeHandler();
            Register(handler);
            handler = new NullableInt64AnyTypeHandler();
            Register(handler);

            #endregion

            #region Single

            handler = new SingleTypeHandler();
            Register(handler);
            handler = new SingleAnyTypeHandler();
            Register(handler);
            handler = new NullableSingleTypeHandler();
            Register(handler);
            handler = new NullableSingleAnyTypeHandler();
            Register(handler);

            #endregion

            #region String

            handler = new StringTypeHandler();
            Register(handler);
            handler = new StringAnyTypeHandler();
            Register(handler);

            #endregion

            #region Guid

            handler = new GuidTypeHandler();
            Register(handler);
            handler = new GuidAnyTypeHandler();
            Register(handler);
            handler = new NullableGuidTypeHandler();
            Register(handler);
            handler = new NullableGuidAnyTypeHandler();
            Register(handler);

            #endregion

            #region TimeSpan

            handler = new TimeSpanTypeHandler();
            Register(handler);
            handler = new TimeSpanAnyTypeHandler();
            Register(handler);
            handler = new NullableTimeSpanTypeHandler();
            Register(handler);
            handler = new NullableTimeSpanAnyTypeHandler();
            Register(handler);

            #endregion

            #region ByteArray

            handler = new ByteArrayTypeHandler();
            Register(handler);
            handler = new ByteArrayAnyTypeHandler();
            Register(handler);

            #endregion

            #region CharArray

            handler = new CharArrayTypeHandler();
            Register(handler);
            handler = new CharArrayAnyTypeHandler();
            Register(handler);

            #endregion

            #region UInt16

            handler = new UInt16TypeHandler();
            Register(handler);
            handler = new UInt16ByteTypeHandler();
            Register(handler);
            handler = new UInt16AnyTypeHandler();
            Register(handler);
            handler = new NullableUInt16TypeHandler();
            Register(handler);
            handler = new NullableUInt16AnyTypeHandler();
            Register(handler);
            #endregion

            #region UInt32

            handler = new UInt32TypeHandler();
            Register(handler);
            handler = new UInt32ByteTypeHandler();
            Register(handler);
            handler = new UInt32UInt16TypeHandler();
            Register(handler);
            handler = new UInt32AnyTypeHandler();
            Register(handler);
            handler = new NullableUInt32TypeHandler();
            Register(handler);
            handler = new NullableUInt32AnyTypeHandler();
            Register(handler);
            
            #endregion

            #region UInt64

            handler = new UInt64TypeHandler();
            Register(handler);
            handler = new UInt64ByteTypeHandler();
            Register(handler);
            handler = new UInt64UInt16TypeHandler();
            Register(handler);
            handler = new UInt64UInt32TypeHandler();
            Register(handler);
            handler = new UInt64AnyTypeHandler();
            Register(handler);
            handler = new NullableUInt64TypeHandler();
            Register(handler);
            handler = new NullableUInt64AnyTypeHandler();
            Register(handler);
            
            #endregion

            #region SByte

            handler = new SByteTypeHandler();
            Register(handler);
            handler = new NullableSByteTypeHandler();
            Register(handler);

            #endregion

            #region Unknown

            Register(UNKNOWN_TYPE_HANDLER);

            #endregion
        }

        public ITypeHandler GetTypeHandler(string handlerName)
        {
            if (_nameHandlerMap.TryGetValue(handlerName, out var typeHandler)) return typeHandler;
            throw new SmartSqlException($"Not Find TypeHandler.Name:{handlerName}!");
        }

        public ITypeHandler GetTypeHandler(Type propertyType)
        {
            if (TryGetTypeHandler(propertyType, propertyType, out var typeHandler)) return typeHandler;
            if (TryGetTypeHandler(propertyType, AnyFieldTypeType.Type, out typeHandler)) return typeHandler;
            var nullUnderType = Nullable.GetUnderlyingType(propertyType);
            var isEnum = (nullUnderType ?? propertyType).IsEnum;
            if (!isEnum)
            {
                return UNKNOWN_TYPE_HANDLER;
            }

            TryRegisterEnumTypeHandler(propertyType, out typeHandler);
            return typeHandler;
        }

        public bool TryGetTypeHandler(Type propertyType, Type fieldType, out ITypeHandler typeHandler)
        {
            if (!_typeHandlerMap.TryGetValue(propertyType, out var fieldTypeHandlerMap))
            {
                typeHandler = default;
                return false;
            }

            return fieldTypeHandlerMap.TryGetValue(fieldType, out typeHandler);
        }

        public bool TryRegisterEnumTypeHandler(Type enumType, out ITypeHandler typeHandler)
        {
            if (TryGetTypeHandler(enumType, AnyFieldTypeType.Type, out typeHandler)) return false;
            var nullUnderType = Nullable.GetUnderlyingType(enumType);
            var enumTypeHandlerType = nullUnderType == null ? ENUM_TYPE_HANDLER_TYPE : NULLABLE_ENUM_TYPE_HANDLER_TYPE;
            typeHandler = _typeHandlerBuilder.Build(enumTypeHandlerType, enumType, null);
            Register(typeHandler);
            return true;
        }

        
        public void Register(ITypeHandler typeHandler)
        {
            lock (this)
            {
                if (!_typeHandlerMap.TryGetValue(typeHandler.PropertyType, out var fieldTypeHandlerMap))
                {
                    fieldTypeHandlerMap = new Dictionary<Type, ITypeHandler>();
                    _typeHandlerMap.Add(typeHandler.PropertyType, fieldTypeHandlerMap);
                }

                if (fieldTypeHandlerMap.ContainsKey(typeHandler.FieldType))
                    fieldTypeHandlerMap[typeHandler.FieldType] = typeHandler;
                else
                    fieldTypeHandlerMap.Add(typeHandler.FieldType, typeHandler);

                TypeHandlerCacheType.SetHandler(typeHandler);
                PropertyTypeHandlerCacheType.SetHandler(typeHandler);
            }
        }

        public void Register(string handlerName, ITypeHandler typeHandler)
        {
            if (_nameHandlerMap.ContainsKey(handlerName))
                _nameHandlerMap[handlerName] = typeHandler;
            else
                _nameHandlerMap.Add(handlerName, typeHandler);
        }

        public IDictionary<string, ITypeHandler> GetNamedTypeHandlers()
        {
            return _nameHandlerMap;
        }

        public void Register(TypeHandler typeHandlerConfig)
        {
            ITypeHandler typeHandler = null;
            if (typeHandlerConfig.HandlerType.IsGenericType)
            {
                var genericArgs = typeHandlerConfig.HandlerType.GetGenericArguments();
                switch (genericArgs.Length)
                {
                    case 2:
                        {
                            typeHandler = _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType
                                , typeHandlerConfig.PropertyType, typeHandlerConfig.FieldType,
                                typeHandlerConfig.Properties);
                            break;
                        }
                    case 1:
                        {
                            if (typeHandlerConfig.PropertyType != null)
                                typeHandler = _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType,
                                    typeHandlerConfig.PropertyType, typeHandlerConfig.Properties);
                            if (typeHandlerConfig.FieldType != null)
                                typeHandler = _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType,
                                    typeHandlerConfig.FieldType, typeHandlerConfig.Properties);
                            break;
                        }
                    default:
                        {
                            throw new SmartSqlException(
                                $"Wrong TypeHandlerConfig.Type:{typeHandlerConfig.HandlerType.FullName}.");
                        }
                }
            }
            else
            {
                typeHandler = _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType, typeHandlerConfig.Properties);
            }

            if (!string.IsNullOrEmpty(typeHandlerConfig.Name))
                Register(typeHandlerConfig.Name, typeHandler);
            else
                Register(typeHandler);
        }
    }
}