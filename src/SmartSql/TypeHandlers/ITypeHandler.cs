using SmartSql.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace SmartSql.TypeHandlers
{
    public interface ITypeHandler : IInitialize
    {
        String Name { get; }
        Type MappedType { get; }
        void SetParameter(IDataParameter dataParameter, object parameterValue);
    }
    public interface ITypeHandler<out T> : ITypeHandler
    {
        T Default { get; }
        T GetValue(DataReaderWrapper dataReader, int columnIndex);
        T GetValue(DataReaderWrapper dataReader, string columnName);
    }
}
