using SmartSql.Data;
using System;
using System.Data;

namespace SmartSql.TypeHandlers
{
    public class EnumTypeHandler<TEnum> : AbstractTypeHandler<TEnum, AnyFieldType>
    {
        private readonly Type _enumUnderlyingType;
        public EnumTypeHandler()
        {
            _enumUnderlyingType = Enum.GetUnderlyingType(PropertyType);
        }
        public override void SetParameter(IDataParameter dataParameter, object parameterValue)
        {
            dataParameter.Value = Convert.ChangeType(parameterValue, _enumUnderlyingType);
        }

        public override TEnum GetValue(DataReaderWrapper dataReader, int columnIndex, Type targetType)
        {
            return (TEnum)Enum.ToObject(PropertyType, dataReader.GetValue(columnIndex));
        }
    }
}
