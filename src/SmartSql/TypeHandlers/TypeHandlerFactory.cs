using System;
using System.Collections.Concurrent;
using SmartSql.Configuration;
using SmartSql.Exceptions;
using SmartSql.Reflection.TypeConstants;

namespace SmartSql.TypeHandlers
{
    public class TypeHandlerFactory : ITypeHandlerFactory
    {
        private readonly static Type ENUM_TYPE_HANDLER_TYPE = typeof(EnumTypeHandler<>);
        private readonly static Type NULLABLE_ENUM_TYPE_HANDLER_TYPE = typeof(NullableEnumTypeHandler<>);

        private readonly ConcurrentDictionary<Type, ITypeHandler> _type_Handler_Map = new ConcurrentDictionary<Type, ITypeHandler>();
        private readonly ConcurrentDictionary<String, ITypeHandler> _name_Handler_Map = new ConcurrentDictionary<String, ITypeHandler>();

        private readonly ITypeHandlerBuilder _typeHandlerBuilder = new TypeHandlerBuilder();
        public TypeHandlerFactory()
        {
            #region Init Type_Handler_Map
            ITypeHandler handler = null;

            handler = new BooleanTypeHandler();
            this.Register(handler);
            handler = new NullableBooleanTypeHandler();
            this.Register(handler);

            handler = new ByteTypeHandler();
            this.Register(handler);
            handler = new NullableByteTypeHandler();
            this.Register(handler);

            handler = new CharTypeHandler();
            this.Register(handler);
            handler = new NullableCharTypeHandler();
            this.Register(handler);

            handler = new DateTimeTypeHandler();
            this.Register(handler);
            handler = new NullableDateTimeTypeHandler();
            this.Register(handler);

            handler = new DecimalTypeHandler();
            this.Register(handler);
            handler = new NullableDecimalTypeHandler();
            this.Register(handler);

            handler = new DoubleTypeHandler();
            this.Register(handler);
            handler = new NullableDoubleTypeHandler();
            this.Register(handler);

            handler = new Int16TypeHandler();
            this.Register(handler);
            handler = new NullableInt16TypeHandler();
            this.Register(handler);

            handler = new Int32TypeHandler();
            this.Register(handler);
            handler = new NullableInt32TypeHandler();
            this.Register(handler);

            handler = new Int64TypeHandler();
            this.Register(handler);
            handler = new NullableInt64TypeHandler();
            this.Register(handler);

            handler = new SingleTypeHandler();
            this.Register(handler);
            handler = new NullableSingleTypeHandler();
            this.Register(handler);

            handler = new StringTypeHandler();
            this.Register(handler);

            handler = new GuidTypeHandler();
            this.Register(handler);
            handler = new NullableGuidTypeHandler();
            this.Register(handler);

            handler = new TimeSpanTypeHandler();
            this.Register(handler);
            handler = new NullableTimeSpanTypeHandler();
            this.Register(handler);

            handler = new ByteArrayTypeHandler();
            this.Register(handler);

            handler = new CharArrayTypeHandler();
            this.Register(handler);

            handler = new ObjectTypeHandler();
            this.Register(handler);

            handler = new UInt16TypeHandler();
            this.Register(handler);
            handler = new NullableUInt16TypeHandler();
            this.Register(handler);

            handler = new UInt32TypeHandler();
            this.Register(handler);
            handler = new NullableUInt32TypeHandler();
            this.Register(handler);

            handler = new UInt64TypeHandler();
            this.Register(handler);
            handler = new NullableUInt64TypeHandler();
            this.Register(handler);

            handler = new SByteTypeHandler();
            this.Register(handler);
            handler = new NullableSByteTypeHandler();
            this.Register(handler);
            #endregion
        }
        public ITypeHandler Get(string handlerName)
        {
            if (_name_Handler_Map.TryGetValue(handlerName, out var typeHandler))
            {
                return typeHandler;
            }
            throw new SmartSqlException($"Not Find TypeHandler.Name:{handlerName}!");
        }
        public ITypeHandler Get(Type mappedType)
        {
            if (_type_Handler_Map.TryGetValue(mappedType, out var typeHandler))
            {
                return typeHandler;
            }
            var nullUnderType = Nullable.GetUnderlyingType(mappedType);
            bool isEnum = (nullUnderType ?? mappedType).IsEnum;
            if (!isEnum) throw new SmartSqlException($"Not Find TypeHandler:{mappedType.FullName}!");
            TryRegisterEnum(mappedType, out typeHandler);
            return typeHandler;
        }

        private bool TryRegisterEnum(Type enumType, out ITypeHandler typeHandler)
        {
            if (_type_Handler_Map.TryGetValue(enumType, out typeHandler))
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
            _type_Handler_Map.AddOrUpdate(typeHandler.MappedType, typeHandler, (type, handler) => typeHandler);
            _name_Handler_Map.AddOrUpdate(typeHandler.Name, typeHandler, (handlerName, handler) => typeHandler);
            TypeHandlerCacheType.SetHandler(typeHandler);
        }

        public void Register(TypeHandler typeHandlerConfig)
        {
            ITypeHandler typeHandler;
            typeHandler = typeHandlerConfig.HandlerType.IsGenericType 
                ? _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType, typeHandlerConfig.MappedType, typeHandlerConfig.Parameters) 
                : _typeHandlerBuilder.Build(typeHandlerConfig.HandlerType, typeHandlerConfig.Parameters);
            Register(typeHandler);
        }
    }
}
