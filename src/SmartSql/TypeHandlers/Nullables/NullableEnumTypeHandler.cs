using SmartSql.Data;
using System;
using System.Data;

namespace SmartSql.TypeHandlers
{
    public class NullableEnumTypeHandler<TEnum> : AbstractNullableTypeHandler<TEnum>
    {
        private readonly Type _enumType;
        private readonly Type _enumUnderlyingType;
        public NullableEnumTypeHandler()
        {
            _enumType = Nullable.GetUnderlyingType(MappedType);
            _enumUnderlyingType = Enum.GetUnderlyingType(_enumType);
        }

        public override void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            if (parameterValue == null && IsNullable)
            {
                dataParameter.Value = DBNull.Value;
            }
            else
            {
                dataParameter.Value = Convert.ChangeType(parameterValue, _enumUnderlyingType);
            }
        }
        protected override TEnum GetValueWhenNotNull(DataReaderWrapper dataReader, int columnIndex)
        {
            return (TEnum)Enum.ToObject(_enumType, dataReader.GetValue(columnIndex));
        }
    }
}
