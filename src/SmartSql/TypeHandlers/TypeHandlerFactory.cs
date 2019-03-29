using System;
using System.Collections.Concurrent;
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

        private readonly Dictionary<Type, IDictionary<Type, ITypeHandler>> _typeHandlerMap = new Dictionary<Type, IDictionary<Type, ITypeHandler>>();
        private readonly Dictionary<String, ITypeHandler> _nameHandlerMap = new Dictionary<String, ITypeHandler>();

        private readonly ITypeHandlerBuilder _typeHandlerBuilder = new TypeHandlerBuilder();
        public TypeHandlerFactory()
        {
            ITypeHandler handler = null;

            #region Boolean
            handler = new BooleanTypeHandler();
            this.Register(handler);
            handler = new BooleanCharTypeHandler();
            this.Register(handler);
            handler = new BooleanStringTypeHandler();
            this.Register(handler);
            handler = new BooleanAnyTypeHandler();
            this.Register(handler);
            handler = new NullableBooleanTypeHandler();
            this.Register(handler);
            handler = new NullableBooleanCharTypeHandler();
            this.Register(handler);
            handler = new NullableBooleanStringTypeHandler();
            this.Register(handler);
            handler = new NullableBooleanAnyTypeHandler();
            this.Register(handler);
            #endregion

            #region Byte

            handler = new ByteTypeHandler();
            this.Register(handler);
            handler = new ByteAnyTypeHandler();
            this.Register(handler);
            handler = new NullableByteTypeHandler();
            this.Register(handler);
            handler = new NullableByteAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Char

            handler = new CharTypeHandler();
            this.Register(handler);
            handler = new CharAnyTypeHandler();
            this.Register(handler);
            handler = new NullableCharTypeHandler();
            this.Register(handler);
            handler = new NullableCharAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region DateTime

            handler = new DateTimeTypeHandler();
            this.Register(handler);
            handler = new NullableDateTimeTypeHandler();
            this.Register(handler);

            #endregion

            #region Decimal

            handler = new DecimalTypeHandler();
            this.Register(handler);
            handler = new DecimalAnyTypeHandler();
            this.Register(handler);
            handler = new NullableDecimalTypeHandler();
            this.Register(handler);
            handler = new NullableDecimalAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Double

            handler = new DoubleTypeHandler();
            this.Register(handler);
            handler = new DoubleAnyTypeHandler();
            this.Register(handler);
            handler = new NullableDoubleTypeHandler();
            this.Register(handler);
            handler = new NullableDoubleAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Int16

            handler = new Int16TypeHandler();
            this.Register(handler);
            handler = new Int16ByteTypeHandler();
            this.Register(handler);
            handler = new Int16AnyTypeHandler();
            this.Register(handler);
            handler = new NullableInt16TypeHandler();
            this.Register(handler);
            handler = new NullableInt16AnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Int32

            handler = new Int32TypeHandler();
            this.Register(handler);
            handler = new Int32ByteTypeHandler();
            this.Register(handler);
            handler = new Int32Int16TypeHandler();
            this.Register(handler);
            handler = new Int32Int64TypeHandler();
            this.Register(handler);
            handler = new Int32AnyTypeHandler();
            this.Register(handler);
            handler = new NullableInt32TypeHandler();
            this.Register(handler);
            handler = new NullableInt32AnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Int64

            handler = new Int64TypeHandler();
            this.Register(handler);
            handler = new Int64ByteTypeHandler();
            this.Register(handler);
            handler = new Int64Int16TypeHandler();
            this.Register(handler);
            handler = new Int64Int32TypeHandler();
            this.Register(handler);
            handler = new Int64AnyTypeHandler();
            this.Register(handler);
            handler = new NullableInt64TypeHandler();
            this.Register(handler);
            handler = new NullableInt64AnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Single

            handler = new SingleTypeHandler();
            this.Register(handler);
            handler = new SingleAnyTypeHandler();
            this.Register(handler);
            handler = new NullableSingleTypeHandler();
            this.Register(handler);
            handler = new NullableSingleAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region String

            handler = new StringTypeHandler();
            this.Register(handler);
            handler = new StringAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Guid

            handler = new GuidTypeHandler();
            this.Register(handler);
            handler = new GuidAnyTypeHandler();
            this.Register(handler);
            handler = new NullableGuidTypeHandler();
            this.Register(handler);
            handler = new NullableGuidAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region TimeSpan

            handler = new TimeSpanTypeHandler();
            this.Register(handler);
            handler = new TimeSpanAnyTypeHandler();
            this.Register(handler);
            handler = new NullableTimeSpanTypeHandler();
            this.Register(handler);
            handler = new NullableTimeSpanAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region ByteArray

            handler = new ByteArrayTypeHandler();
            this.Register(handler);
            handler = new ByteArrayAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region CharArray

            handler = new CharArrayTypeHandler();
            this.Register(handler);
            handler = new CharArrayAnyTypeHandler();
            this.Register(handler);

            #endregion

            #region Object

            handler = new ObjectTypeHandler();
            this.Register(handler);

            #endregion

            #region UInt16

            handler = new UInt16TypeHandler();
            this.Register(handler);
            handler = new NullableUInt16TypeHandler();
            this.Register(handler);

            #endregion

            #region UInt32

            handler = new UInt32TypeHandler();
            this.Register(handler);
            handler = new NullableUInt32TypeHandler();
            this.Register(handler);

            #endregion

            #region UInt64

            handler = new UInt64TypeHandler();
            this.Register(handler);
            handler = new NullableUInt64TypeHandler();
            this.Register(handler);

            #endregion

            #region SByte

            handler = new SByteTypeHandler();
            this.Register(handler);
            handler = new NullableSByteTypeHandler();
            this.Register(handler);

            #endregion
        }
        public ITypeHandler GetTypeHandler(string handlerName)
        {
            if (_nameHandlerMap.TryGetValue(handlerName, out var typeHandler))
            {
                return typeHandler;
            }
            throw new SmartSqlException($"Not Find TypeHandler.Name:{handlerName}!");
        }
        public ITypeHandler GetTypeHandler(Type propertyType)
        {
            if (TryGetTypeHandler(propertyType, propertyType, out var typeHandler))
            {
                return typeHandler;
            }
            if (TryGetTypeHandler(propertyType, AnyFieldTypeType.Type, out typeHandler))
            {
                return typeHandler;
            }
            var nullUnderType = Nullable.GetUnderlyingType(propertyType);
            bool isEnum = (nullUnderType ?? propertyType).IsEnum;
            if (!isEnum) throw new SmartSqlException($"Not Find TypeHandler:{propertyType.FullName}!");
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
            if (TryGetTypeHandler(enumType, AnyFieldTypeType.Type, out typeHandler))
            {
                return false;
            }
            var nullUnderType = Nullable.GetUnderlyingType(enumType);
            var enumTypeHandlerType = nullUnderType == null ? ENUM_TYPE_HANDLER_TYPE : NULLABLE_ENUM_TYPE_HANDLER_TYPE;
            typeHandler = _typeHandlerBuilder.Build(enumTypeHandlerType, enumType, null);
            Register(typeHandler);
            return true;
        }

        public void Register(ITypeHandler typeHandler)
        {
            if (!_typeHandlerMap.TryGetValue(typeHandler.PropertyType, out var fieldTypeHandlerMap))
            {
                fieldTypeHandlerMap = new Dictionary<Type, ITypeHandler>();
                _typeHandlerMap.Add(typeHandler.PropertyType, fieldTypeHandlerMap);
            }
            if (fieldTypeHandlerMap.ContainsKey(typeHandler.FieldType))
            {
                fieldTypeHandlerMap[typeHandler.FieldType] = typeHandler;
            }
            else
            {
                fieldTypeHandlerMap.Add(typeHandler.FieldType, typeHandler);
            }

            TypeHandlerCacheType.SetHandler(typeHandler);
            if (PropertyTypeHandlerCacheType.GetHandler(typeHandler.PropertyType) == null)
            {
                PropertyTypeHandlerCacheType.SetHandler(typeHandler);
            }
        }
        private void Register(string handlerName, ITypeHandler typeHandler)
        {
            if (_nameHandlerMap.ContainsKey(handlerName))
            {
                _nameHandlerMap[handlerName] = typeHandler;
            }
            else
            {
                _nameHandlerMap.Add(handlerName, typeHandler);
            }
            Register(typeHandler);
        }

        public void Register(TypeHandler typeHandlerConfig)
        {
            var isGenericType = typeHandlerConfig.HandlerType.IsGenericType;
            ITypeHandler typeHandler = isGenericType
                ? _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType, typeHandlerConfig.PropertyType, typeHandlerConfig.Properties)
                : _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType, typeHandlerConfig.Properties);
            if (isGenericType)
            {
                Register(typeHandler);
            }
            else
            {
                Register(typeHandlerConfig.Name, typeHandler);
            }
        }
    }
}
