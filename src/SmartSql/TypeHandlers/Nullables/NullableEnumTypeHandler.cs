using SmartSql.Data;
using System;
using System.Data;

namespace SmartSql.TypeHandlers
{
    public class NullableEnumTypeHandler<TEnum> : AbstractNullableTypeHandler<TEnum, AnyFieldType>
    {
        private readonly Type _enumType;
        private readonly Type _enumUnderlyingType;

        public NullableEnumTypeHandler()
        {
            _enumType = Nullable.GetUnderlyingType(PropertyType);
            _enumUnderlyingType = Enum.GetUnderlyingType(_enumType);
        }
        
        protected override object GetSetParameterValueWhenNotNull(object parameterValue)
        {
            return Convert.ChangeType(parameterValue, _enumUnderlyingType);
        }

        protected override TEnum GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return (TEnum) Enum.ToObject(_enumType, dataReader.GetValue(columnIndex));
        }
    }
}